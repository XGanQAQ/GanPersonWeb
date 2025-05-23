using GanPersonWeb.Shared.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GanPersonWeb.Services
{
    public class PictureBedService
    {
        private readonly DatabaseService _dbService;
        private readonly string _imageRootPath;

        public PictureBedService(DatabaseService dbService, IWebHostEnvironment env)
        {
            _dbService = dbService;
            // Images will be saved under wwwroot/uploads
            _imageRootPath = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(_imageRootPath))
                Directory.CreateDirectory(_imageRootPath);
        }

        public async Task<Image?> SaveImageAsync(IFormFile file, string description = "", string tags = "")
        {
            if (file == null || file.Length == 0)
                return null;

            var fileExt = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExt}";
            var filePath = Path.Combine(_imageRootPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new Image
            {
                Url = $"/uploads/{fileName}",
                UploadDate = DateTime.UtcNow,
                Description = description,
                Tags = tags
            };

            await _dbService.AddAsync(image);
            return image;
        }
    }
}
