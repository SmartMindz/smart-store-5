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
                    var customerLoginType = context.HttpContext.Request.Form["CustomerLoginType"];

                    if (customerLoginType == CustomerLoginType.Username)
                    {
                        var username = context.HttpContext.Request.Form["Username"].ToString();
                        customer = await _userManager.FindByNameAsync(username.TrimSafe());
                    }
                    else if (customerLoginType == CustomerLoginType.Email)
                    {
                        var email = context.HttpContext.Request.Form["Email"].ToString();
                        customer = await _userManager.FindByEmailAsync(email.TrimSafe());
                    }
                    else
                    {
                        var userNameOrEmail = context.HttpContext.Request.Form["UsernameOrEmail"].ToString();
                        customer = await _userManager.FindByEmailAsync(userNameOrEmail.TrimSafe()) ?? await _userManager.FindByNameAsync(userNameOrEmail.TrimSafe());
                    }

                    if (customer != null)
                    {
                        var model = new BusinessModel();
                        var customerBusinesses = await _businessService.GetCustomerBusinessAll(customer);

                        if (customerBusinesses.Count == 0)
                        {
                            context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "Index" });
                        }
                        else if (customerBusinesses.Count == 1)
                        {
                            if (customerBusinesses[0].BusinessName.IsNullOrEmpty() || customerBusinesses[0].Description.IsNullOrEmpty() || customerBusinesses[0].Instruction.IsNullOrEmpty())
                            {
                                context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "Index", id = customerBusinesses[0].Id });
                            }
                            else if (customerBusinesses[0].OpenAPIKey.IsNullOrEmpty() || customerBusinesses[0].OpenAPIStatus == false)
                            {
                                context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "ChatConnection", id = customerBusinesses[0].Id });
                            }
                            else
                            {
                                context.Result = new RedirectToRouteResult("BusinessRoute", new { area = Module.ModuleSystemName, controller = "Business", action = "ChatResponse", id = customerBusinesses[0].Id });
                            }
                        }
                        else if (customerBusinesses.Count > 1)
                        {
                            context.Result = new RedirectToActionResult("List", "Product", new { area = "Admin" });
                        }
                        else
                        {
                            // Halt the execution here, do not invoke the next filter or action
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }
    }
}
