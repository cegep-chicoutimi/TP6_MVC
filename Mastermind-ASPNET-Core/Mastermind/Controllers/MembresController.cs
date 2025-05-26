using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Mastermind.Models;
using Mastermind.DataAccessLayer.Factories;
using Mastermind.Resources;
using System.Security.Claims;
using Mastermind.DataAccessLayer;
using Mastermind.Helper;

namespace Mastermind.Controllers
{
    public class MembresController : Controller
    {
        private readonly MemberFactory _memberFactory;
        private readonly IWebHostEnvironment _environment;
        private readonly IStringLocalizer<Resource> _localizer;
        private DAL _dal;

        public MembresController(IWebHostEnvironment environment, IStringLocalizer<Resource> localizer)
        {
            _memberFactory = new MemberFactory();
            _environment = environment;
            _localizer = localizer;
            _dal = new DAL();

        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            // Redirect if user is already logged in
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            Member member = _dal.MemberFact.CreateEmpty();

            return View(member);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Member member, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(member);
            }

            if (!_memberFactory.IsUsernameUnique(member.Username))
            {
                ModelState.AddModelError("Username", _localizer["UsernameTaken"]);
                return View(member);
            }

            // Handle image upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "members");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                member.ImagePath = $"/img/members/{uniqueFileName}";
            }

            // Set role to Standard for new registrations
            member.Role = MemberRoles.Standard;
            member.Password = CryptographyHelper.HashPassword(member.Password);

            // Create the member
            _memberFactory.Create(member);

            // Auto-login after registration
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, member.Username),
                new Claim(ClaimTypes.Role, member.Role),
                new Claim("FullName", member.FullName)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = _localizer["AccountCreated"];
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult EditProfile()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Auth");
            }

            var member = _memberFactory.GetByUsername(username);
            if (member == null)
            {
                return NotFound();
            }

            // Clear password for security
            member.Password = string.Empty;
            return View(member);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Member member, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(member);
            }

            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
            {
                return RedirectToAction("Login", "Auth");
            }

            var existingMember = _memberFactory.GetByUsername(currentUsername);
            if (existingMember == null)
            {
                return NotFound();
            }

            // Ensure we're editing the correct member
            member.Id = existingMember.Id;
            member.Role = existingMember.Role; // Preserve role

            if (!_memberFactory.IsUsernameUnique(member.Username, member.Id))
            {
                ModelState.AddModelError("Username", _localizer["UsernameTaken"]);
                return View(member);
            }

            // Handle image upload
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "img", "members");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingMember.ImagePath))
                {
                    var oldImagePath = Path.Combine(_environment.WebRootPath, existingMember.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{ImageFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                member.ImagePath = $"/img/members/{uniqueFileName}";
            }
            else
            {
                // Keep existing image if no new image uploaded
                member.ImagePath = existingMember.ImagePath;
            }

            _memberFactory.Update(member);

            // Update claims if username or full name changed
            if (currentUsername != member.Username || existingMember.FullName != member.FullName)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, member.Username),
                    new Claim(ClaimTypes.Role, member.Role),
                    new Claim("FullName", member.FullName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);
            }

            TempData["SuccessMessage"] = _localizer["ProfileUpdated"];
            return RedirectToAction("Index", "Home");
        }

        [AcceptVerbs("GET", "POST")]
        [AllowAnonymous]
        public IActionResult IsUsernameAvailable(string username, int? id)
        {
            return Json(_memberFactory.IsUsernameUnique(username, id));
        }
    }
} 