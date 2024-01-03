using System;
using System.ComponentModel.DataAnnotations.Schema;
using Smartstore;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessPage")]
    public class BusinessPageEntity : BaseEntity, IAuditable,IActivatable
    {
        public required string BusinessName { get; set; }
        public string? BusinessId { get; set; } //id from vector db
        public int AdminId { get; set; }
        public long? FBPageId { get; set; }
        public string? FBAccessToken { get; set; }
        public bool FBStatus { get; set; }
        public string? FBWebhookVerifyToken { get; set; }
        public bool FBWebhookStatus { get; set; }
        public string? OpenAPIKey { get; set; }
        public bool OpenAPIStatus { get; set; }
        public string? AzureOpenAPIKey { get; set; }
        public bool AzureOpenAPIStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public string? WelcomeMessage { get; set; }
        public string? Description { get; set; }
        public required string Instruction { get; set; }
    }
}
