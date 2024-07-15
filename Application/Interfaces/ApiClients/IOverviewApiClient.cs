using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface IOverviewApiClient
    {
        public Task<IEnumerable<OverviewItem>> GetOverViewInfo();
        public Task ClickOnObject(OverviewItem item);
        public Task LockTargets(IEnumerable<OverviewItem> overviewItems);
        public Task LockTargetByName(string name);
    }
}
