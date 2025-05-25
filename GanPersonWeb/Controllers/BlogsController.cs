using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanPersonWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController : ControllerBase
    {
        private readonly BlogService _blogService;

        public BlogsController(BlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlogs()
        {
            var blogs = await _blogService.GetBlogsAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlog(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null)
                return NotFound();
            await _blogService.BlogBeViewedAsync(id); // 更新博客的浏览量
            return Ok(blog);
        }

        //获得指定范围的博客
        [HttpGet("range/{start}/{count}")]
        public async Task<IActionResult> GetBlogsInRange(int start, int count)
        {
            var blogs = await _blogService.GetBlogsInRangeAsync(start, count);
            return Ok(blogs);
        }

        //获得总体博客数据
        [HttpGet("data")]
        public async Task<IActionResult> GetBlogData()
        {
            var blogs = await _blogService.GetBlogsAsync();
            var blogsCount = blogs.Count;
            var blogsVisitCount = blogs.Sum(b => b.ViewCount);
            var blogsWriteCount = blogs.Sum(b => b.Content.Length);
            var lastUpdateTime = blogs.Max(b => b.PublishDate);
            return Ok(new BlogsData
            {
                BlogsCount = blogsCount,
                BlogsVisitCount = blogsVisitCount,
                BlogsWriteCount = blogsWriteCount,
                LastUpdateTime = lastUpdateTime
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBlog([FromBody] Blog blog)
        {
            await _blogService.AddBlogAsync(blog);
            return CreatedAtAction(nameof(GetBlog), new { id = blog.Id }, blog);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] Blog blog)
        {
            if (id != blog.Id)
                return BadRequest();

            await _blogService.UpdateBlogAsync(blog);
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            await _blogService.DeleteBlogAsync(id);
            return NoContent();
        }

        // 精简版接口：获取所有博客（不包含Content字段）
        [HttpGet("short")]
        public async Task<IActionResult> GetShortBlogs()
        {
            var blogs = await _blogService.GetBlogsAsync();
            var shortBlogs = blogs.Select(b => new
            {
                b.Id,
                b.Title,
                b.Description,
                b.ImageUrl,
                b.PublishDate,
                b.Tags,
                b.ViewCount,
                b.TalkCount
            });
            return Ok(shortBlogs);
        }

        // 精简版接口：获取指定范围的博客（不包含Content字段）
        [HttpGet("short/range/{start}/{count}")]
        public async Task<IActionResult> GetShortBlogsInRange(int start, int count)
        {
            var blogs = await _blogService.GetBlogsInRangeAsync(start, count);
            var shortBlogs = blogs.Select(b => new
            {
                b.Id,
                b.Title,
                b.Description,
                b.ImageUrl,
                b.PublishDate,
                b.Tags,
                b.ViewCount,
                b.TalkCount
            });
            return Ok(shortBlogs);
        }

        // 获得所有博客标签
        [HttpGet("tags")]
        public async Task<IActionResult> GetBlogTags()
        {
            var tags = await _blogService.GetAllTagsAsync();
            return Ok(tags);
        }

        // 获得指定标签的博客
        [HttpGet("tags/{tag}")]
        public async Task<IActionResult> GetBlogsByTag(string tag)
        {
            var blogs = await _blogService.GetBlogsByTagAsync(tag);
            return Ok(blogs);
        }

        // 获得所有博客类型
        [HttpGet("type")]
        public async Task<IActionResult> GetBlogTypes()
        {
            var types = await _blogService.GetAllTypesAsync();
            return Ok(types);
        }

        // 获得指定类型的博客
        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetBlogsByType(string type)
        {
            var blogs = await _blogService.GetBlogsByTypeAsync(type);
            return Ok(blogs);
        }

        // 获得热门博客（按观看量降序）
        [HttpGet("hot/{start:int}/{count:int}")]
        public async Task<IActionResult> GetHotBlogs(int start, int count)
        {
            var blogs = await _blogService.GetHotBlogsAsync(start, count);
            return Ok(blogs);
        }

        // 获得最新博客（按时间降序）
        [HttpGet("new/{start:int}/{count:int}")]
        public async Task<IActionResult> GetNewBlogs(int start, int count)
        {
            var blogs = await _blogService.GetNewBlogsAsync(start, count);
            return Ok(blogs);
        }

        // 精简版接口：获取指定标签的博客（不包含Content字段）
        [HttpGet("short/tags/{tag}")]
        public async Task<IActionResult> GetShortBlogsByTag(string tag)
        {
            var blogs = await _blogService.GetShortBlogsByTagAsync(tag);
            return Ok(blogs);
        }

        // 精简版接口：获取指定类型的博客（不包含Content字段）
        [HttpGet("short/type/{type}")]
        public async Task<IActionResult> GetShortBlogsByType(string type)
        {
            var blogs = await _blogService.GetShortBlogsByTypeAsync(type);
            return Ok(blogs);
        }

        // 精简版接口：获取热门博客（不包含Content字段）
        [HttpGet("short/hot/{start:int}/{count:int}")]
        public async Task<IActionResult> GetShortHotBlogs(int start, int count)
        {
            var blogs = await _blogService.GetShortHotBlogsAsync(start, count);
            return Ok(blogs);
        }

        // 精简版接口：获取最新博客（不包含Content字段）
        [HttpGet("short/new/{start:int}/{count:int}")]
        public async Task<IActionResult> GetShortNewBlogs(int start, int count)
        {
            var blogs = await _blogService.GetShortNewBlogsAsync(start, count);
            return Ok(blogs);
        }
    }
}
