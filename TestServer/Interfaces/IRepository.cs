using System.Threading.Tasks;
using TestModels;

namespace TestServer.Interfaces
{
    internal interface IRepository
    {
        Task<User> FindUserByNameAsync(string userName);
        Task<User> GetUserAsync(string name);
        Task SaveUserChangesAsync(User user);
        Task AddUsedTestAsync(UsedTest usedTest);
        Task<UsedTest> GetUserLastTest(int userId);
        Task SaveUserLastTestAsync(UsedTest usedTest);
        void Dispose();
    }
}
