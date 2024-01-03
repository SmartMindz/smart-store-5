using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Smartstore;
using Smartstore.Domain;

namespace BizsolTech.Models.Business
{
    public class Business
    {
        [JsonProperty("id")]
        public int Id { get; set; } //id from vector db

        [JsonProperty("collectionName")]
        public string? CollectionName { get; set; }

        [JsonProperty("businessName")]
        public required string BusinessName { get; set; }

        [JsonProperty("facebookPageId")]
        public string? FBPageId { get; set; }

        [JsonProperty("facebookAccessToken")]
        public string? FBAccessToken { get; set; }

        [JsonProperty("facebookAccessTokenStatus")]
        public bool FBStatus { get; set; }

        [JsonProperty("facebookWebhookVerifyToken")]
        public string? FBWebhookVerifyToken { get; set; }

        [JsonProperty("facebookWebhookVerifyTokenStatus")]
        public bool FBWebhookStatus { get; set; }

        [JsonProperty("openAIApiKey")]
        public string? OpenAPIKey { get; set; }

        [JsonProperty("openAIKeyStatus")]
        public bool OpenAPIStatus { get; set; }
        public string? AzureOpenAPIKey { get; set; }
        public bool AzureOpenAPIStatus { get; set; }

        [JsonProperty("welcomeMessage")]
        public string? WelcomeMessage { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("instructions")]
        public string? Instruction { get; set; }
    }
}
