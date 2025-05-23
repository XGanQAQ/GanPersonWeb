using GanPersonWeb.Services;
using GanPersonWeb.Shared.Models;
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string? description, [FromForm] string? tags)
        {
            var image = await _pictureBedService.SaveImageAsync(file, description ?? "", tags ?? "");
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
    }
}
