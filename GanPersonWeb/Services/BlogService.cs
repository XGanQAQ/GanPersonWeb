using GanPersonWeb.Data;
using GanPersonWeb.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Markdig;


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
            var blog = await _databaseService.GetByIdAsync<Blog>(id);

            //将Markdown转换为HTML
            if(blog!=null && string.IsNullOrEmpty(blog.HtmlContent))
            {
                blog.HtmlContent = Markdown.ToHtml(blog.Content);
            }

            return blog;
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
            // 将Markdown转换为HTML
            if (blog != null && string.IsNullOrEmpty(blog.HtmlContent))
            {
                blog.HtmlContent = Markdown.ToHtml(blog.Content);
            }

            // 确保传递的 blog 不为 null
            if (blog == null)
            {
                throw new ArgumentNullException(nameof(blog), "Blog cannot be null.");
            }

            await _databaseService.AddAsync(blog);
        }

        public async Task UpdateBlogAsync(Blog blog)
        {
            //将Markdown转换为HTML
            if (blog != null)
            {
                blog.HtmlContent = Markdown.ToHtml(blog.Content);
            }

            // 确保传递的 blog 不为 null
            if (blog == null)
            {
                throw new ArgumentNullException(nameof(blog), "Blog cannot be null.");
            }

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
                        ImageUrl = "/uploads/default_card.png",
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
                    },
                    new Blog
                    {
                        Title = "Markdown语法",
                        Description = "Markdown 是一种轻量级的标记语言，常用于编写文档、笔记或博客，语法简单易用，以下是一些常用的 Markdown 格式",
                        Content = "### 1. 标题\r\n使用 `#` 号表示标题，`#` 的数量对应标题级别（1-6级）。\r\n```markdown\r\n# 一级标题\r\n## 二级标题\r\n### 三级标题\r\n```\r\n\r\n### 2. 加粗与斜体\r\n- **加粗**：使用 `**加粗内容**` 或 `__加粗内容__`\r\n- *斜体*：使用 `*斜体内容*` 或 `_斜体内容_`\r\n- ***加粗加斜体***：使用 `***加粗加斜体内容***`\r\n\r\n### 3. 列表\r\n- **无序列表**：使用 `-`、`*` 或 `+` 表示每一项\r\n  ```markdown\r\n  - 第一项\r\n  - 第二项\r\n  ```\r\n\r\n- **有序列表**：使用数字后跟 `.` 表示每一项\r\n  ```markdown\r\n  1. 第一项\r\n  2. 第二项\r\n  ```\r\n\r\n### 4. 链接与图片\r\n- **链接**：`[链接文本](URL)`\r\n  ```markdown\r\n  [Google](https://www.google.com)\r\n  ```\r\n\r\n- **图片**：`![图片说明](图片URL)`\r\n  ```markdown\r\n  ![Logo](https://example.com/logo.png)\r\n  ```\r\n\r\n### 5. 引用\r\n使用 `>` 表示引用，支持多层嵌套。\r\n```markdown\r\n> 这是一个引用\r\n>> 嵌套引用\r\n```\r\n\r\n### 6. 代码\r\n- **行内代码**：使用反引号 ``\r\n  ```markdown\r\n  使用 `code` 表示行内代码\r\n  ```\r\n\r\n- **代码块**：使用三个反引号 ``` 包裹代码，并可指定语言（如 `python`）\r\n  ```markdown\r\n  ```python\r\n  print(\"Hello, World!\")\r\n  ```\r\n  ```\r\n\r\n### 7. 表格\r\n使用 `|` 分隔列，`-` 分隔表头和表格内容\r\n| 姓名   | 年龄 |\r\n| ------ | ---- |\r\n| 小明   | 20   |\r\n| 小红   | 22   |\r\n```markdown\r\n\r\n| 姓名   | 年龄 |\r\n| ------ | ---- |\r\n| 小明   | 20   |\r\n| 小红   | 22   |\r\n\r\n```\r\n\r\n\r\n### 8. 分割线\r\n使用 `---`、`***` 或 `___` 表示分割线\r\n```markdown\r\n---\r\n```\r\n\r\n### 9. 任务列表\r\n使用 `[ ]` 或 `[x]` 表示待办事项\r\n为了防止被TODOs识别，我在前面加了//，实际使用时请删除\r\n\r\n// - [x] 已完成\r\n// - [ ] 待完成\r\n```markdown\r\n// - [x] 已完成\r\n// - [ ] 待完成\r\n```\r\n\r\n### 10：换行分隔\r\n在行末添加两个空格，然后按回车：\r\n```markdown\r\n第一行内容  \r\n第二行内容\r\n```\r\n\r\n### 11：段落分隔\r\n在两段文本之间留出一个空行，这将产生一个新的段落：\r\n```markdown\r\n第一段内容。\r\n\r\n第二段内容。\r\n```\r\n\r\n这些是 Markdown 中最常用的格式，灵活运用这些格式可以让你的文档更具结构和可读性。\r\n",
                        ImageUrl = "/uploads/default_card.png",
                        PublishDate = DateTime.Now,
                        Tags = new List<string> { "markdown" },
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
