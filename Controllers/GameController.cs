using GameSecretsAPI.Interfaces;
using GameSecretsAPI.Models;
using GameSecretsAPI.Transactional;
using Microsoft.AspNetCore.Mvc;

namespace GameSecretsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameTRA _gameTRA;

        public GameController(IGameTRA gameTRA)
        {
            _gameTRA = gameTRA;
        }

        [HttpPost("InitPlayer")]
        public IActionResult InitPlayer([FromBody] GameStartPlayer gameStartPlayer)
        {
            if (gameStartPlayer is not null)
            {
                try
                {
                    _gameTRA.GameStart(gameStartPlayer.Player, gameStartPlayer.Password);

                    Game? game = _gameTRA.GetGame();
                    int maxRetries = 5000;
                    int retryCount = 0;

                    while (retryCount < maxRetries)
                    {
                        game = _gameTRA.GetGame();
                        if (game is not null && !string.IsNullOrEmpty(game.Player1) && !string.IsNullOrEmpty(game.Player2))
                        {
                            break;
                        }

                        Thread.Sleep(3000);
                        retryCount++;
                    }

                    if (game is not null && !string.IsNullOrEmpty(game.Player1) && !string.IsNullOrEmpty(game.Player2))
                        return Ok(new { status = "success", message = "Game iniciado", player1 = game.Player1, player2 = game.Player2 });
                    else
                        return Ok(new { status = "success", message = "Aguardando jogadores" });
                }
                catch (ApplicationException ex)
                {
                    return BadRequest(new { Success = false, message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Success = false, message = "Erro interno do servidor: " + ex.Message });
                }
            }
            else
                return BadRequest(new { Success = false, message = "Não foi possível iniciar game" });
        }

        [HttpPost("PlayTurn")]
        public IActionResult PlayTurn([FromBody] GameStartPlayer gameStartPlayer)
        {
            if (gameStartPlayer is not null)
            {
                try
                {
                    _gameTRA.GameTurn(gameStartPlayer.Player, gameStartPlayer.Password);

                    Game game = _gameTRA.GetGame();

                    if (game.RightNumbersTurn < 4)
                    {
                        return Ok(new { status = "success", message = "Rodada efetuada com succeso", numerosCertos = game.RightNumbersTurn });
                    }
                    else
                    {
                        return Ok(new { status = "success", message = gameStartPlayer.Player + " É o vencedor", numerosCertos = game.RightNumbersTurn });
                    }
                }
                catch (ApplicationException ex)
                {
                    return BadRequest(new { Success = false, message = ex.Message });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Success = false, message = "Erro interno do servidor: " + ex.Message });
                }
            }
            else
                return BadRequest(new { Success = false, message = "Não foi possível jogar rodada" });
        }

        [HttpGet("CurrentTurn/{player}")]
        public IActionResult GetTurn(string player)
        {
            try
            {
                Game game = _gameTRA.GetGame();

                int maxRetries = 5000;
                int retryCount = 0;

                string gamePlayingNow = string.Empty;

                while (retryCount < maxRetries)
                {
                    game = _gameTRA.GetGame();
                    if (string.Equals(game.CurrentPlayerTurn, player))
                    {
                        if (string.Equals(game.Player1, player))
                            gamePlayingNow = game.Player2;
                        else
                            gamePlayingNow = game.Player1;

                        break;
                    }

                    Thread.Sleep(3000);
                    retryCount++;
                }

                return Ok(new { status = "success", message = $"{gamePlayingNow} acertou {game.RightNumbersTurn} dígitos", Turn = game.CurrentPlayerTurn });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { Success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, message = "Erro interno do servidor: " + ex.Message });
            }
        }
    }
}
