using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BizsolTech.Chatbot.Helpers;
using BizsolTech.Chatbot.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Smartstore.Scheduling;

namespace BizsolTech.Chatbot.Tasks
{
    public class ReadDocumentContentTask : ITask
    {
        private readonly IBusinessService _businessPageService;
        private readonly IBusinessDocumentService _businessDocumentService;
        private readonly IS3StorageService _s3StorageService;
        private readonly IBusinessAPIService _apiService;

        public ReadDocumentContentTask(IBusinessService businessPageService, IBusinessDocumentService businessDocumentService, IS3StorageService s3StorageService, IBusinessAPIService businessAPIService)
        {
            _businessPageService = businessPageService;
            _businessDocumentService = businessDocumentService;
            _s3StorageService = s3StorageService;
            _apiService = businessAPIService;
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;
        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var documents = await _businessDocumentService.GetAllAsync();
            documents = documents.Where(x => x.UpdateRequired).ToList();
            foreach (var document in documents)
            {
                var content = await _s3StorageService.ReadFileFromS3(document.FileUrl);

                if(!string.IsNullOrEmpty(content))
                {
                    var success = await _apiService.AddDocumentContent(document.BusinessPageId, content);
                }
                else
                {
                    Logger.Error($"S3Bucket: Read file content error. Doc {document.Name}");
                }

            }
        }
    }
}
