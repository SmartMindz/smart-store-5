using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain
{
    [Table("BusinessPageMapping")]
    public class BusinessPageMappingEntity : BaseEntity
    {
        public int EntityId { get; set; }
        public int BusinessId { get; set; }
        public string EntityName { get; set; }
    }
}
