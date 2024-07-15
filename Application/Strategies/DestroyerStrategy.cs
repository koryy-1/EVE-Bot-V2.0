using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Strategies
{
    public class DestroyerStrategy
    {
        private IBotStateService _botStateService;
        public DestroyerStrategy(IBotStateService botStateService)
        {
            _botStateService = botStateService;
        }

        public void DESTRXY_EVERYXNE()
        {
            _botStateService.UpdateCommand(cmd => cmd.AllowToOpenFire = true);
        }
    }
}
