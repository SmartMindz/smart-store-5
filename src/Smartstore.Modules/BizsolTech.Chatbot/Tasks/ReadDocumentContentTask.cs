using BizsolTech.Chatbot.Services;
using Smartstore.Scheduling;

namespace BizsolTech.Chatbot.Tasks
{
    public class ReadDocumentContentTask : ITask
    {
        private readonly IBusinessDocumentService _businessDocumentService;
        private readonly IS3StorageService _s3StorageService;
        private readonly IBusinessAPIService _apiService;

        public ReadDocumentContentTask(IBusinessDocumentService businessDocumentService, IS3StorageService s3StorageService, IBusinessAPIService businessAPIService)
        {
            _businessDocumentService = businessDocumentService;
            _s3StorageService = s3StorageService;
            _apiService = businessAPIService;
        }
        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var documents = await _businessDocumentService.GetAllAsync();
            documents = documents.Where(x => x.UpdateRequired).ToList();
            foreach (var document in documents)
            {
                var content = await _s3StorageService.ReadFileFromS3(document.FileUrl);
                var success = await _apiService.AddDocumentContent(document.BusinessPageId, content);
            }
        }
    }
}
