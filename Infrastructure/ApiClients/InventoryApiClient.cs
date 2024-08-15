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
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<InventoryItem>>();
            else
                return null;
        }

        // todo: separate return bool for success and for opened / closed
        public async Task<bool> IsContainerOpened()
        {
            var response = await _httpClient.GetAsync("/Inventory/IsContainerOpened");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<bool>();
            else
                return false;
        }

        public async Task LootAll()
        {
            var response = await _httpClient.PostAsync("/Inventory/LootAll", null);
        }
    }
}
