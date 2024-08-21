using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class DockToStationCommand : CommandBase
    {
        public OverviewItem Station { get; set; }

        public DockToStationCommand()
        {
            IsFinite = true;
        }
    }
}
