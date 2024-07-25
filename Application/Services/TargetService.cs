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
    public class TargetService : ITargetService, IWorkerService
    {
        private ICoordinator _coordinator;
        private IOverviewApiClient _overviewApiClient;
        private ISelectItemApiClient _selectItemApiClient;

        private CancellationTokenSource _cancellationTokenSource;

        public TargetService(ICoordinator botStateService,
            IOverviewApiClient overviewApiClient,
            ISelectItemApiClient selectItemApiClient)
        {
            _coordinator = botStateService;
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
                    if (_coordinator.Commands.IsBattleModeActivated)
                    {
                        if (!await IsTargetsLocked())
                        {
                            _coordinator.Commands.IsTargetLocked = false;
                            await LockEnemyTargets();
                            await Wait();
                            continue;
                        }
                        _coordinator.Commands.IsTargetLocked = true;

                        if (IsCommandRequested())
                        {
                            await EnsureCommandExecuting();
                            await Task.Delay(1000);
                            continue;
                        }

                        if (await IsExtraTargetsLocked())
                        {
                            await UnlockExtraTargets();
                            continue;
                        }
                        await EnsureLockedTargetsInWeaponRange();
                        await EnsureAimTargetInWeaponRange();
                    }


                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task<bool> IsExtraTargetsLocked()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var extraTargets = ovObjects
                .Where(item => Utils.Color2Text(item.Color) != Colors.Red);
            if (extraTargets.Any())
            {
                return true;
            }
            return false;
        }

        private async Task UnlockExtraTargets()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var extraTargets = ovObjects
                .Where(item => Utils.Color2Text(item.Color) != Colors.Red);
            
            await UnlockTargets(extraTargets);
        }

        private async Task EnsureLockedTargetsInWeaponRange()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var lockedTarget = ovObjects
                .Where(item => item.TargetLocked);
            if (!lockedTarget
                .Where(item => Utils.Distance2Km(item.Distance) < _coordinator.Config.WeaponRange).Any())
            {
                _coordinator.Commands.IsTargetInWeaponRange = false;
                if (lockedTarget.Count() > 3)
                {
                    await UnlockTargets();
                }
            }
        }

        private async Task EnsureAimTargetInWeaponRange()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var lockedTarget = ovObjects
                .Where(item => item.AimOnTargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < _coordinator.Config.WeaponRange);
            if (lockedTarget.Any())
            {
                _coordinator.Commands.IsTargetInWeaponRange = true;
                return;
            }
            _coordinator.Commands.IsTargetInWeaponRange = false;

            await SwitchAimToNearestLockedTarget();
        }

        private async Task<bool> IsTargetsLocked()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            return ovObjects.Where(item => item.TargetLocked).Any();
        }

        private async Task LockEnemyTargets()
        {
            //todo: check lock new targets while still locking
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var enemies = ovObjects
                .Where(item => Utils.Color2Text(item.Color) == Colors.Red)
                .Where(item => Utils.Distance2Km(item.Distance) < _coordinator.Config.WeaponRange);
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
                _coordinator.Commands.IsTargetInWeaponRange = false;
                await ExecuteCommand();
                return;
            }
            _coordinator.Commands.IsTargetInWeaponRange = true;
        }

        private async Task ExecuteCommand()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var primaryTarget = ovObjects
                .Where(item => item.Name == _coordinator.Commands.DestroyTargetCommand.Target.Name)
                .FirstOrDefault();
            if (primaryTarget.TargetLocked)
            {
                await SwitchAimToTarget(primaryTarget);
                return;
            }
            await LockTargets(new List<OverviewItem> { primaryTarget });
        }

        private async Task<bool> IsCommandExecuting()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            return ovObjects
                .Where(item => item.Name == _coordinator.Commands.DestroyTargetCommand.Target.Name && item.AimOnTargetLocked)
                .Any();
        }

        private bool IsCommandRequested()
        {
            return _coordinator.Commands.DestroyTargetCommand.Requested;
        }

        public async Task SwitchAimToNearestLockedTarget()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestTarget = ovObjects
                .Where(item => item.TargetLocked)
                .Where(item => Utils.Distance2Km(item.Distance) < _coordinator.Config.WeaponRange)
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

        public async Task Wait()
        {
            await Task.Delay(5000);
        }

        public Task LockTargetByEffect(string effect)
        {
            throw new NotImplementedException();
        }
    }
}
