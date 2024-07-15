﻿using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class HudInterface
    {
        public HudInterface()
        {
            Pos = new Point();
            HealthPoints = new HealthPoints();
        }
        public Point Pos { get; set; }
        public List<Module> AllModules { get; set; }
        //public List<Module> HighModules { get; set; }
        //public List<Module> MedModules { get; set; }
        //public List<Module> LowModules { get; set; }
        public HealthPoints HealthPoints { get; set; }
        public int CurrentSpeed { get; set; }
        public ShipFlightMode ShipState { get; set; }
    }
}
