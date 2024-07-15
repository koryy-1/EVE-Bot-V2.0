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
        private IBotStateService _botStateService;
        private bool _rescooping;
        private CancellationTokenSource _cancellationTokenSource;

        public DroneService(IDroneApiClient droneApiClient, IBotStateService botStateService)
        {
            _droneApiClient = droneApiClient;
            _botStateService = botStateService;
            _rescooping = false;
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (_rescooping)
                    {
                        await Rescoop();
                    }
                    if (!_rescooping)
                    {
                        await EnsureHpIsNormal();
                        await EnsureEngage();
                    }
                    
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
            Scoop();
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
                return true;
            }
            return false;
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
            if (drones.ToList().Exists(drone => drone.WorkMode != "Fighting")
                && _botStateService.Command.AllowToOpenFire
                )
            {
                await _droneApiClient.Engage();
            }
        }

        public async Task Launch()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.ToList().Exists(drone => drone.Location == "bay")
                // todo: config for bot
                //&& drones.ToList().Where(drone => drone.Location == "space").Count() < config.dronesSpaceCount
                )
            {
                await _droneApiClient.Launch();
            }
        }

        public async Task Scoop()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            if (drones.ToList().Exists(drone => drone.WorkMode != "Returning"))
            {
                await _droneApiClient.Scoop();
            }
        }
    }
}
