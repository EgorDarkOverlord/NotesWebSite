using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesWebSite.Models;
using NotesWebSite.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NotesWebSite.Controllers
{
    public class EnterController : Controller
    {
        private NotesContext db;

        public EnterController(NotesContext context)
        {
            db = context;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await db.Users.FirstOrDefaultAsync(
                u => u.Login == model.Login && u.Password == model.Password
                );
            if (user != null)
            {
                await Authenticate(model.Login); // аутентификация
                return RedirectToAction("UserNotes", "Notes");
            }
            else
            {
                ModelState.AddModelError("LoginError", "Некорректные логин и(или) пароль");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            User user = await db.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (user == null)
            {
                // добавляем пользователя в бд
                db.Users.Add(new User { Login = model.Login, Password = model.Password });
                await db.SaveChangesAsync();

                await Authenticate(model.Login); // аутентификация

                return RedirectToAction("UserNotes", "Notes");
            }
            else
            {
                ModelState.AddModelError("RegisterError", "Некорректные логин и(или) пароль");
                return View();
            }
        }

        public async Task<IActionResult> GoToLink(Guid link)
        {
            Link _link = await db.Links.FirstOrDefaultAsync(l => l.Id == link);
            if (_link != null)
            {
                HttpContext.Response.Cookies.RewriteObjectAsJson<Link>("link", _link);
                return RedirectToAction("LinkNotes", "Notes");
            }
            
            return RedirectToAction("Login");
        }

        public IActionResult GenerateLink()
        {
            var link = new Link();
            db.Links.Add(link);
            db.SaveChanges();
            HttpContext.Response.Cookies.RewriteObjectAsJson<Link>("link", link);

            return RedirectToAction("LinkNotes", "Notes");
        }

        //Аутентификация
        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> ExitFromUser()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("enterState");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ExitFromLink()
        {
            HttpContext.Response.Cookies.Delete("link");
            HttpContext.Response.Cookies.Delete("enterState");
            return RedirectToAction("Index", "Home");
        }
    }
}
