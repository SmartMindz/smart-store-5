using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class ChatStepModel : ModelBase
    {
        public ChatProgressStep ChatProgressStep { get; set; }
    }

    public enum ChatProgressStep
    {
        Initial,
        Connection,
        Response
    }
}
