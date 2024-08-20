using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class NavigationService : BotWorker, INavigationService
    {
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;
        private IHudInterfaceApiClient _hudInterfaceApiClient;
        private IProbeScannerApiClient _probeScannerApiClient;
        private int _prevSpeed;

        public NavigationService(
            ICoordinator coordinator, 
            IOverviewApiClient overviewApiClient, 
            ISelectItemApiClient selectItemApiClient,
            IHudInterfaceApiClient hudInterfaceApiClient,
            IProbeScannerApiClient probeScannerApiClient
        ) : base(coordinator, "navigation-service")
        {
            _overviewApiClient = overviewApiClient;
            _selectItemApiClient = selectItemApiClient;
            _hudInterfaceApiClient = hudInterfaceApiClient;
            _probeScannerApiClient = probeScannerApiClient;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            if (IsWarpOrJumpState())
            {
                await Wait(stoppingToken);
                return;
            }

            if (GotoNextSystemRequested())
            {
                await JumpToMarkedGate();
                await Wait(stoppingToken);
                return;
            }

            if (WarpToAnomalyRequested())
            {
                await WarpToAnomaly();
                await Wait(stoppingToken);
                return;
            }

            await EnsureCommandsExecuting();
            await UpdatePrevSpeed();
        }

        private async Task EnsureCommandsExecuting()
        {
            if (!IsCommandRequested())
            {
                if (!await IsShipStopping())
                    await ShipStop();

                return;
            }

            if (!IsCommandExecuting())
            {
                await ExecuteMovement();
            }
        }

        private async Task UpdatePrevSpeed()
        {
            _prevSpeed = await _hudInterfaceApiClient.GetCurrentSpeed();
        }

        private async Task<bool> IsShipStopping()
        {
            var currentSpeed = await _hudInterfaceApiClient.GetCurrentSpeed();
            if (currentSpeed < _prevSpeed || currentSpeed < 10)
            {
                return true;
            }
            return false;
        }

        private async Task ShipStop()
        {
            await _hudInterfaceApiClient.ShipStop();
        }

        public bool IsCommandRequested()
        {
            return Coordinator.Commands.MoveCommands.Any(cmd => cmd.Value.Requested);
        }

        public bool IsCommandExecuting()
        {
            var cmd = Coordinator.Commands.MoveCommands
                .Where(cmd => cmd.Value.Requested)
                .OrderByDescending(cmd => cmd.Key)
                .FirstOrDefault();
            return Coordinator.ShipState.CurrentMovement == cmd.Value.ExpectingMovementState
                && Coordinator.ShipState.CurrentMovementObject.Contains(cmd.Value.Target.Name);
        }

        public async Task ExecuteMovement()
        {
            var movementCommand = Coordinator.Commands.MoveCommands
                .Where(cmd => cmd.Value.Requested)
                .OrderByDescending(cmd => cmd.Key)
                .FirstOrDefault();

            if (movementCommand.Value.Target == null)
                return;

            await _overviewApiClient.ClickOnObject(movementCommand.Value.Target);
            await _selectItemApiClient.ClickButton(movementCommand.Value.Action.ToString());
        }

        public bool GotoNextSystemRequested()
        {
            return Coordinator.Commands.GotoNextSystemCommand.Requested;
        }

        public bool WarpToAnomalyRequested()
        {
            return Coordinator.Commands.WarpToAnomalyCommand.Requested;
        }

        public bool IsWarpOrJumpState()
        {
            return 
                Coordinator.ShipState.CurrentMovement == FlightMode.Warping
                || Coordinator.ShipState.CurrentMovement == FlightMode.Jumping;
        }

        public async Task JumpToMarkedGate()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();

            var markedGate = GetMarkedGate(ovObjects);

            if (markedGate is null)
                return;

            await _overviewApiClient.ClickOnObject(markedGate);
            var selectedItemInfo = await _selectItemApiClient.GetSelectItemInfo();

            // todo: case when appear miss click to obj on overview?
            if (selectedItemInfo.Buttons.Where(btn => btn.Action == "Jump").Any())
                await _selectItemApiClient.ClickButton("Jump");
            else
                await _selectItemApiClient.ClickButton("Dock");
        }

        private OverviewItem? GetMarkedGate(IEnumerable<OverviewItem> ovObjects)
        {
            if (!string.IsNullOrEmpty(Coordinator.Commands.GotoNextSystemCommand.NextSystemName)
                && Coordinator.Commands.GotoNextSystemCommand.NextSystemName != "string"
                )
            {
                return ovObjects
                    .Where(item => item.Name == Coordinator.Commands.GotoNextSystemCommand.NextSystemName)
                    .FirstOrDefault();
            }
            else
            {
                return ovObjects
                    .Where(item => Utils.Color2Text(item.Color) == Colors.Yellow)
                    .Where(item => item.Name != "Cargo container" && !item.Name.Contains("Wreck")) // Exclude Extra
                    .FirstOrDefault();
            }
        }

        public async Task WarpToAnomaly()
        {
            var scanResults = await _probeScannerApiClient.GetProbeScanResults();
            var anomaly = scanResults
                .FirstOrDefault(res => res.ID == Coordinator.Commands.WarpToAnomalyCommand.Anomaly.ID);

            if (anomaly is not null)
            {
                await _probeScannerApiClient.WarpToAnomaly(anomaly);
            }
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
