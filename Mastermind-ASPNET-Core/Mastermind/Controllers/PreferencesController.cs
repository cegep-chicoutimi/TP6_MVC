using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Mastermind.Models;
using Mastermind.DataAccessLayer;
using System.Security.Claims;

namespace Mastermind.Controllers
{
    [Authorize]
    public class PreferencesController : Controller
    {
        private readonly DAL _dal;

        public PreferencesController()
        {
            _dal = new DAL();
        }

        public IActionResult Index()
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var preferences = _dal.MemberPrefsFact.GetByMemberId(memberId);

            if (preferences == null)
            {
                var configByKey = _dal.ConfigFact.GetAll();
                preferences = new MemberPreferences
                {
                    MemberId = memberId,
                    NbColors = int.Parse(configByKey[Config.NB_COLORS].Value),
                    NbPositions = int.Parse(configByKey[Config.NB_POSITIONS].Value),
                    NbAttempts = int.Parse(configByKey[Config.NB_ATTEMPTS].Value)
                };
            }

            return View(preferences);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(MemberPreferences preferences)
        {
            if (!ModelState.IsValid)
                return View("Index", preferences);

            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            preferences.MemberId = memberId;

            _dal.MemberPrefsFact.CreateOrUpdate(preferences);
            TempData["SuccessMessage"] = "Vos préférences ont été enregistrées avec succès.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reset()
        {
            var memberId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            _dal.MemberPrefsFact.Delete(memberId);
            TempData["SuccessMessage"] = "Vos préférences ont été réinitialisées aux valeurs par défaut.";

            return RedirectToAction(nameof(Index));
        }
    }
} 