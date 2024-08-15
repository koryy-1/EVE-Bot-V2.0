using Application.Interfaces;
using Application.Interfaces.ApiClients;
using Application.Services;
using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Strategies
{
    public class DestroyerStrategy
    {
        private IOverviewApiClient _overviewApiClient;
        private ICoordinator _coordinator;

        public DestroyerStrategy(IOverviewApiClient overviewApiClient, ICoordinator coordinator)
        {
            _overviewApiClient = overviewApiClient;
            _coordinator = coordinator;
        }

        public async Task DESTRXY_EVERYXNE()
        {
            if (!await IsEnemiesInOverview())
                return;

            _coordinator.Commands.IsBattleModeActivated = true;

            while (StartAuthorized() && await IsEnemiesInOverview())
            {
                await Task.Delay(1000);
            }

            _coordinator.Commands.IsBattleModeActivated = false;
        }

        private async Task<bool> IsEnemiesInOverview()
        {
            var ovObjects = await _overviewApiClient.GetOverViewInfo();
            var nearestCont = ovObjects
                .Where(item => Utils.Color2Text(item.Color) == Colors.Red);

            return nearestCont.Any();
        }

        private bool StartAuthorized()
        {
            return _coordinator.Commands.ExecutorAuthorized;
        }
    }
}
