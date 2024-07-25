using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Application.Services;
using Domen.Entities.Commands;
using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Strategies
{
    public class FarmingStrategy
    {
        private IProbeScannerApiClient _probeScannerApiClient;
        private IOverviewApiClient _overviewApiClient;
        private ICoordinator _coordinator;
        private DestroyerStrategy _destroyerStrategy;
        public FarmingStrategy(IProbeScannerApiClient probeScannerApiClient,
            IOverviewApiClient overviewApiClient,
            ICoordinator coordinator,
            DestroyerStrategy destroyerStrategy)
        {
            _probeScannerApiClient = probeScannerApiClient;
            _overviewApiClient = overviewApiClient;
            _coordinator = coordinator;
            _destroyerStrategy = destroyerStrategy;
        }

        public async void Start()
        {
            _coordinator.BotState.IsStrategyRunning = true;

            while (StartAuthorized())
            {
                while (await IsAnomalyInCurrentSystem())
                {
                    await WarpToAnomaly();
                    await _destroyerStrategy.DESTRXY_EVERYXNE();
                    await LootConts(new List<string>() { "Shadow Serpentis", "Dread Guristas" });
                    await EnsureDronesScooped();
                }

                if (!IsMarkedGate())
                    break;

                await GotoNextSystem();
            }
            Stop();
        }

        private void Stop()
        {
            _coordinator.BotState.IsStrategyRunning = false;
        }

        private bool StartAuthorized()
        {
            return _coordinator.Commands.ExecutorAuthorized;
        }

        private async Task EnsureDronesScooped()
        {
            while (!_coordinator.ShipState.DronesScooped)
            {
                await Task.Delay(1000);
            }
        }

        private async Task GotoNextSystem()
        {
            _coordinator.Commands.GotoNextSystemCommand.Requested = true;
            while (_coordinator.Commands.GotoNextSystemCommand.Requested)
            {
                await Task.Delay(1000);
            }
        }

        private bool IsMarkedGate()
        {
            return _coordinator.ShipState.IsDestSet;
        }

        private async Task LootConts(IEnumerable<string> interestContNames)
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();

            var interestCont = ovObjects
                // check gray color on looted cont
                //.Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => 
                {
                    foreach (var contName in interestContNames)
                    {
                        return item.Name.Contains(contName);
                    }
                    return false;
                })
                .FirstOrDefault();

            if (interestCont is null)
                return;

            var cmd = new LootingCommand()
            {
                Requested = true,
                Container = interestCont
            };
            _coordinator.Commands.LootingCommand = cmd;

            while (_coordinator.Commands.LootingCommand.Requested)
                await Task.Delay(3000);
        }

        private async Task<bool> IsAnomalyInCurrentSystem()
        {
            var scanRes = await _probeScannerApiClient.GetProbeScanResults();
            var anomalies = scanRes
                .Where(res =>
                {
                    return res.Name == "Guristas Refuge"
                    || res.Name == "Guristas Hideaway"
                    || res.Name == "Serpentis Refuge"
                    || res.Name == "Serpentis Hideaway";
                });

            return anomalies.Any();
        }

        private async Task WarpToAnomaly()
        {
            var scanRes = await _probeScannerApiClient.GetProbeScanResults();
            var anomaly = scanRes
                .Where(res =>
                {
                    return res.Name == "Guristas Refuge"
                    || res.Name == "Guristas Hideaway"
                    || res.Name == "Serpentis Refuge"
                    || res.Name == "Serpentis Hideaway";
                })
                .OrderBy(anomaly => anomaly.Distance.Value)
                .FirstOrDefault();

            if (anomaly is null)
                return;

            var cmd = new WarpToAnomalyCommand()
            {
                Requested = true,
                Anomaly = anomaly
            };
            _coordinator.Commands.WarpToAnomalyCommand = cmd;

            while (_coordinator.Commands.WarpToAnomalyCommand.Requested)
                await Task.Delay(3000);
        }
    }
}
