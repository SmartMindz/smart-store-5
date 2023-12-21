using BizsolTech.Chatbot.Configuration;
using BizsolTech.Chatbot.Models;
using Microsoft.AspNetCore.Mvc;
using Smartstore.ComponentModel;
using Smartstore.Core.Security;
using Smartstore.Web.Controllers;
using Smartstore.Web.Modelling.Settings;

namespace BizsolTech.Chatbot.Controllers
{
    public class ConfigController : AdminController
    {
        #region Configuration

        [LoadSetting, AuthorizeAdmin]
        public IActionResult Configure(ChatbotSettings settings)
        {
            var model = MiniMapper.Map<ChatbotSettings, ConfigurationModel>(settings);

            return View(model);
        }

        [HttpPost, SaveSetting, AuthorizeAdmin]
        public IActionResult Configure(ConfigurationModel model, ChatbotSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return Configure(settings);
            }

            ModelState.Clear();
            MiniMapper.Map(model, settings);

            NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));

            return RedirectToAction(nameof(Configure));
        }

        #endregion
    }
}
