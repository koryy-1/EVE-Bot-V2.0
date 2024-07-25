using Domen.Entities;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface IHudInterfaceApiClient
    {
        public Task<HudInterface> GetHudInfo();
        public Task<HealthPoints> GetShipHP();
        public Task<int> GetCurrentSpeed();
        public Task<IEnumerable<ShipModule>> GetAllModules();
        public Task<ShipFlightMode> GetShipFlightMode();
        public Task<Point> GetCenterPos();
    }
}
