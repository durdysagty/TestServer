using System.Threading.Tasks;
using TestModels;

namespace TestServer.Interfaces
{
    internal interface ITest
    {
        Task<Test> GetTest(string userName);
        Task<bool> IsTestPassed(UserAnswer userAnswer);
    }
}
