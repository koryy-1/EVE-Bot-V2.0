using Application.Interfaces.ApiClients;
using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ApiClients
{
    public class InventoryApiClient : IInventoryApiClient
    {
        private readonly HttpClient _httpClient;

        public InventoryApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<InventoryItem>> GetInventoryInfo()
        {
            var response = await _httpClient.GetAsync("/Inventory/GetInventoryInfo");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<InventoryItem>>();
        }

        public async Task<bool> IsContainerOpened()
        {
            var response = await _httpClient.GetAsync("/Inventory/IsContainerOpened");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<bool>();
        }

        public async Task LootAll()
        {
            var response = await _httpClient.PostAsync("/Inventory/LootAll", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
