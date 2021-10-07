using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesWebSite.Controllers
{
    public class NotesController : Controller
    {
        private NotesContext db;
        private List<NoteBase> notes;

        public NotesController(NotesContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserNotes()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = db.Users.AsQueryable().FirstOrDefault(u => u.Login == User.Identity.Name).Id;
                notes = db.UserNotes.AsQueryable().Where(n => n.UserId == id).ToList<NoteBase>();
                return View();
            }
            return RedirectToAction("Login", "Enter");
        }

        public IActionResult LinkNotes()
        {
            var link = HttpContext.Request.Cookies.DeserializeObjectFromJson<Link>("link");
            if (link != null)
            {
                notes = db.LinkNotes.AsQueryable().Where(n => n.LinkId == link.Id).ToList<NoteBase>();
                return View();
            }
            return RedirectToAction("Login", "Enter");
        }

        public IActionResult Notes()
        {
            if (User.Identity.IsAuthenticated)
            {
                var id = db.Users.AsQueryable().FirstOrDefault(u => u.Login == User.Identity.Name).Id;
                notes = db.UserNotes.AsQueryable().Where(n => n.UserId == id).ToList<NoteBase>();
            }
            var link = HttpContext.Request.Cookies.DeserializeObjectFromJson<Link>("link");
            if (link != null)
            {
                notes = db.LinkNotes.AsQueryable().Where(n => n.LinkId == link.Id).ToList<NoteBase>();
            }
            if (!User.Identity.IsAuthenticated && link == null)
            {
                return RedirectToAction("Login", "Enter");
            }
            return View();
        }

        public IActionResult EditNote()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SaveNote(string title, string content)
        {
            return RedirectToAction("Login", "Enter");
        }
    }
}
