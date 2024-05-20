using BusinessLogic.Services.Core.Base;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using WebApi.DTOs.Implementation.ShoppingCarts.Incomings;
using WebApi.DTOs.Implementation.ShoppingCarts.Outgoings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IUserProductService _userProductService;

        public ShoppingCartController(IUserProductService userProductService)
        {
            _userProductService = userProductService;
        }

        [HttpGet]
        public async Task<IActionResult> GetShoppingCartDetailAsync(
            CancellationToken cancellationToken)
        {
            // Get the shopping cart from the cookie.
            var shoppingCart = ShoppingCartHelper.GetShoppingCart(HttpContext);

            if (shoppingCart.IsEmpty())
            {
                return NoContent();
            }

            var productIdList = shoppingCart.GetProductIdList();

            var products = await _userProductService.GetAllProductsFromIdListAsync(
                productIdList: productIdList,
                cancellationToken: cancellationToken);

            var shoppingCartDto = new OutputShoppingCartDto();

            shoppingCartDto.CartItems = products.Select(product => new OutputCartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = shoppingCart.GetItemQuantity(product.Id),
                UnitPrice = product.UnitPrice,
                ImageUrl = product.ProductImages.First().StorageUrl
            });

            return Ok(ApiResponse.Success(shoppingCartDto));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItemToCartAsync(
            AddCartItemDto cartItemDto,
            CancellationToken cancellationToken)
        {
            var guestIdResult = HttpContextHelper.GetGuestId(HttpContext);

            if (!guestIdResult.IsSuccess)
            {
                HttpContextHelper.RemoveGuestIdFromCookie(HttpContext);

                return StatusCode(
                    statusCode: StatusCodes.Status400BadRequest,
                    value: ApiResponse.Failed("Invalid customerId cookie."));
            }

            // Get the shopping cart from the cookie.
            var shoppingCart = ShoppingCartHelper.GetShoppingCart(HttpContext);

            // Check the shopping cart guestId.
            var hasGuestId = shoppingCart.GuestId != Guid.Empty;

            // If no guestId found, then set the current guestId for this shopping cart.
            if (!hasGuestId)
            {
                shoppingCart.GuestId = guestIdResult.Value;
            }

            var foundResult = await _userProductService.FindProductByIdForAddingToCartAsync(
                productId: cartItemDto.ProductId,
                cancellationToken: cancellationToken);

            if (!foundResult.IsSuccess)
            {
                return NotFound(ApiResponse.Failed(foundResult.ErrorMessages));
            }

            // Check if the adding quantity of the cart item is valid or not.
            var foundProduct = foundResult.Value;

            var addToCartQuantity = cartItemDto.Quantity + shoppingCart.GetItemQuantity(cartItemDto.ProductId);

            var isValidQuantity = addToCartQuantity <= foundProduct.QuantityInStock;

            if (!isValidQuantity)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status400BadRequest,
                    value: ApiResponse.Failed("Invalid add-to-cart quantity."));
            }

            // Add the item to the shopping cart.
            shoppingCart.AddItem(cartItemDto);

            ShoppingCartHelper.SetShoppingCart(HttpContext, shoppingCart);

            return Ok(ApiResponse.Success(default));
        }

        [HttpDelete("{productId:guid}")]
        public async Task<IActionResult> DeleteItemFromCartAsync(
            [FromRoute]
            [IsValidGuid]
            Guid productId)
        {
            // Get the shopping cart from the cookie.
            var shoppingCart = ShoppingCartHelper.GetShoppingCart(HttpContext);

            shoppingCart.RemoveItem(productId);

            ShoppingCartHelper.SetShoppingCart(HttpContext, shoppingCart);

            return Ok(ApiResponse.Success(default));
        }

        [HttpPut("decrease")]
        public async Task<IActionResult> DecreaseItemFromCartAsync(
            DecreaseCartItemDto cartItemDto)
        {
            // Get the shopping cart from the cookie.
            var shoppingCart = ShoppingCartHelper.GetShoppingCart(HttpContext);

            // Decrease the item from the shopping cart.
            shoppingCart.DecreaseItem(cartItemDto);

            ShoppingCartHelper.SetShoppingCart(HttpContext, shoppingCart);

            return Ok(ApiResponse.Success(default));
        }
    }
}
