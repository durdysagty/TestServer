using System.Threading.Tasks;
using TestModels;

namespace TestServer.Interfaces
{
    internal interface ILogin
    {
        Task<User> LoginAsync(string name);
    }
}
