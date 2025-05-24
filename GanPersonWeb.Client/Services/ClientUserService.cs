using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using System.Security.Claims;
using GanPersonWeb.Shared.Models;
using GanPersonWeb.Client.Pages.Console;


namespace GanPersonWeb.Client.Services
{
    public class ClientUserService
    {
        private class LoginResponse
        {
            public string Token { get; set; } = string.Empty;
        }

        private readonly HttpClient _httpClient;
        private readonly JwtHelperService jwtHelperService;

        public event Action? OnLoginSuccess;
        public event Action? OnLogoutSuccess;

        public ClientUserService(HttpClient httpClient, JwtHelperService jwtHelperService)
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
                    _httpClient.DefaultRequestHeaders.Authorization 
                        = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
                }
                OnLoginSuccess?.Invoke();
                return result?.Token;
            }
            return null;
        }

        public async Task<bool> RegisterAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/register", new { Username = username, Password = password });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RegisterAsync(string username, string password, string email)
        {
            var response = await _httpClient.PostAsJsonAsync("api/users/register", new { Username = username, Password = password ,Email = email});
            return response.IsSuccessStatusCode;
        }

        //验证是否是管理员
        public async Task<bool> IsAdmin()
        {
            var role = await jwtHelperService.GetClaim(ClaimTypes.Role);
            return role == "Admin";
        }

        //验证是否登录
        public async Task<bool> IsAuthenticated()
        {
            //先从本地存储中获取JWT令牌
            var token = await jwtHelperService.GetTokenAsync();
            //如果令牌为空，则表示未登录
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;
            //如果令牌未过期，则表示已登录
            //返回true，并将令牌添加到请求头中
            if (expiration> DateTime.UtcNow)
            {
                _httpClient.DefaultRequestHeaders.Authorization
                    = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            return false;
        }

        //登出
        public async Task LogoutAsync()
        {
            await jwtHelperService.SaveTokenAsync(string.Empty);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            OnLogoutSuccess?.Invoke();
        }

        //调用后端接口获取用户信息
        public async Task<User> GetMyselfUserInformation()
        {
            if(await IsAuthenticated())
            {
                var userInfo = await _httpClient.GetFromJsonAsync<User>("api/users/me");
                if (userInfo!=null)
                {
                    return userInfo;
                }
                else
                {
                    return new User { Username = "未找到" };
                }
            }
            return new User { Username="未登陆"};
        }      
    }
}
