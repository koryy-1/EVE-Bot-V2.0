using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class DestroyTargetCommand : CommandBase
    {
        public OverviewItem Target { get; set; }

        public DestroyTargetCommand()
        {
            IsFinite = true;
        }
    }
}
