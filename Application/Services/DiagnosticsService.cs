using Application.Interfaces;
using Hangfire;
using Microsoft.Extensions.Hosting;
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

        public DiagnosticsService(ICoordinator coordinator) : base(coordinator, "diagnostics-service")
        {
        }

        protected override Task CyclingWork(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
