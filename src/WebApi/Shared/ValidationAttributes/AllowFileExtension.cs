using Helpers.ExtensionMethods;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WebApi.Shared.ValidationAttributes
{
    public class AllowFileExtension : ValidationAttribute
    {
        /// <summary>
        ///     Gets and sets the allowed file extension.
        /// </summary>
        public string[] Extensions { get; init; }

        public AllowFileExtension(params string[] extensions)
            : base($"Only file with extension [{GetFullExtensions(extensions)}] is allowed.")
        {
            Extensions = extensions;
        }

        private static string GetFullExtensions(string[] arr)
        {
            const string commas = ", ";

            var stringBuilder = new StringBuilder();

            for (short i = 0; i < arr.Length; i++)
            {
                var item = arr[i];

                stringBuilder.Append(item);

                // If current item is not the last one.
                if (i < arr.Length - 1)
                {
                    stringBuilder.Append(commas);
                }
            }

            return stringBuilder.ToString();
        }

        public override bool IsValid(object value)
        {
            if (Equals(value, null))
            {
                return false;
            }

            var isFormFile = value is IFormFile;
            var isFormFileCollection = value is IFormFile[];

            if (isFormFile)
            {
                return ValidateExtension(value as IFormFile);
            }

            if (isFormFileCollection)
            {
                var isValid = true;

                var formFileCollection = value as IFormFile[];

                formFileCollection.ForEach(file =>
                {
                    isValid &= ValidateExtension(file);
                });

                return isValid;
            }

            return false;
        }

        private bool ValidateExtension(IFormFile file)
        {
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
