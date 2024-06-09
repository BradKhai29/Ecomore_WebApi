using BusinessLogic.Models.DistributedFileStorages;

namespace BusinessLogic.Services.External.Base
{
    public interface IDistributedFileStorageService
    {
        Task<FileUploadResult> UploadImageFileAsync(
            Guid fileId,
            string fileName,
            Stream fileDataStream);

        Task<FileUploadResult> OverwriteImageFileAsync(
            Guid fileId,
            string fileName,
            Stream fileDataStream);

        Task<bool> RemoveImageFileByIdAsync(string fileId);
    }
}
