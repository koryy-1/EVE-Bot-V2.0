using Application.Interfaces;
using Application.Strategies;
using Domen.Entities.Commands;
using Domen.Enums;
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
        private CommandBase _command;

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

            ExecuteCommandFromQueue();

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

        private async void ExecuteCommandFromQueue()
        {
            if (_command.IsFinite && _command.Requested)
                return;

            _command = Coordinator.Commands.CommandQueue.Dequeue();
            switch (_command)
            {
                case LockTargetsCommand:
                    Coordinator.Commands.LockTargetsCommand = (LockTargetsCommand)_command;
                    break;
                case DestroyTargetCommand:
                    Coordinator.Commands.DestroyTargetCommand = (DestroyTargetCommand)_command;
                    break;
                case LootingCommand:
                    Coordinator.Commands.LootingCommand = (LootingCommand)_command;
                    break;
                case MovementCommand:
                    Coordinator.Commands.MoveCommands[PriorityLevel.Low] = (MovementCommand)_command;
                    break;
                case GotoNextSystemCommand:
                    Coordinator.Commands.GotoNextSystemCommand = (GotoNextSystemCommand)_command;
                    break;
                case WarpToAnomalyCommand:
                    Coordinator.Commands.WarpToAnomalyCommand = (WarpToAnomalyCommand)_command;
                    break;
                default:
                    break;
            }
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            await Task.Delay(5000, stoppingToken);
        }
    }
}
