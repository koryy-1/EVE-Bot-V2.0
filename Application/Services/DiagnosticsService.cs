using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class DiagnosticsService : BotWorker
    {
        // проверяет наличие ракет / дронов и просто докает и выключает executor
        // каждые 10 минут / полчаса / час (ставить запланированные задачи) проверять определенные показатели
        // например, бот 10 минут уничтожает 1 и ту же цель, бот больше 2 часов находится в 1 и той же системе
        // и это должно выставяться в конфиге

        public DiagnosticsService(ICoordinator coordinator) : base(coordinator, "diagnostics-service")
        {
        }

        protected override Task CyclingWork(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
