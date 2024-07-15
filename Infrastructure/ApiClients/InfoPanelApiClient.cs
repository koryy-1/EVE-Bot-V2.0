using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ApiClients
{
    public class InfoPanelApiClient : IInfoPanelApiClient
    {
        private readonly HttpClient _httpClient;

        public InfoPanelApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RoutePanel> GetRoutePanel()
        {
            var response = await _httpClient.GetAsync("/InfoPanel/GetRoutePanel");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RoutePanel>();
        }

        public async Task<LocationInfo> GetLocation()
        {
            var response = await _httpClient.GetAsync("/InfoPanel/GetLocation");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LocationInfo>();
        }

        public Task ClearAllWaypoints()
        {
            throw new NotImplementedException();
        }
    }
}
