using Newtonsoft.Json;
using Smartstore.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;

namespace BizSol.Chatbot.Controllers
{
    public class PricingController : ModuleController
    {
        public ActionResult StripeWebhook()
        {
            // This is your Stripe CLI webhook secret for testing your endpoint locally.
            string endpointSecret = "whsec_4fc58dfd7cfd60f702b4339af279d90f95226064cc337cf41cb0b4fa9532ea31";
            
            string json = new StreamReader(Request.InputStream).ReadToEnd();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret,
                    throwOnApiVersionMismatch: false
                );

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    HandleCheckoutSessionCompleted(json, stripeEvent);
                }
                //if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                //{
                //    HandleCustomerSubscriptionUpdated(json, stripeEvent);
                //}
                if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    HandleCustomerSubscriptionDeleted(json, stripeEvent);
                }

                return new HttpStatusCodeResult(200); // OK status
            }
            catch (StripeException e)
            {
                // Log the exception or handle it accordingly
                return new HttpStatusCodeResult(400); // BadRequest status
            }
        }

        private void HandleCheckoutSessionCompleted(string json, Event stripeEvent)
        {
            StripeConfiguration.ApiKey = _settings.StripeSecretKey;

            var checkout = stripeEvent.Data.Object as Stripe.Checkout.Session;

            var subService = new SubscriptionService();
            var stripeSubscription = subService.Get(checkout.SubscriptionId);
            var item = stripeSubscription.Items.FirstOrDefault();
            var ProductId = item.Price.ProductId;

            var package = _packageService.GetPackageByStripeProductID(ProductId);

            var subscriptionId = checkout.SubscriptionId;

            var customerId = !string.IsNullOrEmpty(checkout.ClientReferenceId) ? Convert.ToInt32(checkout.ClientReferenceId) : 0;
            if (customerId == 0)
            {
                var customerService = new Stripe.CustomerService();
                var stripeCustomer = customerService.Get(stripeSubscription.CustomerId);
                var customer = _customerService.GetCustomerByEmail(stripeCustomer.Email);
                if (customer != null)
                {
                    customerId = customer.Id;
                }
            }

            var subscription = _subscriptionService.GetSubscriptionByCustomerId(customerId: customerId);
            if (subscription == null)
            {
                subscription = new BSTSubscription
                {
                    CustomerId = customerId,
                    PackageId = package.Id,
                    StripeSubscriptionID = stripeSubscription.Id,
                    StripeCustomerID = stripeSubscription.CustomerId,
                    StartDate = stripeSubscription.StartDate,
                    CurrentPeriodStart = stripeSubscription.CurrentPeriodStart,
                    CurrentPeriodEnd = stripeSubscription.CurrentPeriodEnd,
                    ExpiresAt = checkout?.ExpiresAt,
                    Subtotal = checkout?.AmountSubtotal,
                    Total = checkout?.AmountTotal,
                    Discount = checkout?.TotalDetails?.AmountDiscount,
                    Tax = checkout?.TotalDetails?.AmountTax,
                    PaymentStatus = checkout?.PaymentStatus,
                    IsActive = checkout?.PaymentStatus == "paid" ? true : false,
                    Status = stripeSubscription.Status, // Assuming the subscription is active upon creation
                    LastPaymentDate = DateTime.UtcNow, // This could be set based on actual payment data
                    NextBillingDate = stripeSubscription.CurrentPeriodEnd.AddDays(1), // Example for monthly billing cycle
                    CreatedOnUtc = DateTime.UtcNow
                };

                _subscriptionService.InsertSubscription(subscription);
            }
            else
            {
                subscription.CustomerId = customerId;
                subscription.PackageId = package.Id;
                subscription.StripeSubscriptionID = subscriptionId;
                subscription.StripeCustomerID = stripeSubscription.CustomerId;
                subscription.StartDate = stripeSubscription.StartDate;
                subscription.CurrentPeriodStart = stripeSubscription.CurrentPeriodStart;
                subscription.CurrentPeriodEnd = stripeSubscription.CurrentPeriodEnd;
                subscription.ExpiresAt = checkout?.ExpiresAt;
                subscription.Subtotal = checkout?.AmountSubtotal;
                subscription.Total = checkout?.AmountTotal;
                subscription.Discount = checkout?.TotalDetails?.AmountDiscount;
                subscription.Tax = checkout?.TotalDetails?.AmountTax;
                subscription.PaymentStatus = checkout?.PaymentStatus;
                subscription.IsActive = checkout?.PaymentStatus == "paid" ? true : false;
                subscription.Status = stripeSubscription.Status; // Assuming the subscription is active upon creation
                subscription.LastPaymentDate = DateTime.UtcNow; // This could be set based on actual payment data
                subscription.NextBillingDate = stripeSubscription.CurrentPeriodEnd.AddDays(1); // Example for monthly billing cycle
                subscription.UpdatedOnUtc = DateTime.UtcNow;

                _subscriptionService.UpdateSubscription(subscription);
            }
        }
        
        private void HandleCustomerSubscriptionDeleted(string json, Event stripeEvent)
        {
            StripeConfiguration.ApiKey = _settings.StripeSecretKey;

            
            var stripeSubscription = stripeEvent.Data.Object as Subscription;

            var customerService = new Stripe.CustomerService();
            var stripeCustomer = customerService.Get(stripeSubscription.CustomerId);
            var customer = _customerService.GetCustomerByEmail(stripeCustomer.Email);

            var subscription = _subscriptionService.GetSubscriptionByCustomerId(customerId: customer.Id);
            if (subscription != null)
            {
                subscription.StripeSubscriptionID = stripeSubscription.Id;
                subscription.EndedAt = stripeSubscription.EndedAt;
                //subscription.IsActive = true;
                subscription.Status = stripeSubscription.Status; // Assuming the subscription is active upon creation
                subscription.CancelAt = stripeSubscription.CancelAt; // This could be set based on actual payment data
                //subscription.CancelAtPeriodEnd = stripeSubscription.CancelAtPeriodEnd;
                subscription.CancelAtPeriodEnd = true;
                subscription.CanceledAt = stripeSubscription.CanceledAt;
                
                subscription.CancelComment = stripeSubscription.CancellationDetails.Comment;
                subscription.CancelFeedback = stripeSubscription.CancellationDetails.Feedback;
                subscription.CancelReason = stripeSubscription.CancellationDetails.Reason;
                subscription.UpdatedOnUtc = DateTime.UtcNow;

                _subscriptionService.UpdateSubscription(subscription);
            }
        }
    }
}