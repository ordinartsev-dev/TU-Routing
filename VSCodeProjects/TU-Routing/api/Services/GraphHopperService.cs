// Services/GraphHopperService.cs
using System.Net.Http;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class GraphHopperService
    {
        private readonly HttpClient _httpClient;

        public GraphHopperService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        {
            string url = $"http://localhost:8989/route?point={fromLat},{fromLon}&point={toLat},{toLon}&vehicle=foot&locale=ru&instructions=true";
            
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
