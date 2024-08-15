using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
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
    public class TargetService : BotWorker, ITargetService
    {
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;

        public TargetService(
            ICoordinator coordinator,
            IOverviewApiClient overviewApiClient,
            ISelectItemApiClient selectItemApiClient
        ) : base(coordinator, "target-service")
        {
            _overviewApiClient = overviewApiClient;
            _selectItemApiClient = selectItemApiClient;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            if (!Coordinator.Commands.IsBattleModeActivated)
            {
                await Wait(stoppingToken);
                return;
            }

            if (IsCommandRequested())
            {
                await EnsureCommandExecuting();
                await Task.Delay(1000, stoppingToken);
                return;
            }

            if (!await IsTargetsLocked())
            {
                Coordinator.Commands.IsAimTargetInWeaponRange = false;
                await LockEnemyTargets();
                return;
            }

            if (await IsLockInProgress())
            {
                Coordinator.Commands.IsAimTargetInWeaponRange = false;
                await Task.Delay(2000, stoppingToken);
                return;
            }

            if (await IsExtraTargetsLocked())
            {
                await UnlockExtraTargets();
                return;
            }

            await EnsureLockedTargetsInWeaponRange();
            await EnsureAimTargetInWeaponRange();
            await EnsureSwitchingTarget();
        }

        private async Task<bool> IsExtraTargetsLocked()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var extraTargets = ovObjects
                .Where(item => item.TargetLocked)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Red);

            return extraTargets.Any();
        }

        private async Task UnlockExtraTargets()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var extraTargets = ovObjects
                .Where(item => item.TargetLocked)
                .Where(item => Utils.Color2Text(item.Color) != Colors.Red);
            
            await UnlockTargets(extraTargets);
        }

        private async Task EnsureLockedTargetsInWeaponRange()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var lockedTarget = ovObjects
                .Where(item => item.TargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);
            if (!lockedTarget.Any())
            {
                Coordinator.Commands.IsAimTargetInWeaponRange = false;
                if (lockedTarget.Count() > 3)
                {
                    await UnlockTargets();
                }
            }
        }

        private async Task EnsureAimTargetInWeaponRange()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var aimedTarget = ovObjects
                .Where(item => item.AimOnTargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);
            if (aimedTarget.Any())
                Coordinator.Commands.IsAimTargetInWeaponRange = true;
            else
                Coordinator.Commands.IsAimTargetInWeaponRange = false;
        }

        private async Task EnsureSwitchingTarget()
        {
            if (!Coordinator.Commands.IsAimTargetInWeaponRange)
            {
                await SwitchAimToNearestLockedTarget();
            }
        }

        private async Task<bool> IsLockInProgress()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            return ovObjects.Where(item => item.LockInProgress).Any();
        }

        private async Task<bool> IsTargetsLocked()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            return ovObjects.Where(item => item.TargetLocked || item.LockInProgress).Any();
        }

        private async Task LockEnemyTargets()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var enemies = ovObjects
                .Where(item => Utils.Color2Text(item.Color) == Colors.Red)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);
            
            if (enemies.Any())
            {
                await LockTargets(enemies);
            }
        }

        private async Task EnsureCommandExecuting()
        {
            if (!IsCommandRequested())
                return;

            if (!await IsCommandExecuting())
            {
                Coordinator.Commands.IsAimTargetInWeaponRange = false;
                await ExecuteCommand();
                return;
            }
            Coordinator.Commands.IsAimTargetInWeaponRange = true;
        }

        private async Task ExecuteCommand()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var primaryTarget = ovObjects
                .Where(item => item.Name == Coordinator.Commands.DestroyTargetCommand.Target.Name)
                .Where(item => item.Type == Coordinator.Commands.DestroyTargetCommand.Target.Type)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange)
                .FirstOrDefault();
            if (primaryTarget.TargetLocked)
            {
                await SwitchAimToTarget(primaryTarget);
            }
            else
            {
                await LockTargets(new List<OverviewItem> { primaryTarget });
            }
        }

        private async Task<bool> IsCommandExecuting()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            return ovObjects
                .Where(item => item.Name == Coordinator.Commands.DestroyTargetCommand.Target.Name)
                .Where(item => item.Type == Coordinator.Commands.DestroyTargetCommand.Target.Type)
                .Where(item => item.AimOnTargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange)
                .Any();
        }

        private bool IsCommandRequested()
        {
            return Coordinator.Commands.DestroyTargetCommand.Requested;
        }

        public async Task SwitchAimToNearestLockedTarget()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestTarget = ovObjects
                .Where(item => item.TargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange)
                .OrderBy(item => item.Distance.Value)
                .FirstOrDefault();
            if (nearestTarget != null)
            {
                await SwitchAimToTarget(nearestTarget);
            }
        }

        private async Task SwitchAimToTarget(OverviewItem primaryTarget)
        {
            await _overviewApiClient.ClickOnObject(primaryTarget);
        }

        public async Task UnlockTargets()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var lockedTargets = ovObjects.Where(item => item.TargetLocked);
            foreach (var target in lockedTargets)
            {
                await UnlockTarget(target);
                await Task.Delay(1000);
            }
        }

        public async Task UnlockTargets(IEnumerable<OverviewItem> lockedTargets)
        {
            foreach (var target in lockedTargets)
            {
                await UnlockTarget(target);
                await Task.Delay(1000);
            }
        }

        public async Task UnlockTarget(OverviewItem overviewItems)
        {
            await _overviewApiClient.ClickOnObject(overviewItems);
            await Task.Delay(500);
            await _selectItemApiClient.ClickButton("UnLockTarget");
        }

        public async Task LockTargets(IEnumerable<OverviewItem> overviewItems)
        {
            await _overviewApiClient.LockTargets(overviewItems);
        }

        public async Task LockTargetByName(string targetName)
        {
            await _overviewApiClient.LockTargetByName(targetName);
        }

        public Task LockTargetByEffect(string effect)
        {
            throw new NotImplementedException();
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
