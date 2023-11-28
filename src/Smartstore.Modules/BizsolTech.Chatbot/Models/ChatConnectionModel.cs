using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class ChatConnectionModel: EntityModelBase
    {
        public string? OpenAIApiKey { get; set; }
        public string? AzureOpenAIKey { get; set; }
        public bool KeyStatus { get; set; } = false;
    }
}
