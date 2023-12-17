using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.IO.Compression;
using BizsolTech.Chatbot.Configuration;
using NuGet.Configuration;

namespace BizsolTech.Chatbot.Services
{
    public interface IS3StorageService
    {
        Task<bool> S3FileUpload(string fileKey, string prevFileKey, byte[] byteArray, bool isArchive);
    }

    public class S3StorageService : IS3StorageService
    {
        private readonly ChatbotSettings _settings;

        public S3StorageService(ChatbotSettings settings)
        {
            _settings = settings;
        }
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
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
