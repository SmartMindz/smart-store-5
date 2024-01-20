using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using BizsolTech.Chatbot.Domain.Subscription;

namespace BizSol.Chatbot.Services
{
    public interface IBSTSubscriptionService
    {
        Task<BSTSubscription> InsertSubscription(BSTSubscription subscription);
        Task<BSTSubscription> UpdateSubscription(BSTSubscription subscription);
        Task DeleteSubscription(int subscriptionId);
        Task<BSTSubscription> GetSubscriptionByCustomerId(string stripeCustomerId = "", int customerId = 0);
        Task<IList<BSTSubscription>> GetSubscriptionByStripeSubscriptionID(string stripeSubscriptionId);
        Task<IList<BSTSubscription>> GetAllSubscriptions();
        //BSTPackage AddUserFreeSubscription();
        //PackagesLimit CheckPackageUsageLimit();
        //Task DeactiveExpiredSubscriptions();
    }
}