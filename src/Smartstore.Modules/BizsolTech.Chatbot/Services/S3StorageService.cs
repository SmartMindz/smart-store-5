using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.IO.Compression;
using BizsolTech.Chatbot.Configuration;
using NuGet.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BizsolTech.Chatbot.Domain;
using iText.Kernel.Pdf;
using System.Text;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace BizsolTech.Chatbot.Services
{
    public interface IS3StorageService
    {
        Task<bool> AddDocument(BusinessDocumentEntity businessDocument, string businessName, string fileName, string prevFileKey, byte[] byteArray, bool isArchive);
        Task<bool> S3FileUpload(string fileKey, string prevFileKey, byte[] byteArray, bool isArchive);
        Task<string> ReadFileFromS3(string key);
    }

    public class S3StorageService : IS3StorageService
    {
        private readonly IBusinessDocumentService _documentService;
        private readonly ChatbotSettings _settings;

        public S3StorageService(IBusinessDocumentService businessDocumentService, ChatbotSettings settings)
        {
            _documentService = businessDocumentService;
            _settings = settings;
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;

        public async Task<bool> AddDocument(BusinessDocumentEntity businessDocument, string businessName, string fileName, string prevKey, byte[] byteArray, bool isArchive)
        {
            try
            {
                string fileKey = string.Empty;
                DateTime uploadedDate = DateTime.UtcNow;

                string folderPath = $"{businessName}/{businessDocument.BusinessPageId}/";
                string fileNameAWS = $"Doc-{fileName}-{uploadedDate.Ticks}";
                fileKey = folderPath + fileNameAWS + businessDocument.Extension;

                var success = await S3FileUpload(fileKey, prevKey, byteArray, isArchive);
                if (success)
                {
                    businessDocument.FileUrl = fileKey;
                    var isUpdated = await _documentService.Update(businessDocument);
                    return isUpdated;
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public async Task<bool> S3FileUpload(string fileKey, string prevFileKey, byte[] byteArray, bool isArchive)
        {
            try
            {
                string accessKey = "AKIAUDAYUBV76LWJTAOH";
                string accessSecret = "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";
                string bucketName = "bizsolchatdocs";
                IAmazonS3 client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.USEast1);
                TransferUtility utility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();


                Stream stream = new MemoryStream(byteArray);
                request.BucketName = bucketName;
                request.Key = fileKey;
                request.InputStream = stream;
                utility.Upload(request);

                if (isArchive)
                {
                    string archiveFolderKey = "Archive/" + Path.GetFileNameWithoutExtension(prevFileKey) + "-" + DateTime.Now.ToString("yyyyMMdd") + Path.GetExtension(prevFileKey);

                    var copyFileRequest = new CopyObjectRequest
                    {
                        SourceBucket = _settings.BucketName,
                        SourceKey = prevFileKey,
                        DestinationBucket = _settings.BucketName,
                        DestinationKey = archiveFolderKey
                    };
                    CopyObjectResponse copyFileResponse = await client.CopyObjectAsync(copyFileRequest);
                    if (copyFileResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var deleteFileRequest = new DeleteObjectRequest
                        {
                            BucketName = _settings.BucketName,
                            Key = prevFileKey
                        };
                        DeleteObjectResponse fileDeleteResponse = await client.DeleteObjectAsync(deleteFileRequest);
                    }
                }

                return true; //indicate that the file was sent
            }
            catch (AmazonS3Exception e)
            {
                Logger.Error(e);
                return false;
            }
        }

        public async Task<string> ReadFileFromS3(string fileKey)
        {
            try
            {
                string accessKey = "AKIAUDAYUBV76LWJTAOH";
                string accessSecret = "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";
                string bucketName = "bizsolchatdocs";

                IAmazonS3 client = new AmazonS3Client(accessKey, accessSecret, Amazon.RegionEndpoint.USEast1);
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileKey
                };

                using (GetObjectResponse response = await client.GetObjectAsync(getObjectRequest))
                //using (StreamReader reader = new StreamReader(response.ResponseStream))
                //{
                //    string content = await reader.ReadToEndAsync();
                //    return content;

                //}
                using (MemoryStream memStream = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(memStream);
                    memStream.Position = 0; // Reset position to start

                    PdfReader pdfReader = new PdfReader(memStream);
                    PdfDocument pdfDocument = new PdfDocument(pdfReader);

                    StringBuilder builder = new StringBuilder();

                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        var page = pdfDocument.GetPage(i);
                        var text = PdfTextExtractor.GetTextFromPage(page);
                        builder.Append(text);
                    }

                    pdfReader.Close();
                    pdfDocument.Close();

                    return builder.ToString();
                }
            }
            catch (AmazonS3Exception e)
            {
                Logger.Error(e);
                return string.Empty;
            }
        }
    }
}
