using WebApi.DTOs.Implementation.ShoppingCarts.Incomings;
using WebApi.Shared.AppConstants;

namespace WebApi.Helpers
{
    public static class ShoppingCartHelper
    {
        public static InputShoppingCartDto GetShoppingCart(HttpContext context)
        {
            var cookies = context.Request.Cookies;

            var shoppingCartCookie = cookies.FirstOrDefault(
                predicate: cookie => cookie.Key.Equals(CookieNames.ShoppingCart));

            return InputShoppingCartDto.ConvertFromJson(shoppingCartCookie.Value);
        }

        /// <summary>
        ///     Set the input guestId to current customer's shopping cart.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="guestId"></param>
        public static void SetGuestIdToShoppingCart(HttpContext context, Guid guestId)
        {
            var shoppingCart = GetShoppingCart(context);

            shoppingCart.GuestId = guestId;

            SetShoppingCart(context, shoppingCart);
        }

        /// <summary>
        ///     Remove the shopping cart cookie.
        /// </summary>
        /// <param name="context"></param>
        public static void RemoveShoppingCartFromCookie(HttpContext context)
        {
            context.Response.Cookies.Delete(CookieNames.ShoppingCart);
        }

        public static void SetShoppingCart(
            HttpContext context,
            InputShoppingCartDto shoppingCartDto)
        {
            var key = CookieNames.ShoppingCart;
            var value = shoppingCartDto.ToJson();
            var cookieOptions = new CookieOptions
            {
                MaxAge = TimeSpan.FromDays(7),
                SameSite = SameSiteMode.Strict
            };

            context.Response.Cookies.Append(key, value, cookieOptions);
        }
    }
}
