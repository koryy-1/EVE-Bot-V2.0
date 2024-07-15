using Application.Interfaces.ApiClients;
using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ApiClients
{
    public class OverviewApiClient : IOverviewApiClient
    {
        private readonly HttpClient _httpClient;

        public OverviewApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<OverviewItem>> GetOverViewInfo()
        {
            var response = await _httpClient.GetAsync("/OverView/GetOverViewInfo");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<OverviewItem>>();
        }

        public async Task ClickOnObject(OverviewItem spaceObject)
        {
            var json = JsonSerializer.Serialize(spaceObject);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/OverView/ClickOnObject", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task LockTargets(IEnumerable<OverviewItem> overviewItems)
        {
            var json = JsonSerializer.Serialize(overviewItems);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/OverView/LockTargets", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task LockTargetByName(string name)
        {
            var json = JsonSerializer.Serialize(name);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/OverView/LockTargetByName", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
