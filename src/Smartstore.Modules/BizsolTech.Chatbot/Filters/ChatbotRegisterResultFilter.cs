using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Smartstore.Core.Identity;
using Smartstore.Web.Controllers;

namespace BizsolTech.Chatbot.Filters
{
    public class ChatbotRegisterResultFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var type = context.Controller.GetType();
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (type == typeof(IdentityController) && actionDescriptor.ActionName.Equals("RegisterResult"))
            {
                if (context.HttpContext.Request.Method == "GET")
                {
                    if (context.HttpContext.Request.Query.TryGetValue("resultId", out var resultId))
                    {
                        if (!string.IsNullOrEmpty(resultId))
                        {
                            var id = int.Parse(resultId);
                            if ((UserRegistrationType)id == UserRegistrationType.Standard) //registration is successful
                            {
                                context.Result = new RedirectToRouteResult("BusinessRoute", new { area = "BizsolTech.Chatbot", controller = "Business", action = "Index" });

                                // Halt the execution here, do not invoke the next filter or action
                                return;
                            }
                        }
                    }
                }
            }

            await next();
        }
    }
}
