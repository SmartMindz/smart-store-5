using System.Threading.Tasks;
using Smartstore;
using Smartstore.Engine.Modularity;
using Smartstore.Http;
using Smartstore.Scheduling;

namespace Smartstore.PayPal.Subscription
{
    public class Module : ModuleBase
    {
        public static string ModuleSystemName => "Smartstore.PayPal.Subscription";

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
