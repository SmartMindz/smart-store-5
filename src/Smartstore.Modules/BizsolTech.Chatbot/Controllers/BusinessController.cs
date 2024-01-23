using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BizsolTech.Chatbot.Configuration;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Helpers;
using BizsolTech.Chatbot.Models;
using BizsolTech.Chatbot.Services;
using BizsolTech.Models.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Smartstore;
using Smartstore.ComponentModel;
using Smartstore.Core;
using Smartstore.Core.Catalog.Products;
using Smartstore.Core.Content.Media;
using Smartstore.Core.Data;
using Smartstore.Core.Identity;
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
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;

        public BusinessController(SmartDbContext dbContext,
            IBusinessService businessService,
            IBusinessDocumentService businessDocumentService,
            IBusinessAPIService businessAPIService,
            IS3StorageService s3StorageService,
            IMediaService mediaService,
            IMediaTypeResolver mediaTypeResolver,
            MediaSettings mediaSettings,
            MediaExceptionFactory mediaExceptionFactory,
            IHttpContextAccessor httpContextAccessor,
            ICustomerService customerService,
            IWorkContext workContext)
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
            _customerService = customerService;
            _workContext = workContext;
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
        public async Task<IActionResult> Index()
        {
            var model = new BusinessModel();

            var customer = _workContext.CurrentCustomer;
            var businessAll = await _businessAPI.GetBusinessAll();
            var businessMapping = await _businessService.GetBusinessMappings();
            if (businessMapping.Count > 0)
            {
                var businessId = businessMapping.FirstOrDefault(m => m.EntityId.Equals(customer.Id) && m.EntityName.Equals(nameof(Customer)) && businessAll.Exists(b => b.Id.Equals(m.BusinessId)));
                if (businessId != null)
                {
                    var business = businessAll.FirstOrDefault(b => b.Id.Equals(customer.Id));

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
        public async Task<IActionResult> ChatInput()
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
                var businessAll = await _businessAPI.GetBusinessAll();
                if (ModelState.IsValid)
                {
                    var businessPage = new Business
                    {
                        BusinessName = model.BusinessName,
                        WelcomeMessage = model.WelcomeMessage,
                        Instruction = model.Instruction,
                        Description = model.Description,
                        CollectionName = "Collection_" + model.BusinessName,
                    };

                    if (businessAll.Any(b => b.BusinessName == model.BusinessName))
                    {
                        throw new Exception($"Server error: Business with name '{model.BusinessName}' already exists.");
                    }

                    var response = await _businessAPI.AddBusiness(businessPage);
                    if (response == null)
                    {
                        throw new Exception($"Server error: Failed to create business. '{model.BusinessName}'");
                    }

                    var businessMapping = new BusinessPageMappingEntity();
                    businessMapping.EntityId = _workContext.CurrentCustomer.Id;
                    businessMapping.EntityName = nameof(Customer);
                    businessMapping.BusinessId = response.Id; //businessId
                    await _businessService.InsertBusinessMapping(businessMapping);

                    var businessDocs = await _businessDocumentService.GetAllAsync();
                    for (var i = 0; i < numFiles; ++i)
                    {
                        var file = Request.Form.Files[i];
                        using var stream = file.OpenReadStream();

                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        var extension = Path.GetExtension(file.FileName);
                        var fileSize = stream.Length;
                        var byteArray = stream.ToByteArray();

                        var crc = CRC32Calculator.CalculateCRC32FromFile(file);
                        var document = new BusinessDocumentEntity
                        {
                            BusinessPageId = response.Id,
                            Name = fileName,
                            Size = int.Parse(fileSize.ToString()),
                            UpdateRequired = true,
                            FileUrl = "fileURL",
                            Extension = extension,
                            CRC = crc.ToString(),
                            OpenAIFileID = "OPENAIFILEID"
                        };

                        if (businessDocs.Any(d => d.Name == fileName && d.Extension == extension))
                        {
                            var existingFile = businessDocs.FirstOrDefault(d => d.Name == fileName && d.Extension == extension);
                            var existingCRC = uint.Parse(existingFile?.CRC);
                            if (crc != existingCRC)
                            {
                                existingFile.CRC = crc.ToString();
                                existingFile.Size = int.Parse(fileSize.ToString());
                                existingFile.UpdateRequired = true;
                                await _businessDocumentService.Update(existingFile);
                            }
                        }
                        else
                        {
                            await _businessDocumentService.Insert(document);
                        }
                        model.Id = response.Id;
                        model.Documents.Add(document);
                        var success = await _s3StorageService.AddDocument(document, response.BusinessName, fileName, "prevKey", byteArray, false);
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
        public async Task<IActionResult> TestOpenAIConnection(BusinessModel model, IFormCollection form)
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
