using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;

namespace GanPersonWeb.Client.Services
{
    public class JwtHelperService
    {
        private readonly ILocalStorageService _localStorageService;

        public JwtHelperService(ILocalStorageService localStorageService) 
        {
            _localStorageService = localStorageService;

        }

        //保存JWT令牌
        public async Task SaveTokenAsync(string token)
        {
            await _localStorageService.SetItemAsync("authToken", token);
        }

        //读取JWT令牌
        public async Task<string> GetTokenAsync()
        {
            var token = await _localStorageService.GetItemAsync<string>("authToken");
            return token;
        }

        //获取JWT令牌Clainm信息
        public async Task<string> GetClaim(string claimType)
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim?.Value ?? string.Empty;
        }
    }
}
