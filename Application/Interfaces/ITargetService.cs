using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITargetService
    {
        public Task LockTargetByName(string targetName);
        public Task LockTargets(IEnumerable<OverviewItem> overviewItems);
        /// <summary>
        /// effect is a buff or debuff to current ship, 
        /// ex: effect by Stasis Webifier, Disraptor or Scrambler
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public Task LockTargetByEffect(string effect);
        public Task UnlockTarget(OverviewItem overviewItems);
        public Task SwitchAimToNearestLockedTarget();
    }
}
