using System.Threading.Tasks;
using BizsolTech.Chatbot.Tasks;
using Smartstore;
using Smartstore.Engine.Modularity;
using Smartstore.Http;
using Smartstore.Scheduling;

namespace BizsolTech.Chatbot
{
    public class Module:ModuleBase, IConfigurable
    {
        public static string ModuleSystemName => "BizsolTech.Chatbot"; 
        private readonly ITaskStore _taskStore;

        public Module(ITaskStore taskStore)
        {
            _taskStore = taskStore;
        }
        public RouteInfo GetConfigurationRoute()
            => new("Configure", "Config", new { area = "Admin" });

        public override async Task InstallAsync(ModuleInstallationContext context)
        {
            var task = await _taskStore.GetTaskByTypeAsync(typeof(ReadDocumentContentTask).AssemblyQualifiedNameWithoutVersion());
            if (task == null) {
                var taskDescriptor = new TaskDescriptor();
                taskDescriptor.Name = "Read Document Content Task";
                taskDescriptor.Type = typeof(ReadDocumentContentTask).AssemblyQualifiedNameWithoutVersion();
                taskDescriptor.CronExpression = "*/10 * * * *"; //every ten minutes
                taskDescriptor.Enabled = true;
                taskDescriptor.StopOnError = false;
                taskDescriptor.IsHidden = false;

                await _taskStore.InsertTaskAsync(taskDescriptor);
            }
            await base.InstallAsync(context);
        }

        public override async Task UninstallAsync()
        {
            await base.UninstallAsync();
        }
    }
}
