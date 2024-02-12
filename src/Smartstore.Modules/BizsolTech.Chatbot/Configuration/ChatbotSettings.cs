using Smartstore.Core.Configuration;

namespace BizsolTech.Chatbot.Configuration
{
    public class ChatbotSettings : ISettings
    {
        public string BucketName { get; set; } = string.Empty;
        public string AccessKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string FacebookCallbackUrlBase { get; set; } = string.Empty;
        public string ChatPrompt { get; set; } = string.Empty;
    }
}
