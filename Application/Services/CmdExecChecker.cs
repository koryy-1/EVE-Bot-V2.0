using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.Entities.Commands;
using Domen.Enums;
using Domen.ValueObjects;
using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CmdExecChecker : BotWorker
    {
        private IOverviewApiClient _overviewApiClient;
        private IInfoPanelApiClient _infoPanelApiClient;
        private IProbeScannerApiClient _probeScannerApiClient;
        private string _nextSystem;

        public CmdExecChecker(
            IOverviewApiClient overviewApiClient, 
            IInfoPanelApiClient infoPanelApiClient, 
            IProbeScannerApiClient probeScannerApiClient,
            ICoordinator coordinator
        ) : base(coordinator, "exec-checker")
        {
            _overviewApiClient = overviewApiClient;
            _infoPanelApiClient = infoPanelApiClient;
            _probeScannerApiClient = probeScannerApiClient;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            await EnsureGotoNextSysExecuted();
            await EnsurePrimaryTargetDestroyed();
            await EnsureNearestAnomalyIsRight();
        }

        private async Task EnsureNearestAnomalyIsRight()
        {
            if (!Coordinator.Commands.WarpToAnomalyCommand.Requested)
                return;

            var scanResults = await _probeScannerApiClient.GetProbeScanResults();
            var anomaly = scanResults
                .FirstOrDefault(res => res.ID == Coordinator.Commands.WarpToAnomalyCommand.Anomaly.ID);

            if (anomaly is null 
                || (
                    (anomaly.Distance.Measure == "km" || anomaly.Distance.Measure == "m")
                    && Coordinator.ShipState.CurrentMovement != FlightMode.Warping
                    )
                )
            {
                Coordinator.Commands.WarpToAnomalyCommand.Requested = false;
            }
        }

        private async Task EnsurePrimaryTargetDestroyed()
        {
            if (!Coordinator.Commands.DestroyTargetCommand.Requested)
                return;

            var OvObjects = await _overviewApiClient.GetOverViewInfo();
            if (!OvObjects.Where(obj => obj.Name == Coordinator.Commands.DestroyTargetCommand.Target.Name).Any())
            {
                Coordinator.Commands.DestroyTargetCommand.Requested = false;
            }
        }

        private async Task EnsureGotoNextSysExecuted()
        {
            if (!Coordinator.Commands.GotoNextSystemCommand.Requested)
                return;

            if (string.IsNullOrEmpty(_nextSystem))
            {
                _nextSystem = await GetActualNextSystem();
            }

            // todo: если GotoNextSystemCommand.NextSystemName выставлен то он никогда не выполнится
            // написать другое условие для выполнения команды
            var actualNextSystem = await GetActualNextSystem();

            if (_nextSystem != actualNextSystem)
            {
                Coordinator.Commands.GotoNextSystemCommand = new GotoNextSystemCommand();
                _nextSystem = null;
            }
        }

        private async Task<string> GetActualNextSystem()
        {
            if (string.IsNullOrEmpty(Coordinator.Commands.GotoNextSystemCommand.NextSystemName)
                || Coordinator.Commands.GotoNextSystemCommand.NextSystemName == "string"
                )
            {
                var asd = _infoPanelApiClient.GetRoutePanel().GetAwaiter().GetResult().NextSystemInRoute;
                return asd;
            }
            else
            {
                return Coordinator.Commands.GotoNextSystemCommand.NextSystemName;
            }
        }
    }
}
