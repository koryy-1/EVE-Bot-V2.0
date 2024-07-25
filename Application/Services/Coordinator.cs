using Application.Interfaces;
using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class Coordinator : ICoordinator
    {
        public BotCommands Commands { get; set; }
        public ShipState ShipState { get; set; }
        public BotState BotState {  get; set; }
        public BotConfig Config { get; private set; }

        public Coordinator()
        {
            Commands = new BotCommands();
            ShipState = new ShipState();
            BotState = new BotState();
        }

        public void SetConfig(BotConfig config)
        {
            Config = config;
        }
    }
}
