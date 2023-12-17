using Smartstore.Core.Configuration;

namespace BizsolTech.Chatbot.Configuration
{
    public class ChatbotSettings : ISettings
    {
        public string? BucketName { get; set; }
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
    }
}
