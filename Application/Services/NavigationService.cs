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
    public class NavigationService : INavigationService, IWorkerService
    {
        private ICoordinator _coordinator;
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;
        private IProbeScannerApiClient _probeScannerApiClient;

        private CancellationTokenSource _cancellationTokenSource;

        public NavigationService(ICoordinator coordinator, 
            IOverviewApiClient overviewApiClient, 
            ISelectItemApiClient selectItemApiClient,
            IProbeScannerApiClient probeScannerApiClient)
        {
            _coordinator = coordinator;
            _overviewApiClient = overviewApiClient;
            _selectItemApiClient = selectItemApiClient;
            _probeScannerApiClient = probeScannerApiClient;
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (IsWarpOrJumpState())
                    {
                        await Wait();
                        continue;
                    }

                    if (GotoNextSystemRequested())
                    {
                        await JumpToMarkedGate();
                        await Wait();
                        continue;
                    }

                    if (WarpToAnomalyRequested())
                    {
                        await WarpToAnomaly();
                        await Wait();
                        continue;
                    }

                    await EnsureCommandsExecuting();

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task EnsureCommandsExecuting()
        {
            // ship stop if not requested?
            if (!IsCommandRequested())
                return;

            if (!IsCommandExecuting())
            {
                await ExecuteMovement();
            }
        }

        public bool IsCommandRequested()
        {
            return _coordinator.Commands.MoveCommands.Any(cmd => cmd.Value.Requested);
        }

        public bool IsCommandExecuting()
        {
            var cmd = _coordinator.Commands.MoveCommands
                .Where(cmd => cmd.Value.Requested)
                .OrderByDescending(cmd => cmd.Key)
                .FirstOrDefault();
            return _coordinator.ShipState.CurrentMovement == cmd.Value.ExpectingMovementState;
        }

        public async Task ExecuteMovement()
        {
            var cmd = _coordinator.Commands.MoveCommands
                .Where(cmd => cmd.Value.Requested)
                .OrderByDescending(cmd => cmd.Key)
                .FirstOrDefault();

            await _overviewApiClient.ClickOnObject(cmd.Value.Target);
            await _selectItemApiClient.ClickButton(cmd.Value.Action.ToString());
        }

        public bool GotoNextSystemRequested()
        {
            return _coordinator.Commands.GotoNextSystemCommand.Requested;
        }

        public bool WarpToAnomalyRequested()
        {
            return _coordinator.Commands.WarpToAnomalyCommand.Requested;
        }

        public bool IsWarpOrJumpState()
        {
            return 
                _coordinator.ShipState.CurrentMovement == FlightMode.Warping
                || _coordinator.ShipState.CurrentMovement == FlightMode.Jumping;
        }

        public async Task JumpToMarkedGate()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            
            var markedGate = ovObjects
                .Where(item => item.Name == _coordinator.Commands.GotoNextSystemCommand.NextSystemName)
                .FirstOrDefault();

            if (markedGate is null)
            {
                markedGate = ovObjects
                    .Where(item => Utils.Color2Text(item.Color) == Colors.Yellow)
                    .Where(item => item.Name != "Cargo container" && !item.Name.Contains("Wreck")) // Exclude Extra
                    .FirstOrDefault();
            }
            if (markedGate is null)
                return;

            _coordinator.Commands.GotoNextSystemCommand.NextSystemName = markedGate.Name;

            await _overviewApiClient.ClickOnObject(markedGate);
            var selectedItemInfo = await _selectItemApiClient.GetSelectItemInfo();

            // todo: case when appear miss click to obj on overview?
            if (selectedItemInfo.Buttons.Where(btn => btn.Action == "Jump").Any())
                await _selectItemApiClient.ClickButton("Jump");
            else
                await _selectItemApiClient.ClickButton("Dock");
        }

        public async Task WarpToAnomaly()
        {
            var nearlyAnomaly = _coordinator.Commands.WarpToAnomalyCommand.Anomaly;
            await _probeScannerApiClient.WarpToAnomaly(nearlyAnomaly);
        }

        public async Task Wait()
        {
            await Task.Delay(5000);
        }
    }
}
