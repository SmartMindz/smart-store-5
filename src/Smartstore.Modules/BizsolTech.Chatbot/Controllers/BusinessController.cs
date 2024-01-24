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
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
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

        private void PrepareBusinessModel(BusinessModel model, Business entity)
        {
            if (entity == null) { return; }
            else
            {
                model.Id = entity.Id;
                //model.AdminId = entity.AdminId;
                model.BusinessName = entity.BusinessName;
                model.Description = entity.Description;
                model.Instruction = entity.Instruction;
                model.CollectionName = entity.CollectionName;
                model.FBPageId = long.Parse(entity.FBPageId);
                model.FBAccessToken = entity.FBAccessToken;
                model.FBStatus = entity.FBStatus;
                model.FBWebhookVerifyToken = entity.FBWebhookVerifyToken;
                model.FBWebhookStatus = entity.FBWebhookStatus;
                model.OpenAPIKey = entity.OpenAPIKey;
                model.OpenAPIStatus = entity.OpenAPIStatus;
                model.AzureOpenAPIKey = entity.AzureOpenAPIKey;
                model.AzureOpenAPIStatus = entity.AzureOpenAPIStatus;
                model.IsActive = true;
                //model.CreatedOnUtc = entity.CreatedOnUtc;
                //model.UpdatedOnUtc = entity.UpdatedOnUtc;
            }
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> Index(int? id)
        {
            var model = new BusinessModel();
            var session = _httpContextAccessor.HttpContext?.Session;
            if (id.HasValue && id != null)
            {
                var business = await _businessAPI.GetBusiness(id.Value);
                PrepareBusinessModel(model, business);
                return View(model);
            }
            else if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
            {
                return View(inputModel);
            }
            else
            {
                return View(new BusinessModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChatInput(int? id)
        {
            var model = new BusinessModel();
            var session = _httpContextAccessor.HttpContext?.Session;
            if (id.HasValue)
            {
                var business = await _businessAPI.GetBusiness(id.Value);
                PrepareBusinessModel(model, business);
                return PartialView("_ChatInput", model);
            }
            else if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
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
                var customer = _workContext.CurrentCustomer;
                var customerBusinesses = await _businessService.GetCustomerBusinessAll(customer);

                Business _response;
                if (ModelState.IsValid)
                {
                    string bName = model.BusinessName.Replace(" ", "_"); //xy_z

                    if (model.Id != 0 && customerBusinesses.Exists(b => b.Id.Equals(model.Id)))
                    {
                        var businessPage = customerBusinesses.FirstOrDefault(b => b.Id == model.Id);
                        businessPage.BusinessName = bName;
                        businessPage.WelcomeMessage = model.WelcomeMessage;
                        businessPage.Instruction = model.Instruction;
                        businessPage.Description = model.Description;

                        //update
                        var success = await _businessAPI.UpdateBusiness(businessPage);
                        if (!success)
                        {
                            throw new Exception($"Server error: Failed to update business. '{model.BusinessName}'");
                        }
                        _response = businessPage;
                    }
                    else
                    {
                        if (customerBusinesses.Any(b => b.BusinessName == bName))
                        {
                            throw new Exception($"Server error: Business with name '{model.BusinessName}' already exists.");
                        }

                        var businessPage = new Business
                        {
                            BusinessName = bName,
                            WelcomeMessage = model.WelcomeMessage,
                            Instruction = model.Instruction,
                            Description = model.Description,
                            CollectionName = "Collection_" + bName,
                        };

                        _response = await _businessAPI.AddBusiness(businessPage);
                        if (_response == null)
                        {
                            throw new Exception($"Server error: Failed to create business. '{model.BusinessName}'");
                        }

                        var businessMapping = new BusinessPageMappingEntity();
                        businessMapping.EntityId = _workContext.CurrentCustomer.Id;
                        businessMapping.EntityName = nameof(Customer);
                        businessMapping.BusinessId = _response.Id; //businessId
                        await _businessService.InsertBusinessMapping(businessMapping);
                    }

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
                            BusinessPageId = _response.Id,
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
                        model.Id = _response.Id;
                        model.Documents.Add(document);
                        var success = await _s3StorageService.AddDocument(document, _response.BusinessName, fileName, "prevKey", byteArray, false);
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
        public async Task<IActionResult> ChatConnection(int? id)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var model = new BusinessModel();
            if (id.HasValue)
            {
                var business = await _businessAPI.GetBusiness(id.Value);
                PrepareBusinessModel(model, business);
                return PartialView("_ChatConnection", model);
            }
            else if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
            {
                return PartialView("_ChatConnection", inputModel);
            }
            else
            {
                NotifyError("Input data from session is missing!");
                return PartialView("_ChatConnection", model);
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
        public async Task<IActionResult> ChatResponse(int? id)
        {
            var model = new BusinessModel();
            var session = _httpContextAccessor.HttpContext?.Session;

            if (id.HasValue)
            {
                var business = await _businessAPI.GetBusiness(id.Value);
                PrepareBusinessModel(model, business);
                return PartialView("_ChatResponse", model);
            }
            else if (session != null && session.TryGetObject<BusinessModel>("BusinessInput", out var inputModel))
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
                var customer = _workContext.CurrentCustomer;
                var businessList = await _businessService.GetCustomerBusinessAll(customer);

                var rows = businessList.Select(b =>
                new BusinessModel
                {
                    Id = b.Id,
                    AdminId = 0,
                    BusinessName = b.BusinessName,
                    CollectionName = b.CollectionName,
                    Description = b.Description,
                    Instruction = b.Instruction,
                    FBPageId = long.Parse(b.FBPageId),
                    FBAccessToken = b.FBAccessToken,
                    FBStatus = b.FBStatus,
                    FBWebhookVerifyToken = b.FBWebhookVerifyToken,
                    FBWebhookStatus = b.FBWebhookStatus,
                    OpenAPIKey = b.OpenAPIKey,
                    OpenAPIStatus = b.OpenAPIStatus,
                    AzureOpenAPIKey = b.AzureOpenAPIKey,
                    AzureOpenAPIStatus = b.AzureOpenAPIStatus,
                    IsActive = true,
                    CreatedOnUtc = DateTime.Now.ToUniversalTime(),
                    UpdatedOnUtc = DateTime.Now.ToUniversalTime(),
                    EditUrl = Url.Action(nameof(Index), "Business", new { id = b.Id })
                }
                ).ToList();

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
            var business = await _businessAPI.GetBusiness(id);

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
            var business = await _businessAPI.GetBusiness(model.Id);
            if (business == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    business.FBPageId = model.FBPageId.ToString();
                    business.FBAccessToken = model.FBAccessToken;
                    business.FBStatus = model.FBStatus;
                    business.FBWebhookVerifyToken = model.FBWebhookVerifyToken;
                    business.FBWebhookStatus = model.FBWebhookStatus;
                    business.OpenAPIKey = model.OpenAPIKey;
                    business.OpenAPIStatus = model.OpenAPIStatus;
                    business.AzureOpenAPIKey = model.AzureOpenAPIKey;
                    business.AzureOpenAPIStatus = model.AzureOpenAPIStatus;

                    await _businessAPI.UpdateBusiness(business);

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
