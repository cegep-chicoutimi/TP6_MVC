using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mastermind.Models;
using Mastermind.DataAccessLayer.Factories;
using System.ComponentModel.DataAnnotations;

namespace Mastermind.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MembresController : Controller
    {
        private readonly MemberFactory _memberFactory;

        public MembresController()
        {
            _memberFactory = new MemberFactory();
        }

        public IActionResult Index()
        {
            var members = _memberFactory.GetAll();
            return View(members);
        }

        public IActionResult Create()
        {
            ViewBag.Roles = MemberRoles.AllRoles;
            return View(new Member());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Member member)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = MemberRoles.AllRoles;
                return View(member);
            }

            if (!_memberFactory.IsUsernameUnique(member.Username))
            {
                ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà utilisé");
                ViewBag.Roles = MemberRoles.AllRoles;
                return View(member);
            }

            _memberFactory.Create(member);
            TempData["SuccessMessage"] = "Le membre a été créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var member = _memberFactory.GetById(id);
            if (member == null)
            {
                return NotFound();
            }

            // Clear password for security
            member.Password = string.Empty;
            ViewBag.Roles = MemberRoles.AllRoles;
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Member member)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = MemberRoles.AllRoles;
                return View(member);
            }

            if (!_memberFactory.IsUsernameUnique(member.Username, member.Id))
            {
                ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà utilisé");
                ViewBag.Roles = MemberRoles.AllRoles;
                return View(member);
            }

            _memberFactory.Update(member);
            TempData["SuccessMessage"] = "Le membre a été modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var member = _memberFactory.GetById(id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _memberFactory.Delete(id);
            TempData["SuccessMessage"] = "Le membre a été supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsUsernameUnique(string username, int? id)
        {
            return Json(_memberFactory.IsUsernameUnique(username, id));
        }
    }
} 