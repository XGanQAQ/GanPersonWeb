using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GanPersonWeb.Client.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/login", new { Username = username, Password = password });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            return null;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/register", new { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }

        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
