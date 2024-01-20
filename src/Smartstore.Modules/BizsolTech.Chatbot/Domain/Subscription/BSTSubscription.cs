using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smartstore.Domain;

namespace BizsolTech.Chatbot.Domain.Subscription
{
    [Table("BST_Subscription")]
    public class BSTSubscription : BaseEntity
    {
        public int CustomerId { get; set; }
        public long? Subtotal { get; set; }
        public long? Total { get; set; }
        public long? Discount { get; set; }
        public long? Tax { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string PaymentStatus { get; set; }
        public int PackageId { get; set; }
        public virtual BSTPackage Package { get; set; }
        public string StripeSubscriptionID { get; set; }
        public string StripeCustomerID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime? CurrentPeriodEnd { get; set; }
        public DateTime? EndedAt { get; set; } // Nullable for ongoing subscriptions
        public string Status { get; set; }

        public DateTime? CancelAt { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public DateTime? CanceledAt { get; set; }

        public string CancelComment { get; set; }
        public string CancelFeedback { get; set; }
        public string CancelReason { get; set; }


        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextBillingDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsRecurring { get; set; }
        public string Frequency { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
    }
}
