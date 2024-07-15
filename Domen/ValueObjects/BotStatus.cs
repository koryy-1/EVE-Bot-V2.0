using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.ValueObjects
{
    public class BotStatus
    {
        public bool IsServicesRunning { get; set; }
        public string ExecWorkerStatus { get; set; }
        public IEnumerable<string> WorkerStatuses { get; set; }
    }
}
