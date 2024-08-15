using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface INavigationService
    {
        public bool GotoNextSystemRequested();
        public bool IsCommandExecuting();
        public bool IsCommandRequested();
        public Task ExecuteMovement();
        public bool IsWarpOrJumpState();
        public Task JumpToMarkedGate();
        public Task WarpToAnomaly();
    }
}
