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
    public class DroneApiClient : IDroneApiClient
    {
        private readonly HttpClient _httpClient;

        public DroneApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Drone>> GetDronesInfo()
        {
            var response = await _httpClient.GetAsync("/Drones/GetDronesInfo");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<IEnumerable<Drone>>();
            else
                return null;
        }

        public async Task Engage()
        {
            var response = await _httpClient.PostAsync("/Drones/Engage", null);
        }

        public async Task Launch()
        {
            var response = await _httpClient.PostAsync("/Drones/Launch", null);
        }

        public async Task Scoop()
        {
            var response = await _httpClient.PostAsync("/Drones/Scoop", null);
        }
    }
}
