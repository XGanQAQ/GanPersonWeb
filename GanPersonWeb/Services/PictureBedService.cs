using GanPersonWeb.Shared.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GanPersonWeb.Services
{
    public class PictureBedService
    {
        public readonly string ImageRootPath;
        private readonly DatabaseService _dbService;

        public PictureBedService(DatabaseService dbService, IWebHostEnvironment env)
        {
            _dbService = dbService;
            // Images will be saved under wwwroot/uploads
            ImageRootPath = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(ImageRootPath))
                Directory.CreateDirectory(ImageRootPath);
        }
        public async Task<Image?> SaveImageAsync(IFormFile file, string filename, string description = "", string tags = "")
        {
            if (file == null || file.Length == 0)
                return null;

            var fileExt = Path.GetExtension(file.FileName);
            var fileName = filename;
            var filePath = Path.Combine(ImageRootPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var image = new Image
            {
                Filename = fileName,
                Url = $"/uploads/{fileName}",
                UploadDate = DateTime.UtcNow,
                Description = description,
                Tags = tags
            };

            await _dbService.AddAsync(image);
            return image;
        }

        //删除
        public async Task<bool> DeleteImageAsync(int id)
        {
            var image = await _dbService.GetByIdAsync<Image>(id);
            if (image == null)
                return false;
            var filePath = Path.Combine(ImageRootPath, image.Filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            await _dbService.UpdateAsync(image);
            return true;
        }
    }
}
