using System.ComponentModel.DataAnnotations;

namespace WebApi.Shared.ValidationAttributes
{
    public class AllowItemRange : ValidationAttribute
    {
        public int MinRange { get; set; } = 1;

        public int MaxRange { get; set; } = 1;

        public AllowItemRange(int minRange, int maxRange) :
            base($"The number of uploaded items must be in range [{minRange}, {maxRange}]")
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

            var isObjectCollection = value is object[];

            if (!isObjectCollection)
            {
                return false;
            }

            var itemCollection = value as object[];
            var numberOfItems = itemCollection.Length;

            return numberOfItems >= MinRange 
                && numberOfItems <= MaxRange;
        }
    }
}
