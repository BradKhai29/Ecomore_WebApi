namespace BusinessLogic.Models
{
    public static class AppResult
    {
        public static class DefaultMessage
        {
            public const string InvalidToken = "The token is expired or not valid.";

            public const string InvalidAccessToken = "The access token is expired or not valid.";

            public const string InvalidRefreshToken = "The refresh token is expired or not valid.";
        }
    }
}
