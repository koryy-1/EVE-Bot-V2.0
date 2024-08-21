using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.Entities.Commands;
using Domen.Enums;
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
                EnsureUnsetLockTargetsCommand();
                await Wait(stoppingToken);
                return;
            }

            // todo: сделать метод для свертывания боевых систем
            // (скуп дронов, изменение режима на скоростной у джекдо)

            await EnsureSetupLockTargetsCommand();

            UpdateOpenFireAuthorized();
            await EnsureSetupMovementCommand();
        }

        private async Task EnsureSetupLockTargetsCommand()
        {
            if (!await IsActualLockTargetsCommand())
                await SetLockTargetsCommand();
        }

        private async Task<bool> IsActualLockTargetsCommand()
        {
            // todo: если прилетела DestroyTargetCommand (а до этого ее не было),
            // то нужно тутже вызвать SetLockTargetsCommand, вернув здесь false
            return Coordinator.Commands.LockTargetsCommand.Requested
                && await IsCommandTargetsInWeaponRange();
        }

        private async Task<bool> IsCommandTargetsInWeaponRange()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            foreach (var target in Coordinator.Commands.LockTargetsCommand.Targets)
            {
                var tgt = ovObjects
                    .Where(item => item.Name == target.Name)
                    .Where(item => item.Type == target.Type)
                    .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);

                if (tgt.Any())
                {
                    return true;
                }
            }

            return false;
        }

        private async Task SetLockTargetsCommand()
        {
            var targets = GetTargets().GetAwaiter().GetResult()
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);

            if (!targets.Any())
                return;

            var cmd = new LockTargetsCommand()
            {
                Requested = true,
                Targets = targets
            };

            Coordinator.Commands.LockTargetsCommand = cmd;
        }

        private async Task<IEnumerable<OverviewItem>> GetTargets()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            if (Coordinator.Commands.DestroyTargetCommand.Requested)
            {
                return ovObjects
                    .Where(item => item.Name == Coordinator.Commands.DestroyTargetCommand.Target.Name)
                    .Where(item => item.Type == Coordinator.Commands.DestroyTargetCommand.Target.Type)
                    .OrderBy(item => Utils.Distance2Km(item.Distance));
            }
            else
            {
                return ovObjects
                    .Where(item => Utils.Color2Text(item.Color) == Colors.Red)
                    .OrderBy(item => Utils.Distance2Km(item.Distance));
            }
        }

        private void EnsureUnsetMovementCommand()
        {
            if (Coordinator.Commands.MoveCommands[PriorityLevel.Medium].Requested)
                UnsetMovementCommand();
        }

        private void EnsureUnsetLockTargetsCommand()
        {
            if (Coordinator.Commands.LockTargetsCommand.Requested)
                UnsetLockTargetsCommand();
        }

        private void UpdateOpenFireAuthorized()
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
            var primary = GetTargets().GetAwaiter().GetResult().FirstOrDefault();

            // todo: change logic
            if (primary is null)
                return false;

            return Utils.Distance2Km(primary.Distance) < Coordinator.Config.WeaponRange;
        }

        private async Task SetMovementCommand()
        {
            // todo: кейс когда есть 2 одиннаковые праймари цели, 1 за дист лока другой по близости
            // TargetService пытается залочить цель за дистанцией лока
            // а CombatService выставляет команду на апроч цели по близости
            // как решение выставлять праймари в броадкасте
            
            var primary = GetTargets().GetAwaiter().GetResult().FirstOrDefault();

            if (primary is null)
                return;

            var cmd = new MovementCommand()
            {
                Requested = true,
                Target = primary,
                Action = SpaceObjectAction.Approach,
                ExpectingMovementState = FlightMode.Approaching
            };

            Coordinator.Commands.MoveCommands[PriorityLevel.Medium] = cmd;
        }

        private void UnsetMovementCommand()
        {
            Coordinator.Commands.MoveCommands[PriorityLevel.Medium].Requested = false;
        }

        private void UnsetLockTargetsCommand()
        {
            Coordinator.Commands.LockTargetsCommand.Requested = false;
        }

        public bool IsAimTargetInWeaponRange()
        {
            return Coordinator.Commands.IsAimTargetInWeaponRange;
        }

        public bool IsTargetLocked()
        {
            return Coordinator.Commands.IsTargetLocked;
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
