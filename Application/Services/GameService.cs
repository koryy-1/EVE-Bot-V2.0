using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class GameService : IGameService
    {
        private IGameApiClient _gameApiClient;

        public GameService(IGameApiClient gameApiClient)
        {
            _gameApiClient = gameApiClient;
        }

        public async Task<SearchingStatus> CheckRootAddressActuality()
        {
            return await _gameApiClient.CheckRootAddressActuality();
        }

        public async Task<ClientParams> GetClientParams()
        {
            return await _gameApiClient.GetClientParams();
        }

        public async Task<ClientParams> StartSearch()
        {
            return await _gameApiClient.StartSearch();
        }

        public async Task UpdateClientParams(ClientParams clientParams)
        {
            await _gameApiClient.UpdateClientParams(clientParams);
        }
    }
}
