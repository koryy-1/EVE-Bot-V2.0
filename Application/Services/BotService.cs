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
        public bool IsConfigLoaded { get; private set; }

        private ICoordinator _coordinator;
        private bool _isRunning;
        private readonly object _lock = new object();

        public BotService(ICoordinator coordinator)
        {
            IsConfigLoaded = false;

            _coordinator = coordinator;
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
            _coordinator.Commands.BotServiceAuthorized = true;

            // after checking output "Core Systems Operational"
        }

        public void StopBotServices()
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Bot services is not running.");
            }

            _isRunning = false;
            _coordinator.Commands.BotServiceAuthorized = false;
            DenyExecutorAuthorization();
        }

        public void AuthorizeExecutor()
        {
            if (!_coordinator.Commands.ExecutorAuthorized)
                _coordinator.Commands.ExecutorAuthorized = true;
        }

        public void DenyExecutorAuthorization()
        {
            if (_coordinator.Commands.ExecutorAuthorized)
                _coordinator.Commands.ExecutorAuthorized = false;
        }

        public BotState GetBotState()
        {
            lock (_lock)
            {
                return _coordinator.BotState;
            }
        }

        public BotStatus GetBotServiceStatus()
        {
            lock (_lock)
            {
                if (_isRunning)
                {
                    return new BotStatus { 
                        IsBotServicesRunning = _isRunning,
                    };
                }
                else
                {
                    return new BotStatus();
                }
            }
        }
        public BotConfig GetConfig()
        {
            lock (_lock)
            {
                return _coordinator.Config;
            }
        }

        public void LoadConfig(BotConfig config)
        {
            lock ( _lock)
            {
                _coordinator.SetConfig(config);
            }
            IsConfigLoaded = true;
        }
    }
}
