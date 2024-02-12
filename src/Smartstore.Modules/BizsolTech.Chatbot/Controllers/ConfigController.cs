using System.Threading.Tasks;
using BizsolTech.Chatbot.Configuration;
using BizsolTech.Chatbot.Models;
using BizsolTech.Chatbot.Models.Business;
using BizsolTech.Chatbot.Services;
using Microsoft.AspNetCore.Mvc;
using Smartstore.ComponentModel;
using Smartstore.Core.Security;
using Smartstore.Web.Controllers;
using Smartstore.Web.Modelling.Settings;

namespace BizsolTech.Chatbot.Controllers
{
    public class ConfigController : AdminController
    {
        private readonly IBusinessAPIService _businessAPIService;

        public ConfigController(IBusinessAPIService businessAPI)
        {
            _businessAPIService = businessAPI;
        }
        #region Configuration

        [LoadSetting, AuthorizeAdmin]
        public async Task<IActionResult> Configure(ChatbotSettings settings)
        {
            var model = MiniMapper.Map<ChatbotSettings, ConfigurationModel>(settings);

            var chatPrompt = await _businessAPIService.GetChatPrompt(1); //Fixed because only have one prompt at moment
            if (chatPrompt != null)
            {
                model.ChatPromptId = chatPrompt.Id;
                model.ChatPrompt = chatPrompt.ChatPrompt;
            }

            return View(model);
        }

        [HttpPost, SaveSetting, AuthorizeAdmin]
        public async Task<IActionResult> Configure(ConfigurationModel model, ChatbotSettings settings)
        {
            if (!ModelState.IsValid)
            {
                return await Configure(settings);
            }
            if (model.ChatPromptId > 0)
            {
                var template = new BusinessChatPrompt();
                template.Id = model.ChatPromptId;
                template.ChatPrompt = model.ChatPrompt;
                var success = await _businessAPIService.UpdateChatPrompt(template);
                if (success)
                {
                    NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));
                }
                else
                {
                    NotifySuccess(T("Common.Error"));
                    return await Configure(settings);
                }
            }
            if (model.ChatPromptId == 0)
            {
                var context = new BusinessChatPrompt();
                context.ChatPrompt = model.ChatPrompt;
                var result = await _businessAPIService.AddChatPrompt(context);
                if (result != null)
                {
                    NotifySuccess(T("Admin.Common.DataSuccessfullySaved"));
                }
                else
                {
                    NotifySuccess(T("Common.Error"));
                    return await Configure(settings);
                }
            }

            ModelState.Clear();
            MiniMapper.Map(model, settings);

            return RedirectToAction(nameof(Configure));
        }

        #endregion
    }
}
