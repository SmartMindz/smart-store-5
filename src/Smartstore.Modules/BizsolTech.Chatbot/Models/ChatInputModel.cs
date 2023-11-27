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
        [LocalizedDisplay("Chatbot.Fields.Documents")]
        public IFormFileCollection? Documents { get; set; }
    }
}
