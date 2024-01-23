using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Smartstore.Core.Identity;
using Smartstore.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Smartstore;
using BizsolTech.Chatbot.Services;
using BizsolTech.Chatbot.Models;
using Microsoft.AspNetCore.Http;

namespace BizsolTech.Chatbot.Filters
{
    internal class ChatbotLoginFilter : IAsyncActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Customer> _userManager;

        private readonly IBusinessService _businessService;
        private readonly IBusinessAPIService _businessAPI;

        public ChatbotLoginFilter(IHttpContextAccessor httpContextAccessor, UserManager<Customer> userManager, IBusinessService businessService, IBusinessAPIService businessAPIService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

            _businessService = businessService;
            _businessAPI = businessAPIService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();

            var type = context.Controller.GetType();
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (type == typeof(IdentityController) && actionDescriptor.ActionName.Equals("Login"))
            {
                if (context.HttpContext.Request.Method == "POST")
                {
                    Customer customer;
                    var customerLoginType = context.HttpContext.Request.Form["model.CustomerLoginType"];

                    if (customerLoginType == CustomerLoginType.Username)
                    {
                        var username = context.HttpContext.Request.Form["model.Username"].ToString();
                        customer = await _userManager.FindByNameAsync(username.TrimSafe());
                    }
                    else if (customerLoginType == CustomerLoginType.Email)
                    {
                        var email = context.HttpContext.Request.Form["model.Email"].ToString();
                        customer = await _userManager.FindByEmailAsync(email.TrimSafe());
                    }
                    else
                    {
                        var userNameOrEmail = context.HttpContext.Request.Form["model.UsernameOrEmail"].ToString();
                        customer = await _userManager.FindByEmailAsync(userNameOrEmail.TrimSafe()) ?? await _userManager.FindByNameAsync(userNameOrEmail.TrimSafe());
                    }

                    if (customer != null)
                    {
                        var model = new BusinessModel();
                        var mappings = await _businessService.GetBusinessMappings();
                        var bMapping = mappings.FirstOrDefault(m => m.EntityId.Equals(customer.Id) && m.EntityName.Equals(nameof(Customer)));

                        if (bMapping != null)
                        {
                            var business = await _businessAPI.GetBusiness(bMapping.BusinessId);
                            if (business != null)
                            {
                                model.Id = business.Id; //businessId from server
                                model.BusinessName = business.BusinessName;
                                model.WelcomeMessage = business.WelcomeMessage;
                                model.Description = business.Description;
                                model.Instruction = business.Instruction;
                                model.OpenAPIKey = business.OpenAPIKey;
                                model.OpenAPIStatus = business.OpenAPIStatus;
                                model.FBPageId = int.Parse(business.FBPageId);
                                model.FBAccessToken = business.FBAccessToken;
                                model.FBStatus = business.FBStatus;
                                var sessionStored = _httpContextAccessor.HttpContext?.Session.TrySetObject<BusinessModel>("BusinessInput", model);
                            }
                        }

                        if (model.BusinessName.IsNullOrEmpty() || model.Description.IsNullOrEmpty() || model.Instruction.IsNullOrEmpty())
                        {
                            context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "Index" });
                        }
                        else if (model.OpenAPIKey.IsNullOrEmpty() || model.OpenAPIStatus == false)
                        {
                            context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "ChatConnection" });
                        }
                        else (!model.FBPageId.HasValue || model.FBAccessToken.IsNullOrEmpty())
                        {
                            context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "ChatResponse" });
                        }


                        // Halt the execution here, do not invoke the next filter or action
                        return;
                    }
                }
            }
        }
    }
}
