using Application.Interfaces;
using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class BotStateService : IBotStateService
    {
        private BotState _botState;
        private BotCommand _botCommand;
        public BotState State => _botState;
        public BotCommand Command => _botCommand;

        public BotStateService()
        {
            _botState = new BotState();
            _botCommand = new BotCommand();
        }

        public void UpdateState(Action<BotState> updateAction)
        {
            lock (_botState)
            {
                updateAction(_botState);
            }
        }

        public void UpdateCommand(Action<BotCommand> updateAction)
        {
            lock (_botCommand)
            {
                updateAction(_botCommand);
            }
        }
    }
}
