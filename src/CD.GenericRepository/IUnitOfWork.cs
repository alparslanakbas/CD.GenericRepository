namespace CD.GenericRepository;

/// <summary>Represents the transaction boundary used to persist repository changes.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    int SaveChanges();
}
