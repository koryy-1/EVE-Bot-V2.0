﻿using Application.Interfaces;
using Application.Services;
using Domen.Entities;
using Domen.Entities.Commands;
using Domen.Enums;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/command")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private readonly IBotService _botService;
        private readonly ICoordinator _coordinator;
        private readonly ILogger<CommandController> _logger;

        public CommandController(IBotService botService, 
            ICoordinator coordinator, 
            ILogger<CommandController> logger)
        {
            _botService = botService;
            _coordinator = coordinator;
            _logger = logger;
        }

        [HttpPost("GetShipState", Name = "GetShipState")]
        public ActionResult<ShipState> GetShipState()
        {
            var state = _coordinator.ShipState;
            return Ok(state);
        }

        [HttpPost("GetCommands", Name = "GetCommands")]
        public ActionResult<BotCommands> GetCommands()
        {
            var state = _coordinator.Commands;
            return Ok(state);
        }

        [HttpPost("SetDestroyTargetCmd", Name = "SetDestroyTargetCmd")]
        public IActionResult SetDestroyTargetCmd([FromBody] DestroyTargetCommand cmd)
        {
            if (!_botService.GetBotStatus().IsServicesRunning)
                return BadRequest(new { message = $"Bot services are not started" });

            _coordinator.Commands.DestroyTargetCommand = cmd;
            return Ok(new { message = $"Command set" });
        }

        [HttpPost("SetMovementCmd", Name = "SetMovementCmd")]
        public IActionResult SetMovementCmd([FromBody] MovementCommand cmd)
        {
            if (!_botService.GetBotStatus().IsServicesRunning)
                return BadRequest(new { message = $"Bot services are not started" });

            _coordinator.Commands.MoveCommands[PriorityLevel.Low] = cmd;
            return Ok(new { message = $"Command set" });
        }

        [HttpPost("SetWarpToAnomalyCmd", Name = "SetWarpToAnomalyCmd")]
        public IActionResult SetWarpToAnomalyCmd([FromBody] WarpToAnomalyCommand cmd)
        {
            if (!_botService.GetBotStatus().IsServicesRunning)
                return BadRequest(new { message = $"Bot services are not started" });

            _coordinator.Commands.WarpToAnomalyCommand = cmd;
            return Ok(new { message = $"Command set" });
        }

        [HttpPost("SetGotoNextSystemCmd", Name = "SetGotoNextSystemCmd")]
        public IActionResult SetGotoNextSystemCmd([FromBody] GotoNextSystemCommand cmd)
        {
            if (!_botService.GetBotStatus().IsServicesRunning)
                return BadRequest(new { message = $"Bot services are not started" });

            _coordinator.Commands.GotoNextSystemCommand = cmd;
            return Ok(new { message = $"Command set" });
        }

        [HttpPost("SetLootingCmd", Name = "SetLootingCmd")]
        public IActionResult SetLootingCmd([FromBody] LootingCommand cmd)
        {
            if (!_botService.GetBotStatus().IsServicesRunning)
                return BadRequest(new { message = $"Bot services are not started" });

            _coordinator.Commands.LootingCommand = cmd;
            return Ok(new { message = $"Command set" });
        }
    }
}
