using System.ComponentModel.DataAnnotations;

namespace WebApi.Shared.ValidationAttributes
{
    public class AllowFileExtension : ValidationAttribute
    {
        public AllowFileExtension(params string[] extensions)
        {
            Extensions = extensions;
        }

        /// <summary>
        ///     Gets and sets the allowed file extension.
        /// </summary>
        /// <value></value>
        public string[] Extensions { get; init; }

        public override bool IsValid(object value)
        {
            if (value is not IFormFile file)
            {
                return false;
            }

            var inputFileExtension = GetFileExtension(fileName: file.FileName);

            var isValid = Extensions.Any(predicate: allowedExtension =>
                allowedExtension.Equals(value: inputFileExtension));

            return isValid;
        }

        private static string GetFileExtension(string fileName)
        {
            return fileName.Split(separator: ".").Last();
        }
    }
}
