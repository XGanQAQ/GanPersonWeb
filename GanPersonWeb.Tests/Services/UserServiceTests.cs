using GanPersonWeb.Data;
using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GanPersonWeb.Tests.Services
{
    public class UserServiceTests
    {
        private DbContextOptions<GanPersonDbContext> CreateNewContextOptions()
        {
            return new DbContextOptionsBuilder<GanPersonDbContext>()
                .UseInMemoryDatabase($"TestDatabase_{Guid.NewGuid()}")
                .Options;
        }

        private IConfiguration CreateConfiguration()
        {
            var inMemorySettings = new Dictionary<string, string>
           {
               { "JwtSettings:Issuer", "GanPersonWeb" },
               { "JwtSettings:Audience", "GanPersonWebUsers" },
               { "JwtSettings:SecretKey", "ThisIsASecretKeyForJwtToken12345" }
           };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task Register_ShouldHashPassword()
        {
            var options = CreateNewContextOptions();
            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var configuration = CreateConfiguration();
                var jwtService = new JwtService(configuration);
                var userService = new UserService(databaseService, jwtService);

                var user = new User { Username = "testuser", Password = "password123" };
                await userService.RegisterAsync(user);

                var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
                Assert.NotNull(savedUser);
                Assert.NotEqual("password123", savedUser.Password); // Password should be hashed
            }
        }

        [Fact]
        public async Task Login_ShouldReturnJwtToken()
        {
            var options = CreateNewContextOptions();
            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var configuration = CreateConfiguration();
                var jwtService = new JwtService(configuration);
                var userService = new UserService(databaseService, jwtService);

                var user = new User { Username = "testuser", Password = "password123" };
                await userService.RegisterAsync(user);

                var token = await userService.LoginAsync("testuser", "password123");
                Assert.NotNull(token);
            }
        }

        [Fact]
        public async Task Login_ShouldReturnNullForInvalidCredentials()
        {
            var options = CreateNewContextOptions();
            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var configuration = CreateConfiguration();
                var jwtService = new JwtService(configuration);
                var userService = new UserService(databaseService, jwtService);

                var user = new User { Username = "testuser", Password = "password123" };
                await userService.RegisterAsync(user);

                var token = await userService.LoginAsync("testuser", "wrongpassword");
                Assert.Null(token);
            }
        }

        [Fact]
        public async Task GetUserById_ShouldReturnCorrectUser()
        {
            var options = CreateNewContextOptions();
            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var configuration = CreateConfiguration();
                var jwtService = new JwtService(configuration);
                var userService = new UserService(databaseService, jwtService);

                var user = new User { Username = "testuser", Password = "password123" };
                await userService.RegisterAsync(user);

                var retrievedUser = await userService.GetUserByIdAsync(user.Id);
                Assert.NotNull(retrievedUser);
                Assert.Equal("testuser", retrievedUser?.Username);
            }
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyUser()
        {
            var options = CreateNewContextOptions();
            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var configuration = CreateConfiguration();
                var jwtService = new JwtService(configuration);
                var userService = new UserService(databaseService, jwtService);

                var user = new User { Username = "testuser", Password = "password123" };
                await userService.RegisterAsync(user);

                user.Username = "updateduser";
                await userService.UpdateUserAsync(user);

                var updatedUser = await context.Users.FindAsync(user.Id);
                Assert.NotNull(updatedUser);
                Assert.Equal("updateduser", updatedUser?.Username);
            }
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUser()
        {
            var options = CreateNewContextOptions();
            using (var context = new GanPersonDbContext(options))
            {
                var databaseService = new DatabaseService(context);
                var configuration = CreateConfiguration();
                var jwtService = new JwtService(configuration);
                var userService = new UserService(databaseService, jwtService);

                var user = new User { Username = "testuser", Password = "password123" };
                await userService.RegisterAsync(user);

                await userService.DeleteUserAsync(user.Id);

                var deletedUser = await context.Users.FindAsync(user.Id);
                Assert.Null(deletedUser);
            }
        }
    }
}
