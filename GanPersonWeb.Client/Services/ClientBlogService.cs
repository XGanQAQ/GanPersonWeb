using GanPersonWeb.Shared.Models;
using System.Net.Http.Json;

namespace GanPersonWeb.Client.Services
{
    public class BlogsData
    {
        public int BlogsCount { get; set; } // 博客总数
        public int BlogsVisitCount { get; set; } // 博客访问量
        public int BlogsWriteCount { get; set; } // 博客写作量
        public DateTime LastUpdateTime { get; set; } // 最后更新时间
    }

    public class ClientBlogService
    {
        private readonly HttpClient _httpClient;

        public ClientBlogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 获取所有博客
        public async Task<List<Blog>?> GetBlogsAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Blog>>("api/Blogs");
        }

        // 获取单个博客并自动增加浏览量
        public async Task<Blog?> GetBlogAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Blog>($"api/Blogs/{id}");
        }

        // 获取指定范围的博客
        public async Task<List<Blog>?> GetBlogsInRangeAsync(int start, int count)
        {
            return await _httpClient.GetFromJsonAsync<List<Blog>>($"api/Blogs/range/{start}/{count}");
        }
        // 精简版 获取指定范围的博客
        public async Task<List<Blog>?> GetBlogsInRangeShortAsync(int start, int count)
        {
            return await _httpClient.GetFromJsonAsync<List<Blog>>($"/api/Blogs/short/range/{start}/{count}");
        }


        // 获取博客统计数据
        public async Task<BlogsData?> GetBlogDataAsync()
        {
            return await _httpClient.GetFromJsonAsync<BlogsData>("api/Blogs/data");
        }

        // 新增博客（需要认证）
        public async Task<Blog?> AddBlogAsync(Blog blog)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Blogs", blog);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Blog>();
            }
            return null;
        }

        // 更新博客（需要认证）
        public async Task<bool> UpdateBlogAsync(int id, Blog blog)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Blogs/{id}", blog);
            return response.IsSuccessStatusCode;
        }

        // 删除博客（需要认证）
        public async Task<bool> DeleteBlogAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Blogs/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
