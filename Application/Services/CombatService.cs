using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities.Commands;
using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CombatService : ICombatService, IWorkerService
    {
        private ICoordinator _coordinator;
        private IOverviewApiClient _overviewApiClient;
        private CancellationTokenSource _cancellationTokenSource;

        public CombatService(ICoordinator coordinator, IOverviewApiClient overviewApiClient)
        {
            _coordinator = coordinator;
            _overviewApiClient = overviewApiClient;
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
                        EnsureAimTargetInWeaponRange();
                        await EnsureSetupMovementCommand();
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void EnsureAimTargetInWeaponRange()
        {
            if (IsAimTargetInWeaponRange())
                _coordinator.Commands.OpenFireAuthorized = true;
            else
                _coordinator.Commands.OpenFireAuthorized = false;
        }

        private async Task EnsureSetupMovementCommand()
        {
            if (!IsAimTargetInWeaponRange())
                await SetMovementCommand();
            else
                UnsetMovementCommand();
        }

        private async Task SetMovementCommand()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestTarget = ovObjects
                .Where(item => Utils.Color2Text(item.Color) == Colors.Red)
                .OrderBy(item => item.Distance.Value)
                .FirstOrDefault();

            var cmd = new MovementCommand()
            {
                Requested = true,
                Target = nearestTarget,
                Action = SpaceObjectAction.Approach,
                ExpectingMovementState = FlightMode.Approaching
            };

            _coordinator.Commands.MoveCommands[PriorityLevel.Medium] = cmd;
        }

        private void UnsetMovementCommand()
        {
            _coordinator.Commands.MoveCommands[PriorityLevel.Medium].Requested = false;
        }

        public bool IsAimTargetInWeaponRange()
        {
            return _coordinator.Commands.IsTargetInWeaponRange;
        }

        public bool IsTargetLocked()
        {
            return _coordinator.Commands.IsTargetLocked;
        }

        public void SetDestroyTargetCommand()
        {
            throw new NotImplementedException();
        }
    }
}
