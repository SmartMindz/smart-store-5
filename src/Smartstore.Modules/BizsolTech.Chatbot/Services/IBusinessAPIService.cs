using System.Text;
using Azure.Core;
using BizsolTech.Chatbot.Domain;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NUglify.JavaScript;

namespace BizsolTech.Chatbot.Services
{
    public interface IBusinessAPIService
    {
        Task<bool> VerifyFacebookCredentials(int businessId, string pageId, string accessToken);
        Task<bool> VerifyOpenAICredentials(int businessId, string apiKey);
        Task<bool> AddDocumentContent(int businessId, string content);

        Task<string> AddBusiness(BusinessPageEntity businessPage);
    }

    public class BusinessAPIService : IBusinessAPIService
    {
        private const string baseUrl = "https://chatbot.bizsoltech.com";
        private readonly IHttpClientFactory _httpClientFactory;

        public BusinessAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> AddBusiness(BusinessPageEntity businessPage)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = baseUrl + "/api/Business/Insert";

                var requestBody = new
                {
                    collectionName = businessPage.BusinessName //required
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return responseData;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> AddDocumentContent(int businessId, string documentContent)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = baseUrl + "/api/Business/AddMemory";

                var requestBody = new
                {
                    fact = "fact",
                    text = documentContent
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                businessId = 1; //fixed temporary
                apiUrl += $"?businessId={businessId}";

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> VerifyFacebookCredentials(int businessId, string pageId, string accessToken)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = baseUrl + "/api/Business/VerifyFacebookCredentials";

                var requestBody = new
                {
                    pageId = pageId,
                    accessToken = accessToken
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                businessId = 1; //fixed temporary
                apiUrl += $"?businessId={businessId}";

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task<bool> VerifyOpenAICredentials(int businessId, string apiKey)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var apiUrl = baseUrl + "/api/Business/VerifyOpenAICredentials";

                var requestBody = new
                {
                    apiKey = apiKey
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                businessId = 1; //fixed temporary
                apiUrl += $"?businessId={businessId}";

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }


    }
}
