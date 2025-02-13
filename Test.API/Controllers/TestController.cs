using GenericRepository.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {

    }

    public sealed class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public interface IUserRepository : IRepository<User>
    {

    }

    public class UserRepository : Repository<User, AppDbContext>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }
    }

    public interface IUserService
    {
        Task<User> GetByIdAsync(int id);
        void Add(User user);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(User user)
        {
            _userRepository.Add(user);
            _unitOfWork.SaveChanges();
        }

        public async Task<User> GetByIdAsync(int id)
            => await _userRepository.GetByExpressionAsync(x => x.Id == id);

       
    }
}
