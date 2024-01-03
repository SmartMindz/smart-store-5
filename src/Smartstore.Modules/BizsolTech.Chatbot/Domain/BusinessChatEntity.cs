using System;
using System.ComponentModel.DataAnnotations.Schema;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessChat")]
    public class BusinessChatEntity : BaseEntity, IAuditable
    {
        public int BusinessPageId { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
