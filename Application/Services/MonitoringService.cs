using Application.Interfaces;
using Application.Interfaces.ApiClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MonitoringService : IMonitoringService, IWorkerService
    {
        private IInfoPanelApiClient _infoPanelApiClient;
        private IHudInterfaceApiClient _hudInterfaceApiClient;
        private IDroneApiClient _droneApiClient;
        private ICoordinator _coordinator;
        private CancellationTokenSource _cancellationTokenSource;

        public MonitoringService(IInfoPanelApiClient infoPanelApiClient, 
            IHudInterfaceApiClient hudInterfaceApiClient,
            IDroneApiClient droneApiClient,
            ICoordinator coordinator)
        {
            _infoPanelApiClient = infoPanelApiClient;
            _hudInterfaceApiClient = hudInterfaceApiClient;
            _droneApiClient = droneApiClient;
            _coordinator = coordinator;

        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await UpdateShipState();
                    await UpdateCurrentSystem();
                    await EnsureDestSet();
                    await UpdateDronesInBayCount();

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task UpdateCurrentSystem()
        {
            var location = await _infoPanelApiClient.GetLocation();
            _coordinator.ShipState.CurrentSystem = location.Name;
        }

        public async Task UpdateShipState()
        {
            var shipState = await _hudInterfaceApiClient.GetShipFlightMode();
            _coordinator.ShipState.CurrentMovement = shipState.FlightMode;
            _coordinator.ShipState.CurrentMovementObject = shipState.ItemAndDistance;
        }

        private async Task UpdateDronesInBayCount()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.Where(drone => drone.Location == "space").Any())
                _coordinator.ShipState.DronesScooped = false;
            else
                _coordinator.ShipState.DronesScooped = true;
        }

        public async Task EnsureDestSet()
        {
            var routePanel = await _infoPanelApiClient.GetRoutePanel();
            _coordinator.ShipState.IsDestSet = routePanel.Systems.Any();
        }
    }
}
