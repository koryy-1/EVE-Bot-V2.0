using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<ShipFlightMode>();
            else
                return null;
        }

        public async Task<HealthPoints> GetShipHP()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetShipHP");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<HealthPoints>();
            else
                return null;
        }

        public async Task<IEnumerable<ShipModule>> GetAllModules()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetAllModules");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<ShipModule>>();
            else
                return null;
        }

        public async Task<HudInterface> GetHudInfo()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetHudInfo");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<HudInterface>();
            else
                return null;
        }

        // todo: if warping return -1, if nothing return -2
        public async Task<int> GetCurrentSpeed()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetCurrentSpeed");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<int>();
            else
                return -1;
        }

        public async Task<Point> GetCenterPos()
        {
            var response = await _httpClient.GetAsync("/HudInterface/GetCenterPos");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<Point>();
            else
                return null;
        }

        public async Task ShipStop()
        {
            var response = await _httpClient.PostAsync("/HudInterface/ShipStop", null);
        }

        public async Task SetFullSpeed()
        {
            var response = await _httpClient.PostAsync("/HudInterface/SetFullSpeed", null);
        }

        public async Task ToggleActivationModule(string moduleName)
        {
            var json = JsonSerializer.Serialize(moduleName);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/HudInterface/ToggleActivationModule", content);
        }
    }
}
