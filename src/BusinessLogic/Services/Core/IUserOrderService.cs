using BusinessLogic.Models;
using DataAccess.Entities;

namespace BusinessLogic.Services.Core
{
    public interface IUserOrderService
    {
        /// <summary>
        ///     Save the order into the database 
        ///     with WaitForPurchased order status.
        /// </summary>
        /// <remarks>
        ///     This order will be later update to pending status
        ///     if the user completes their order payment.
        /// </remarks>
        /// <param name="order"></param>
        /// <param name="orderItems"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The <see cref="Task{TResult}"/> that represent the
        ///     result of the operation.
        /// </returns>
        Task<bool> SaveOrderForCheckoutAsync(
            OrderEntity order,
            IEnumerable<OrderItemEntity> orderItems,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Permanently remove the order with specified 
        ///     <paramref name="orderCode"/> from the database.
        ///     And return the information of the removed order.
        /// </summary>
        /// <param name="orderCode">
        ///     The order code that used to specify the order to remove.
        /// </param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        ///     The <see cref="OrderEntity"/> instance that contains the information of the removed order.
        /// </returns>
        Task<AppResult<OrderEntity>> RemoveOrderPermanentlyByOrderCodeAsync(
            long orderCode,
            CancellationToken cancellationToken);

        /// <summary>
        ///     Confirm the order payment by the order code.
        /// </summary>
        /// <param name="orderCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> ConfirmPaymentByOrderCodeAsync(
            long orderCode,
            CancellationToken cancellationToken);

        Task<IEnumerable<OrderEntity>> GetAllOrdersByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

        Task<IEnumerable<OrderEntity>> GetAllOrdersByGuestIdAsync(
            Guid guestId,
            CancellationToken cancellationToken);

        Task<bool> IsOrderExistedByIdAsync(
            Guid orderId,
            CancellationToken cancellationToken);

        Task<OrderEntity> FindOrderByIdForDetailDisplayAsync(
            Guid orderId,
            CancellationToken cancellationToken);
    }
}
