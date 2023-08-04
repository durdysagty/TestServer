using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TestModels;

namespace TestServer
{
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<TestDbContext>();
            builder.UseSqlServer("Server=DESKTOP-U2F5M3R\\SQLEXPRESS;Database=TestDb;User ID=sa;Password=123456;MultipleActiveResultSets=true;TrustServerCertificate=True;");

            return new TestDbContext(builder.Options);
        }
    }

    internal class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UsedTest> UsedTests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().HasIndex(i => i.Name).IsUnique();

            builder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "User",
                    IsTestPassed = true,
                },
                new User
                {
                    Id = 2,
                    Name = "Bob",
                    IsTestPassed = false,
                },
                new User
                {
                    Id = 3,
                    Name = "Vadim",
                    IsTestPassed = true,
                },
                new User
                {
                    Id = 4,
                    Name = "David",
                    IsTestPassed = false,
                },
                new User
                {
                    Id = 5,
                    Name = "Anna",
                    IsTestPassed = true,
                },
                new User
                {
                    Id = 6,
                    Name = "Vasya",
                    IsTestPassed = false,
                },
                new User
                {
                    Id = 7,
                    Name = "Ivan",
                    IsTestPassed = true,
                },
                new User
                {
                    Id = 8,
                    Name = "Helen",
                    IsTestPassed = false,
                },
                new User
                {
                    Id = 9,
                    Name = "Igor",
                    IsTestPassed = true,
                },
                new User
                {
                    Id = 10,
                    Name = "Jane",
                    IsTestPassed = false,
                });
        }
    }
}
