using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class Drone
    {
        public Drone()
        {
            Pos = new Point();
            HealthPoints = new HealthPoints();
        }
        public Point Pos { get; set; }
        public string Location { get; set; }
        public string WorkMode { get; set; }
        public HealthPoints HealthPoints { get; set; }
    }
}
