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
    public class HudInterfaceApiClient : IHudInterfaceApiClient
    {
        private readonly HttpClient _httpClient;

        public HudInterfaceApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ShipFlightMode> GetShipFlightMode()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetShipFlightMode");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ShipFlightMode>();
        }

        public async Task<HealthPoints> GetShipHP()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetShipHP");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HealthPoints>();
        }

        public async Task<IEnumerable<ShipModule>> GetAllModules()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetAllModules");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ShipModule>>();
        }

        public async Task<HudInterface> GetHudInfo()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetHudInfo");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<HudInterface>();
        }

        public async Task<int> GetCurrentSpeed()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetCurrentSpeed");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<Point> GetCenterPos()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetCenterPos");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Point>();
        }
    }
}
