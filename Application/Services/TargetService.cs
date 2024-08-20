using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using Domen.Enums;
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

            if (!Coordinator.Commands.LockTargetsCommand.Requested)
            {
                await Wait(stoppingToken);
                return;
            }

            // кейс когда залочены 5 целей из команды LockTargetsCommand
            // потом команда обновилась и теперь в массиве только 1 цель
            // если макс лок = 5, разлочить лишние таргеты
            //      - которые не коррелируют с обновленным массивом
            //      и для освобождения места под новые таргеты

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

            await EnsureExtraTargetsUnlocked();
            await EnsureLockedTargetsInWeaponRange();
            await EnsureAimTargetInWeaponRange();
            await EnsureSwitchingTarget();
        }

        private async Task EnsureLockedTargetsInWeaponRange()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var lockedTarget = ovObjects
                .Where(item => item.TargetLocked);

            var lockedTargetInWeaponRange = lockedTarget
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);

            if (!lockedTargetInWeaponRange.Any())
            {
                // todo: create method UnlockUnnecessaryTargets
                Coordinator.Commands.IsAimTargetInWeaponRange = false;
                if (lockedTarget.Count() > 3) // todo: instead 3 put value from config
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

        private async Task EnsureExtraTargetsUnlocked()
        {
            if (await IsExtraTargetsLocked())
            {
                await UnlockExtraTargets();
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
            if (!Coordinator.Commands.LockTargetsCommand.Requested)
                return;

            var targets = GetActualTargetsForLocking().GetAwaiter().GetResult()
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange);
            
            if (targets.Any())
            {
                await LockTargets(targets);
            }
        }

        private async Task<IEnumerable<OverviewItem>> GetActualTargetsForLocking()
        {
            List<OverviewItem> actualTargets = new List<OverviewItem>();
            foreach (var target in Coordinator.Commands.LockTargetsCommand.Targets)
            {
                var ovObjects = await _overviewApiClient.GetOverViewInfo();
                var tgt = ovObjects
                    .Where(item => item.Name == target.Name)
                    .Where(item => item.Type == target.Type)
                    .FirstOrDefault();

                if (tgt is not null)
                {
                    actualTargets.Add(tgt);
                }
            }

            return actualTargets;
        }

        public async Task SwitchAimToNearestLockedTarget()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestTarget = ovObjects
                .Where(item => item.TargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < Coordinator.Config.WeaponRange)
                .OrderBy(item => item.Distance.Value)
                .FirstOrDefault();
            if (nearestTarget is not null)
            {
                await SwitchAimToTarget(nearestTarget);
            }
        }

        private async Task SwitchAimToTarget(OverviewItem primaryTarget)
        {
            await _overviewApiClient.ClickOnObject(primaryTarget);
        }

        private async Task<bool> IsExtraTargetsLocked()
        {
            foreach (var target in Coordinator.Commands.LockTargetsCommand.Targets)
            {
                var ovObjects = await _overviewApiClient.GetOverViewInfo();
                var tgt = ovObjects
                    .Where(item => item.Name != target.Name && item.Type != target.Type)
                    .Where(item => item.TargetLocked);

                if (tgt.Any())
                {
                    return true;
                }
            }

            return false;
        }

        private async Task UnlockExtraTargets()
        {
            List<OverviewItem> extraTargets = new List<OverviewItem>();
            foreach (var target in Coordinator.Commands.LockTargetsCommand.Targets)
            {
                var ovObjects = await _overviewApiClient.GetOverViewInfo();
                var tgt = ovObjects
                    .Where(item => item.Name != target.Name && item.Type != target.Type)
                    .Where(item => item.TargetLocked)
                    .FirstOrDefault();

                if (tgt is not null)
                {
                    extraTargets.Add(tgt);
                }
            }

            await UnlockTargets(extraTargets);
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
