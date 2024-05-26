using BusinessLogic.Models;
using DataAccess.DataSeedings;
using DataAccess.DbContexts;
using DataAccess.Entities;
using DataAccess.UnitOfWorks.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    
                    foundOrder.StatusId = OrderStatuses.Pending.Id;

                    await _unitOfWork.OrderRepository.BulkUpdateOrderStatusAsync(
                        orderToUpdate: foundOrder,
                        cancellationToken: cancellationToken);

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
    }
}
