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
    public class CmdExecChecker : IWorkerService
    {
        private IOverviewApiClient _overviewApiClient;
        private IInfoPanelApiClient _infoPanelApiClient;
        private IProbeScannerApiClient _probeScannerApiClient;
        private ICoordinator _coordinator;
        private CancellationTokenSource _cancellationTokenSource;

        public CmdExecChecker(IOverviewApiClient overviewApiClient, 
            IInfoPanelApiClient infoPanelApiClient, 
            IProbeScannerApiClient probeScannerApiClient,
            ICoordinator coordinator)
        {
            _overviewApiClient = overviewApiClient;
            _infoPanelApiClient = infoPanelApiClient;
            _probeScannerApiClient = probeScannerApiClient;
            _coordinator = coordinator;
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    await EnsureGotoNextSysExecuted();
                    await EnsurePrimaryTargetDestroyed();
                    await EnsureNearestAnomalyIsRight();

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task EnsureNearestAnomalyIsRight()
        {
            if (!_coordinator.Commands.WarpToAnomalyCommand.Requested)
                return;

            var scanResults = await _probeScannerApiClient.GetProbeScanResults();
            var anomaly = scanResults
                .FirstOrDefault(res => res.ID == _coordinator.Commands.WarpToAnomalyCommand.Anomaly.ID);

            // measure == "m" mean anomaly nearest
            if (anomaly?.Distance.Measure == "m")
            {
                _coordinator.Commands.WarpToAnomalyCommand.Requested = false;
            }
        }

        private async Task EnsurePrimaryTargetDestroyed()
        {
            if (!_coordinator.Commands.DestroyTargetCommand.Requested)
                return;

            var OvObjects = await _overviewApiClient.GetOverViewInfo();
            if (OvObjects.Where(obj => obj == _coordinator.Commands.DestroyTargetCommand.Target).Any())
            {
                _coordinator.Commands.DestroyTargetCommand.Requested = false;
            }
        }

        private async Task EnsureGotoNextSysExecuted()
        {
            if (!_coordinator.Commands.GotoNextSystemCommand.Requested)
                return;

            // пока парсинг InfoPanel не корректный можно Requested = false вставить в NavigService
            if (_coordinator.ShipState.CurrentSystem == _coordinator.Commands.GotoNextSystemCommand.NextSystemName)
            {
                _coordinator.Commands.GotoNextSystemCommand.Requested = false;
            }
        }
    }
}
