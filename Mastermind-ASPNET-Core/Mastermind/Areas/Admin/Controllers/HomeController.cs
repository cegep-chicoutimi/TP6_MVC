using Mastermind.Areas.Admin.ViewModels;
using Mastermind.DataAccessLayer;
using Mastermind.DataAccessLayer.Factories;
using Mastermind.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mastermind.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly MemberFactory _memberFactory;

        public HomeController(MemberFactory memberFactory)
        {
            _memberFactory = memberFactory;
        }

        public IActionResult Index()
        {
            Dictionary<string, Config> configByKey = new DAL().ConfigFact.GetAll();

            int.TryParse(configByKey[Config.NB_COLORS].Value, out int nbColors);
            int.TryParse(configByKey[Config.NB_POSITIONS].Value, out int nbPositions);
            int.TryParse(configByKey[Config.NB_ATTEMPTS].Value, out int nbAttempts);

            var monthlySignups = _memberFactory.GetMonthlySignups();

            HomeVM viewModel = new(nbAttempts, nbPositions, nbColors, monthlySignups);

            return View(viewModel);
        }
    }
}
