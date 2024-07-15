using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Services;
using Domen.Enums;

namespace Application.Strategies
{
    public class Autopilot
    {
        private IBotStateService _botStateService;

        public Autopilot(IBotStateService botStateService)
        {
            _botStateService = botStateService;
        }

        public async void Start()
        {
            _botStateService.UpdateState(state => state.IsStrategyRunning = true);
            //todo: не завершать стратку если корабль в варпе
            while (IsDestSet() && IsNotWarpState() && StartAuthorized())
            {
                GotoNextSystem();
                await Task.Delay(5000);
            }
            Stop();
        }

        private void Stop()
        {
            _botStateService.UpdateState(state => state.IsStrategyRunning = false);
        }

        private bool StartAuthorized()
        {
            return _botStateService.Command.ExecutorAuthorized;
        }

        private bool IsDestSet()
        {
            return _botStateService.State.IsDestSet;
        }

        private bool IsNotWarpState()
        {
            return _botStateService.State.CurrentMovement != FlightMode.Warping;
        }

        private void GotoNextSystem()
        {
            _botStateService.UpdateCommand(cmd => cmd.GotoNextSystemRequested = true);
        }
    }
}
