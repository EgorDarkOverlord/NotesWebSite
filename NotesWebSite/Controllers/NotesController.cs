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

        private bool CorrectEnter()
        {
            return
                (User.Identity.IsAuthenticated) ||
                (HttpContext.Request.Cookies.DeserializeObjectFromJson<Link>("link") != null);
        }

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
            HttpContext.Response.Cookies.RewriteObjectAsJson<string>("enterState", "User");
            if (CorrectEnter())
            {
                var id = db.Users.AsQueryable().FirstOrDefault(u => u.Login == User.Identity.Name).Id;
                notes = db.UserNotes.AsQueryable().Where(n => n.UserId == id).ToList<NoteBase>();
                return View();
            }
            return RedirectToAction("Login", "Enter");
        }

        public IActionResult LinkNotes()
        {
            HttpContext.Response.Cookies.RewriteObjectAsJson<string>("enterState", "Notes");
            var link = HttpContext.Request.Cookies.DeserializeObjectFromJson<Link>("link");
            if (CorrectEnter())
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
            if (!CorrectEnter())
                return RedirectToAction("Login", "Enter");
            return View();
        }

        [HttpPost]
        public IActionResult SaveNote(string title, string content)
        {
            if (!CorrectEnter())
                return RedirectToAction("Login", "Enter");
            Guid noteId = HttpContext.Request.Cookies.DeserializeObjectFromJson<Guid>("noteId");
            string enterState = HttpContext.Request.Cookies.DeserializeObjectFromJson<string>("enterState");

            NoteBase note = db.Notes.Find(noteId);
            if (note != null)
            {
                note.Title = title;
                note.Content = content;
                note.EditDate = DateTime.Now;
            }
            else
            {
                switch (enterState)
                {
                    case "User":
                        note = new UserNote
                        {
                            Title = title,
                            Content = content,
                            CreateDate = DateTime.Now,
                            EditDate = DateTime.Now
                        };
                        ((UserNote)note).UserId = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name).Id;
                        break;
                    case "Link":
                        note = new UserNote
                        {
                            Title = title,
                            Content = content,
                            CreateDate = DateTime.Now,
                            EditDate = DateTime.Now
                        };
                        ((LinkNote)note).LinkId = HttpContext.Request.Cookies.DeserializeObjectFromJson<Link>("link").Id;
                        break;
                }
                db.Notes.Add(note);
            }
            db.SaveChanges();
            switch (enterState)
            {
                case "User":
                    return RedirectToAction("UserNotes");
                case "Link":
                    return RedirectToAction("LinkNotes");
            }
            return RedirectToAction("Login", "Enter");
        }
    }
}
