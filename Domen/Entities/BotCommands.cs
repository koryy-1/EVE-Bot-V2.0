using Domen.Entities.Commands;
using Domen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domen.Entities
{
    public class BotCommands
    {
        public bool ExecutorAuthorized { get; set; }
        public bool IsBattleModeActivated { get; set; }
        public bool IsTargetLocked { get; set; }
        public bool IsTargetInWeaponRange { get; set; }
        public bool OpenFireAuthorized { get; set; }

        public Dictionary<PriorityLevel, MovementCommand> MoveCommands { get; set; } = new()
        {
            { PriorityLevel.High, new MovementCommand() },
            { PriorityLevel.Medium, new MovementCommand() },
            { PriorityLevel.Low, new MovementCommand() },
        };
        public DestroyTargetCommand? DestroyTargetCommand { get; set; } = new();

        public LootingCommand? LootingCommand { get; set; } = new();

        public GotoNextSystemCommand? GotoNextSystemCommand { get; set; } = new();
        public WarpToAnomalyCommand? WarpToAnomalyCommand { get; set; } = new();
        public DockToStationCommand? DockToStationCommand { get; set; } = new();
        public WarpToCommand? WarpToCommand { get; set; } = new();
    }
}
