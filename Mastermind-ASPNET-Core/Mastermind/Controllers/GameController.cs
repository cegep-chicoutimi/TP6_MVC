using Mastermind.GameModels;
using Mastermind.DataAccessLayer.Factories;
using Mastermind.Models;
using Mastermind.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Mastermind.Controllers
{
    [Authorize(Roles = "Standard,Admin")]
    public class GameController : Controller
    {
        private const string SESSION_GAME_NAME = "CurrentGame";
        private readonly GameStatsFactory _gameStatsFactory;

        public GameController(GameStatsFactory gameStatsFactory)
        {
            _gameStatsFactory = gameStatsFactory;
        }

        private Game CreateOrGetGame()
        {
            Game? game = null;

            string? currentJsonGame = HttpContext.Session.GetString(SESSION_GAME_NAME);
            if (currentJsonGame != null)
                game = JsonSerializer.Deserialize<Game>(currentJsonGame);

            if (game == null)
            {
                Dictionary<string, Config> configByKey = new DataAccessLayer.DAL().ConfigFact.GetAll();

                int.TryParse(configByKey[Config.NB_COLORS].Value, out int nbColors);
                int.TryParse(configByKey[Config.NB_POSITIONS].Value, out int nbPositions);
                int.TryParse(configByKey[Config.NB_ATTEMPTS].Value, out int nbAttempts);

                game = new(nbColors, nbPositions, nbAttempts);
                HttpContext.Session.SetString(SESSION_GAME_NAME, JsonSerializer.Serialize(game));
            }

            return game;
        }

        public IActionResult Index()
        {
            Game game = CreateOrGetGame();
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var stats = _gameStatsFactory.GetStatsByMemberId(memberId);
            GameVM viewModel = new(game, stats);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(IFormCollection collection)
        {
            Game? game = null;
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            string? currentJsonGame = HttpContext.Session.GetString("CurrentGame");
            if (currentJsonGame != null)
            {
                game = JsonSerializer.Deserialize<Game>(currentJsonGame);

                if (game != null)
                {
                    PlayerRow playerRow = new();
                    for (int position = 1; position <= game.NbPositions; position++)
                    {
                        int.TryParse(collection["color-index-" + position], out int color);
                        playerRow.Pawns.Add(new Pawn { Color = color });
                    }

                    game.Validate(playerRow);

                    // Update stats if game ended
                    if (game.State != Game.GameState.Running)
                    {
                        bool isWin = game.State == Game.GameState.PlayerWin;
                        int? attempts = isWin ? game.CurrentPlayingRow - 1 : null;
                        _gameStatsFactory.CreateOrUpdateStats(memberId, isWin, attempts);
                    }

                    HttpContext.Session.SetString("CurrentGame", JsonSerializer.Serialize(game));
                }
            }

            game ??= CreateOrGetGame();
            var stats = _gameStatsFactory.GetStatsByMemberId(memberId);
            GameVM viewModel = new(game, stats);

            return PartialView("PartialGame", viewModel);
        }

        public IActionResult Replay()
        {
            HttpContext.Session.Remove(SESSION_GAME_NAME);

            return RedirectToAction("Index");
        }
    }
}
