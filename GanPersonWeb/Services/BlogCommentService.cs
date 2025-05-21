using GanPersonWeb.Shared.Models;

namespace GanPersonWeb.Services
{
    public class BlogCommentService
    {
        private readonly DatabaseService _dbService;

        public BlogCommentService(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // 获取指定博客的所有评论（按时间升序）
        public async Task<List<Comment>> GetCommentsByBlogIdAsync(int blogId)
        {
            var allComments = await _dbService.GetAllAsync<Comment>();
            return allComments
                .Where(c => c.BlogId == blogId)
                .OrderBy(c => c.PublishDate)
                .ToList();
        }

        // 添加评论
        public async Task AddCommentAsync(Comment comment)
        {
            comment.PublishDate = DateTime.UtcNow;
            await _dbService.AddAsync(comment);
        }

        // 删除评论
        public async Task DeleteCommentAsync(int commentId)
        {
            await _dbService.DeleteAsync<Comment>(commentId);
        }

        // 获取单条评论
        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            return await _dbService.GetByIdAsync<Comment>(commentId);
        }
    }
}
