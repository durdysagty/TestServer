using System.Threading.Tasks;
using TestModels;
using TestServer.Interfaces;

namespace TestServer.Services
{
    internal class LoginService : ILogin
    {
        private readonly IRepository _repository;

        public LoginService(IRepository repositoryService)
        {
            _repository = repositoryService;
        }
        public async Task<User> LoginAsync(string name)
        {
            User user = await _repository.GetUserAsync(name);
            OnlineUsers.List.Add(user);
            return user;
        }
    }
}
