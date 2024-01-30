using System;
using System.ComponentModel.DataAnnotations.Schema;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessChat")]
    public class BusinessChatEntity : BaseEntity
    {
        public long BusinessId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
