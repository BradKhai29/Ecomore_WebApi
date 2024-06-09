using BusinessLogic.Models;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Services.Core.Implemetation
{
    internal class UserOrderService :
        IUserOrderService
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;

        public UserOrderService(IUnitOfWork<AppDbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> SaveOrderForCheckoutAsync(
            OrderEntity order,
            IEnumerable<OrderItemEntity> orderItems, CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                try
                {
                    await _unitOfWork.OrderRepository.AddAsync(
                        newEntity: order,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.OrderItemRepository.AddRangeAsync(
                        newEntities: orderItems,
                        cancellationToken: cancellationToken);

                    if (!Equals(order.OrderGuestDetail, null))
                    {
                        await _unitOfWork.OrderGuestDetailRepository.AddAsync(
                            newEntity: order.OrderGuestDetail,
                            cancellationToken: cancellationToken);
                    }

                    await _unitOfWork.SaveChangesToDatabaseAsync(cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                    result = true;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public async Task<AppResult<OrderEntity>> RemoveOrderPermanentlyByOrderCodeAsync(
            long orderCode,
            CancellationToken cancellationToken)
        {
            var result = AppResult<OrderEntity>.Failed();
            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(operation: async () =>
            {
                await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                try
                {
                    var order = await _unitOfWork.OrderRepository.FindOrderByOrderCodeAsync(
                        orderCode: orderCode,
                        cancellationToken: cancellationToken);

                    if (!Equals(order, null))
                    {
                        _unitOfWork.OrderRepository.Remove(order);

                        await _unitOfWork.SaveChangesToDatabaseAsync(
                            cancellationToken: cancellationToken);

                        await _unitOfWork.CommitTransactionAsync(
                            cancellationToken: cancellationToken);

                        result = AppResult<OrderEntity>.Success(order);
                    }
                }
                catch
                {
                    await _unitOfWork.RollBackTransactionAsync(
                        cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        // This method has complex business logic.
        public async Task<bool> ConfirmPaymentByOrderCodeAsync(
            long orderCode,
            CancellationToken cancellationToken)
        {
            var result = false;

            var executionStrategy = _unitOfWork.CreateExecutionStrategy();

            await executionStrategy.ExecuteAsync(async () =>
            {
                await _unitOfWork.CreateTransactionAsync(cancellationToken: cancellationToken);

                try
                {
                    var foundOrder = await _unitOfWork.OrderRepository.FindOrderByOrderCodeAsync(
                        orderCode: orderCode,
                        cancellationToken: cancellationToken);

                    var purchaseProductIds = foundOrder.OrderItems.Select(orderItem => orderItem.ProductId);

                    var purchaseProducts = await _unitOfWork.ProductRepository.FindAllProductsByIdListForOrderConfirmAsync(
                        productIds: purchaseProductIds,
                        cancellationToken: cancellationToken);

                    var isValidToConfirm = true;

                    foreach (var purchaseProduct in purchaseProducts)
                    {
                        var purchaseOrderItem = foundOrder.OrderItems.FirstOrDefault(
                            predicate: item => item.ProductId == purchaseProduct.Id);

                        if (purchaseOrderItem == null)
                        {
                            isValidToConfirm = false;
                            break;
                        }

                        // Process to decrease the quantity of the purchased product item.
                        purchaseProduct.QuantityInStock -= purchaseOrderItem.SellingQuantity;

                        var isValid = purchaseProduct.QuantityInStock >= 0;
                        if (!isValid)
                        {
                            isValidToConfirm = false;
                            break;
                        }
                    }

                    if (!isValidToConfirm)
                    {
                        foundOrder.StatusId = OrderStatuses.Cancelled.Id;
                    }
                    else
                    {
                        foundOrder.StatusId = OrderStatuses.Pending.Id;
                        
                        foreach(var product in purchaseProducts)
                        {
                            await _unitOfWork.ProductRepository.BulkUpdateQuantityInStockAsync(
                                productToUpdate: product,
                                cancellationToken: cancellationToken);
                        }
                    }

                    await _unitOfWork.OrderRepository.BulkUpdateOrderStatusAsync(
                        orderToUpdate: foundOrder,
                        cancellationToken: cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);

                    result = isValidToConfirm;
                }
                catch (Exception)
                {
                    await _unitOfWork.RollBackTransactionAsync(cancellationToken: cancellationToken);
                }
                finally
                {
                    await _unitOfWork.DisposeTransactionAsync(cancellationToken: cancellationToken);
                }
            });

            return result;
        }

        public Task<IEnumerable<OrderEntity>> GetAllOrdersByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.OrderRepository.GetAllOrdersByUserIdAsync(
                userId: userId,
                cancellationToken: cancellationToken);
        }

        public Task<IEnumerable<OrderEntity>> GetAllOrdersByGuestIdAsync(
            Guid guestId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.OrderRepository.GetAllOrdersByGuestIdAsync(
                guestId: guestId,
                cancellationToken: cancellationToken);
        }

        public Task<bool> IsOrderExistedByIdAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.OrderRepository.IsFoundByExpressionAsync(
                findExpresison: order => order.Id == orderId,
                cancellationToken: cancellationToken);
        }

        public Task<OrderEntity> FindOrderByIdForDetailDisplayAsync(
            Guid orderId,
            CancellationToken cancellationToken)
        {
            return _unitOfWork.OrderRepository.FindOrderByIdForDetailDisplayAsync(
                orderId: orderId,
                cancellationToken: cancellationToken);
        }
    }
}
