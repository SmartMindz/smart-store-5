using BizsolTech.Chatbot.Domain.Subscription;
using BizsolTech.Chatbot.Extensions;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core;
using Smartstore.Core.Common.Services;
using Smartstore.Core.Data;
using Smartstore.Core.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BizSol.Chatbot.Services
{
    public class BSTSubscriptionService : IBSTSubscriptionService
    {
        private readonly SmartDbContext _db;
        private readonly ICommonServices _service;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        public BSTSubscriptionService(SmartDbContext db, 
            
            ICommonServices service, IGenericAttributeService genericAttributeService, IWorkContext workContext, 
            IStoreContext storeContext)
        {
            _db = db;
            _service = service;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _storeContext = storeContext;
        }

        public async Task<BSTSubscription> InsertSubscription(BSTSubscription subscription)
        {
            subscription.CreatedOnUtc = DateTime.UtcNow;
            await _db.BSTSubscriptions().AddAsync(subscription);
            await _db.SaveChangesAsync();
            return subscription;
        }

        public async Task<BSTSubscription> UpdateSubscription(BSTSubscription subscription)
        {
            subscription.UpdatedOnUtc = DateTime.UtcNow;
            await _db.BSTSubscriptions().AddAsync(subscription);
            await _db.SaveChangesAsync();
            return subscription;
        }

        public async Task DeleteSubscription(int subscriptionId)
        {
            var subscription = _db.BSTSubscriptions().FirstOrDefault(a => a.Id == subscriptionId);
            if (subscription != null)
            {
                _db.BSTSubscriptions().Remove(subscription);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Subscription not found");
            }
        }

        public async Task<BSTSubscription> GetSubscriptionByCustomerId(string stripeCustomerId = "", int customerId = 0)
        {
            if (!string.IsNullOrEmpty(stripeCustomerId))
            {
                return _db.BSTSubscriptions().Where(s => s.StripeCustomerID == stripeCustomerId).OrderByDescending(a => a.CreatedOnUtc).FirstOrDefault();
            }
            else
            {
                return _db.BSTSubscriptions().Where(s => s.IsActive && s.CustomerId == customerId && s.Status == "active").OrderByDescending(a => a.CreatedOnUtc).FirstOrDefault();
            }
        }

        public async Task<IList<BSTSubscription>> GetSubscriptionByStripeSubscriptionID(string stripeSubscriptionId)
        {
            return await _db.BSTSubscriptions().Where(s => s.StripeSubscriptionID == stripeSubscriptionId).ToListAsync();
        }

        public async Task<IList<BSTSubscription>> GetAllSubscriptions()
        {
            return await _db.BSTSubscriptions().ToListAsync();
        }

        public void DeactiveExpiredSubscriptions()
        {
            var today = DateTime.UtcNow;
            //var subscriptions = _subscriptionRepository.Table.Where(a => a.CurrentPeriodEnd.Value.Date <= today.Date && a.EndedAt <= today.Date && a.Status != "active").ToList();
            var subscriptions = _db.BSTSubscriptions().Where(a => a.CurrentPeriodEnd.Value.Date <= today.Date).ToList();
            if (subscriptions.Count > 0)
            {
                subscriptions.ForEach(a => 
                {
                    a.IsActive = false;
                });
                
                _db.BSTSubscriptions().UpdateRange(subscriptions);
            }
        }

        //public BSTPackage AddUserFreeSubscription()
        //{
        //    var package = _packageService.GetPackageByName("indie");
        //    if (package == null)
        //    {
        //        package = new BSTPackage
        //        {
        //            //StripeProductID = package.StripeProductID,
        //            Name = "indie",
        //            Amount = 0,
        //            Currency = "usd",
        //            Description = "Free",
        //            IsActive = true,
        //            CreatedOnUtc = DateTime.UtcNow,
        //            BusinessLimit = 1,
        //            StorageLimitMB = 50
        //        };

        //        _packageService.InsertPackage(package);
        //    }

            //var subscription = new BSTSubscription
            //{
            //    CustomerId = _workContext.CurrentCustomer.Id,
            //    PackageId = package.Id,
            //    StartDate = DateTime.UtcNow,
            //    CurrentPeriodStart = DateTime.UtcNow,
            //    CurrentPeriodEnd = DateTime.UtcNow.AddYears(20),
            //    IsActive = true,
            //    CreatedOnUtc = DateTime.UtcNow,
            //    Total = 0,
            //    Subtotal = 0,
            //    Discount = 0
            //};
            //_subscriptionRepository.Insert(subscription);

        //    return package;
        //}

        //public PackagesLimit CheckPackageUsageLimit()
        //{
        //    var packagesLimit = new PackagesLimit();
        //    var subscription = _subscriptionService.GetSubscriptionByCustomerId(_workContext.CurrentCustomer.Id);

        //    var subscription = GetSubscriptionByCustomerId(customerId: _workContext.CurrentCustomer.Id);
        //    if (subscription == null)
        //    {
        //        var package = AddUserFreeSubscription();

        //        packagesLimit.PackageName = package.Name.ToUpper();



        //    }
        //    else
        //    {
        //        var package = _packageService.GetPackageById(subscription.PackageId);

        //        packagesLimit.PackageName = package.Name.ToUpper();


        //    }


        //    return packagesLimit;
        //}
    }

}