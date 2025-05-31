using System.Net.Http.Json;
using GanPersonWeb.Shared.Models;

namespace GanPersonWeb.Client.Services
{
    public class ClientProjectService
    {
        private readonly HttpClient _httpClient;

        public ClientProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<List<Project>>("api/projects");
            return result ?? new List<Project>();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<Project>($"api/projects/{id}");
            return result;
        }

        public async Task<bool> AddProjectAsync(Project project)
        {
            var response = await _httpClient.PostAsJsonAsync("api/projects", project);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProjectAsync(int id, Project project)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/projects/{id}", project);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/projects/{id}");
            return response.IsSuccessStatusCode;
        }

        // 新增：分页获取项目
        public async Task<List<Project>> GetProjectsInRangeAsync(int start, int count)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Project>>($"api/projects/range/{start}/{count}");
            return result ?? new List<Project>();
        }

        // 新增：获取项目总数（假设有此API，否则可用GetProjectsAsync().Count）
        public async Task<int> GetProjectsCountAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<int>("api/projects/count");
            return result;
        }

        // 通过Tag筛选项目
        public async Task<List<Project>> GetProjectsByTagAsync(string tag)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Project>>($"api/projects/tag/{tag}");
            return result ?? new List<Project>();
        }

        // 通过Tag筛选并分页获取项目
        public async Task<List<Project>> GetProjectsByTagInRangeAsync(string tag, int start, int count)
        {
            var result = await _httpClient.GetFromJsonAsync<List<Project>>($"api/projects/tag/{tag}/range/{start}/{count}");
            return result ?? new List<Project>();
        }
    }
}
