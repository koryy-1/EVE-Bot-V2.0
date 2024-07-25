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
    public class Executor : IExecutor
    {
        private Autopilot _autopilot;
        private FarmingStrategy _farmingStrategy;
        private ICoordinator _coordinator;
        private CancellationTokenSource _cancellationTokenSource;

        public Executor(ICoordinator coordinator,
            Autopilot autopilot,
            FarmingStrategy farmingStrategy
            )
        {
            _coordinator = coordinator;
            _autopilot = autopilot;
            _farmingStrategy = farmingStrategy;

        }
        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (_coordinator.Commands.ExecutorAuthorized
                    //&& !_autopilot.IsCompleted()
                    && !_coordinator.BotState.IsStrategyRunning
                    )
                    {
                        //switch (switch_on)
                        //{
                        //    default:
                        //}
                        //_autopilot.Start();
                        _farmingStrategy.Start();
                    }
                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _coordinator.Commands.ExecutorAuthorized = false;
            _cancellationTokenSource?.Cancel();
        }
    }
}
