using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class DestroyTargetCommand
    {
        public bool Requested { get; set; }
        public OverviewItem Target { get; set; }
        public IEnumerable<ShipModule> ActivatedModules {  get; set; }
        public IEnumerable<ShipModule> DisactivatedModules { get; set; }
    }
}
