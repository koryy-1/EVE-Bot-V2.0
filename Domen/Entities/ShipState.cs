using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class ShipState
    {
        public string CurrentSystem { get; set; }
        public FlightMode CurrentMovement { get; set; }
        public string CurrentMovementObject { get; set; }
        public bool IsDestSet { get; set; }
        public bool DronesScooped { get; set; }
    }
}
