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
        private IBotStateService _botStateService;
        private CancellationTokenSource _cancellationTokenSource;

        public Executor(IBotStateService botStateService)
        {
            _botStateService = botStateService;
            _autopilot = new Autopilot(_botStateService);

        }
        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    if (_botStateService.Command.ExecutorAuthorized
                    && !_botStateService.State.IsStrategyRunning
                    )
                    {
                        _autopilot.Start();
                    }
                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _botStateService.UpdateCommand(cmd => cmd.ExecutorAuthorized = false);
            _cancellationTokenSource?.Cancel();
        }
    }
}
