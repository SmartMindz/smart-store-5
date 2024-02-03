using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smartstore.Web.Modelling;

namespace BizsolTech.Chatbot.Models
{
    public class BusinessChatModel : ModelBase
    {
        public int Id { get; set; }
        public long BusinessId { get; set; }
        public long SenderId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public string BusinessName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
