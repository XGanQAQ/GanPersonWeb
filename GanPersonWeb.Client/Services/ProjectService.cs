using System.Net.Http.Json;
using GanPersonWeb.Shared.Models;

namespace GanPersonWeb.Client.Services
{
    public class ProjectService
    {
        private readonly HttpClient _httpClient;

        public ProjectService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Project>>("api/projects");
        }

        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Project>($"api/projects/{id}");
        }

        public async Task AddProjectAsync(Project project)
        {
            await _httpClient.PostAsJsonAsync("api/projects", project);
        }

        public async Task UpdateProjectAsync(int id, Project project)
        {
            await _httpClient.PutAsJsonAsync($"api/projects/{id}", project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            await _httpClient.DeleteAsync($"api/projects/{id}");
        }
    }
}
