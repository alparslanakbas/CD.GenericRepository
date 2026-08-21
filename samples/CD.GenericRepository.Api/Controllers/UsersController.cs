using CD.Results;
using Microsoft.AspNetCore.Mvc;

namespace CD.GenericRepository.Api;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        Result<User> result = await userService.GetByIdAsync(id, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user, CancellationToken cancellationToken)
    {
        Result<User> result = await userService.AddAsync(user, cancellationToken);
        return StatusCode((int)result.StatusCode, result);
    }
}

public sealed class User
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }
}

public interface IUserRepository : IRepository<User>;

public sealed class UserRepository(AppDbContext context)
    : Repository<User, AppDbContext>(context), IUserRepository;

public interface IUserService
{
    Task<Result<User>> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<Result<User>> AddAsync(User user, CancellationToken cancellationToken = default);
}

public sealed class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    : IUserService
{
    public async Task<Result<User>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        User? user = await userRepository.GetAsync(
            candidate => candidate.Id == id,
            cancellationToken: cancellationToken);

        return user is null
            ? Result<User>.NotFound($"User not found with id: {id}")
            : Result<User>.Success(user);
    }

    public async Task<Result<User>> AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<User>.Created(user);
    }
}
