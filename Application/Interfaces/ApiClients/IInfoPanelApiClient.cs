using Domen.Entities;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface IInfoPanelApiClient
    {
        Task<RoutePanel> GetRoutePanel();
        Task<LocationInfo> GetLocation();
        Task ClearAllWaypoints();
    }
}
