using System.ComponentModel.DataAnnotations;
using Smartstore.Core.Content.Media;
using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class ChatInputModel : EntityModelBase
    {
        public ChatInputModel()
        {
            Documents = new List<ChatDocumentModel>();
        }

        [LocalizedDisplay("Chatbot.Fields.WelcomeMessage")]
        public string? WelcomeMessage { get; set; }
        [LocalizedDisplay("Chatbot.Fields.BusinessDescription")]
        public string? BusinessDescription { get; set; }
        [LocalizedDisplay("Chatbot.Fields.Instructions")]
        public string? Instructions { get; set; }

        [UIHint("Media"), AdditionalMetadata("album", "catalog"), AdditionalMetadata("typeFilter", "image,video")]
        [LocalizedDisplay("Chatbot.Fields.Documents")]
        public int DocumentId { get; set; }
        public List<ChatDocumentModel> Documents { get; set; }
    }

    public class ChatDocumentModel : EntityModelBase, IMediaFile
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public int MediaFileId { get; set; }
        public virtual MediaFile MediaFile { get; set; }
        public int DisplayOrder { get; set; }
    }
}
