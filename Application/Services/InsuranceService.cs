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
    public class InsuranceService : BotWorker
    {
        // проверить дскан, овервью на наличие опасных шипов
        // проверить чат
        // проверить хп корабля и выставить команду на апроч ближайшей станции

        public InsuranceService(ICoordinator coordinator) : base(coordinator, "insurance-service")
        {
        }

        protected override Task CyclingWork(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
