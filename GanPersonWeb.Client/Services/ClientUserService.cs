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

        //��֤�Ƿ��ǹ���Ա
        public async Task<bool> IsAdmin()
        {
            var role = await jwtHelperService.GetClaim(ClaimTypes.Role);
            return role == "Admin";
        }

        //��֤�Ƿ��¼
        public async Task<bool> IsAuthenticated()
        {
            //�ȴӱ��ش洢�л�ȡJWT����
            var token = await jwtHelperService.GetTokenAsync();
            //�������Ϊ�գ����ʾδ��¼
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var expiration = jwtToken.ValidTo;
            //�������δ���ڣ����ʾ�ѵ�¼
            //����true������������ӵ�����ͷ��
            if (expiration> DateTime.UtcNow)
            {
                _httpClient.DefaultRequestHeaders.Authorization
                    = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                return true;
            }
            return false;
        }

        //�ǳ�
        public async Task LogoutAsync()
        {
            await jwtHelperService.SaveTokenAsync(string.Empty);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            OnLogoutSuccess?.Invoke();
        }

        //���ú�˽ӿڻ�ȡ�û���Ϣ
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
                    return new User { Username = "δ�ҵ�" };
                }
            }
            return new User { Username="δ��½"};
        }      
    }
}
