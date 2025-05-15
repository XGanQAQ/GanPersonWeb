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

        public async Task CreateInitialProjectsAsync()
        {
            var existingProjects = await _databaseService.GetAllAsync<Project>();
            if (existingProjects != null && existingProjects.Count > 0)
                return;

            var initialProjects = new List<Project>
            {
                new Project
                {
                    Title = "个人博客系统",
                    Description = "一个基于Blazor WebAssembly的个人博客系统，支持文章发布、评论、标签等功能。",
                    ImageUrl = "/images/projects/blog.png",
                    PublishDate = DateTime.Now.AddMonths(-2),
                    Tags = new List<string> { "Blazor", "WebAssembly", "博客" },
                    Link = "https://github.com/example/blog"
                },
                new Project
                {
                    Title = "项目管理工具",
                    Description = "简单易用的项目管理工具，支持任务分配、进度跟踪和团队协作。",
                    ImageUrl = "/images/projects/pm.png",
                    PublishDate = DateTime.Now.AddMonths(-1),
                    Tags = new List<string> { "管理", "协作", "工具" },
                    Link = "https://github.com/example/pmtool"
                }
            };

            foreach (var project in initialProjects)
            {
                await _databaseService.AddAsync(project);
            }
        }

        // 分页获取项目
        public async Task<List<Project>> GetProjectsInRangeAsync(int start, int count)
        {
            return await _databaseService.GetRangeAsync<Project>(start, count);
        }

        // 获取项目总数
        public async Task<int> GetProjectsCountAsync()
        {
            return await _databaseService.GetCountAsync<Project>();
        }
    }
}
