using GanPersonWeb.Data;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace GanPersonWeb.Services
{
    //��������
    public class BlogsData
    {
        public int BlogsCount { get; set; } // ��������
        public int BlogsVisitCount { get; set; } // ���ͷ�����
        public int BlogsWriteCount { get; set; } // ����д����
        public DateTime LastUpdateTime { get; set; } // ������ʱ��
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

        //���ָ����Χ�Ĳ���
        public async Task<List<Blog>> GetBlogsInRangeAsync(int start, int count)
        {
            return await _databaseService.GetRangeAsync<Blog>(start, count);
        }

        //��Ϊ�������ݺ��ڿ��ܻ�ܴ����Բ���ֱ�Ӳ������ݿ������Ľ��ж�ȡ���������Ƕ�ȡ���в������ݣ�
        public async Task<BlogsData> GetBlogsDataAsync()
        {
            // ֱ���� DbContext ��ѯ�ۺ�����
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

        // ��ʼ���������ݣ��������ݿ�Ϊ��ʱ����һЩ��ʼ���ͣ�
        public async Task CreateInitialBlogsAsync()
        {
            using var context = _databaseService.GetDbContext();
            if (!await context.Blogs.AnyAsync())
            {
                var initialBlogs = new List<Blog>
                {
                    new Blog
                    {
                        Title = "��ӭ�����ҵĲ���",
                        Description = "�����ҵĵ�һƪ���ͣ���ӭ��ң�",
                        Content = "�����ǲ�������ʾ����������������������뷨�͹��¡�",
                        ImageUrl = "",
                        PublishDate = DateTime.Now,
                        Tags = new List<string> { "����", "��ӭ" },
                        ViewCount = 0,
                        TalkCount = 0
                    },
                    new Blog
                    {
                        Title = "Blazor WebAssembly ������",
                        Description = "��¼��ʹ�� Blazor WebAssembly ��һЩ�ĵá�",
                        Content = "Blazor WebAssembly ��ǰ�˿�����ø�����Ȥ�͸�Ч��",
                        ImageUrl = "",
                        PublishDate = DateTime.Now,
                        Tags = new List<string> { "Blazor", "WebAssembly", "����" },
                        ViewCount = 0,
                        TalkCount = 0
                    }
                };

                await context.Blogs.AddRangeAsync(initialBlogs);
                await context.SaveChangesAsync();
            }
        }

        // ������в��ͱ�ǩ��ȥ�أ�
        public async Task<List<string>> GetAllTagsAsync()
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .SelectMany(b => b.Tags)
                .Distinct()
                .ToListAsync();
        }

        // ���ָ����ǩ�Ĳ���
        public async Task<List<Blog>> GetBlogsByTagAsync(string tag)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Where(b => b.Tags.Contains(tag))
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();
        }

        // ������в������ͣ�ȥ�أ�
        public async Task<List<string>> GetAllTypesAsync()
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Select(b => b.Type)
                .Distinct()
                .ToListAsync();
        }

        // ���ָ�����͵Ĳ���
        public async Task<List<Blog>> GetBlogsByTypeAsync(string type)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .Where(b => b.Type == type)
                .OrderByDescending(b => b.PublishDate)
                .ToListAsync();
        }

        // ������Ų��ͣ����ۿ�������
        public async Task<List<Blog>> GetHotBlogsAsync(int start, int count)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .OrderByDescending(b => b.ViewCount)
                .Skip(start)
                .Take(count)
                .ToListAsync();
        }

        // ������²��ͣ���ʱ�併��
        public async Task<List<Blog>> GetNewBlogsAsync(int start, int count)
        {
            using var context = _databaseService.GetDbContext();
            return await context.Blogs
                .OrderByDescending(b => b.PublishDate)
                .Skip(start)
                .Take(count)
                .ToListAsync();
        }

        // ����棺��ȡָ����ǩ�Ĳ��ͣ�������Content�ֶΣ�
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

        // ����棺��ȡָ�����͵Ĳ��ͣ�������Content�ֶΣ�
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

        // ����棺��ȡ���Ų��ͣ�������Content�ֶΣ�
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

        // ����棺��ȡ���²��ͣ�������Content�ֶΣ�
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
