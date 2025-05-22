using GanPersonWeb.Shared.Models;
using System.Net.Http.Json;

namespace GanPersonWeb.Client.Services
{
    public class ClientBlogCommentService
    {
        private readonly HttpClient _http;

        public ClientBlogCommentService(HttpClient http)
        {
            _http = http;
        }

        // 获取某博客的所有评论
        public async Task<List<Comment>> GetCommentsByBlogIdAsync(int blogId)
        {
            var result = await _http.GetFromJsonAsync<List<Comment>>($"api/BlogComment/blog/{blogId}");
            return result ?? new List<Comment>();
        }

        // 添加评论
        public async Task<bool> AddCommentAsync(Comment comment)
        {
            var response = await _http.PostAsJsonAsync("api/BlogComment", comment);
            return response.IsSuccessStatusCode;
        }

        // 删除评论
        public async Task<bool> DeleteCommentAsync(int commentId)
        {
            var response = await _http.DeleteAsync($"api/BlogComment/{commentId}");
            return response.IsSuccessStatusCode;
        }

        // 获取单条评论
        public async Task<Comment?> GetCommentByIdAsync(int commentId)
        {
            return await _http.GetFromJsonAsync<Comment>($"api/BlogComment/{commentId}");
        }
    }
}
