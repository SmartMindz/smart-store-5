using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Smartstore;
using Smartstore.Core.Data;
using Smartstore.Data;
using Smartstore.Data.Providers;
using Smartstore.Engine;
using Smartstore.Engine.Builders;
using Smartstore.Events;
using Smartstore.Scheduling;
using Smartstore.Web.Controllers;

namespace Smartstore.PayPal.Subscription
{
    public class Startup : StarterBase
    {
        public override void MapRoutes(EndpointRoutingBuilder builder)
        {
            builder.MapRoutes(StarterOrdering.DefaultRoute, routes =>
            {
                routes.MapAreaControllerRoute(
                    "PaypalSubscriptionRoute",
                    "Smartstore.PayPal.Subscription",
                    "paypalsubscription/{action}/{id?}",
                    new { controller = "PaypalSubscription", action = "Index" },
                    null,
                    new { area = Module.ModuleSystemName }
                    );
            });
        }
    }
}
