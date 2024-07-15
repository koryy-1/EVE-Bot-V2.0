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
        private IBotStateService _botStateService;
        private CancellationTokenSource _cancellationTokenSource;

        public MonitoringService(IInfoPanelApiClient infoPanelApiClient, 
            IHudInterfaceApiClient hudInterfaceApiClient,
            IBotStateService botStateService)
        {
            _infoPanelApiClient = infoPanelApiClient;
            _hudInterfaceApiClient = hudInterfaceApiClient;
            _botStateService = botStateService;

        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await UpdateShipState();
                    //await UpdateCurrentSystem();
                    await EnsureDestSet();

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public async Task UpdateShipState()
        {
            var shipState = await _hudInterfaceApiClient.GetShipFlightMode();
            _botStateService.UpdateState(state =>
            {
                state.CurrentMovement = shipState.FlightMode;
                state.CurrentMovementObject = shipState.ItemAndDistance;
            });
        }

        public async Task EnsureDestSet()
        {
            var routePanel = await _infoPanelApiClient.GetRoutePanel();
            if (routePanel.Systems.Any())
            {
                _botStateService.UpdateState(state => state.IsDestSet = true);
            }
            else
            {
                _botStateService.UpdateState(state => state.IsDestSet = false);
            }
        }
    }
}
