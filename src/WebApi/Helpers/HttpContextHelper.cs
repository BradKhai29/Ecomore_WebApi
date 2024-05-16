using BusinessLogic.Commons;
using BusinessLogic.Models;
using System.IdentityModel.Tokens.Jwt;

namespace WebApi.Helpers
{
    public static class HttpContextHelper
    {
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
    }
}
