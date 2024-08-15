using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class GotoNextSystemCommand
    {
        public bool Requested { get; set; }
        public string? NextSystemName { get; set; }
    }
}
