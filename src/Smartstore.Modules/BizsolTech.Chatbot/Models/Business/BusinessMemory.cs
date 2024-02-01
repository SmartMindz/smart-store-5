using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BizsolTech.Chatbot.Models.Business
{
    public class BusinessMemory
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("BusinessId")]
        public int BusinessId { get; set; }
        [JsonProperty("Fact")]
        public string Fact { get; set; } = string.Empty;
        [JsonProperty("Text")]
        public string Text { get; set; } = string.Empty;
        [JsonProperty("Reference")]
        public string Reference { get; set; } = string.Empty;
    }
}
