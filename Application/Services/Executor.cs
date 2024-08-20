using Application.Interfaces;
using Application.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
    public class Executor : BotWorker, IExecutor
    {
        private Autopilot _autopilot;
        private FarmingStrategy _farmingStrategy;

        public Executor(
            ICoordinator coordinator,
            Autopilot autopilot,
            FarmingStrategy farmingStrategy
        ) : base(coordinator, "executor")
        {
            _autopilot = autopilot;
            _farmingStrategy = farmingStrategy;
        }

        protected override async Task CyclingWork(CancellationToken stoppingToken)
        {
            if (!Coordinator.Commands.ExecutorAuthorized)
            {
                await Task.Delay(1000, stoppingToken);
                return;
            }

            if (!Coordinator.BotState.IsStrategyRunning
            //&& !_autopilot.IsCompleted()
            )
            {
                //switch (switch_on)
                //{
                //    default:
                //}
                //_autopilot.Start();
                _farmingStrategy.Start();
            }
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
