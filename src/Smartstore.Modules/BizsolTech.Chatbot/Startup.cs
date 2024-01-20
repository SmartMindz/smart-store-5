using System;
using BizSol.Chatbot.Services;
using BizsolTech.Chatbot.Events;
using BizsolTech.Chatbot.Filters;
using BizsolTech.Chatbot.Services;
using BizsolTech.Chatbot.Tasks;
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
                services.AddScoped<IBSTSubscriptionService, BSTSubscriptionService>();

                services.AddScoped<ITask, ReadDocumentContentTask>();

                // action filter for RegisterResult
                services.Configure<MvcOptions>(o =>
                {
                    o.Filters.AddConditional<ChatbotRegisterResultFilter>(
                        context => context.ControllerIs<IdentityController>());
                });

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

        /*public void ConfigureContainer(ContainerBuilder builder)
        {
            // Register services with Autofac
            builder.RegisterType<SmartDbContextConfigurer>().As<IDbContextConfigurationSource<SmartDbContext>>().InstancePerLifetimeScope();
            builder.RegisterType<BusinessService>().As<IBusinessService>().InstancePerLifetimeScope();
            builder.RegisterType<BusinessDocumentService>().As<IBusinessDocumentService>().InstancePerLifetimeScope();
            builder.RegisterType<BusinessAPIService>().As<IBusinessAPIService>().InstancePerLifetimeScope();
            builder.RegisterType<S3StorageService>().As<IS3StorageService>().InstancePerLifetimeScope();

            builder.RegisterType<ReadDocumentContentTask>().As<ITask>().InstancePerLifetimeScope();

            // Configure action filter for RegisterResult
            builder.Register(context =>
            {
                var options = new MvcOptions();
                options.Filters.AddConditional<ChatbotRegisterResultFilter>(
                    context => context.ControllerIs<IdentityController>());
                return options;
            }).SingleInstance();

            // Configure MiniProfiler
            builder.Register(context =>
            {
                var options = new MiniProfilerOptions();
                options.IgnoredPaths.Clear();
                options.IgnorePath("/favicon.ico");
                // Other MiniProfiler configurations here
                return options;
            }).SingleInstance()
            .AsSelf();
        }*/


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
