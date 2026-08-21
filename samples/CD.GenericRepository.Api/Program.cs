using CD.GenericRepository;
using CD.GenericRepository.Api;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("GenericRepositorySample"));
builder.Services.AddScoped<IUnitOfWork>(services => services.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
