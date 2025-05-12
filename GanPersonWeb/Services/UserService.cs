using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GanPersonWeb.Services
{
    public class UserService
    {
        private readonly DatabaseService _databaseService;
        private readonly JwtService _jwtService;

        public UserService(DatabaseService databaseService, JwtService jwtService)
        {
            _databaseService = databaseService;
            _jwtService = jwtService;

        }

        public async Task RegisterAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            await _databaseService.AddAsync(user);
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var user = await _databaseService.GetAllAsync<User>()
                .ContinueWith(t => t.Result.FirstOrDefault(u => u.Username == username));

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null;

            return GenerateJwtToken(user);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _databaseService.GetByIdAsync<User>(id);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _databaseService.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _databaseService.DeleteAsync<User>(id);
        }

        private string GenerateJwtToken(User user)
        {
            return _jwtService.GenerateToken(user.Id.ToString(), user.Role);
        }
    }
}
