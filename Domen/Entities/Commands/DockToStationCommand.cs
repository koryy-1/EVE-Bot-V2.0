using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class DockToStationCommand
    {
        public bool Requested { get; set; }
        public OverviewItem Station { get; set; }
    }
}
