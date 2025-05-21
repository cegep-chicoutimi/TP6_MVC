using Mastermind.DataAccessLayer;
using Mastermind.Models;
using Mastermind.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Mastermind.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Dictionary<string, Config> configByKey = new DAL().ConfigFact.GetAll();

            int.TryParse(configByKey[Config.NB_COLORS].Value, out int nbColors);
            int.TryParse(configByKey[Config.NB_POSITIONS].Value, out int nbPositions);
            int.TryParse(configByKey[Config.NB_ATTEMPTS].Value, out int nbAttempts);

            HomeVM viewModel = new(nbColors, nbPositions, nbAttempts);

            return View(viewModel);
        }
    }
}