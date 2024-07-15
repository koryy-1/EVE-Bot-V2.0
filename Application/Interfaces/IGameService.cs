using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IGameService
    {
        public Task<ClientParams> GetClientParams();
        public Task<SearchingStatus> CheckRootAddressActuality();
        public Task UpdateClientParams(ClientParams clientParams);
        public Task<ClientParams> StartSearch();
    }
}
