using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using System.Security.Claims;


namespace GanPersonWeb.Client.Services
{
    public class AuthService
    {
        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        private readonly HttpClient _httpClient;
        private readonly JwtHelperService jwtHelperService;
        public AuthService(HttpClient httpClient, JwtHelperService jwtHelperService)
        {
            _httpClient = httpClient;
            this.jwtHelperService = jwtHelperService;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", new { Username = username, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result != null)
                {
                    // Store the token in local storage
                    await jwtHelperService.SaveTokenAsync(result.Token);
                }
                return result?.Token;
            }
            return null;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/register", new { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }

        //验证是否是管理员
        public bool IsAdmin()
        {

            var role = jwtHelperService.GetClaim(ClaimTypes.Role);
            return role.Result == "Admin";
        }
    }
}
