using System;
using System.ComponentModel.DataAnnotations.Schema;
using Smartstore;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessPage")]
    public class BusinessPageEntity : BaseEntity
    {
        public string CollectionName { get; set; } = string.Empty;
        public string FacebookPageId { get; set; } = string.Empty;
        public string FacebookAccessToken { get; set; } = string.Empty;
        public bool FacebookAccessTokenStatus { get; set; }
        public string FacebookWebhookVerifyToken { get; set; } = string.Empty;
        public bool FacebookWebhookVerifyTokenStatus { get; set; }
        public string OpenAIApiKey { get; set; } = string.Empty;
        public bool OpenAIKeyStatus { get; set; }
        public string TextEmbeddingModel { get; set; } = string.Empty;
        public string TextCompletionModel { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string WelcomeMessage { get; set; } = string.Empty;
    }
}
