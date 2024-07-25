using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities.Commands
{
    public class WarpToAnomalyCommand
    {
        public bool Requested { get; set; }
        public ProbeScanItem Anomaly { get; set; }
    }
}
