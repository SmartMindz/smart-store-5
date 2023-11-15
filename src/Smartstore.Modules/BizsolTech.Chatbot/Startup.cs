using BizsolTech.Chatbot.Services;
using Microsoft.EntityFrameworkCore;
using Smartstore.Core.Data;
using Smartstore.Data;
using Smartstore.Data.Providers;
using Smartstore.Engine;
using Smartstore.Engine.Builders;
using StackExchange.Profiling;

namespace BizsolTech.Chatbot
{
    public class Startup:StarterBase
    {
        public override void ConfigureServices(IServiceCollection services, IApplicationContext appContext)
        {
            services.AddTransient<IDbContextConfigurationSource<SmartDbContext>, SmartDbContextConfigurer>();
            services.AddScoped<IBusinessService,BusinessService>();

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
