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
                    Title = "���˲���ϵͳ",
                    Description = "һ������Blazor WebAssembly�ĸ��˲���ϵͳ��֧�����·��������ۡ���ǩ�ȹ��ܡ�",
                    ImageUrl = "/images/projects/blog.png",
                    PublishDate = DateTime.Now.AddMonths(-2),
                    Tags = new List<string> { "Blazor", "WebAssembly", "����" },
                    Link = "https://github.com/example/blog"
                },
                new Project
                {
                    Title = "��Ŀ������",
                    Description = "�����õ���Ŀ�����ߣ�֧��������䡢���ȸ��ٺ��Ŷ�Э����",
                    ImageUrl = "/images/projects/pm.png",
                    PublishDate = DateTime.Now.AddMonths(-1),
                    Tags = new List<string> { "����", "Э��", "����" },
                    Link = "https://github.com/example/pmtool"
                }
            };

            foreach (var project in initialProjects)
            {
                await _databaseService.AddAsync(project);
            }
        }

        // ��ҳ��ȡ��Ŀ
        public async Task<List<Project>> GetProjectsInRangeAsync(int start, int count)
        {
            return await _databaseService.GetRangeAsync<Project>(start, count);
        }

        // ��ȡ��Ŀ����
        public async Task<int> GetProjectsCountAsync()
        {
            return await _databaseService.GetCountAsync<Project>();
        }
    }
}
