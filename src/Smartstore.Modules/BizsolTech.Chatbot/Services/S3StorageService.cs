using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.IO.Compression;
using BizsolTech.Chatbot.Configuration;
using NuGet.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BizsolTech.Chatbot.Services
{
    public interface IS3StorageService
    {
        Task<bool> S3FileUpload(string fileKey, string prevFileKey, byte[] byteArray, bool isArchive);
        Task<string> ReadFileFromS3(string key);
    }

    public class S3StorageService : IS3StorageService
    {
        private readonly ChatbotSettings _settings;

        public S3StorageService(ChatbotSettings settings)
        {
            _settings = settings;
        }

        public ILogger Logger { get; set; } = NullLogger.Instance;
        public async Task<bool> S3FileUpload(string fileKey, string prevFileKey, byte[] byteArray, bool isArchive)
        {
            try
            {
                IAmazonS3 client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, Amazon.RegionEndpoint.USEast1);
                TransferUtility utility = new TransferUtility(client);
                TransferUtilityUploadRequest request = new TransferUtilityUploadRequest();


                Stream stream = new MemoryStream(byteArray);
                request.BucketName = _settings.BucketName;
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
            try {
                IAmazonS3 client = new AmazonS3Client(_settings.AccessKey, _settings.SecretKey, Amazon.RegionEndpoint.USEast1);
                var getObjectRequest = new GetObjectRequest
                {
                    BucketName = _settings.BucketName,
                    Key = fileKey
                };

                using (GetObjectResponse response = await client.GetObjectAsync(getObjectRequest))
                using (StreamReader reader = new StreamReader(response.ResponseStream))
                {
                    string content = await reader.ReadToEndAsync();
                    return content;
                }
            }
            catch(AmazonS3Exception e) {
                Logger.Error(e);
                return e.Message;
            }
        }
    }
}
