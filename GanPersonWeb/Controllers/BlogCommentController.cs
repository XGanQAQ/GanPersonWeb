using GanPersonWeb.Shared.Models;
using GanPersonWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GanPersonWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogCommentController : ControllerBase
    {
        private readonly BlogCommentService _commentService;

        public BlogCommentController(BlogCommentService commentService)
        {
            _commentService = commentService;
        }

        // 获取某博客的所有评论
        [HttpGet("blog/{blogId}")]
        public async Task<ActionResult<List<Comment>>> GetCommentsByBlogId(int blogId)
        {
            var comments = await _commentService.GetCommentsByBlogIdAsync(blogId);
            return Ok(comments);
        }

        // 添加评论
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment([FromBody] Comment comment)
        {
            await _commentService.AddCommentAsync(comment);
            return Ok();
        }

        // 删除评论
        [Authorize(Roles = "Admin")]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            await _commentService.DeleteCommentAsync(commentId);
            return Ok();
        }

        // 获取单条评论
        [HttpGet("{commentId}")]
        public async Task<ActionResult<Comment?>> GetCommentById(int commentId)
        {
            var comment = await _commentService.GetCommentByIdAsync(commentId);
            if (comment == null) return NotFound();
            return Ok(comment);
        }
    }
}
