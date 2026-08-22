using Microsoft.EntityFrameworkCore;

namespace CD.GenericRepository.Api;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}
