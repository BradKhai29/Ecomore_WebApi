using BusinessLogic.Services.Core;
using BusinessLogic.Services.Core.Base;
using BusinessLogic.Services.External.Base;
using DataAccess.Commons.SystemConstants;
using DataAccess.DataSeedings;
using DataAccess.Entities;
using DTOs.Implementation.PayOs.Incomings;
using Helpers.ExtensionMethods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Options.Models;
using WebApi.DTOs.Implementation.Checkouts.Outgoings;
using WebApi.DTOs.Implementation.Orders.Incomings;
using WebApi.DTOs.Implementation.Orders.Outgoings;
using WebApi.DTOs.Implementation.ShoppingCarts.Incomings;
using WebApi.Helpers;
using WebApi.Models;
using WebApi.Shared.AppConstants;
using WebApi.Shared.ValidationAttributes;

namespace WebApi.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class UserOrderController : ControllerBase
    {
        private readonly IUserProductService _userProductService;
        private readonly IPayOsService _payOsService;
        private readonly IUserOrderService _orderService;
        private readonly ProtectionOptions _dataProtectionOptions;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public UserOrderController(
            IUserProductService userProductService,
            IPayOsService payOsService,
            IUserOrderService orderService,
            ProtectionOptions dataProtectionOptions,
            IDataProtectionProvider dataProtectionProvider)
        {
            _userProductService = userProductService;
            _payOsService = payOsService;
            _orderService = orderService;
            _dataProtectionOptions = dataProtectionOptions;
            _dataProtectionProvider = dataProtectionProvider;
        }

        [HttpPost]
        public async Task<IActionResult> CheckoutAsync(
            OrderBillingDetailDto billingDetailDto,
            CancellationToken cancellationToken)
        {
            var getResult = ShoppingCartHelper.GetShoppingCart(billingDetailDto.CartId);

            if (!getResult.IsSuccess)
            {
                return BadRequest(ApiResponse.Failed("CartId is not found."));
            }

            var shoppingCart = getResult.Value;

            if (shoppingCart.IsEmpty())
            {
                return BadRequest(ApiResponse.Failed("Cannot checkout because shopping cart is empty."));
            }

            // Get the products from the shopping cart to create the order.
            var productIdList = shoppingCart.GetProductIdList();

            var products = await _userProductService.GetAllProductsFromIdListForDisplayShoppingCartAsync(
                productIdList: productIdList,
                cancellationToken: cancellationToken);

            var userIdResult = HttpContextHelper.GetUserId(HttpContext);
            var userId = userIdResult.IsSuccess ? userIdResult.Value : AppUsers.DoNotRemove.Id;

            var guestId = shoppingCart.GuestId;

            // Save the order to the database with WaitForPurchased status.
            var dateTimeUtcNow = DateTime.UtcNow;
            var order = new OrderEntity
            {
                Id = Guid.NewGuid(),
                OrderCode = OrderEntity.GenerateOrderCode(dateTimeUtcNow),
                GuestId = guestId,
                UserId = userId,
                PaymentMethodId = PaymentMethods.OnlineBanking.Id,
                StatusId = OrderStatuses.WaitForPurchased.Id,
                OrderNote = billingDetailDto.OrderNote,
                CreatedAt = dateTimeUtcNow,
                DeliveredAddress = billingDetailDto.DeliveryAddress,
                DeliveredAt = dateTimeUtcNow,
                UpdatedAt = dateTimeUtcNow,
                UpdatedBy = DefaultValues.SystemId,
            };

            // Get all product items from the customer shopping cart to create order item list.
            var orderItems = products.Select(product => new OrderItemEntity
            {
                OrderId = order.Id,
                ProductId = product.Id,
                SellingPrice = product.UnitPrice,
                SellingQuantity = shoppingCart.GetItemQuantity(product.Id),
            });

            // Get the total amount to create the check out link for order payment.
            var totalAmount = decimal.ToInt32(
                d: orderItems.Sum(item => item.SellingPrice * item.SellingQuantity));
            order.TotalPrice = totalAmount;

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

            // Save the order to the database.
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
                orderCode: order.OrderCode,
                totalAmount: totalAmount);

            if (!createPaymentResult.IsSuccess)
            {
                return BadRequest(
                    ApiResponse.Failed(ApiResponse.DefaultMessage.ServiceError));
            }

            // Create the state code and return to the client for security purpose.
            var protector = _dataProtectionProvider.CreateProtector(_dataProtectionOptions.PurposeKey);

            var checkoutInformation = createPaymentResult.Value;
            var checkoutInformationDto = new CheckoutInformationDto
            {
                StateCode = protector.Protect($"{order.OrderCode}"),
                CheckoutUrl = checkoutInformation.checkoutUrl
            };

            // Clear the shopping cart for the customer.
            shoppingCart.Clear();

            return Ok(ApiResponse.Success(checkoutInformationDto));
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelCheckoutAsync(
            [FromBody] PayOsReturnDto returnDto,
            CancellationToken cancellationToken)
        {
            returnDto.NormalizeAllProperties();

            // Verify if the state code is valid or not.
            var protector = _dataProtectionProvider.CreateProtector(_dataProtectionOptions.PurposeKey);
            var stateCode = protector.Unprotect(returnDto.StateCode);

            if (!stateCode.Equals(returnDto.OrderCode.ToString()))
            {
                return BadRequest(ApiResponse.Failed("Invalid request credentials."));
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

            var getResult = ShoppingCartHelper.GetShoppingCart(removedOrder.GuestId);

            if (getResult.IsSuccess)
            {
                var shoppingCart = getResult.Value;

                removedOrder.OrderItems.ForEach(orderItem =>
                {
                    var cartItem = new AddCartItemDto
                    {
                        ProductId = orderItem.ProductId,
                        Quantity = orderItem.SellingQuantity
                    };

                    shoppingCart.AddItem(cartItem);
                });
            }

            return Ok(ApiResponse.Success(default));
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmCheckoutAsync(
            [FromBody] PayOsReturnDto returnDto,
            CancellationToken cancellationToken)
        {
            returnDto.NormalizeAllProperties();

            // Verify if the state code is valid or not.
            var protector = _dataProtectionProvider.CreateProtector(_dataProtectionOptions.PurposeKey);
            var stateCode = protector.Unprotect(returnDto.StateCode);

            if (!stateCode.Equals(returnDto.OrderCode.ToString()))
            {
                return BadRequest(ApiResponse.Failed("Invalid request credentials."));
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

        [HttpGet("user")]
        [Authorize(AuthenticationSchemes = CustomAuthenticationSchemes.UserAccountScheme)]
        public async Task<IActionResult> GetAllUserOrdersAsync(
            CancellationToken cancellationToken)
        {
            var getResult = HttpContextHelper.GetUserId(HttpContext);

            if (!getResult.IsSuccess)
            {
                return Unauthorized(ApiResponse.Failed(ApiResponse.DefaultMessage.InvalidToken));
            }

            var userId = getResult.Value;

            var orders = await _orderService.GetAllOrdersByUserIdAsync(
                userId: userId,
                cancellationToken: cancellationToken);

            var orderHistoryItems = orders.Select(order => new OrderHistoryItemDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CreatedAt = order.CreatedAt,
                ItemCount = order.OrderItems.Sum(orderItem => orderItem.SellingQuantity),
                TotalPrice = order.TotalPrice,
                OrderStatus = OrderStatuses.GetStatusOrderity(order.StatusId),
            });

            return Ok(ApiResponse.Success(orderHistoryItems));
        }

        [HttpGet("guest/{guestId:guid}")]
        public async Task<IActionResult> GetAllOrdersByGuestIdAsync(
            [IsValidGuid] Guid guestId,
            CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrdersByGuestIdAsync(
                guestId: guestId,
                cancellationToken: cancellationToken);

            var orderHistoryItems = orders.Select(order => new OrderHistoryItemDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CreatedAt = order.CreatedAt,
                TotalPrice = order.TotalPrice,
                OrderStatus = OrderStatuses.GetStatusOrderity(order.StatusId),
            });

            return Ok(ApiResponse.Success(orderHistoryItems));
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrderByIdForDetailDisplayAsync(
            [IsValidGuid] Guid orderId,
            CancellationToken cancellationToken)
        {
            var isExisted = await _orderService.IsOrderExistedByIdAsync(
                orderId: orderId,
                cancellationToken: cancellationToken);

            if (!isExisted)
            {
                return NotFound(ApiResponse.Failed($"Order with Id [{orderId}] is not found."));
            }

            // Get order from the database.
            var foundOrder = await _orderService.FindOrderByIdForDetailDisplayAsync(
                orderId: orderId,
                cancellationToken: cancellationToken);

            var purchasedProductIdList = foundOrder.OrderItems.Select(item => item.ProductId);

            var purchasedProducts = await _userProductService.GetAllProductsFromIdListForDisplayOrderAsync(
                productIdList: purchasedProductIdList,
                cancellationToken: cancellationToken);

            // Create dto object for response.
            var orderDetailDto = new OrderDetailDto
            {
                Id = orderId,
                OrderCode = foundOrder.OrderCode,
                CreatedAt = foundOrder.CreatedAt,
                OrderStatus = OrderStatuses.GetStatusOrderity(foundOrder.StatusId),
                OrderItems = new List<OrderItemDto>(),
                TotalPrice = foundOrder.TotalPrice,
            };

            foreach (var purchasedProduct in purchasedProducts)
            {
                var orderItem = foundOrder.OrderItems.FirstOrDefault(item => item.ProductId == purchasedProduct.Id);
                var orderItemDto = new OrderItemDto
                {
                    ProductId = purchasedProduct.Id,
                    ProductName = purchasedProduct.Name,
                    SellingPrice = orderItem.SellingPrice,
                    SellingQuantity = orderItem.SellingQuantity,
                };

                orderDetailDto.OrderItems.Add(orderItemDto);
            }

            orderDetailDto.OrderItems.TrimExcess();

            return Ok(ApiResponse.Success(orderDetailDto));
        }
    }
}
