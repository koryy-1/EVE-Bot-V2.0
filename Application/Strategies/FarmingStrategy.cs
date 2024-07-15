using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Strategies
{
    public class FarmingStrategy
    {
        private IBotStateService _botStateService;
        private DestroyerStrategy _destroyerStrategy;
        public FarmingStrategy(IBotStateService botStateService, DestroyerStrategy destroyerStrategy)
        {
            _botStateService = botStateService;
            _destroyerStrategy = destroyerStrategy;
        }

        public void Start()
        {
            while (true)
            {
                while (IsAnomalyInCurrentSystem())
                {
                    WarpToAnomaly();
                    _destroyerStrategy.DESTRXY_EVERYXNE();
                    LootConts();
                }

                if (!IsMarkedGate())
                    break;

                GotoNextSystem();
            }
        }

        private void GotoNextSystem()
        {
            _botStateService.UpdateCommand(cmd => cmd.GotoNextSystemRequested = true);
        }

        private bool IsMarkedGate()
        {
            throw new NotImplementedException();
        }

        private void LootConts()
        {
            throw new NotImplementedException();
        }

        private void WarpToAnomaly()
        {
            throw new NotImplementedException();
        }

        private bool IsAnomalyInCurrentSystem()
        {
            throw new NotImplementedException();
        }
    }
}
