using BizsolTech.Chatbot.Services;
using Smartstore.Scheduling;

namespace BizsolTech.Chatbot.Tasks
{
    public class ReadDocumentContentTask : ITask
    {
        private readonly BusinessDocumentService _businessDocumentService;
        private readonly IS3StorageService _s3StorageService;

        public ReadDocumentContentTask(BusinessDocumentService businessDocumentService, IS3StorageService s3StorageService)
        {
            _businessDocumentService = businessDocumentService;
            _s3StorageService = s3StorageService;
        }
        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var documents = await _businessDocumentService.GetAllAsync();
            documents = documents.Where(x => x.UpdateRequired).ToList();
            foreach (var document in documents)
            {
                var content = _s3StorageService.ReadFileFromS3(document.FileUrl);
            }
        }
    }
}
