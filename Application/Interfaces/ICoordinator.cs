using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICoordinator
    {
        BotCommands Commands { get; }
        ShipState ShipState { get; set; }
        BotState BotState { get; }
        BotConfig Config { get; }
        void SetConfig(BotConfig config);
    }
}
