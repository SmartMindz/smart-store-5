using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class ConfigurationModel: ModelBase
    {
        [LocalizedDisplay("Bucket Name")]
        public string? BucketName { get; set; }
        [LocalizedDisplay("Access Key")]
        public string? AccessKey { get; set; }
        [LocalizedDisplay("Secret Key")]
        public string? SecretKey { get; set; }
    }
}
