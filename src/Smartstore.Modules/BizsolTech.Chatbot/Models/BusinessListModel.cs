using System;
using System.Collections.Generic;
using BizsolTech.Chatbot.Domain;
using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class BusinessListModel:ModelBase
    {
    }
    public class BusinessModel: TabbableModel
    {
        public BusinessModel()
        {
            Documents = new List<BusinessDocumentEntity>();
        }
        public int AdminId { get; set; }
        [LocalizedDisplay("Chatbot.Fields.FBPageId")]
        public long? FBPageId { get; set; }
        [LocalizedDisplay("Chatbot.Fields.FBAccessToken")]
        public string FBAccessToken { get; set; }
        public bool FBStatus { get; set; }
        public string FBWebhookVerifyToken { get; set; }
        public bool FBWebhookStatus { get; set; }
        [LocalizedDisplay("Chatbot.Fields.OpenAPIKey")]
        public string OpenAPIKey { get; set; }
        public bool OpenAPIStatus { get; set; }
        [LocalizedDisplay("Chatbot.Fields.AzureOpenAPIKey")]
        public string AzureOpenAPIKey { get; set; }
        public bool AzureOpenAPIStatus { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        [LocalizedDisplay("Chatbot.Fields.BusinessName")]
        public string BusinessId { get; set; }
        public string BusinessName { get; set; }
        public string CollectionName { get; set; }
        [LocalizedDisplay("Chatbot.Fields.WelcomeMessage")]
        public string WelcomeMessage { get; set; }

        [LocalizedDisplay("Chatbot.Fields.BusinessDescription")]
        public string Description { get; set; }

        [LocalizedDisplay("Chatbot.Fields.Instructions")]
        public string Instruction { get; set; }

        [LocalizedDisplay("Chatbot.Fields.Documents")]
        public virtual IList<BusinessDocumentEntity> Documents { get; set; } 
        public string EditUrl { get; set; }
    }
}
