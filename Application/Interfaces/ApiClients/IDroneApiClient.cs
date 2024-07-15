using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface IDroneApiClient
    {
        public Task<IEnumerable<Drone>> GetDronesInfo();
        public Task Launch();
        public Task Engage();
        public Task Scoop();
    }
}
