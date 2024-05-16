namespace WebApi.Models
{
    public class ApiResponse
    {
        public bool IsSuccess { get; set; }

        public DateTime ResponsedAt { get; set; }

        public IEnumerable<string> ErrorMessages { get; set; }

        public object Body { get; set; }

        /// <summary>
        ///     Create a failed api-response with input error messages.
        /// </summary>
        /// <param name="errorMessages">
        ///     The error messages that lead to the failure.
        /// </param>
        /// <returns></returns>
        public static ApiResponse Failed(params string[] errorMessages)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                ResponsedAt = DateTime.Now,
                ErrorMessages = errorMessages,
                Body = default
            };
        }

        /// <summary>
        ///     Create a failed api-response with input error messages.
        /// </summary>
        /// <param name="errorMessages">
        ///     The error messages that lead to the failure.
        /// </param>
        /// <returns></returns>
        public static ApiResponse Failed(IEnumerable<string> errorMessages)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                ResponsedAt = DateTime.Now,
                ErrorMessages = errorMessages,
                Body = default
            };
        }

        /// <summary>
        ///     Create a success api-response with input body.
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public static ApiResponse Success(object body)
        {
            return new ApiResponse
            {
                IsSuccess = true,
                ResponsedAt = DateTime.Now,
                ErrorMessages = Enumerable.Empty<string>(),
                Body = body
            };
        }

        public static class DefaultMessage
        {
            public const string DatabaseError = "Something wrong happen with database operation.";

            public const string FailedToSendEmail = "Fail to send email.";

            public const string InvalidToken = "The token is expired or not valid.";

            public const string InvalidAccessToken = "The access token is expired or not valid.";

            public const string InvalidRefreshToken = "The refresh token is expired or not valid.";
        }
    }
}
