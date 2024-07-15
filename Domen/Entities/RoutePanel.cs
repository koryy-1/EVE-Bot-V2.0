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
        public RoutePanel()
        {
            ButtonLoc = new Point();
        }
        public List<Color> Systems { get; set; }
        public Point ButtonLoc { get; set; }
    }
}
