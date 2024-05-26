using System.ComponentModel.DataAnnotations;

namespace WebApi.Shared.ValidationAttributes
{
    public sealed class AllowFileRange : ValidationAttribute
    {
        public int MinRange { get; set; } = 1;

        public int MaxRange { get; set; } = 1;

        public AllowFileRange(int minRange, int maxRange) :
            base($"The number of uploaded files must be in range [{minRange}, {maxRange}]")
        {
            if (minRange < 0 ||
                maxRange < 0 ||
                (minRange > maxRange)
            )
            {
                throw new ArgumentException("Invalid min range or max range.");
            }

            MinRange = minRange;
            MaxRange = maxRange;
        }

        public override bool IsValid(object value)
        {
            if (Equals(value, null))
            {
                return false;
            }

            var isFormCollection = value is IFormFile[];

            if (!isFormCollection)
            {
                return false;
            }

            var formCollection = value as IFormFile[];
            var numberOfUploadFiles = formCollection.Length;

            return numberOfUploadFiles >= MinRange 
                && numberOfUploadFiles <= MaxRange;
        }
    }
}
