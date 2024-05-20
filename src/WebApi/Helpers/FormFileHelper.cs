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
    }
}
