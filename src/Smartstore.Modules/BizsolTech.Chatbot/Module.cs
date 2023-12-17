using Smartstore.Engine.Modularity;
using Smartstore.Http;

namespace BizsolTech.Chatbot
{
    public class Module:ModuleBase, IConfigurable
    {
        public RouteInfo GetConfigurationRoute()
            => new("Configure", "Business", new { area = "Admin" });

        public override async Task InstallAsync(ModuleInstallationContext context)
        {
            await base.InstallAsync(context);
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}
