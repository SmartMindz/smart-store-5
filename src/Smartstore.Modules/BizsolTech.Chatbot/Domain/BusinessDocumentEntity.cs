using Smartstore.Domain;
using Smartstore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessDocument")]
    public class BusinessDocumentEntity : BaseEntity, ISoftDeletable, IActivatable, IAuditable
    {
        public int BusinessPageId { get; set; }
        public required string Name { get; set; }
        public required string FileUrl { get; set; }
        public required string CRC { get; set; }
        public int? Size { get; set; }
        public required string Extension { get; set; }
        public required string OpenAIFileID { get; set; } //fileid when uploaded to OpenAI      
        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public bool Deleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
