using Mastermind.Areas.Admin.ViewModels;
using Mastermind.DataAccessLayer;
using Mastermind.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mastermind.Areas.Admin.Controllers
{
    [Area("Admin")]
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
