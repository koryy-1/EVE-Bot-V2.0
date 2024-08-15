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
using System.Threading.Tasks;

namespace Application.Services
{
    public class CombatService : BotWorker, ICombatService
    {
        private IOverviewApiClient _overviewApiClient;

        public CombatService(
            ICoordinator coordinator, 
            IOverviewApiClient overviewApiClient
        ) : base(coordinator, "combat-service")
        {
            _overviewApiClient = overviewApiClient;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            if (!Coordinator.Commands.IsBattleModeActivated)
            {
                EnsureUnsetMovementCommand();
                await Wait(stoppingToken);
                return;
            }

            EnsureAimTargetInWeaponRange();
            await EnsureSetupMovementCommand();
        }

        private void EnsureUnsetMovementCommand()
        {
            if (Coordinator.Commands.MoveCommands[PriorityLevel.Medium].Requested)
                UnsetMovementCommand();
        }

        private void EnsureAimTargetInWeaponRange()
        {
            if (IsAimTargetInWeaponRange())
                Coordinator.Commands.OpenFireAuthorized = true;
            else
                Coordinator.Commands.OpenFireAuthorized = false;
        }

        private async Task EnsureSetupMovementCommand()
        {
            if (!await IsTargetsInWeaponRange())
                await SetMovementCommand();
            else
                UnsetMovementCommand();
        }

        private async Task<bool> IsTargetsInWeaponRange()
        {
            var primary = await GetPrimaryTarget();

            // todo: change logic
            if (primary is null)
                return true;

            return Utils.Distance2Km(primary.Distance) < Coordinator.Config.WeaponRange;
        }

        private async Task SetMovementCommand()
        {
            // todo: кейс когда есть 2 одиннаковые праймари цели, 1 за дист лока другой по близости
            // TargetService пытается залочить цель за дистанцией лока
            // а CombatService выставляет команду на апроч цели по близости
            // как решение выставлять праймари в броадкасте
            
            var praimaryTarget = await GetPrimaryTarget();

            if (praimaryTarget == null)
                return;

            var cmd = new MovementCommand()
            {
                Requested = true,
                Target = praimaryTarget,
                Action = SpaceObjectAction.Approach,
                ExpectingMovementState = FlightMode.Approaching
            };

            Coordinator.Commands.MoveCommands[PriorityLevel.Medium] = cmd;
        }

        private async Task<OverviewItem> GetPrimaryTarget()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            if (Coordinator.Commands.DestroyTargetCommand.Target is not null)
            {
                return ovObjects
                    .Where(item => item.Name == Coordinator.Commands.DestroyTargetCommand.Target.Name)
                    .Where(item => item.Type == Coordinator.Commands.DestroyTargetCommand.Target.Type)
                    .OrderBy(item => item.Distance.Value)
                    .FirstOrDefault();
            }
            else
            {
                return ovObjects
                    .Where(item => Utils.Color2Text(item.Color) == Colors.Red)
                    .OrderBy(item => item.Distance.Value)
                    .FirstOrDefault();
            }
        }

        private void UnsetMovementCommand()
        {
            Coordinator.Commands.MoveCommands[PriorityLevel.Medium].Requested = false;
        }

        public bool IsAimTargetInWeaponRange()
        {
            return Coordinator.Commands.IsAimTargetInWeaponRange;
        }

        public bool IsTargetLocked()
        {
            return Coordinator.Commands.IsTargetLocked;
        }

        public void SetDestroyTargetCommand()
        {
            throw new NotImplementedException();
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
