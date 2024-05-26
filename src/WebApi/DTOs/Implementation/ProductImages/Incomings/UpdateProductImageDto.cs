using WebApi.Shared.Enums;

namespace WebApi.DTOs.Implementation.ProductImages.Incomings
{
    public class UpdateProductImageDto
    {
        public Guid ImageId { get; set; }

        /// <summary>
        ///     The update status of this image item.
        /// </summary>
        /// <remarks>
        ///     The default value is <see cref="ImageUploadActionCode.Keep"/>
        /// </remarks>
        public ImageUploadActionCode Status { get; set; } = ImageUploadActionCode.Keep;

        public int UploadOrder { get; set; }

        /// <summary>
        ///     The name of the target file that uploaded with the request.
        ///     This file name can be used for later traversing in IFormFile list
        ///     to get the correct form file to update.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        ///     This field is used to store the iform file instance
        ///     that will be used to process in the update operation.
        /// </summary>
        private IFormFile _fileToProcess;

        public void SetFileToProcess(IFormFile fileToProcess)
        {
            _fileToProcess = fileToProcess;
        }

        public IFormFile GetFileToProcess() => _fileToProcess;
    }
}
