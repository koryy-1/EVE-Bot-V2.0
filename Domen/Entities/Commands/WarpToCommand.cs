﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class WarpToCommand : CommandBase
    {
        public string Name { get; set; }
        public OverviewItem SpaceObject { get; set; }

        public WarpToCommand()
        {
            IsFinite = true;
        }
    }
}
