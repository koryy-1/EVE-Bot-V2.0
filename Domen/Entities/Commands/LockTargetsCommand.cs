﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class LockTargetsCommand : CommandBase
    {
        public IEnumerable<OverviewItem> Targets { get; set; }
    }
}
