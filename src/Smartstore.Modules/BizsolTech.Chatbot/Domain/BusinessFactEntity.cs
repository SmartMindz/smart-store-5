using System;
using System.ComponentModel.DataAnnotations.Schema;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessFact")]
    public class BusinessFactEntity : BaseEntity, IAuditable
    {
        public int BusinessPageId { get; set; }
        public string? Fact { get; set; }
        public string? Text { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
