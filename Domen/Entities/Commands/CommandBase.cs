﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public abstract class CommandBase
    {
        public bool Requested { get; set; }
        public bool IsFinite { get; set; }
    }
}
