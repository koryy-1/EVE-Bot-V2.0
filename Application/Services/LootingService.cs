using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities.Commands;
using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LootingService : IWorkerService
    {
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;
        private IInventoryApiClient _inventoryApiClient;
        private ICoordinator _coordinator;
        private CancellationTokenSource _cancellationTokenSource;

        public LootingService(IOverviewApiClient overviewApiClient,
            ISelectItemApiClient selectItemApiClient,
            IInventoryApiClient inventoryApiClient,
            ICoordinator coordinator)
        {
            _overviewApiClient = overviewApiClient;
            _selectItemApiClient = selectItemApiClient;
            _inventoryApiClient = inventoryApiClient;
            _coordinator = coordinator;
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (!LootingRequested())
                    {
                        await Wait();
                        continue;
                    }

                    await EnsureSetupMovementCommand();
                    await EnsureContAvailableForLoot();
                    await EnsureCommandExecuted();

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private bool LootingRequested()
        {
            return _coordinator.Commands.LootingCommand.Requested;
        }

        private async Task EnsureSetupMovementCommand()
        {
            if (!await IsContainerAvailableForLoot())
                await SetMovementCommand();
            else
                UnsetMovementCommand();
        }

        private async Task<bool> IsContainerAvailableForLoot()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                // check gray color on looted cont
                //.Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => item.Name == _coordinator.Commands.LootingCommand.Container.Name)
                .Where(item => Utils.Distance2Km(item.Distance) < 2.5);

            return nearestCont.Any();
        }

        private async Task SetMovementCommand()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                // check gray color on looted cont
                //.Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => item.Name == _coordinator.Commands.LootingCommand.Container.Name)
                .OrderBy(item => item.Distance.Value)
                .FirstOrDefault();

            var cmd = new MovementCommand()
            {
                Requested = true,
                Target = nearestCont,
                Action = SpaceObjectAction.Approach,
                ExpectingMovementState = FlightMode.Approaching
            };

            _coordinator.Commands.MoveCommands[PriorityLevel.Low] = cmd;
        }

        private void UnsetMovementCommand()
        {
            _coordinator.Commands.MoveCommands[PriorityLevel.Low].Requested = false;
        }

        private async Task EnsureContAvailableForLoot()
        {
            if (await IsContainerAvailableForLoot())
            {
                await LootCont();
            }
        }

        private async Task LootCont()
        {
            var cont = _coordinator.Commands.LootingCommand.Container;
            await _overviewApiClient.ClickOnObject(cont);
            await _selectItemApiClient.ClickButton("OpenCargo");
            await Task.Delay(1000);
            if (await _inventoryApiClient.IsContainerOpened())
            {
                // если карго полное закрыть конт (и выставить флаг выгрузить карго)
                // если дешевый мусор то закрыть конт
                await _inventoryApiClient.LootAll();
            }
        }

        private async Task EnsureCommandExecuted()
        {
            // условия выполнения команды
            // цвет иконки стал темно серым
            // иконка поменялась на пустую
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                // check gray color on looted cont
                //.Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => item.Name == _coordinator.Commands.LootingCommand.Container.Name)
                .FirstOrDefault();

            if (await _inventoryApiClient.IsContainerOpened()

                )
            {
                _coordinator.Commands.LootingCommand.Requested = false;
            }
        }

        public async Task Wait()
        {
            await Task.Delay(5000);
        }
    }
}
