using BusinessLogic.Models.DistributedFileStorages;
using BusinessLogic.Services.External.Base;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Options.Models;
using System.Net;

namespace BusinessLogic.Services.External.Implementation
{
    internal class CloudinaryService : IDistributedFileStorageService
    {
        private readonly CloudinaryOptions _cloudinaryOptions;

        public CloudinaryService(CloudinaryOptions cloudinaryOptions)
        {
            _cloudinaryOptions = cloudinaryOptions;
        }

        public async Task<bool> RemoveImageFileByIdAsync(string fileId)
        {
            try
            {
                var cloudinaryConnection = CreateConnection();

                var removeResult = await cloudinaryConnection.DeleteResourcesAsync(
                    type: ResourceType.Image,
                    publicIds: fileId);

                if (removeResult.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<FileUploadResult> OverwriteImageFileAsync(
            Guid fileId,
            string fileName,
            Stream fileDataStream)
        {
            // Reset the position to 0 to read the data bytes.
            fileDataStream.Position = 0;

            var imageUploadParams = new ImageUploadParams
            {
                PublicId = fileId.ToString(),
                DisplayName = fileName,
                File = new FileDescription(fileName, fileDataStream),
                Overwrite = true,
                Invalidate = true,
            };

            try
            {
                var cloudinaryConnection = CreateConnection();

                var uploadResult = await cloudinaryConnection.UploadAsync(imageUploadParams);

                var isSuccess = uploadResult.StatusCode == HttpStatusCode.OK;

                if (!isSuccess)
                {
                    return FileUploadResult.Failed();
                }

                CleanUpFileDataStream(fileDataStream);

                return new FileUploadResult
                {
                    IsSuccess = true,
                    AssetId = uploadResult.AssetId,
                    StorageUrl = uploadResult.SecureUrl.ToString(),
                };
            }
            catch (Exception)
            {
                return FileUploadResult.Failed();
            }
        }

        private static void CleanUpFileDataStream(Stream fileDataStream)
        {
            fileDataStream.Flush();
            fileDataStream.Close();
            fileDataStream.Dispose();
        }

        public async Task<FileUploadResult> UploadImageFileAsync(
            Guid fileId,
            string fileName,
            Stream fileDataStream)
        {
            // Reset the position to 0 to read the data bytes.
            fileDataStream.Position = 0;

            var imageUploadParams = new ImageUploadParams
            {
                PublicId = fileId.ToString(),
                DisplayName = fileName,
                File = new FileDescription(fileName, fileDataStream),
            };

            try
            {
                var cloudinaryConnection = CreateConnection();

                var uploadResult = await cloudinaryConnection.UploadAsync(imageUploadParams);

                var isSuccess = uploadResult.StatusCode == HttpStatusCode.OK;

                if (!isSuccess)
                {
                    return FileUploadResult.Failed();
                }

                CleanUpFileDataStream(fileDataStream);

                return new FileUploadResult
                {
                    IsSuccess = true,
                    AssetId = uploadResult.AssetId,
                    StorageUrl = uploadResult.SecureUrl.ToString(),
                };
            }
            catch (Exception)
            {
                return FileUploadResult.Failed();
            }
        }

        private Cloudinary CreateConnection()
        {
            var connection = new Cloudinary(cloudinaryUrl: _cloudinaryOptions.CloudinaryUrl);

            connection.Api.Secure = true;

            return connection;
        }
    }
}
