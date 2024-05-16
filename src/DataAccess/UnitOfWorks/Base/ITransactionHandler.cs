namespace DataAccess.UnitOfWorks.Base;

/// <summary>
///     This interface contains methods to work with
///     the database transaction.
/// </summary>
public interface ITransactionHandler
{
    Task CreateTransactionAsync(CancellationToken cancellationToken);

    Task CommitTransactionAsync(CancellationToken cancellationToken);

    Task RollBackTransactionAsync(CancellationToken cancellationToken);

    ValueTask DisposeTransactionAsync(CancellationToken cancellationToken);
}
