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
        private IBotStateService _botStateService;
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;
        private IProbeScannerApiClient _probeScannerApiClient;
        private CancellationTokenSource _cancellationTokenSource;

        public NavigationService(IBotStateService botStateService, 
            IOverviewApiClient overviewApiClient, 
            ISelectItemApiClient selectItemApiClient)
        {
            _botStateService = botStateService;
            _overviewApiClient = overviewApiClient;
            _selectItemApiClient = selectItemApiClient;
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

                    //if (GotoAnomalyRequested)
                    //{
                    //    WarpToAnomaly();
                    //    await Wait();
                    //    continue;
                    //}

                    if (!IsCommandExecuting())
                    {
                        await ExecuteMovement();
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public bool GotoNextSystemRequested()
        {
            return _botStateService.Command.GotoNextSystemRequested;
        }

        public bool IsCommandExecuting()
        {
            if (!IsMovementCommandSet())
            {
                return true;
            }

            return _botStateService.State.CurrentMovement == _botStateService.Command.ExpectingMovementState;
        }

        public bool IsMovementCommandSet()
        {
            return _botStateService.Command.MovementCommand != SpaceObjectAction.None;
        }

        public async Task ExecuteMovement()
        {
            await _overviewApiClient.ClickOnObject(_botStateService.Command.MovementObject);
            await _selectItemApiClient.ClickButton(_botStateService.Command.MovementCommand.ToString());
        }

        public bool IsWarpOrJumpState()
        {
            return 
                _botStateService.State.CurrentMovement == FlightMode.Warping
                || _botStateService.State.CurrentMovement == FlightMode.Jumping;
        }

        public async Task JumpToMarkedGate()
        {
            var items = await _overviewApiClient.GetOverViewInfo();

            var markedGate = items.Where(item => Utils.Color2Text(item.Color) == Colors.Yellow).FirstOrDefault();
            // добавить исключения типа cargo container, wreck и тд

            if (markedGate != null)
            {
                await _overviewApiClient.ClickOnObject(markedGate);
                var selectedItemInfo = await _selectItemApiClient.GetSelectItemInfo();
                if (selectedItemInfo.Buttons.Where(btn => btn.Action == "Jump").Any())
                {
                    await _selectItemApiClient.ClickButton("Jump");
                }
                else
                {
                    await _selectItemApiClient.ClickButton("Dock");
                }
            }

            // todo: finish GotoNextSystemRequested when system changing or dest is null
            _botStateService.UpdateCommand(cmd => cmd.GotoNextSystemRequested = false);
        }

        // todo: WarpToAnomaly will used rarely, separate to another "Strategy" class
        public async Task WarpToAnomaly()
        {
            var anomalies = await _probeScannerApiClient.GetProbeScanResults();
            if (!anomalies.Any())
            {
                return;
            }
            var nearlyAnomaly = anomalies.OrderBy(anomaly => anomaly.Distance.value).FirstOrDefault();
            await _probeScannerApiClient.WarpToAnomaly(nearlyAnomaly);

            // todo: finish GotoAnomalyRequested when ship locating near anomaly in 2-3 km
            //_botStateService.UpdateCommand(cmd => cmd.GotoAnomalyRequested = false);
        }

        public async Task Wait()
        {
            await Task.Delay(5000);
        }
    }
}
