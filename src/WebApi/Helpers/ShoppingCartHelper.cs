using BusinessLogic.Models;
using System.Collections.Concurrent;
using WebApi.DTOs.Implementation.ShoppingCarts.Incomings;
using WebApi.Shared.AppConstants;

namespace WebApi.Helpers
{
    public static class ShoppingCartHelper
    {
        /// <summary>
        ///     The dictionary of shopping carts, with key is the cartId.
        /// </summary>
        private static ConcurrentDictionary<Guid, InputShoppingCartDto> _shoppingCarts;

        private static readonly object _lock = new();

        static ShoppingCartHelper()
        {
            lock (_lock)
            {
                _shoppingCarts = new ConcurrentDictionary<Guid, InputShoppingCartDto>();
            }
        }

        /// <summary>
        ///     Create a new shopping cart by the input <paramref name="cartId"/>
        ///     and add it to the managed list of shopping carts.
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public static InputShoppingCartDto CreateShoppingCartById(Guid cartId)
        {
            var isIdExisted = _shoppingCarts.ContainsKey(cartId);

            if (isIdExisted)
            {
                return _shoppingCarts[cartId];
            }

            // Create new shopping cart.
            var shoppingCart = new InputShoppingCartDto
            {
                CartId = cartId,
                GuestId = cartId,
                CartItems = new List<AddCartItemDto>()
            };

            _shoppingCarts.TryAdd(cartId, shoppingCart);
            return shoppingCart;
        }

        /// <summary>
        ///     Try to get the shopping cart with provided <paramref name="cartId"/> from this application.
        ///     If no shopping cart is found, return <see cref="AppResult{T}.Failed(string[])"/>.
        /// </summary>
        /// <param name="cartId"></param>
        /// <returns></returns>
        public static AppResult<InputShoppingCartDto> GetShoppingCart(Guid cartId)
        {
            var isIdExisted = _shoppingCarts.TryGetValue(cartId, out InputShoppingCartDto shoppingCart);
            if (!isIdExisted)
            {
                return AppResult<InputShoppingCartDto>.Failed("CartId is not found.");
            }

            return AppResult<InputShoppingCartDto>.Success(shoppingCart);
        }

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
