using BusinessLogic.Services.Core;
using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.Commons.SystemConstants;
using DataAccess.DataSeedings;
using DataAccess.Entities;
using DTOs.Implementation.PayOs.Incomings;
using Helpers.ExtensionMethods;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs.Implementation.Checkouts.Outgoings;
using WebApi.DTOs.Implementation.Orders.Incomings;
using WebApi.DTOs.Implementation.Orders.Outgoings;
using WebApi.DTOs.Implementation.ShoppingCarts.Incomings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.AppConstants;

namespace WebApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        private readonly IUserProductService _userProductService;
        private readonly IPayOsService _payOsService;
        private readonly IUserOrderService _orderService;

        public UserOrderController(
            IUserProductService userProductService,
            IPayOsService payOsService,
            IUserOrderService orderService)
        {
            _userProductService = userProductService;
            _payOsService = payOsService;
            _orderService = orderService;
        }

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

            // Set again the guestId if it is removed.
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

            // Get the total amount to create the check out link for user order payment.
            var totalAmount = decimal.ToInt32(orderDto.Items.Sum(item => item.SellingPrice * item.SellingQuantity));

            // Save the order to the database with WaitForPurchased status.
            var order = new OrderEntity
            {
                Id = Guid.NewGuid(),
                OrderCode = orderDto.OrderCode,
                GuestId = guestId,
                UserId = userId,
                PaymentMethodId = PaymentMethods.OnlineBanking.Id,
                StatusId = OrderStatuses.WaitForPurchased.Id,
                OrderNote = billingDetailDto.OrderNote,
                TotalPrice = totalAmount,
                CreatedAt = DateTime.UtcNow,
                DeliveredAddress = billingDetailDto.DeliveryAddress,
                DeliveredAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };

            var orderItems = new List<OrderItemEntity>(orderDto.Items.Count());

            foreach (var item in orderDto.Items)
            {
                var orderItem = new OrderItemEntity
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    SellingPrice = item.SellingPrice,
                    SellingQuantity = item.SellingQuantity,
                };

                orderItems.Add(orderItem);
            }

            // If user is not logged in, then save the detail.
            var isUserNotLogIn = !userIdResult.IsSuccess;

            if (isUserNotLogIn)
            {
                order.OrderGuestDetail = new OrderGuestDetailEntity
                {
                    OrderId = order.Id,
                    GuestName = billingDetailDto.LastName + " " + billingDetailDto.FirstName,
                    Email = billingDetailDto.Email,
                    PhoneNumber = billingDetailDto.PhoneNumber,
                };
            }

            var saveResult = await _orderService.SaveOrderForCheckoutAsync(
                order: order,
                orderItems: orderItems,
                cancellationToken: cancellationToken);

            if (!saveResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            var createPaymentResult = await _payOsService.CreatePaymentResultFromOrderAsync(
                orderCode: orderDto.OrderCode,
                totalAmount: totalAmount);

            if (!createPaymentResult.IsSuccess)
            {
                return BadRequest(
                    ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
            }

            var checkoutInformation = createPaymentResult.Value;
            var checkoutInformationDto = new CheckoutInformationDto
            {
                OrderCode = orderDto.OrderCode,
                CheckoutUrl = checkoutInformation.checkoutUrl
            };

            // Clear the shopping cart for the customer.
            shoppingCart.Clear();
            ShoppingCartHelper.SetShoppingCart(HttpContext, shoppingCart);

            return Ok(ApiResponse.Success(checkoutInformationDto));
        }

        [HttpGet("cancel")]
        public async Task<IActionResult> CancelCheckoutAsync(
            [FromQuery] PayOsReturnDto returnDto,
            CancellationToken cancellationToken)
        {
            returnDto.NormalizeAllProperties();

            var isValidStatus = PayOsPaymentStatuses.Cancelled.Equals(returnDto.Status);

            if (!isValidStatus)
            {
                return BadRequest(ApiResponse.Failed("Invalid operation is found."));
            }

            // Check if the return dto contains valid information or not.
            var isCorrectStatus = await _payOsService.VerifyPaymentStatusByOrderCodeAsync(
                orderCode: returnDto.OrderCode,
                paymentStatus: PayOsPaymentStatuses.Cancelled);

            if (!isCorrectStatus)
            {
                return BadRequest(ApiResponse.Failed("Invalid operation is found."));
            }

            var removeOrderResult = await _orderService.RemoveOrderPermanentlyByOrderCodeAsync(
                returnDto.OrderCode,
                cancellationToken: cancellationToken);

            if (!removeOrderResult.IsSuccess)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            // Restore again the shopping cart based on the removed order.
            var removedOrder = removeOrderResult.Value;

            var shoppingCart = ShoppingCartHelper.GetShoppingCart(HttpContext);

            removedOrder.OrderItems.ForEach(orderItem =>
            {
                var cartItem = new AddCartItemDto
                {
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.SellingQuantity
                };

                shoppingCart.AddItem(cartItem);
            });

            ShoppingCartHelper.SetShoppingCart(HttpContext, shoppingCart);

            return Ok();
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmCheckoutAsync(
            [FromQuery] PayOsReturnDto returnDto,
            CancellationToken cancellationToken)
        {
            returnDto.NormalizeAllProperties();

            var isValidStatus = PayOsPaymentStatuses.Paid.Equals(returnDto.Status);

            if (!isValidStatus)
            {
                return BadRequest(ApiResponse.Failed("Invalid operation is found."));
            }

            // Check if the return dto contains valid information or not.
            var isCorrectStatus = await _payOsService.VerifyPaymentStatusByOrderCodeAsync(
                orderCode: returnDto.OrderCode,
                paymentStatus: PayOsPaymentStatuses.Paid);

            if (!isCorrectStatus)
            {
                return BadRequest(ApiResponse.Failed("Invalid operation is found."));
            }

            var confirmResult = await _orderService.ConfirmPaymentByOrderCodeAsync(
                orderCode: returnDto.OrderCode,
                cancellationToken: cancellationToken);

            if (!confirmResult)
            {
                return StatusCode(
                    statusCode: StatusCodes.Status500InternalServerError,
                    value: ApiResponse.Failed(ApiResponse.DefaultMessage.ServerError));
            }

            return Ok();
        }
    }
}
