using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BizsolTech.Chatbot.Models.Business
{
    public class BusinessChat
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("businessId")]
        public int BusinessId { get; set; }
        [JsonProperty("senderId")]
        public long SenderId { get; set; }
        [JsonProperty("question")]
        public string Question { get; set; }
        [JsonProperty("answer")]
        public string Answer { get; set; }
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
