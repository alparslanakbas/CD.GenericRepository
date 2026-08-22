using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CD.GenericRepository.Tests;

public sealed class RepositoryTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private TestDbContext _context = null!;
    private Repository<TestEntity> _repository = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();

        DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(_connection)
            .Options;

        _context = new TestDbContext(options);
        await _context.Database.EnsureCreatedAsync();
        _repository = new Repository<TestEntity>(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task Query_methods_do_not_track_by_default()
    {
        await SeedAsync(new TestEntity { Name = "Ada" });
        _context.ChangeTracker.Clear();

        TestEntity? entity = await _repository.GetAsync(candidate => candidate.Name == "Ada");

        Assert.NotNull(entity);
        Assert.Empty(_context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task Query_methods_can_enable_tracking_explicitly()
    {
        await SeedAsync(new TestEntity { Name = "Grace" });
        _context.ChangeTracker.Clear();

        TestEntity? entity = await _repository.GetAsync(
            candidate => candidate.Name == "Grace",
            asTracking: true);

        Assert.NotNull(entity);
        Assert.Single(_context.ChangeTracker.Entries<TestEntity>());
    }

    [Fact]
    public async Task RemoveByIdAsync_returns_false_when_entity_does_not_exist()
    {
        bool removed = await _repository.RemoveByIdAsync(404);

        Assert.False(removed);
        Assert.Empty(_context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task RemoveByIdAsync_marks_existing_entity_for_deletion()
    {
        TestEntity entity = new() { Name = "Linus" };
        await SeedAsync(entity);
        _context.ChangeTracker.Clear();

        bool removed = await _repository.RemoveByIdAsync(entity.Id);
        await _context.SaveChangesAsync();

        Assert.True(removed);
        Assert.False(await _repository.ExistsAsync(candidate => candidate.Id == entity.Id));
    }

    [Fact]
    public async Task Async_queries_propagate_cancellation()
    {
        using CancellationTokenSource source = new();
        await source.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _repository.ListAsync(cancellationToken: source.Token));
    }

    [Fact]
    public async Task Projection_materializes_only_the_requested_shape()
    {
        await SeedAsync(new TestEntity { Name = "Ada" }, new TestEntity { Name = "Grace" });

        IReadOnlyList<EntitySummary> items = await _repository.ProjectAsync(
            entity => new EntitySummary(entity.Id, entity.Name),
            entity => entity.Name.StartsWith('A'));

        EntitySummary item = Assert.Single(items);
        Assert.Equal("Ada", item.Name);
    }

    [Fact]
    public async Task PageAsync_returns_ordered_metadata_and_projected_items()
    {
        await SeedAsync(
            new TestEntity { Name = "Delta" },
            new TestEntity { Name = "Alpha" },
            new TestEntity { Name = "Charlie" },
            new TestEntity { Name = "Bravo" });

        PagedList<EntitySummary> page = await _repository.PageAsync(
            entity => new EntitySummary(entity.Id, entity.Name),
            entity => entity.Name,
            pageNumber: 2,
            pageSize: 2);

        Assert.Equal(["Charlie", "Delta"], page.Items.Select(item => item.Name));
        Assert.Equal(4, page.TotalCount);
        Assert.Equal(2, page.TotalPages);
        Assert.True(page.HasPreviousPage);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async Task Update_persists_a_detached_entity()
    {
        TestEntity original = new() { Name = "Before" };
        await SeedAsync(original);
        _context.ChangeTracker.Clear();

        _repository.Update(new TestEntity { Id = original.Id, Name = "After" });
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        TestEntity? updated = await _repository.GetAsync(entity => entity.Id == original.Id);
        Assert.Equal("After", updated?.Name);
    }

    [Fact]
    public async Task RemoveByKeyAsync_supports_composite_primary_keys()
    {
        CompositeEntity entity = new() { Tenant = "acme", Number = 7, Value = "saved" };
        _context.Add(entity);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        Repository<CompositeEntity> repository = new(_context);

        bool removed = await repository.RemoveByKeyAsync(["acme", 7]);
        await _context.SaveChangesAsync();

        Assert.True(removed);
        Assert.False(await repository.ExistsAsync(item => item.Tenant == "acme" && item.Number == 7));
    }

    [Fact]
    public async Task Transaction_rolls_back_changes_when_an_operation_fails()
    {
        EfUnitOfWork<TestDbContext> unitOfWork = new(_context);

        await Assert.ThrowsAsync<InvalidOperationException>(() => unitOfWork.ExecuteInTransactionAsync(
            async cancellationToken =>
            {
                await _repository.AddAsync(new TestEntity { Name = "Rollback" }, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                throw new InvalidOperationException("stop");
            }));

        _context.ChangeTracker.Clear();
        Assert.False(await _repository.ExistsAsync(entity => entity.Name == "Rollback"));
    }

    [Fact]
    public async Task Transaction_commits_and_saves_once_the_operation_succeeds()
    {
        EfUnitOfWork<TestDbContext> unitOfWork = new(_context);

        int value = await unitOfWork.ExecuteInTransactionWithResultAsync(
            async cancellationToken =>
            {
                await _repository.AddAsync(new TestEntity { Name = "Committed" }, cancellationToken);
                return 42;
            });

        _context.ChangeTracker.Clear();
        Assert.Equal(42, value);
        Assert.True(await _repository.ExistsAsync(entity => entity.Name == "Committed"));
    }

    [Fact]
    public async Task Dependency_injection_registration_resolves_repository_and_shared_unit_of_work()
    {
        ServiceCollection services = new();
        services.AddDbContext<TestDbContext>(options => options.UseSqlite(_connection));
        services.AddGenericRepositories<TestDbContext>();

        await using ServiceProvider provider = services.BuildServiceProvider();
        await using AsyncServiceScope scope = provider.CreateAsyncScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IRepository<TestEntity>>());
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        ITransactionalUnitOfWork transactional = scope.ServiceProvider
            .GetRequiredService<ITransactionalUnitOfWork>();
        Assert.Same(unitOfWork, transactional);
    }

    private async Task SeedAsync(params TestEntity[] entities)
    {
        await _repository.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }

    private sealed class TestDbContext(DbContextOptions<TestDbContext> options)
        : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(entity => entity.Id);
            modelBuilder.Entity<CompositeEntity>().HasKey(entity => new { entity.Tenant, entity.Number });
        }
    }

    private sealed class TestEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; }
    }

    private sealed class CompositeEntity
    {
        public required string Tenant { get; set; }

        public int Number { get; set; }

        public required string Value { get; set; }
    }

    private sealed record EntitySummary(int Id, string Name);
}
