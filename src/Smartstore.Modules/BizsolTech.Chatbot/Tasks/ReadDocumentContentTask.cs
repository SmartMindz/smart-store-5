using BizsolTech.Chatbot.Helpers;
using BizsolTech.Chatbot.Services;
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
        public async Task Run(TaskExecutionContext ctx, CancellationToken cancelToken = default)
        {
            var documents = await _businessDocumentService.GetAllAsync();
            documents = documents.Where(x => x.UpdateRequired).ToList();
            foreach (var document in documents)
            {
                var page = await _businessPageService.Get(document.BusinessPageId);
                var content = await _s3StorageService.ReadFileFromS3(document.FileUrl);

                var crc32 = CRC32Calculator.CalculateCRC32FromContent(content);
                if(crc32 != 0 && string.Equals(crc32.ToString(), document.CRC))
                {
                    //var success = await _apiService.AddDocumentContent(int.Parse(page.BusinessId), content);
                }
                else
                {
                    Console.WriteLine("File from s3 and from db are not matched!");
                }

            }
        }
    }
}
