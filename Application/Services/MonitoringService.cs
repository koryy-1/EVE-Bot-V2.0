using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MonitoringService : BotWorker, IMonitoringService
    {
        private IInfoPanelApiClient _infoPanelApiClient;
        private IHudInterfaceApiClient _hudInterfaceApiClient;
        private IDroneApiClient _droneApiClient;

        public MonitoringService(
            IInfoPanelApiClient infoPanelApiClient, 
            IHudInterfaceApiClient hudInterfaceApiClient,
            IDroneApiClient droneApiClient,
            ICoordinator coordinator
        ) : base(coordinator, "monitoring-service")
        {
            _infoPanelApiClient = infoPanelApiClient;
            _hudInterfaceApiClient = hudInterfaceApiClient;
            _droneApiClient = droneApiClient;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            await UpdateShipState();
            await UpdateCurrentSystem();
            await EnsureDestSet();
            await UpdateDronesInBayCount();
        }

        private async Task UpdateCurrentSystem()
        {
            var location = await _infoPanelApiClient.GetLocation();
            Coordinator.ShipState.CurrentSystem = location.Name;
        }

        public async Task UpdateShipState()
        {
            var shipState = await _hudInterfaceApiClient.GetShipFlightMode();
            Coordinator.ShipState.CurrentMovement = shipState.FlightMode;
            Coordinator.ShipState.CurrentMovementObject = shipState.ItemAndDistance;
        }

        private async Task UpdateDronesInBayCount()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.Where(drone => drone.Location == "space").Any())
                Coordinator.ShipState.DronesScooped = false;
            else
                Coordinator.ShipState.DronesScooped = true;
        }

        public async Task EnsureDestSet()
        {
            var routePanel = await _infoPanelApiClient.GetRoutePanel();
            if (routePanel is not null)
                Coordinator.ShipState.IsDestSet = routePanel.Systems.Any();
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
