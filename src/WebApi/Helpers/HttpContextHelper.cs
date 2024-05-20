using BusinessLogic.Commons;
using BusinessLogic.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Helpers
{
    public static class HttpContextHelper
    {
        /// <summary>
        ///     Get the guestId that stored from the cookie.
        /// </summary>
        /// <remarks>
        ///     If there is no cookie found, this method will return
        ///     <see cref="AppResult{TValue}.Failed(string[])"/>.
        /// </remarks>
        /// <param name="context"></param>
        /// <returns>
        ///     The <see cref="AppResult"/> represent the result
        ///     of getting guestId from cookie.
        /// </returns>
        public static AppResult<Guid> GetGuestId(HttpContext context)
        {
            var cookies = context.Request.Cookies;
            var guestIdCookie = cookies.FirstOrDefault(predicate: cookie =>
                cookie.Key.Equals(CookieNames.GuestId));

            if (string.IsNullOrEmpty(guestIdCookie.Value))
            {
                return AppResult<Guid>.Failed();
            }

            var canParse = Guid.TryParse(guestIdCookie.Value, out Guid guestId);

            if (!canParse)
            {
                return AppResult<Guid>.Failed();
            }

            return AppResult<Guid>.Success(guestId);
        }

        public static void SetGuestId(HttpContext context, Guid guestId)
        {
            // Set the cookie to identify the guest.
            var key = CookieNames.GuestId;
            var value = guestId.ToString();
            var cookieOptions = new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                MaxAge = TimeSpan.FromDays(7),
            };

            context.Response.Cookies.Append(key, value, cookieOptions);
        }

        public static void RemoveGuestIdFromCookie(HttpContext context)
        {
            context.Response.Cookies.Delete(CookieNames.GuestId);
        }

        public static AppResult<Guid> GetUserId(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return AppResult<Guid>.Failed();
            }

            var userIdClaim = context.User.FindFirst(
                match: claim => claim.Type.Equals(UserCustomClaimTypes.UserId));

            if (Equals(userIdClaim, null))
            {
                return AppResult<Guid>.Failed();
            }

            var hasUserId = Guid.TryParse(userIdClaim.Value, out var userId);

            if (!hasUserId)
            {
                return AppResult<Guid>.Failed();
            }

            return AppResult<Guid>.Success(userId);
        }

        public static AppResult<Guid> GetSystemAccountId(HttpContext context)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                return AppResult<Guid>.Failed();
            }

            var systemAccountIdClaim = context.User.FindFirst(
                match: claim => claim.Type.Equals(SystemAccountCustomClaimTypes.SystemAccountId));

            if (Equals(systemAccountIdClaim, null))
            {
                return AppResult<Guid>.Failed();
            }

            var hasUserId = Guid.TryParse(systemAccountIdClaim.Value, out var userId);

            if (!hasUserId)
            {
                return AppResult<Guid>.Failed();
            }

            return AppResult<Guid>.Success(userId);
        }
    }
}
