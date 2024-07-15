using Application.Interfaces;
using Domen.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("Game")]
    public class GameController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;

        public GameController(IGameService gameService, ILogger<GameController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        [HttpGet("GetClientParams", Name = "GetClientParams")]
        public async Task<ActionResult<ClientParams>> GetClientParams()
        {
            return Ok(await _gameService.GetClientParams());
        }

        [HttpGet("CheckRootAddressActuality", Name = "CheckRootAddressActuality")]
        public async Task<ActionResult<SearchingStatus>> CheckRootAddressActuality()
        {
            return Ok(await _gameService.CheckRootAddressActuality());
        }

        [HttpPost("UpdateClientParams", Name = "UpdateClientParams")]
        public async Task<IActionResult> UpdateClientParams([FromBody] ClientParams clientParams)
        {
            await _gameService.UpdateClientParams(clientParams);
            return Ok(new { message = "client params updated" });
        }

        [HttpPost("StartSearch", Name = "StartSearch")]
        public async Task<ActionResult<ClientParams>> StartSearch()
        {
            var clientParams = await _gameService.StartSearch();
            return Ok(clientParams);
        }
    }
}
