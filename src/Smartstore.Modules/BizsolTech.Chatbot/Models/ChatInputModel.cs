using System.ComponentModel.DataAnnotations;
using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class ChatInputModel : EntityModelBase
    {
        [LocalizedDisplay("Chatbot.Fields.WelcomeMessage")]
        public string? WelcomeMessage { get; set; }
        [LocalizedDisplay("Chatbot.Fields.BusinessDescription")]
        public string? BusinessDescription { get; set; }
        [LocalizedDisplay("Chatbot.Fields.Instructions")]
        public string? Instructions { get; set; }

        [UIHint("Media"), AdditionalMetadata("album", "catalog"), AdditionalMetadata("typeFilter", "image,video")]
        [LocalizedDisplay("Chatbot.Fields.Documents")]
        public int DocumentId { get; set; }
        public List<IFormFile> Documents { get; set; } = new List<IFormFile>();
    }
}
