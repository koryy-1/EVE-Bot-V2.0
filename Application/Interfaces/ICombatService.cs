using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICombatService
    {
        public void PrepareToAttack();
        public void LockTarget();
        public void UnlockTarget();
        public void DestroyTarget();
    }
}
