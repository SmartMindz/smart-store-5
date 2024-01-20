using Smartstore;
using System;
using System.Threading.Tasks;
using BizSol.Chatbot.Services;
using BizsolTech.Chatbot.Domain.Subscription;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core.Checkout.Orders.Events;
using Smartstore.Core.Checkout.Payment;
using Smartstore.Core.Data;
using Smartstore.Events;
using System.Linq;
using Smartstore.Core;
using BizsolTech.Chatbot.Extensions;

namespace BizsolTech.Chatbot.Events
{
    internal class OrderPlacedEventConsumer : IConsumer
    {
        private readonly SmartDbContext _db;
        private readonly ICommonServices _services;
        private readonly IPaymentService _paymentService;
        private readonly IBSTSubscriptionService _subscriptionService;
        public OrderPlacedEventConsumer(SmartDbContext db, ICommonServices services, IPaymentService paymentService, IBSTSubscriptionService subscriptionService)
        {
            _db = db;
            _services = services;
            _paymentService = paymentService;
            _subscriptionService = subscriptionService;
        }
        public async Task HandleEventAsync(OrderPlacedEvent message)
        {
            try {
                var order = message.Order;
                var customer = order.Customer;
                var orderItem /*package*/ = order.OrderItems.FirstOrDefault(); //we have only one product in the order
                var recurringPayment = await _db.RecurringPayments
                .Include(x => x.InitialOrder).ThenInclude(x => x.Customer)
                .FindByIdAsync(order.Id);
                var subscription = await _subscriptionService.GetSubscriptionByCustomerId(customerId: customer.Id);
                if(subscription == null)
                {
                    var newSubscription = new BSTSubscription();
                    newSubscription.PackageId = orderItem.ProductId;
                    newSubscription.CustomerId = customer.Id;
                    newSubscription.Subtotal = (long)order.OrderSubTotalDiscountExclTax;
                    newSubscription.Total = (long)order.OrderTotal;

                    if(recurringPayment != null)
                    {
                        var nextPaymentDate = await _paymentService.GetNextRecurringPaymentDateAsync(recurringPayment);
                        newSubscription.PaymentStatus = order.PaymentStatus.ToString();
                        //newSubscription.StartDate = _services.DateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc);
                        newSubscription.StartDate = recurringPayment.StartDateUtc;
                        newSubscription.CurrentPeriodStart = recurringPayment.StartDateUtc;
                        newSubscription.CurrentPeriodEnd = nextPaymentDate;
                        newSubscription.Status = recurringPayment.IsActive ? "active" : "inactive";

                        var paymentHistory = recurringPayment.RecurringPaymentHistory.OrderByDescending(rh => rh.CreatedOnUtc).FirstOrDefault();
                        newSubscription.CancelAtPeriodEnd = true;
                        newSubscription.LastPaymentDate = order.PaymentStatus == PaymentStatus.Paid ? paymentHistory.CreatedOnUtc : null;
                        newSubscription.NextBillingDate = nextPaymentDate;
                        await _db.BSTSubscriptions().AddAsync(newSubscription);
                        await _db.SaveChangesAsync();
                    }
                }
                else
                {
                    var nextPayment = await _paymentService.GetNextRecurringPaymentDateAsync(recurringPayment);
                    var paymentHistory = recurringPayment.RecurringPaymentHistory.OrderByDescending(rh => rh.CreatedOnUtc).FirstOrDefault();

                    subscription.PaymentStatus = order.PaymentStatus.ToString();
                    subscription.CurrentPeriodStart = paymentHistory.CreatedOnUtc;
                    subscription.CurrentPeriodEnd = nextPayment;
                    subscription.Status = recurringPayment.IsActive ? "active" : "inactive";

                    subscription.LastPaymentDate = order.PaymentStatus == PaymentStatus.Paid ? paymentHistory.CreatedOnUtc : null;
                    subscription.NextBillingDate = nextPayment;
                    _db.BSTSubscriptions().Update(subscription);
                    await _db.SaveChangesAsync();
                }

            } catch(Exception ex) { }
        }
    }
}
