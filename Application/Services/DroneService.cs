using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Domen.Entities;
using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DroneService : BotWorker, IDroneService
    {
        private IDroneApiClient _droneApiClient;
        private bool _rescooping;

        public DroneService(
            IDroneApiClient droneApiClient, 
            ICoordinator coordinator
        ) : base(coordinator, "drone-service")
        {
            _droneApiClient = droneApiClient;
            _rescooping = false;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            if (!Coordinator.Commands.IsBattleModeActivated)
            {
                if (!Coordinator.ShipState.DronesScooped)
                {
                    await Scoop();
                }
                await Wait(stoppingToken);
                return;
            }

            if (_rescooping)
            {
                await Rescoop();
                await Wait(stoppingToken);
                return;
            }

            await EnsureDronesLaunched();
            await EnsureHpIsNormal();
            await EnsureEngage();
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
                && Coordinator.Commands.OpenFireAuthorized
                )
            {
                await _droneApiClient.Engage();
            }
        }

        private async Task EnsureDronesLaunched()
        {
            var drones = await _droneApiClient.GetDronesInfo();
            var countInSpace = drones.Where(drone => drone.Location == "space").Count();
            var countInBay = drones.Where(drone => drone.Location == "bay").Count();
            if (countInBay != 0 && countInSpace < Coordinator.Config.DronesInSpaceCount)
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

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
