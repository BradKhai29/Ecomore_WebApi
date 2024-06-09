namespace WebApi.Helpers
{
    public static class FormFileHelper
    {
        public static Stream GetFileDataStream(IFormFile formFile)
        {
            var dataStream = new MemoryStream();

            formFile.CopyTo(dataStream);

            return dataStream;
        }

        public static string GetFileExtension(IFormFile formFile)
        {
            if (formFile == null)
            {
                return string.Empty;
            }

            return formFile.FileName.Split(separator: ".").Last();
        }

        public static IFormFile GetFormFileByName(
            IEnumerable<IFormFile> formFiles,
            string fileName)
        {
            foreach (IFormFile formFile in formFiles)
            {
                if (formFile.FileName.Equals(fileName))
                {
                    return formFile;
                }
            }

            return null;
        }
    }
}
