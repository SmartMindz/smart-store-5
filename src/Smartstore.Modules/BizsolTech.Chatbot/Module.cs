using Smartstore.Engine.Modularity;

namespace BizsolTech.Chatbot
{
    public class Module:ModuleBase
    {
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
