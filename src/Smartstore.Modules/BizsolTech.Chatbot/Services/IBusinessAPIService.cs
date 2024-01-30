using Smartstore;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Extensions;
using BizsolTech.Chatbot.Helpers;
using BizsolTech.Models.Business;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NUglify.JavaScript;
using Microsoft.Extensions.Logging;
using System;

namespace BizsolTech.Chatbot.Services
{
    public interface IBusinessAPIService
    {
        Task<bool> VerifyFacebookCredentials(int businessId, string pageId, string accessToken);
        Task<bool> VerifyOpenAICredentials(int businessId, string apiKey);
        Task<string> AddDocumentContent(int businessId, string content);
        Task<bool> DeleteDocumentContent(int businessId, string semanticRef);

        Task<List<Business>> GetBusinessAll();
        Task<Business> GetBusiness(int businessId);
        Task<Business> AddBusiness(Business business);
        Task<bool> UpdateBusiness(Business business);
    }

    public class BusinessAPIService : IBusinessAPIService
    {
        private const string baseUrl = "https://chatbot.bizsoltech.com";
        private readonly CBApiRequestHandler _apiHandler;

        public BusinessAPIService(IHttpClientFactory httpClientFactory)
        {
            _apiHandler = new CBApiRequestHandler(httpClientFactory, baseUrl);
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public async Task<Business> GetBusiness(int businessId)
        {
            try {
                var apiUrl = "/api/Business/Get";
                Dictionary<string, string> parameters = new Dictionary<string, string>() {
                    { "businessId", $"{businessId}" }
                };

                var response = await _apiHandler.GetAsync(apiUrl, parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseData = response.ResponseContent;
                    Business business = new Business() { BusinessName = ""};
                    return business.ToModel(responseData);
                }
                else
                {
                    Logger.Error($"GetBusiness: API response:{response.StatusCode}");
                    return null;
                }
            }
            catch(Exception e) {
                Logger.Error(e);
                return null;
            }
        }

        public async Task<List<Business>> GetBusinessAll()
        {
            try
            {
                var apiUrl = "/api/Business/GetAll";

                var response = await _apiHandler.GetAsync(apiUrl);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseData = response.ResponseContent;
                    List<Business> businessList = JsonConvert.DeserializeObject<List<Business>>(responseData);
                    return businessList;
                }
                else
                {
                    Logger.Error($"GetBusinessAll: API response:{response.StatusCode}");
                    return new List<Business>();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new List<Business>();
            }
        }

        public async Task<Business> AddBusiness(Business business)
        {
            try
            {
                var apiUrl = "/api/Business/Insert";

                var requestBody = new
                {
                    collectionName = business.CollectionName, //required
                    businessName = business.BusinessName,
                    description = business.Description,
                    instructions = business.Instruction,
                    welcomeMessage = business.WelcomeMessage
                };

                var response = await _apiHandler.PostAsync(apiUrl, requestBody);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Business model = new Business() { BusinessName = string.Empty};
                    return model.ToModel(response.ResponseContent);
                }
                else
                {
                    Logger.Error($"AddBusiness: API response:{response.StatusCode}");
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateBusiness(Business business)
        {
            try
            {
                var apiUrl = "/api/Business/Update";

                Dictionary<string, string> parameters = new Dictionary<string, string>() {
                    { "id", $"{business.Id}" }
                };
                var requestBody = new
                {
                    businessName = business.BusinessName,
                    description = business.Description,
                    instructions = business.Instruction,
                    welcomeMessage = business.WelcomeMessage
                };

                var response = await _apiHandler.PutAsync(apiUrl, requestBody, parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    Logger.Error($"UpdateBusiness: API response:{response.StatusCode}");
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> AddDocumentContent(int businessId, string documentContent)
        {
            try
            {
                var apiUrl = "/api/Business/AddMemory";

                var requestBody = new
                {
                    fact = "fact",
                    text = documentContent
                };

                //parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>() {
                    { "businessId", $"{businessId}" }
                };

                var response = await _apiHandler.PostAsync(apiUrl, requestBody, parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseData = response.ResponseContent;
                    return responseData;
                }
                else
                {
                    Logger.Error($"AddDocumentContent: API response:{response.StatusCode}");
                    return string.Empty;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return string.Empty;
            }
        }

        public async Task<bool> DeleteDocumentContent(int businessId, string semanticRefKey)
        {
            try
            {
                var apiUrl = "/api/Business/RemoveMemory";

                //parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>() {
                    { "businessId", $"{businessId}" },
                    { "fact", $"{semanticRefKey}" }
                };

                var response = await _apiHandler.GetAsync(apiUrl, parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    Logger.Error($"DeleteDocumentContent: API response:{response.StatusCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public async Task<bool> VerifyFacebookCredentials(int businessId, string pageId, string accessToken)
        {
            try
            {
                var apiUrl = "/api/Business/VerifyFacebookCredentials";

                var requestBody = new
                {
                    pageId = pageId,
                    accessToken = accessToken
                };

                //parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>() {
                    { "businessId", $"{businessId}" }
                };

                var response = await _apiHandler.PostAsync(apiUrl, requestBody, parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseData = response.ResponseContent;
                    return true;
                }
                else
                {
                    Logger.Error($"VerifyFacebookCredentials: API response:{response.StatusCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public async Task<bool> VerifyOpenAICredentials(int businessId, string apiKey)
        {
            try
            {
                var apiUrl = "/api/Business/VerifyOpenAICredentials";

                var requestBody = new
                {
                    apiKey = apiKey
                };

                //parameters
                Dictionary<string, string> parameters = new Dictionary<string, string>() {
                    { "businessId", $"{businessId}" }
                };

                var response = await _apiHandler.PostAsync(apiUrl, requestBody, parameters);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var responseData = response.ResponseContent;
                    return true;
                }
                else
                {
                    Logger.Error($"VerifyOpenAICredentials: API response:{response.StatusCode}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }


    }
}
