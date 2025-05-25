using GanPersonWeb.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace GanPersonWeb.Client.Services
{
    public class ClientPictureBedService
    {
        private readonly HttpClient _http;

        public ClientPictureBedService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Image>> GetImagesAsync()
        {
            var result = await _http.GetFromJsonAsync<List<Image>>("api/PictureBed/list");
            return result ?? new List<Image>();
        }

        public async Task<Image?> UploadImageAsync(Stream fileStream, string filename, string description, string tags)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(fileStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "file", filename);
            content.Add(new StringContent(filename ?? ""), "filename");
            content.Add(new StringContent(description ?? ""), "description");
            content.Add(new StringContent(tags ?? ""), "tags");

            var response = await _http.PostAsync("api/PictureBed/upload", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Image>();
            }
            return null;
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            // If you have a delete API, use it. Otherwise, remove this method.
            var response = await _http.DeleteAsync($"api/PictureBed/delete/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
