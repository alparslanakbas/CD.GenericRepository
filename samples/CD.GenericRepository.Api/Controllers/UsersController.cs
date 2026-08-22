using CD.Results;
using CD.Results.AspNetCore;
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
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        Result<PagedList<UserSummary>> result = await userService
            .GetPageAsync(page, pageSize, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create(User user, CancellationToken cancellationToken)
    {
        Result<User> result = await userService.AddAsync(user, cancellationToken);
        return result.ToActionResult(StatusCodes.Status201Created);
    }
}

public sealed record UserSummary(int Id, string Name);

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

    Task<Result<PagedList<UserSummary>>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

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
        return Result<User>.Success(user);
    }

    public async Task<Result<PagedList<UserSummary>>> GetPageAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100)
        {
            return Result<PagedList<UserSummary>>.Validation(
                "Page must be positive and pageSize must be between 1 and 100.",
                "pagination.invalid");
        }

        PagedList<UserSummary> users = await userRepository.PageAsync(
            user => new UserSummary(user.Id, user.Name),
            user => user.Name,
            page,
            pageSize,
            cancellationToken: cancellationToken);

        return Result<PagedList<UserSummary>>.Success(users);
    }
}
