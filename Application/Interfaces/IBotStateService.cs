using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBotStateService
    {
        BotState State { get; }
        BotCommand Command { get; }
        void UpdateState(Action<BotState> updateAction);
        void UpdateCommand(Action<BotCommand> updateAction);
    }
}
