using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class LootingCommand : CommandBase
    {
        public OverviewItem Container { get; set; }

        public LootingCommand()
        {
            IsFinite = true;
        }
    }
}
