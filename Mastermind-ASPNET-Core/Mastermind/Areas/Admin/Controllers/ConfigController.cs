using Google.Protobuf.WellKnownTypes;
using Mastermind.Areas.Admin.ViewModels;
using Mastermind.DataAccessLayer;
using Mastermind.Models;
using Microsoft.AspNetCore.Mvc;

namespace Mastermind.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ConfigController : Controller
    {
        public IActionResult Edit()
        {
            Dictionary<string, Config> configByKey = new DAL().ConfigFact.GetAll();

            int.TryParse(configByKey[Config.NB_COLORS].Value, out int nbColors);
            int.TryParse(configByKey[Config.NB_POSITIONS].Value, out int nbPositions);
            int.TryParse(configByKey[Config.NB_ATTEMPTS].Value, out int nbAttempts);

            ConfigEditVM viewModel = new(nbColors, nbPositions, nbAttempts);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ConfigEditVM viewModel)
        {
            if (viewModel != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(viewModel);
                }

                List<Config> configs = new()
                {
                    new Config() { Key = Config.NB_COLORS, Value = viewModel.NbColors.ToString() },
                    new Config() { Key = Config.NB_POSITIONS, Value = viewModel.NbPositions.ToString() },
                    new Config() { Key = Config.NB_ATTEMPTS, Value = viewModel.NbAttempts.ToString() }
                };

                new DAL().ConfigFact.Save(configs);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
