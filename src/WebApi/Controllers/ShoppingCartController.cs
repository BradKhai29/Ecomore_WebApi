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

        [HttpPost("init")]
        public IActionResult InitNewShoppingCart()
        {
            Guid cartId = Guid.NewGuid();
            ShoppingCartHelper.CreateShoppingCartById(cartId);

            return Ok(ApiResponse.Success(new
            {
                CartId = cartId
            }));
        }

        [HttpGet("{cartId:guid}")]
        public async Task<IActionResult> GetShoppingCartDetailAsync(
            [IsValidGuid] Guid cartId,
            CancellationToken cancellationToken)
        {
            // Get the shopping cart from the in-memory dictionary.
            var getResult = ShoppingCartHelper.GetShoppingCart(cartId);

            if (!getResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(getResult.ErrorMessages));
            }

            var shoppingCart = getResult.Value;
            
            // If current shopping cart is empty, then immediately
            // return to client to prevent uneccessary database query.
            if (shoppingCart.IsEmpty())
            {
                return Ok(ApiResponse.Success(
                    body: OutputShoppingCartDto.Empty(cartId))); 
            }

            var productIdList = shoppingCart.GetProductIdList();

            var products = await _userProductService.GetAllProductsFromIdListForDisplayShoppingCartAsync(
                productIdList: productIdList,
                cancellationToken: cancellationToken);

            var shoppingCartDto = new OutputShoppingCartDto
            {
                CartId = cartId,
            };

            shoppingCartDto.CartItems = products.Select(product => new OutputCartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.UnitPrice,
                ImageUrl = product.ProductImages.First().StorageUrl,
                Quantity = shoppingCart.GetItemQuantity(product.Id),
            });

            shoppingCartDto.TotalPrice = shoppingCartDto.CartItems.Sum(
                selector: product => product.SubTotal);

            return Ok(ApiResponse.Success(shoppingCartDto));
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItemToCartAsync(
            AddCartItemDto cartItemDto,
            CancellationToken cancellationToken)
        {
            // Get the shopping cart from the in-memory dictionary.
            var getResult = ShoppingCartHelper.GetShoppingCart(cartItemDto.CartId);

            if (!getResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(getResult.ErrorMessages));
            }

            var shoppingCart = getResult.Value;

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

            return Ok(ApiResponse.Success(default));
        }

        [HttpDelete("{cartId:guid}/{productId:guid}")]
        public async Task<IActionResult> DeleteItemFromCartAsync(
            [FromRoute]
            [IsValidGuid] Guid cartId,
            [FromRoute]
            [IsValidGuid] Guid productId)
        {
            // Get the shopping cart from the cookie.
            var getResult = ShoppingCartHelper.GetShoppingCart(cartId);

            if (!getResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(getResult.ErrorMessages));
            }

            var shoppingCart = getResult.Value;

            shoppingCart.RemoveItem(productId);

            return Ok(ApiResponse.Success(default));
        }

        [HttpPut("decrease")]
        public async Task<IActionResult> DecreaseItemFromCartAsync(
            DecreaseCartItemDto cartItemDto)
        {
            // Get the shopping cart from the cookie.
            var getResult = ShoppingCartHelper.GetShoppingCart(cartItemDto.CartId);

            if (!getResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed(getResult.ErrorMessages));
            }

            var shoppingCart = getResult.Value;

            // Decrease the item from the shopping cart.
            shoppingCart.DecreaseItem(cartItemDto);

            ShoppingCartHelper.SetShoppingCart(HttpContext, shoppingCart);

            return Ok(ApiResponse.Success(default));
        }
    }
}
