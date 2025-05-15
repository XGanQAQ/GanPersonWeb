using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using GanPersonWeb.Shared.Models;


namespace GanPersonWeb.Client.Services
{
    public class ClientSiteVisitService
    {
        private readonly HttpClient _httpClient;

        public ClientSiteVisitService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 调用 POST /api/visit/record
        public async Task RecordVisitAsync()
        {
            await _httpClient.PostAsync("api/visit/record", null);
        }

        // 调用 GET /api/visit/data
        public async Task<List<SiteVisit>?> GetVisitDataAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<SiteVisit>>("api/visit/data");
        }
    }
}
