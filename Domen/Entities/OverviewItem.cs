﻿using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class OverviewItem
    {
        public OverviewItem()
        {
            Pos = new Point();
            Color = new Color();
            Distance = new Distance();
        }
        public Point Pos { get; set; }

        //aggressive / hostile / neutral
        public string IconType { get; set; }
        public bool TargetLocked { get; set; }
        public bool AimOnTargetLocked { get; set; }
        public Color Color { get; set; }
        public Distance Distance { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long Speed { get; set; }
    }
}
