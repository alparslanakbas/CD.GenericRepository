using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace CD.GenericRepository;

/// <summary>Provides save and transaction boundaries for an Entity Framework Core context.</summary>
public sealed class EfUnitOfWork<TContext> : ITransactionalUnitOfWork
    where TContext : DbContext
{
    private readonly TContext _context;

    public EfUnitOfWork(TContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public int SaveChanges() => _context.SaveChanges();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await ExecuteInTransactionWithResultAsync(
            async token =>
            {
                await operation(token).ConfigureAwait(false);
                return true;
            },
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResult> ExecuteInTransactionWithResultAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        IExecutionStrategy strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await _context.Database
                .BeginTransactionAsync(cancellationToken)
                .ConfigureAwait(false);

            TResult result = await operation(cancellationToken).ConfigureAwait(false);
            await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }).ConfigureAwait(false);
    }
}
