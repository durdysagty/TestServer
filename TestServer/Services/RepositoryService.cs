using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestModels;
using TestServer.Interfaces;

namespace TestServer.Services
{
    internal class RepositoryService : IRepository, IDisposable
    {
        private readonly TestDbContext _testDbContext;
        private readonly DbContextOptionsBuilder<TestDbContext> _optionsBuilder;

        public RepositoryService()
        {
            _optionsBuilder = new();
            _ = _optionsBuilder.UseSqlServer("Server=DESKTOP-U2F5M3R\\SQLEXPRESS;Database=TestDb;User ID=sa;Password=123456;MultipleActiveResultSets=true;TrustServerCertificate=True;");
            _testDbContext = new TestDbContext(_optionsBuilder.Options);
        }

        public async Task<User> FindUserByNameAsync(string userName)
        {
            // need to check null
            User user = await _testDbContext.Users.FirstOrDefaultAsync(x => x.Name == userName);
            return user;
        }

        public async Task<User> GetUserAsync(string name)
        {
            User user = await FindUserByNameAsync(name);
            if (user == null)
            {
                user = new()
                {
                    Name = name,
                    IsTestPassed = false
                };
                _testDbContext.Users.Add(user);
                await _testDbContext.SaveChangesAsync();
            }
            return user;
        }

        public async Task SaveUserChangesAsync(User user)
        {
            _testDbContext.Entry(user).State = EntityState.Modified;
            await _testDbContext.SaveChangesAsync();
        }

        public async Task AddUsedTestAsync(UsedTest usedTest)
        {
            await _testDbContext.UsedTests.AddAsync(usedTest);
            await _testDbContext.SaveChangesAsync();
        }

        public async Task<UsedTest> GetUserLastTest(int userId)
        {
            UsedTest usedTest = await _testDbContext.UsedTests.Where(ut => ut.UserId == userId).OrderByDescending(ut => ut.Id).FirstOrDefaultAsync();
            return usedTest;
        }

        public async Task SaveUserLastTestAsync(UsedTest usedTest)
        {
            _testDbContext.Entry(usedTest).State = EntityState.Modified;
            await _testDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _testDbContext.Dispose();
        }
    }
}
