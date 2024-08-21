using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class GotoNextSystemCommand : CommandBase
    {
        public string? NextSystemName { get; set; }

        public GotoNextSystemCommand()
        {
            IsFinite = true;
        }
    }
}
