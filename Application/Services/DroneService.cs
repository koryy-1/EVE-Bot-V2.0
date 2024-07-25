using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DroneService : IDroneService, IWorkerService
    {
        private IDroneApiClient _droneApiClient;
        private ICoordinator _coordinator;
        private bool _rescooping;
        private CancellationTokenSource _cancellationTokenSource;

        public DroneService(IDroneApiClient droneApiClient, ICoordinator coordinator)
        {
            _droneApiClient = droneApiClient;
            _coordinator = coordinator;
            _rescooping = false;
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (!_coordinator.Commands.IsBattleModeActivated)
                    {
                        if (!_coordinator.ShipState.DronesScooped)
                        {
                            await Scoop();
                        }
                        await Wait();
                        continue;
                    }

                    if (_rescooping)
                    {
                        await Rescoop();
                        await Wait();
                        continue;
                    }

                    await EnsureDronesLaunched();
                    await EnsureHpIsNormal();
                    await EnsureEngage();

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        private async Task Rescoop()
        {
            await Scoop();
            if (await AllDronesInBay())
            {
                _rescooping = false;
            }
        }

        private async Task<bool> AllDronesInBay()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.Where(drone => drone.Location == "space").Any())
            {
                return false;
            }
            return true;
        }

        private async Task EnsureHpIsNormal()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.Where(drone => drone.HealthPoints.Shield < 12).Any())
            {
                _rescooping = true;
            }
            _rescooping = false;
        }

        public async Task EnsureEngage()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.ToList()
                .Exists(drone => drone.Location == "space" && drone.WorkMode != "Fighting")
                && _coordinator.Commands.OpenFireAuthorized
                )
            {
                await _droneApiClient.Engage();
            }
        }

        private async Task EnsureDronesLaunched()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            var countInSpace = drones.ToList().Where(drone => drone.Location == "space").Count();
            if (countInSpace < _coordinator.Config.DronesInSpaceCount)
            {
                await Launch();
            }
        }

        public async Task Launch()
        {
            await _droneApiClient.Launch();
        }

        public async Task Scoop()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.ToList()
                .Exists(drone => drone.Location == "space" && drone.WorkMode != "Returning"))
            {
                await _droneApiClient.Scoop();
            }
        }

        public async Task Wait()
        {
            await Task.Delay(5000);
        }
    }
}
