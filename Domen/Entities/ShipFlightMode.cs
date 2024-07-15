using Domen.Enums;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class ShipFlightMode
    {
        public string ItemAndDistance { get; set; }
        public string ObjectName { get; set; }
        public Distance Distance { get; set; }
        public FlightMode FlightMode { get; set; }
    }
}
