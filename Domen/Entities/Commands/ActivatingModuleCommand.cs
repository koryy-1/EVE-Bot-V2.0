﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class ActivatingModuleCommand : CommandBase
    {
        public IEnumerable<ShipModule> ActivatedModules { get; set; }
    }
}
