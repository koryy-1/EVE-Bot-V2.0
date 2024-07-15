using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.ValueObjects;
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
    public class GameApiClient : IGameApiClient
    {
        private readonly HttpClient _httpClient;

        public GameApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ClientParams> GetClientParams()
        {
            var response = await _httpClient.GetAsync("/GameClient/GetClientParams");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientParams>();
        }

        public async Task<SearchingStatus> GetStatus()
        {
            var response = await _httpClient.GetAsync("/GameClient/GetStatus");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SearchingStatus>();
        }
        public async Task<SearchingStatus> CheckRootAddressActuality()
        {
            var response = await _httpClient.GetAsync("/GameClient/CheckRootAddressActuality");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<SearchingStatus>();
        }

        public async Task UpdateClientParams(ClientParams clientParams)
        {
            var json = JsonSerializer.Serialize(clientParams);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/GameClient/UpdateClientParams", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ClientParams> StartSearch()
        {
            var response = await _httpClient.PostAsync("/GameClient/StartSearch", null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientParams>();
        }

        public async Task StopSearch()
        {
            var response = await _httpClient.PostAsync("/GameClient/StopSearch", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
