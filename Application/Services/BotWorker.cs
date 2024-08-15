using Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public abstract class BotWorker : BackgroundService
    {
        public ICoordinator Coordinator { get; set; }
        private DateTime _lastExecutionTime;
        private string _serviceName;

        public BotWorker(ICoordinator coordinator, string serviceName)
        {
            Coordinator = coordinator;
            _serviceName = serviceName;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Coordinator.Commands.BotServiceAuthorized)
                {
                    await Wait(stoppingToken);
                    continue;
                }

                await CyclingWork(stoppingToken);

                AddBgJob();

                await Task.Delay(1000, stoppingToken);
            }
        }

        protected abstract Task CyclingWork(CancellationToken stoppingToken);

        private void AddBgJob()
        {
            if (DateTime.UtcNow - _lastExecutionTime < TimeSpan.FromSeconds(5))
                return;

            _lastExecutionTime = DateTime.UtcNow;

            BackgroundJob.Enqueue(() => CheckerStatus(_serviceName));
        }

        public void CheckerStatus(string serviceName)
        {
            Console.WriteLine($"{serviceName} status checked via Hangfire.");
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
