using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface IInventoryApiClient
    {
        public Task<IEnumerable<InventoryItem>> GetInventoryInfo();
        public Task<bool> IsContainerOpened();
        public Task LootAll();
    }
}
