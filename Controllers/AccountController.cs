using System.Security.Claims;
using System.Threading.Tasks;
using MediaStore.Data;
using MediaStore.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Identity;

namespace MediaStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AimsContext db;
        public AccountController(AimsContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult SignIn(string? returnUrl = null) => View();

        [HttpPost]
        public async Task<IActionResult> SignIn(string username, string password, string SignInAsGuest, string? returnUrl)
        {
            if (SignInAsGuest == "true")
            {
                var guestClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "Guest"),
                    new Claim(ClaimTypes.Role, "Guest")
                };
                var guestIdentity = new ClaimsIdentity(guestClaims, "MyCookieAuth");
                var guestPrincipal = new ClaimsPrincipal(guestIdentity);
                await HttpContext.SignInAsync("MyCookieAuth", guestPrincipal);
                return RedirectToAction("Index", "Home");
            }
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                ViewBag.Error = "Username does not exist";
                return View();
            }
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            if (result == PasswordVerificationResult.Failed)
            {
                ViewBag.Error = "Incorrect password";
                return View();
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username), // Ten nguoi dung
                new Claim("UserID", user.UserId.ToString()), // ID neu can
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Customer"),
                new Claim(ClaimTypes.Email, user.Email) // Email nguoi dung => loc don hang
            };
            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            if (user.IsAdmin)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]

        public IActionResult SignUp() => View();

        [HttpPost]
        public async Task<IActionResult> SignUp(string username, string email, string password, string wantToSignIn)
        {
            if (wantToSignIn == "true")
            {
                return RedirectToAction("SignIn");
            }
            if (db.Users.Any(u => u.Username == username))
            {
                ViewBag.Error = "Username already exists";
                return View();
            }
            // Additional validation and user creation logic would go here
            var newUser = new User
            {
                Username = username,
                Email = email,
                IsAdmin = false,
            };

            var passwordHasher = new PasswordHasher<User>();
            newUser.Password = passwordHasher.HashPassword(newUser, password);

            db.Users.Add(newUser);
            await db.SaveChangesAsync();

            TempData["Sign up success"] = "Sign up successful. Please log in.";
            return RedirectToAction("SignIn");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("SignIn");
        }

        public IActionResult AccessDenied() => View();

    }
}