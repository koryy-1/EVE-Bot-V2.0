using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class RoutePanel
    {
        public string NextSystemInRoute { get; set; }
        public string CurrentDestination { get; set; }
        public List<Color> Systems { get; set; }
        public Point ButtonLoc { get; set; }

        public RoutePanel()
        {
            ButtonLoc = new Point();
        }
    }
}
