namespace BusinessLogic.Models.DistributedFileStorages
{
    /// <summary>
    ///     The result of uploading operation 
    ///     from the distributed file storage service.
    /// </summary>
    public sealed class FileUploadResult
    {
        public bool IsSuccess { get; set; }

        public string AssetId { get; set; }

        public string StorageUrl { get; set; }

        public static FileUploadResult Failed()
        {
            return new FileUploadResult { IsSuccess = false };
        }
    }
}
