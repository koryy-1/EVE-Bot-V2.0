using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using Domen.Entities.Commands;
using Domen.Enums;

namespace Application.Strategies
{
    public class Autopilot
    {
        private ICoordinator _coordinator;

        public Autopilot(ICoordinator botStateService)
        {
            _coordinator = botStateService;
        }

        public async void Start()
        {
            _coordinator.BotState.IsStrategyRunning = true;

            while (StartAuthorized() && !IsCompleted())
            {
                GotoNextSystem();
                await Task.Delay(5000);
            }
            Stop();
        }

        private void Stop()
        {
            _coordinator.BotState.IsStrategyRunning = false;
        }

        private bool StartAuthorized()
        {
            return _coordinator.Commands.ExecutorAuthorized;
        }

        public bool IsCompleted()
        {
            return !_coordinator.ShipState.IsDestSet;
        }

        private bool IsNotWarpState()
        {
            return _coordinator.ShipState.CurrentMovement != FlightMode.Warping;
        }

        private void GotoNextSystem()
        {
            _coordinator.Commands.GotoNextSystemCommand.Requested = true;
        }
    }
}
