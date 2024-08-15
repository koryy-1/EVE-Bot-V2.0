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
    public class SelectItemApiClient : ISelectItemApiClient
    {
        private readonly HttpClient _httpClient;

        public SelectItemApiClient(HttpClient httpClient)
        {

            _httpClient = httpClient;
        }

        public async Task<SelectedItemInfo> GetSelectItemInfo()
        {
            var response = await _httpClient.GetAsync("/SelectedItem/GetSelectItemInfo");
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<SelectedItemInfo>();
            else
                return null;
        }

        public async Task<bool> ClickButton(string btnName)
        {
            var json = JsonSerializer.Serialize(btnName);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/SelectedItem/ClickButton", content);
            return response.IsSuccessStatusCode;
        }
    }
}
