using Microsoft.AspNetCore.Mvc;
using BookstoreApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace BookstoreApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string login, string password, string address)
        {
            var success = await _userService.RegisterUserAsync(firstName, lastName, login, password, address);
            if (success)
            {
                TempData["Success"] = "Rejestracja klienta przebiegła pomyślnie!";
                return RedirectToAction("Login");
            }
            ViewBag.Error = "Rejestracja nie powiodła się. Login może być już zajęty.";
            return View();
        }

        [HttpGet]
        [Route("Users/RegisterEmployee")]
        public IActionResult RegisterEmployee()
        {
            return View();
        }

        [HttpPost]
        [Route("Users/RegisterEmployee")]
        public async Task<IActionResult> RegisterEmployee(string firstName, string lastName, string login, string password)
        {
            var success = await _userService.RegisterEmployeeAsync(firstName, lastName, login, password);
            if (success)
            {
                TempData["Success"] = "Rejestracja pracownika przebiegła pomyślnie!";
                return RedirectToAction("Login");
            }
            ViewBag.Error = "Rejestracja nie powiodła się. Login może być już zajęty.";
            return View();
        }

        [HttpGet]
        [Route("Users/RegisterManager")]
        public IActionResult RegisterManager()
        {
            return View();
        }

        [HttpPost]
        [Route("Users/RegisterManager")]
        public async Task<IActionResult> RegisterManager(string firstName, string lastName, string login, string password)
        {
            var success = await _userService.RegisterManagerAsync(firstName, lastName, login, password);
            if (success)
            {
                TempData["Success"] = "Rejestracja kierownika przebiegła pomyślnie!";
                return RedirectToAction("Login");
            }
            ViewBag.Error = "Rejestracja nie powiodła się. Login może być już zajęty.";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            var user = await _userService.LoginAsync(login, password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserLogin),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("UserType", user.GetType().Name)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(24)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return user.GetType().Name switch
                {
                    "Client" => RedirectToAction("Dashboard", "Client"),
                    "Employee" => RedirectToAction("Dashboard", "Employee"),
                    "Manager" => RedirectToAction("Dashboard", "Manager"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            ViewBag.Error = "Nieprawidłowe dane logowania.";
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Profile()
        {
            var userType = User.FindFirst("UserType")?.Value;

            return userType switch
            {
                "Client" => RedirectToAction("Profile", "Client"),
                "Employee" => RedirectToAction("Dashboard", "Employee"),
                "Manager" => RedirectToAction("Dashboard", "Manager"),
                _ => View()
            };
        }
        [HttpGet]
        [Route("Users/ChooseRegistration")]
        public IActionResult ChooseRegistration()
        {
            return View();
        }
    }
}