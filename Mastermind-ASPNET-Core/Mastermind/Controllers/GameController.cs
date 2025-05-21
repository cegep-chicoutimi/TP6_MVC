using Mastermind.GameModels;
using Mastermind.DataAccessLayer;
using Mastermind.Models;
using Mastermind.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Mastermind.Controllers
{
    [Authorize(Roles = "Standard,Admin")]
    public class GameController : Controller
    {
        private const string SESSION_GAME_NAME = "CurrentGame";

        private Game CreateOrGetGame()
        {
            Game? game = null;

            string? currentJsonGame = HttpContext.Session.GetString(SESSION_GAME_NAME);
            if (currentJsonGame != null)
                game = JsonSerializer.Deserialize<Game>(currentJsonGame);

            if (game == null)
            {
                Dictionary<string, Config> configByKey = new DAL().ConfigFact.GetAll();

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
            GameVM viewModel = new(game);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Validate(IFormCollection collection)
        {
            Game? game = null;

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

                    HttpContext.Session.SetString("CurrentGame", JsonSerializer.Serialize(game));
                }
            }

            game ??= CreateOrGetGame();

            return PartialView("PartialGame", game);
        }

        public IActionResult Replay()
        {
            HttpContext.Session.Remove(SESSION_GAME_NAME);

            return RedirectToAction("Index");
        }
    }
}
