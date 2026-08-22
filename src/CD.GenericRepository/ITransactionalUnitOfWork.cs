namespace CD.GenericRepository;

/// <summary>Executes multiple repository operations as a single database transaction.</summary>
public interface ITransactionalUnitOfWork : IUnitOfWork
{
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    Task<TResult> ExecuteInTransactionWithResultAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
