using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class BotCommand
    {
        public bool ExecutorAuthorized { get; set; }
        public bool AllowToOpenFire { get; set; }
        public bool GotoNextSystemRequested { get; set; }
        public SpaceObjectAction MovementCommand { get; set; }
        public FlightMode ExpectingMovementState { get; set; }
        public OverviewItem MovementObject { get; set; }
    }
}
