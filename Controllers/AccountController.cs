using CookiesAuth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CookiesAuth.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserStoreService _userStoreService;
        private const string CookieUserId = "UserId";
        private const string CookieUserName = "UserName";

        public AccountController(UserStoreService userStoreService)
        {
            _userStoreService = userStoreService;
        }

        //Get : /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            if (Request.Cookies.ContainsKey(CookieUserId))
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
                ModelState.AddModelError("", "Invalid usernaem or password");
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

            Response.Cookies.Append(CookieUserId, user.Id.ToString(), options);
            Response.Cookies.Append(CookieUserName, user.UserName, options);
            return RedirectToAction("Dashboard", "Account");
        }

        public IActionResult Dashboard()
        {
            if (!Request.Cookies.ContainsKey(CookieUserId))
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = int.Parse(Request.Cookies[CookieUserId]!);
            var user = _userStoreService.GetUser(userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View(user);

        }

        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey(CookieUserId))
            {
                Response.Cookies.Delete(CookieUserId);
            }
            if (Request.Cookies.ContainsKey(CookieUserName))
            {
                Response.Cookies.Delete(CookieUserName);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
