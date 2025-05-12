using GanPersonWeb.Shared.Models;

namespace GanPersonWeb.Services
{
    public class ProjectService
    {
        private readonly DatabaseService _databaseService;

        public ProjectService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            return await _databaseService.GetAllAsync<Project>();
        }

        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _databaseService.GetByIdAsync<Project>(id);
        }

        public async Task AddProjectAsync(Project project)
        {
            await _databaseService.AddAsync(project);
        }

        public async Task UpdateProjectAsync(Project project)
        {
            await _databaseService.UpdateAsync(project);
        }

        public async Task DeleteProjectAsync(int id)
        {
            await _databaseService.DeleteAsync<Project>(id);
        }
    }
}
