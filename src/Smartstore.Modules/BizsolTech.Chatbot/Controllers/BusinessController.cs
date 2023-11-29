using System.Net;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Models;
using BizsolTech.Chatbot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Smartstore;
using Smartstore.Core.Catalog.Products;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Data;
using Smartstore.Core.Security;
using Smartstore.Web.Controllers;
using Smartstore.Web.Modelling;
using Smartstore.Web.Models.DataGrid;

namespace BizsolTech.Chatbot.Controllers
{
    [Route("[area]/Business/{action=index}/{id?}")]
    public class BusinessController : AdminController
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            this._businessService = businessService;
        }

        #region Utilities

        private void PrepareBusinessModel(BusinessModel model, BusinessPageEntity entity)
        {
            if (entity == null) { return; }
            else
            {
                model.AdminId = entity.AdminId;
                model.FBPageId = entity.FBPageId;
                model.FBAccessToken = entity.FBAccessToken;
                model.FBStatus = entity.FBStatus;
                model.FBWebhookVerifyToken = entity.FBWebhookVerifyToken;
                model.FBWebhookStatus = entity.FBWebhookStatus;
                model.OpenAPIKey = entity.OpenAPIKey;
                model.OpenAPIStatus = entity.OpenAPIStatus;
                model.AzureOpenAPIKey = entity.AzureOpenAPIKey;
                model.AzureOpenAPIStatus = entity.AzureOpenAPIStatus;
                model.IsActive = entity.IsActive;
                model.CreatedOnUtc = entity.CreatedOnUtc;
                model.UpdatedOnUtc = entity.UpdatedOnUtc;
            }
        }

        #endregion

        [HttpGet]
        public IActionResult Index()
        {
            var model = new BusinessModel();
            return View(model);
        }

        [HttpGet]
        public IActionResult ChatInput()
        {
            var model = new ChatInputModel();
            return PartialView("_ChatInput", model);
        }
        [HttpPost]
        public IActionResult ChatInput(ChatInputModel model,IFormCollection form)
        {
            model.WelcomeMessage = form["WelcomeMessage"];
            model.Instructions = form["Instructions"];
            model.BusinessDescription = form["BusinessDescription"];
            if (model.Instructions.HasValue())
            {
                return RedirectToAction(nameof(ChatConnection));
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChatInputDocumentDelete(int id)
        {
            var pages = await _businessService.GetAllAsync();
            NotifySuccess("Document is deleted.");
            return StatusCode((int)HttpStatusCode.OK);
        }

        [HttpPost]
        [Permission(Permissions.Catalog.Product.EditPicture)]
        public async Task<IActionResult> ChatInputDocumentsAdd(string mediaFileIds, int entityId)
        {
            var ids = mediaFileIds
                .ToIntArray()
                .Distinct()
                .ToArray();

            if (ids.Length == 0)
            {
                throw new ArgumentException("Missing document identifiers.");
            }

            return Json(new
            {
                success = true,
                response = "Document uploaded.",
                message = T("Admin.Product.Picture.Added").JsValue.ToString()
            });
        }

        [HttpGet]
        public IActionResult ChatConnection()
        {
            var model = new ChatConnectionModel();
            return PartialView("_ChatConnection", model);
        }
        [HttpPost]
        public IActionResult ChatConnection(ChatConnectionModel model, IFormCollection form)
        {
            model.AzureOpenAIKey = form["AzureOpenAIKey"];
            model.OpenAIApiKey = form["OpenAIApiKey"];
            if(model.AzureOpenAIKey.HasValue() || model.OpenAIApiKey.HasValue())
            {
                return RedirectToAction(nameof(ChatResponse));
            }
            else
            {
                return RedirectToAction(nameof(ChatConnection));
            }
        }

        [HttpPost]
        public IActionResult TestConnection(IFormCollection form)
        {
            var OpenAIApiKey = form["OpenAIApiKey"];
            var AzureOpenAIKey = form["AzureOpenAIKey"];
            return Json(new { success = true });
        }

        [HttpGet]
        public IActionResult ChatResponse()
        {
            var model = new ChatResponseModel();
            return PartialView("_ChatResponse", model);
        }

        [HttpGet]
        public IActionResult List()
        {
            var model = new BusinessListModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> BusinessList(GridCommand command, BusinessListModel model)
        {
            try
            {
                var businessList = await _businessService.GetAllAsync();

                var rows = businessList.Select(b => new BusinessModel
                {
                    AdminId = b.AdminId,
                    FBPageId = b.FBPageId,
                    FBAccessToken = b.FBAccessToken,
                    FBStatus = b.FBStatus,
                    FBWebhookVerifyToken = b.FBWebhookVerifyToken,
                    FBWebhookStatus = b.FBWebhookStatus,
                    OpenAPIKey = b.OpenAPIKey,
                    OpenAPIStatus = b.OpenAPIStatus,
                    AzureOpenAPIKey = b.AzureOpenAPIKey,
                    AzureOpenAPIStatus = b.AzureOpenAPIStatus,
                    IsActive = b.IsActive,
                    CreatedOnUtc = b.CreatedOnUtc,
                    UpdatedOnUtc = b.UpdatedOnUtc,
                    EditUrl = Url.Action(nameof(Edit), "Business", new { id = b.Id }),
                }).ToList();

                return Ok(new GridModel<BusinessModel>
                {
                    Rows = rows,
                    Total = businessList.Count
                });
            } catch(Exception e)
            {
                NotifyError(e.Message);
                return BadRequest();
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var business = await _businessService.Get(id);

            if (business == null)
            {
                return NotFound();
            }

            var businessModel = new BusinessModel();

            PrepareBusinessModel(businessModel, business);

            return View(businessModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        [SaveChanges(typeof(SmartDbContext), false)]
        public async Task<IActionResult> Edit(BusinessModel model, IFormCollection form)
        {
            var business = await _businessService.Get(model.Id);
            if (business == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    business.AdminId = model.AdminId;
                    business.FBPageId = model.FBPageId;
                    business.FBAccessToken = model.FBAccessToken;
                    business.FBStatus = model.FBStatus;
                    business.FBWebhookVerifyToken = model.FBWebhookVerifyToken;
                    business.FBWebhookStatus = model.FBWebhookStatus;
                    business.OpenAPIKey = model.OpenAPIKey;
                    business.OpenAPIStatus = model.OpenAPIStatus;
                    business.AzureOpenAPIKey = model.AzureOpenAPIKey;
                    business.AzureOpenAPIStatus = model.AzureOpenAPIStatus;
                    business.IsActive = model.IsActive;
                    business.CreatedOnUtc   = model.CreatedOnUtc;
                    business.UpdatedOnUtc = DateTime.UtcNow;

                    await _businessService.Update(business);

                    NotifySuccess("Business page updated!");
                    return RedirectToAction(nameof(Edit), business.Id);
                }
                catch (Exception ex)
                {
                    NotifyError(ex.Message);
                }
            }

            PrepareBusinessModel(model, business);
            return View(model);
        }

    }
}
