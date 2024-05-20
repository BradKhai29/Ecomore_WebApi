using System.ComponentModel.DataAnnotations;

namespace WebApi.Shared.ValidationAttributes
{
    /// <summary>
    ///     This attribute is used to validate the input phone number
    ///     conform the Vietnamese phone number format or not.
    /// </summary>
    /// <remarks>
    ///     More information about the requirement of validation: https://vietnamnet.vn/so-thue-bao-di-dong-chi-con-lai-10-so-215512.html
    /// </remarks>
    public class IsValidPhoneNumber : ValidationAttribute
    {
        private const string ErrorMessage = "Số điện thoại không đúng định dạng.";
        private const int ValidLength = 10;

        /// <summary>
        ///     Gets and sets the value to indicate this
        ///     validation attribute not to skip the empty input.
        /// </summary>
        /// <remarks>
        ///     The default value of this property is <see langword="true"/>.
        /// </remarks>
        public bool NotAllowEmpty { get; set; } = true;

        public IsValidPhoneNumber() : base(ErrorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            // If not NotAllowEmpty is true, then continue
            // the normal validation flow.
            if (NotAllowEmpty)
            {
                return NormalValidation(value);
            }

            // If the NotAllowEmpty is false, then conduct
            // the adhoc validation flow. This adhoc
            // flow is only executed if the user left phone number
            // input as blank.
            if (!Equals(value, null) && string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            return NormalValidation(value);
        }

        private static bool NormalValidation(object value)
        {
            if (Equals(value, null))
            {
                return false;
            }

            var phoneNumberString = value.ToString();

            if (phoneNumberString.Length != ValidLength)
            {
                return false;
            }

            var hasLeadingZero = phoneNumberString.StartsWith('0');

            if (!hasLeadingZero)
            {
                return false;
            }

            return phoneNumberString.All(char.IsDigit);
        }
    }
}
