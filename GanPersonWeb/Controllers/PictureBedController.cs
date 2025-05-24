using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GanPersonWeb.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PictureBedController : ControllerBase
    {
        private readonly PictureBedService _pictureBedService;

        public PictureBedController(PictureBedService pictureBedService)
        {
            _pictureBedService = pictureBedService;
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? filename, [FromForm] string? description, [FromForm] string? tags)
        {
            if (file == null)
            {
                return BadRequest("File is required.");
            }
            if (filename == null)
            {
                filename = file.FileName;
            }

            //先检查有没有同名文件
            if (filename != null && System.IO.File.Exists(Path.Combine(_pictureBedService.ImageRootPath, filename)))
            {
                return BadRequest("File with the same name already exists.");
            }


            var image = await _pictureBedService.SaveImageAsync(file, filename ?? "未命名.png", description ?? "", tags ?? "");
            if (image == null)
                return BadRequest("Invalid image file.");

            return Ok(image);
        }

        // Optionally, add a route to get all image URLs
        [HttpGet("list")]
        public async Task<IActionResult> List([FromServices] DatabaseService dbService)
        {
            var images = await dbService.GetAllAsync<Image>();
            return Ok(images);
        }


        //删除图片
        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid image ID.");
            }
            var result = await _pictureBedService.DeleteImageAsync(id);
            if (!result)
            {
                return NotFound("Image not found.");
            }
            return Ok("Image deleted successfully.");

        }
    }
}
