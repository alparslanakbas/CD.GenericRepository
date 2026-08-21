using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
        }
    }

    private sealed class TestEntity
    {
        public int Id { get; set; }

        public required string Name { get; set; }
    }
}
