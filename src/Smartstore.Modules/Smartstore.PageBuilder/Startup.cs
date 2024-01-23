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

namespace Smartstore.PageBuilder
{
    public class Startup : StarterBase
    {
        public override void MapRoutes(EndpointRoutingBuilder builder)
        {
            builder.MapRoutes(StarterOrdering.DefaultRoute, routes =>
            {
                routes.MapAreaControllerRoute(
                    "PageBuilderRoute",
                    "Smartstore.PageBuilder",
                    "builder/{action}/{id?}",
                    new { controller = "PageBuilder", action = "Index" },
                    null,
                    new { area = Module.ModuleSystemName }
                    );
            });
        }
    }
}
