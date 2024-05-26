using WebApi.Shared.AppConstants;

namespace Presentation.Middlewares
{
    public class GuestIdCookieMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var guestId = Guid.NewGuid();

            // Check if cookie is set or not.
            var cookies = context.Request.Cookies;
            var isCookieSet = cookies.ContainsKey(key: CookieNames.GuestId);

            if (!isCookieSet)
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

            return next.Invoke(context);
        }
    }
}
