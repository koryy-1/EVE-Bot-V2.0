using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.Entities.Commands;
using Domen.Enums;
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
    public class LootingService : BotWorker
    {
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;
        private IInventoryApiClient _inventoryApiClient;

        public LootingService(
            IOverviewApiClient overviewApiClient,
            ISelectItemApiClient selectItemApiClient,
            IInventoryApiClient inventoryApiClient,
            ICoordinator coordinator
        ) : base(coordinator, "looting-service")
        {
            _overviewApiClient = overviewApiClient;
            _selectItemApiClient = selectItemApiClient;
            _inventoryApiClient = inventoryApiClient;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            if (!LootingRequested())
            {
                await Wait(stoppingToken);
                return;
            }

            await EnsureSetupMovementCommand();
            await EnsureContAvailableForLoot();
            await EnsureCommandExecuted();
        }

        private bool LootingRequested()
        {
            return Coordinator.Commands.LootingCommand.Requested;
        }

        private async Task EnsureSetupMovementCommand()
        {
            if (!GetFilteredConts().GetAwaiter().GetResult().Any())
            {
                UnsetMovementCommand();
                return;
            }

            if (!await IsContainerAvailableForLoot())
                await SetMovementCommand();
            else
                UnsetMovementCommand();
        }

        private async Task<bool> IsContainerAvailableForLoot()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                .Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => Utils.Color2Text(item.Color) != Colors.DarkYellow)
                .Where(item => item.Name == Coordinator.Commands.LootingCommand.Container.Name)
                .Where(item => Utils.Distance2Km(item.Distance) < 2.5);

            return nearestCont.Any();
        }

        private async Task SetMovementCommand()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                .Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => Utils.Color2Text(item.Color) != Colors.DarkYellow)
                .Where(item => item.Name == Coordinator.Commands.LootingCommand.Container.Name)
                .OrderBy(item => Utils.Distance2Km(item.Distance))
                .FirstOrDefault();

            if (nearestCont is null)
                return;

            var cmd = new MovementCommand()
            {
                Requested = true,
                Target = nearestCont,
                Action = SpaceObjectAction.Approach,
                ExpectingMovementState = FlightMode.Approaching
            };

            Coordinator.Commands.MoveCommands[PriorityLevel.Low] = cmd;
        }

        private void UnsetMovementCommand()
        {
            Coordinator.Commands.MoveCommands[PriorityLevel.Low].Requested = false;
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
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                .Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => Utils.Color2Text(item.Color) != Colors.DarkYellow)
                .Where(item => item.Name == Coordinator.Commands.LootingCommand.Container.Name)
                .OrderBy(item => Utils.Distance2Km(item.Distance))
                .FirstOrDefault();

            if (nearestCont is null)
                return;

            await _overviewApiClient.ClickOnObject(nearestCont);
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
            var conts = await GetFilteredConts();

            if (!await _inventoryApiClient.IsContainerOpened()
                && !conts.Any()
                )
            {
                Coordinator.Commands.LootingCommand.Requested = false;
            }
        }

        private async Task<IEnumerable<OverviewItem>> GetFilteredConts()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            return ovObjects
                .Where(item => Utils.Color2Text(item.Color) != Colors.Gray)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Yellow)
                .Where(item => Utils.Color2Text(item.Color) != Colors.DarkYellow)
                .Where(item => item.Name == Coordinator.Commands.LootingCommand.Container.Name);
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
