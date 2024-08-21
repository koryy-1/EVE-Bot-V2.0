using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class MovementCommand : CommandBase
    {
        public OverviewItem Target { get; set; }
        public SpaceObjectAction Action { get; set; }
        public FlightMode ExpectingMovementState { get; set; }
    }
}
