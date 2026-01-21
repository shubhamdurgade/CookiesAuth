using Microsoft.AspNetCore.Mvc;

namespace CookiesAuth.Controllers
{
    public class CookieController : Controller
    {
        public IActionResult AboutCookies()
        {
            return View();
        }

        public IActionResult SetSampleCookie()
        {
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(3),
                HttpOnly = true,
                IsEssential = true
            };

            Response.Cookies.Append("DemoCookies", "HelloFromCookieDemo", options);
            ViewBag.Message = "A persistent cookies named 'DemoCookie' has been set! (Experied in 3 Days";
            return View();
        }

        public IActionResult ReadSampleCookies()
        {
            string? value = Request.Cookies["DemoCookie"];
            ViewBag.Messsage = value == null ? "Cookie not found." : $"Cookie value : {value}";
            return View();
        }

        public IActionResult DeleteSampleCookie()
        {
            Response.Cookies.Delete("DemoCookie");
            ViewBag.Message = "Cookie has been deleted";
            return View();
        }

        public IActionResult ViewCookiesInView()
        {
            return View();
        }
    }
}
