using GanPersonWeb.Shared.Models;

namespace GanPersonWeb.Services
{
    public class BlogService
    {
        private readonly DatabaseService _databaseService;

        public BlogService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<List<Blog>> GetBlogsAsync()
        {
            return await _databaseService.GetAllAsync<Blog>();
        }

        public async Task<Blog?> GetBlogByIdAsync(int id)
        {
            return await _databaseService.GetByIdAsync<Blog>(id);
        }

        public async Task AddBlogAsync(Blog blog)
        {
            await _databaseService.AddAsync(blog);
        }

        public async Task UpdateBlogAsync(Blog blog)
        {
            await _databaseService.UpdateAsync(blog);
        }

        public async Task DeleteBlogAsync(int id)
        {
            await _databaseService.DeleteAsync<Blog>(id);
        }
    }
}
