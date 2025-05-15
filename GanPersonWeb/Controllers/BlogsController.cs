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


    }
}
