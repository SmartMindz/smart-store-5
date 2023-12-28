using System.Net;
using BizsolTech.Chatbot.Configuration;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Helpers;
using BizsolTech.Chatbot.Models;
using BizsolTech.Chatbot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json.Linq;
using Smartstore;
using Smartstore.ComponentModel;
using Smartstore.Core.Catalog.Products;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Data;
using Smartstore.Core.Security;
using Smartstore.Web.Controllers;
using Smartstore.Web.Modelling;
using Smartstore.Web.Modelling.Settings;
using Smartstore.Web.Models.DataGrid;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BizsolTech.Chatbot.Controllers
{
    public class BusinessController : ModuleController
    {
        private readonly SmartDbContext _db;
        private readonly IBusinessService _businessService;
        private readonly IBusinessDocumentService _businessDocumentService;
        private readonly IBusinessAPIService _businessAPI;
        private readonly IS3StorageService _s3StorageService;
        private readonly IMediaService _mediaService;
        private readonly IMediaTypeResolver _mediaTypeResolver;
        private readonly MediaSettings _mediaSettings;
        private readonly MediaExceptionFactory _exceptionFactory;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public BusinessController(SmartDbContext dbContext, 
            IBusinessService businessService, 
            IBusinessDocumentService businessDocumentService, 
            IBusinessAPIService businessAPIService,
            IS3StorageService s3StorageService,
            IMediaService mediaService, 
            IMediaTypeResolver mediaTypeResolver, 
            MediaSettings mediaSettings, 
            MediaExceptionFactory mediaExceptionFactory, 
            IHttpContextAccessor httpContextAccessor)
        {
            _db = dbContext;
            _businessService = businessService;
            _businessDocumentService = businessDocumentService;
            _businessAPI = businessAPIService;
            _s3StorageService = s3StorageService;
            _mediaService = mediaService;
            _mediaTypeResolver = mediaTypeResolver;
            _mediaSettings = mediaSettings;
            _exceptionFactory = mediaExceptionFactory;
            _httpContextAccessor = httpContextAccessor;
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

            var session = _httpContextAccessor.HttpContext?.Session;

            if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
            {
                return View(inputModel);
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ChatInput()
        {
            var model = new BusinessModel();
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
            {
                return PartialView("_ChatInput", inputModel);
            }
            else
            {
                return PartialView("_ChatInput", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChatInput(BusinessModel model, IFormCollection form)
        {
            var numFiles = Request.Form.Files.Count;
            try
            {
                if (ModelState.IsValid)
                {
                    var businessPage = new BusinessPageEntity
                    {
                        BusinessName = model.BusinessName,
                        WelcomeMessage = model.WelcomeMessage,
                        Instruction = model.Instruction ?? "ins",
                        Description = model.Description
                    };
                    await _businessService.Insert(businessPage);

                    //var response = await _businessAPI.AddBusiness(businessPage);
                    var response = @"{
                            ""id"": 5,
                            ""collectionName"": ""PageUzair"",
                            ""facebookPageId"": """",
                            ""facebookAccessToken"": """",
                            ""facebookAccessTokenStatus"": false,
                            ""facebookWebhookVerifyToken"": """",
                            ""facebookWebhookVerifyTokenStatus"": false,
                            ""openAIApiKey"": """",
                            ""openAIKeyStatus"": false,
                            ""textEmbeddingModel"": ""text-embedding-ada-002"",
                            ""textCompletionModel"": ""gpt-3.5-turbo""
                        }";
                    if (string.IsNullOrEmpty(response))
                    {
                        throw new Exception($"Server error: Failed to create business. '{model.BusinessName}'");
                    }
                    var jsonResponse = JObject.Parse(response);
                    businessPage.BusinessId = jsonResponse["id"].Value<string>();
                    await _businessService.Update(businessPage);

                    for (var i = 0; i < numFiles; ++i)
                    {
                        var file = Request.Form.Files[i];
                        using var stream = file.OpenReadStream();

                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var extension = Path.GetExtension(fileName);
                        var fileSize = stream.Length;
                        var byteArray = stream.ToByteArray();
                        var document = new BusinessDocumentEntity
                        {
                            BusinessPageId = businessPage.Id,
                            Name = fileName,
                            Size = int.Parse(fileSize.ToString()),
                            UpdateRequired = true,
                            FileUrl = "fileURL",
                            Extension = extension,
                            CRC = CRC32Calculator.CalculateCRC32FromFile(file).ToString(),
                            OpenAIFileID = "OPENAIFILEID"
                        };

                        await _businessDocumentService.Insert(document);
                        model.Id = businessPage.Id;
                        model.Documents.Add(document);
                        var success = await _s3StorageService.AddDocument(document, businessPage.BusinessName, fileName, "prevKey", byteArray, false);
                    }

                    var sessionStored = _httpContextAccessor.HttpContext?.Session.TrySetObject<BusinessModel>("BusinessInput", model);

                    string redirectUrl = Url.Action(nameof(ChatConnection));
                    return Json(new { success = true, redirectUrl = redirectUrl });
                }
                else
                {
                    return Json(new { success = false, redirectUrl = Url.Action(nameof(ChatInput)) });
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ChatConnection()
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
            {
                return PartialView("_ChatConnection", inputModel);
            }
            else
            {
                NotifyError("Input data from session is missing!");
                return PartialView("_ChatConnection", new BusinessModel());
            }

        }

        [HttpPost]
        public async  Task<IActionResult> TestOpenAIConnection(BusinessModel model,IFormCollection form)
        {
            var success = await _businessAPI.VerifyOpenAICredentials(model.Id, model.OpenAPIKey);
            //store params
            var sessionStored = _httpContextAccessor.HttpContext?.Session.TrySetObject<BusinessModel>("BusinessInput", model);
            return Json(new { success = success });
        }

        [HttpPost]
        public async Task<IActionResult> TestFacebookPageConnection(BusinessModel model, IFormCollection form)
        {
            var success = await _businessAPI.VerifyFacebookCredentials(model.Id, model.FBPageId.ToString(), model.FBAccessToken);
            //store params
            var sessionStored = _httpContextAccessor.HttpContext?.Session.TrySetObject<BusinessModel>("BusinessInput", model);
            return Json(new { success = success });
        }

        [HttpGet]
        public IActionResult ChatResponse()
        {
            var model = new BusinessModel();
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
            {
                return PartialView("_ChatResponse", inputModel);
            }
            else
            {
                return PartialView("_ChatResponse", model);
            }
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
            }
            catch (Exception e)
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
                    business.CreatedOnUtc = model.CreatedOnUtc;
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
