using CookiesAuth.Models;
using CookiesAuth.Constants;
using CookiesAuth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace CookiesAuth.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserStoreService _userStoreService;
        private readonly IUserDataProtector _dataProtector;

        public AccountController(
            UserStoreService userStoreService,
            IUserDataProtector dataProtector)
        {
            _userStoreService = userStoreService;
            _dataProtector = dataProtector;
        }

        //Get : /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (Request.Cookies.ContainsKey(AuthConstants.CookieUserId))
            {
                return RedirectToAction("Dashboard", "Account");
            }
            return View(new LoginViewModel());
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _userStoreService.ValidateUser(model.UserName, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var options = new CookieOptions()
            {
                Path = "/",
                HttpOnly = true,
                IsEssential = true,
                Secure = true
            };

            if (model.RememberMe)
            {
                options.Expires = DateTime.UtcNow.AddDays(7);
            }
            else
            {
                options.Expires = null;
            }

            // Protect values before writing to cookies via IUserDataProtector
            var protectedUserId = _dataProtector.Protect(user.Id.ToString());
            var protectedUserName = _dataProtector.Protect(user.UserName);

            Response.Cookies.Append(AuthConstants.CookieUserId, protectedUserId, options);
            Response.Cookies.Append(AuthConstants.CookieUserName, protectedUserName, options);
            return RedirectToAction("Dashboard", "Account");
        }

        public IActionResult Dashboard()
        {
            if (!Request.Cookies.ContainsKey(AuthConstants.CookieUserId))
            {
                return RedirectToAction("Login", "Account");
            }

            var protectedUserId = Request.Cookies[AuthConstants.CookieUserId]!;
            string userIdString;
            try
            {
                userIdString = _dataProtector.Unprotect(protectedUserId);
            }
            catch (CryptographicException)
            {
                // Invalid or tampered cookie -> force re-login
                Response.Cookies.Delete(AuthConstants.CookieUserId);
                Response.Cookies.Delete(AuthConstants.CookieUserName);
                return RedirectToAction("Login", "Account");
            }

            if (!int.TryParse(userIdString, out var userId))
            {
                Response.Cookies.Delete(AuthConstants.CookieUserId);
                Response.Cookies.Delete(AuthConstants.CookieUserName);
                return RedirectToAction("Login", "Account");
            }

            var user = _userStoreService.GetUser(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Optionally unprotect username if needed in the view
            if (Request.Cookies.ContainsKey(AuthConstants.CookieUserName))
            {
                try
                {
                    var protectedUserName = Request.Cookies[AuthConstants.CookieUserName]!;
                    var userName = _dataProtector.Unprotect(protectedUserName);
                    ViewData["UserName"] = userName;
                }
                catch (CryptographicException)
                {
                    // ignore, not critical
                }
            }

            return View(user);
        }

        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey(AuthConstants.CookieUserId))
            {
                Response.Cookies.Delete(AuthConstants.CookieUserId);
            }
            if (Request.Cookies.ContainsKey(AuthConstants.CookieUserName))
            {
                Response.Cookies.Delete(AuthConstants.CookieUserName);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
