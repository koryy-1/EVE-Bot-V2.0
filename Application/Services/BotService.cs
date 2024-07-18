using Application.Interfaces;
using Domen.Entities;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BotService : IBotService
    {
        private readonly IEnumerable<IWorkerService> _workerServices;
        private IEnumerable<Task> _workers;
        private readonly IExecutor _executor;
        private Task _execWorker;
        private IBotStateService _botStateService;
        private bool _isRunning;
        private readonly object _lock = new object();

        public BotService(IEnumerable<IWorkerService> workerServices, IExecutor executor, IBotStateService botStateService)
        {
            _workerServices = workerServices;
            _executor = executor;
            _botStateService = botStateService;
            _isRunning = false;
        }

        public void StartBotServices()
        {
            if (_isRunning)
            {
                // todo: http exception
                throw new InvalidOperationException("Bot services is already running.");
            }

            _isRunning = true;

            var workers = _workerServices.Select(service => service.StartAsync());
            var execWorker = _executor.StartAsync();

            // after checking output "Core Systems Operational"

            _workers = workers;
            _execWorker = execWorker;

            Task.WhenAll(workers);
            Task.WhenAll(execWorker);
        }

        public void StopBotServices()
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Bot services is not running.");
            }

            _isRunning = false;

            foreach (var service in _workerServices)
            {
                service.Stop();
            }
            _executor.Stop();
        }

        public void AuthorizeExecutor()
        {
            _botStateService.UpdateCommand(cmd => cmd.ExecutorAuthorized = true);
        }

        public void DenyExecutorAuthorization()
        {
            _botStateService.UpdateCommand(cmd => cmd.ExecutorAuthorized = false);
        }

        public BotState GetBotState()
        {
            lock (_lock)
            {
                return _botStateService.State;
            }
        }

        public BotStatus GetBotStatus()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    return new BotStatus { 
                        IsServicesRunning = _isRunning,
                        ExecWorkerStatus = _execWorker.Status.ToString(),
                        WorkerStatuses = _workers.Select(x => x.Status.ToString()),
                    };
                }
                else
                {
                    return new BotStatus();
                }
            }
        }

        public void LoadConfig(BotConfig config)
        {
            throw new NotImplementedException();
        }
    }
}
