using BizsolTech.Models.Business;
using Newtonsoft.Json;

namespace BizsolTech.Chatbot.Extensions
{
    public static class ChatbotHelperExtension
    {
        public static string ToJsonString<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static Business ToModel(this Business business, string response)
        {
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);

            return new Business
            {
                Id = jsonResponse.id,
                CollectionName = jsonResponse.collectionName,
                BusinessName = jsonResponse.businessName ?? "",
                FBPageId = jsonResponse.facebookPageId,
                FBAccessToken = jsonResponse.facebookAccessToken ?? "",
                FBAppSecret = jsonResponse.facebookAppSecret ?? "",
                FBStatus = jsonResponse.facebookAccessTokenStatus,
                FBWebhookVerifyToken = jsonResponse.facebookWebhookVerifyToken ?? "",
                FBWebhookStatus = jsonResponse.facebookWebhookVerifyTokenStatus,
                OpenAPIKey = jsonResponse.openAIApiKey ?? "",
                OpenAPIStatus = jsonResponse.openAIKeyStatus,
                AzureOpenAPIKey = "",
                AzureOpenAPIStatus = false,
                WelcomeMessage = jsonResponse.welcomeMessage ?? "",
                Description = jsonResponse.description ?? "",
                Instruction = jsonResponse.instructions ?? ""
            };
        }
    }
}
