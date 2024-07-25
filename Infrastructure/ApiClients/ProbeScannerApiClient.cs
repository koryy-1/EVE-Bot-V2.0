using Application.Interfaces.ApiClients;
using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.ApiClients
{
    public class ProbeScannerApiClient : IProbeScannerApiClient
    {
        private readonly HttpClient _httpClient;

        public ProbeScannerApiClient(HttpClient httpClient)
        {

            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ProbeScanItem>> GetProbeScanResults()
        {
            var response = await _httpClient.GetAsync("/ProbeScanner/GetProbeScanResults");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ProbeScanItem>>();
        }

        public async Task WarpToAnomaly(ProbeScanItem probeScanItem)
        {
            var json = JsonSerializer.Serialize(probeScanItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/ProbeScanner/WarpToAnomaly", content);
            response.EnsureSuccessStatusCode();
        }
    }
}
