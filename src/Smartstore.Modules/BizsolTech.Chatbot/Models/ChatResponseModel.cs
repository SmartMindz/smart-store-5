using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class ChatResponseModel: EntityModelBase
    {
        public string? FacebookPageId { get; set; }
        public string? Response { get; set; }
        public bool Status { get; set; }
    }
}
