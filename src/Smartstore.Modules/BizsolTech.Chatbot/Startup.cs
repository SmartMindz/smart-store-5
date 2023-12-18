using BizsolTech.Chatbot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Smartstore;
using Smartstore.Admin.Controllers;
using Smartstore.Core.Data;
using Smartstore.Core.Seo.Routing;
using Smartstore.Data;
using Smartstore.Data.Providers;
using Smartstore.Engine;
using Smartstore.Engine.Builders;
using Smartstore.Web.Controllers;
using StackExchange.Profiling;

namespace BizsolTech.Chatbot
{
    public class Startup : StarterBase
    {
        public override void ConfigureServices(IServiceCollection services, IApplicationContext appContext)
        {
            if (appContext.IsInstalled)
            {
                services.AddTransient<IDbContextConfigurationSource<SmartDbContext>, SmartDbContextConfigurer>();
                services.AddScoped<IBusinessService, BusinessService>();
                services.AddScoped<IBusinessDocumentService, BusinessDocumentService>();
                services.AddScoped<IBusinessAPIService, BusinessAPIService>();
                services.AddScoped<IS3StorageService, S3StorageService>();

                services.AddMiniProfiler(o =>
                {
                    //o.EnableDebugMode = true;
                    //o.EnableMvcFilterProfiling = true;
                    //o.EnableMvcViewProfiling = true;
                    //o.MaxUnviewedProfiles = 5;

                    //o.ShouldProfile = ShouldProfile;
                    //o.ResultsAuthorize = ResultsAuthorize;
                    //o.ResultsListAuthorize = ResultsAuthorize;

                    o.IgnoredPaths.Clear();
                    o.IgnorePath("/favicon.ico");

                    //// INFO: Handled by settings now.
                    //o.IgnorePath("/admin/");
                    //o.IgnorePath("/themes/");
                    //o.IgnorePath("/taskscheduler/");
                    //o.IgnorePath("/bundle/");
                    //o.IgnorePath("/media/");
                    //o.IgnorePath("/js/");
                    //o.IgnorePath("/css/");
                    //o.IgnorePath("/images/");
                }).AddEntityFramework();
            }
        }

        public override void MapRoutes(EndpointRoutingBuilder builder)
        {
            builder.MapRoutes(StarterOrdering.DefaultRoute, routes =>
            {
                routes.MapAreaControllerRoute(
                    "BusinessRoute",
                    "BizsolTech.Chatbot",
                    "business/{action}/{id?}",
                    new { controller = "Business", action = "Index" },
                    null,
                    new { area = "BizsolTech.Chatbot" }
                    );
            });
        }

        class SmartDbContextConfigurer : IDbContextConfigurationSource<SmartDbContext>
        {
            public void Configure(IServiceProvider services, DbContextOptionsBuilder builder)
            {
                builder.UseDbFactory(b =>
                {
                    b.AddModelAssembly(this.GetType().Assembly);
                });
            }
        }
    }
}
