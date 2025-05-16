using GanPersonWeb.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GanPersonWeb.Client.Services
{
    public class ClientPersonInfoService
    {
        private readonly HttpClient _httpClient;

        public ClientPersonInfoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// 获取个人信息
        /// </summary>
        public async Task<PersonalInfo?> GetPersonalInfoAsync()
        {
            // 调用后端 API 获取个人信息
            return await _httpClient.GetFromJsonAsync<PersonalInfo>("api/PersonalInfo");
        }

        /// <summary>
        /// 更新个人信息
        /// </summary>
        public async Task<bool> UpdatePersonalInfoAsync(PersonalInfo personalInfo)
        {
            var response = await _httpClient.PutAsJsonAsync("api/PersonalInfo", personalInfo);
            return response.IsSuccessStatusCode;
        }
    }
}
