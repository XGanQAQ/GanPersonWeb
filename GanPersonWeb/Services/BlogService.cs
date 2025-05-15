using GanPersonWeb.Data;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GanPersonWeb.Services
{
    //总体数据
    public class BlogsData
    {
        public int BlogsCount { get; set; } // 博客总数
        public int BlogsVisitCount { get; set; } // 博客访问量
        public int BlogsWriteCount { get; set; } // 博客写作量
        public DateTime LastUpdateTime { get; set; } // 最后更新时间
    }

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

        //获得指定范围的博客
        public async Task<List<Blog>> GetBlogsInRangeAsync(int start, int count)
        {
            return await _databaseService.GetRangeAsync<Blog>(start, count);
        }

        //因为博客数据后期可能会很大，所以采用直接操作数据库上下文进行读取，（而不是读取所有博客数据）
        public async Task<BlogsData> GetBlogsDataAsync()
        {
            // 直接用 DbContext 查询聚合数据
            using var context = _databaseService.GetDbContext();

            var blogsCount = await context.Blogs.CountAsync();
            var blogsVisitCount = await context.Blogs.SumAsync(b => b.ViewCount);
            var blogsWriteCount = await context.Blogs.SumAsync(b => b.Content.Length);
            var lastUpdateTime = await context.Blogs.AnyAsync()
                ? await context.Blogs.MaxAsync(b => b.PublishDate)
                : DateTime.MinValue;

            return new BlogsData
            {
                BlogsCount = blogsCount,
                BlogsVisitCount = blogsVisitCount,
                BlogsWriteCount = blogsWriteCount,
                LastUpdateTime = lastUpdateTime
            };
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

        public async Task BlogBeViewedAsync(int id)
        {
            using var context = _databaseService.GetDbContext();
            var blog = await context.Blogs.FirstOrDefaultAsync(b => b.Id == id);
            if (blog != null)
            {
                blog.ViewCount += 1;
                await context.SaveChangesAsync();
            }
        }

        // 初始化博客数据（仅在数据库为空时插入一些初始博客）
        public async Task CreateInitialBlogsAsync()
        {
            using var context = _databaseService.GetDbContext();
            if (!await context.Blogs.AnyAsync())
            {
                var initialBlogs = new List<Blog>
                {
                    new Blog
                    {
                        Title = "欢迎来到我的博客",
                        Description = "这是我的第一篇博客，欢迎大家！",
                        Content = "这里是博客内容示例。你可以在这里分享你的想法和故事。",
                        ImageUrl = "",
                        PublishDate = DateTime.Now,
                        Tags = new List<string> { "公告", "欢迎" },
                        ViewCount = 0,
                        TalkCount = 0
                    },
                    new Blog
                    {
                        Title = "Blazor WebAssembly 初体验",
                        Description = "记录我使用 Blazor WebAssembly 的一些心得。",
                        Content = "Blazor WebAssembly 让前端开发变得更加有趣和高效。",
                        ImageUrl = "",
                        PublishDate = DateTime.Now,
                        Tags = new List<string> { "Blazor", "WebAssembly", "开发" },
                        ViewCount = 0,
                        TalkCount = 0
                    }
                };

                await context.Blogs.AddRangeAsync(initialBlogs);
                await context.SaveChangesAsync();
            }
        }

        // 获得所有博客标签（去重）
        public async Task<List<string>> GetAllTagsAsync()
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .SelectMany(b => b.Tags)
                .Distinct()
                .ToListAsync();
        }

        // 获得指定标签的博客
        public async Task<List<Blog>> GetBlogsByTagAsync(string tag)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Where(b => b.Tags.Contains(tag))
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();
        }

        // 获得所有博客类型（去重）
        public async Task<List<string>> GetAllTypesAsync()
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Select(b => b.Type)
                .Distinct()
                .ToListAsync();
        }

        // 获得指定类型的博客
        public async Task<List<Blog>> GetBlogsByTypeAsync(string type)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Where(b => b.Type == type)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();
        }

        // 获得热门博客（按观看量降序）
        public async Task<List<Blog>> GetHotBlogsAsync(int start, int count)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .OrderByDescending(b => b.ViewCount)
                .Skip(start)
                .Take(count)
                .ToListAsync();
        }

        // 获得最新博客（按时间降序）
        public async Task<List<Blog>> GetNewBlogsAsync(int start, int count)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .OrderByDescending(b => b.PublishDate)
                .Skip(start)
                .Take(count)
                .ToListAsync();
        }

        // 精简版：获取指定标签的博客（不包含Content字段）
        public async Task<List<object>> GetShortBlogsByTagAsync(string tag)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Where(b => b.Tags.Contains(tag))
                .OrderByDescending(b => b.PublishDate)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.PublishDate,
                    b.Tags,
                    b.ViewCount,
                    b.TalkCount
                })
                .ToListAsync<object>();
        }

        // 精简版：获取指定类型的博客（不包含Content字段）
        public async Task<List<object>> GetShortBlogsByTypeAsync(string type)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Where(b => b.Type == type)
                .OrderByDescending(b => b.PublishDate)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.PublishDate,
                    b.Tags,
                    b.ViewCount,
                    b.TalkCount
                })
                .ToListAsync<object>();
        }

        // 精简版：获取热门博客（不包含Content字段）
        public async Task<List<object>> GetShortHotBlogsAsync(int start, int count)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .OrderByDescending(b => b.ViewCount)
                .Skip(start)
                .Take(count)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.PublishDate,
                    b.Tags,
                    b.ViewCount,
                    b.TalkCount
                })
                .ToListAsync<object>();
        }

        // 精简版：获取最新博客（不包含Content字段）
        public async Task<List<object>> GetShortNewBlogsAsync(int start, int count)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .OrderByDescending(b => b.PublishDate)
                .Skip(start)
                .Take(count)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.ImageUrl,
                    b.PublishDate,
                    b.Tags,
                    b.ViewCount,
                    b.TalkCount
                })
                .ToListAsync<object>();
        }
    }
}
