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
        public string? Instruction { get; set; }
        public virtual IList<BusinessDocumentEntity> Documents { get; set; } 
        public string? EditUrl { get; set; }
    }
}
