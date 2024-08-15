using Application.Interfaces;
using Application.Services;
using Domen.Entities;
using Domen.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/bot")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotService _botService;
        private readonly IGameService _gameService;
        private readonly ILogger<BotController> _logger;

        public BotController(IBotService botService, IGameService gameService, ILogger<BotController> logger)
        {
            _botService = botService;
            _gameService = gameService;
            _logger = logger;
        }

        [HttpGet("GetBotState", Name = "GetBotState")]
        public ActionResult<BotState> GetBotState()
        {
            var state = _botService.GetBotState();
            return Ok(state);
        }

        [HttpGet("GetBotStatus", Name = "GetBotStatus")]
        public ActionResult<BotStatus> GetBotStatus()
        {
            var state = _botService.GetBotStatus();
            return Ok(state);
        }

        [HttpGet("GetConfig", Name = "GetConfig")]
        public ActionResult<BotConfig> GetConfig()
        {
            var state = _botService.GetConfig();
            return Ok(state);
        }

        [HttpPost("load-config")]
        public IActionResult LoadConfig([FromBody] BotConfig config)
        {
            _botService.LoadConfig(config);
            _logger.LogInformation("Bot configuration loaded.");
            return Ok("Configuration loaded");
        }

        [HttpPost("StartBotServices", Name = "StartBotServices")]
        public async Task<IActionResult> StartBotServices()
        {
            var searchingStatus = await _gameService.CheckRootAddressActuality();
            if (!searchingStatus.IsActualRootAddress)
                return BadRequest(new { message = $"UI root address is not actual" });

            if (!_botService.IsConfigLoaded)
                return BadRequest(new { message = $"Configuration is not loaded" });

            _botService.StartBotServices();
            return Ok(new { message = $"Bot services started" });
        }

        [HttpPost("StopBotServices", Name = "StopBotServices")]
        public IActionResult StopBotServices()
        {
            _botService.StopBotServices();
            return Ok(new { message = $"Bot services stopped" });
        }

        [HttpPost("AuthorizeExecutor", Name = "AuthorizeExecutor")]
        public IActionResult AuthorizeExecutor()
        {
            if (!_botService.GetBotStatus().IsBotServicesRunning)
                return BadRequest(new { message = $"Bot services are not started" });

            _botService.AuthorizeExecutor();
            return Ok(new { message = $"Executor authorized" });
        }

        [HttpPost("DenyExecutorAuthorization", Name = "DenyExecutorAuthorization")]
        public IActionResult DenyExecutorAuthorization()
        {
            _botService.DenyExecutorAuthorization();
            return Ok(new { message = $"Executor authorization denied" });
        }
    }
}
