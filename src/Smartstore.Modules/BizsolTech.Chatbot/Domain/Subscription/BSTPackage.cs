using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain.Subscription
{
    [Table("BST_Package")]
    public class BSTPackage : BaseEntity
    {
        public string StripeProductID { get; set; }
        public string Name { get; set; }
        public string StripePriceID { get; set; }
        public decimal? Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int BusinessLimit { get; set; }
        public int StorageLimitMB { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
