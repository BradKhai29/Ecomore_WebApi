using BusinessLogic.Services.Core.Base;
using DataAccess.DataSeedings;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.Orders.Incomings;
using WebApi.DTOs.Implementation.Orders.Outgoings;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IUserProductService _userProductService;

        [HttpPost]
        public async Task<IActionResult> CheckoutAsync(
            OrderBillingDetailDto billingDetailDto,
            CancellationToken cancellationToken)
        {
            var shoppingCart = ShoppingCartHelper.GetShoppingCart(HttpContext);

            if (shoppingCart.IsEmpty())
            {
                return BadRequest(ApiResponse.Failed("Cannot checkout because shopping cart is empty."));
            }

            // Get the products from the shopping cart to create the order.
            var productIdList = shoppingCart.GetProductIdList();

            var products = await _userProductService.GetAllProductsFromIdListAsync(
                productIdList: productIdList,
                cancellationToken: cancellationToken);

            var userIdResult = HttpContextHelper.GetUserId(HttpContext);
            var userId = userIdResult.IsSuccess ? userIdResult.Value : AppUsers.DoNotRemove.Id;

            var guestIdResult = HttpContextHelper.GetGuestId(HttpContext);
            var guestId = guestIdResult.IsSuccess ? guestIdResult.Value : Guid.NewGuid();

            HttpContextHelper.SetGuestId(HttpContext, guestId);
            ShoppingCartHelper.SetGuestIdToShoppingCart(HttpContext, guestId);

            var orderDto = new OrderDto
            {
                OrderCode = OrderEntity.GenerateOrderCode(DateTime.Now),
                GuestId = guestId,
                UserId = userId,
                FirstName = billingDetailDto.FirstName,
                LastName = billingDetailDto.LastName,
                Email = billingDetailDto.Email,
                PhoneNumber = billingDetailDto.PhoneNumber,
                DeliveryAddress = billingDetailDto.DeliveryAddress,
                SaveInformation = billingDetailDto.SaveInformation,
                OrderNote = billingDetailDto.OrderNote,
                CreatedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddDays(7),
            };

            orderDto.Items = products.Select(product => new OrderItemDto
            {
                ProductId = product.Id,
                SellingPrice = product.UnitPrice,
                SellingQuantity = shoppingCart.GetItemQuantity(product.Id),
            });

            return Ok();
        }
    }
}
