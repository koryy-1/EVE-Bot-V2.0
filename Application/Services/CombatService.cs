using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CombatService : ICombatService, IWorkerService
    {
        private IBotStateService _botStateService;
        private CancellationTokenSource _cancellationTokenSource;

        public CombatService(IBotStateService botStateService)
        {
            _botStateService = botStateService;
        }

        public Task StartAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            return Task.Run(async () =>
            {
                while (!_cancellationTokenSource.Token.IsCancellationRequested)
                {

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void DestroyTarget()
        {
            throw new NotImplementedException();
        }

        public void LockTarget()
        {
            throw new NotImplementedException();
        }

        public void PrepareToAttack()
        {
            throw new NotImplementedException();
        }

        public void UnlockTarget()
        {
            throw new NotImplementedException();
        }
    }
}
