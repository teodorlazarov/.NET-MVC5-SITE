using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using TeddySite.Models;
using System.Net;

namespace TeddySite.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private FeedbackContext _db = new FeedbackContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            var mostRecentEntries =
            (from entry in _db.Entries
             orderby entry.DateAdded descending, entry.Username
             select entry).Take(20);

            //ViewBag.Entries = mostRecentEntries.ToList();
            return View(mostRecentEntries.ToList());
        }

        [HttpPost]
        public ActionResult Index(FeedbackEntry entry)
        {
            var mostRecentEntries =
               (from entries in _db.Entries
                orderby entries.DateAdded descending, entries.Username
                select entries).Take(20);

            if (string.IsNullOrEmpty(entry.Message) || entry.Message.Length < 4)
            {
                ModelState.AddModelError("Message", "Comment is below 4 characters!");
                //return View(mostRecentEntries.ToList());
            }

            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    entry.Username = User.Identity.Name;
                }
                entry.DateAdded = DateTime.Now;
                _db.Entries.Add(entry);
                _db.SaveChanges();
                return PartialView("_AllComments", mostRecentEntries.ToList());
            }
            else
            {
                return View(mostRecentEntries.ToList());
            }
        }

        /*public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FeedbackEntry entry)
        {
            if (User.Identity.IsAuthenticated)
            {
                entry.Username = User.Identity.Name;
            }
            entry.DateAdded = DateTime.Now;
            _db.Entries.Add(entry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }*/

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            var entry = _db.Entries.Find(id);
            if (User.Identity.Name == entry.Username || User.IsInRole("Admin"))
            {
                return View(entry);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(FeedbackEntry entry)
        {
            var editEntry = _db.Entries.Find(entry.Id);
            if (User.Identity.Name == editEntry.Username || User.IsInRole("Admin"))
            {
                editEntry.Message = entry.Message;
                _db.Entry(editEntry).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult Comments(string userName)
        {
            var mostRecentEntriesPerUser =
                (from entry in _db.Entries
                 where entry.Username == userName
                 orderby entry.DateAdded descending
                 select entry).Take(20);

            ViewBag.Entries = mostRecentEntriesPerUser.ToList();
            ViewBag.UserName = userName;
            return View();
        }

        [AllowAnonymous]
        public ViewResult Show(int id)
        {
            var userRecentEntries =
           (from identry in _db.Entries
            where identry.Id == id
            orderby identry.DateAdded descending, identry.Username

            select identry).Take(20);

            var entry = _db.Entries.Find(id);
            bool hasPermission = User.Identity.Name == entry.Username;
            ViewData["hasPermission"] = hasPermission;
            ViewBag.userEntries = userRecentEntries;
            return View(entry);
        }

        [AllowAnonymous]
        public ActionResult CommentsByDate(string userDate)
        {
            DateTime myDate = new DateTime();
            DateTime myUpToDate = new DateTime();
            if (!string.IsNullOrEmpty(userDate))
            {
                myDate = DateTime.Parse(userDate.Replace("!", ":"));
                myUpToDate = myDate.AddDays(1);
            }
            var entriesPerDate =
                (from entry in _db.Entries
                 where entry.DateAdded <= myUpToDate
                 orderby entry.Username descending
                 select entry).Take(20);
            ViewBag.Entries = entriesPerDate.ToList();
            return View();
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var entry = _db.Entries.Find(id);
            if (User.Identity.Name == entry.Username || User.IsInRole("Admin"))
            {
                return View(entry);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(/*TeddySite.Models.FeedbackEntry entry*/ int id)
        //{
        //    var editEntry = _db.Entries.Find(/*entry.Id*/ id);
        //    if (User.Identity.Name == editEntry.Username ||User.IsInRole("Admin"))
        //    {
        //        _db.Entries.Remove(editEntry);
        //        _db.SaveChanges();
        //    }
        //    if (User.IsInRole("Admin"))
        //    {
        //        return RedirectToAction("Admin");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        [HttpPost]
        [ActionName("DeleteEntry")]
        public ActionResult DelEntry(int id)
        {
            System.Diagnostics.Debug.WriteLine(id);
            var delEntry = _db.Entries.Find(id);
            if (User.Identity.Name == delEntry.Username || User.IsInRole("Admin"))
            {
                _db.Entries.Remove(delEntry);
                _db.SaveChanges();
            }
            if (User.IsInRole("Admin"))
            {
                var mostRecentEntries =
                    (from entry in _db.Entries
                     orderby entry.DateAdded descending, entry.Username
                     select entry).Take(20);
                return PartialView("_AdminView", mostRecentEntries.ToList());
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            var mostRecentEntries =
           (from entry in _db.Entries
            orderby entry.DateAdded descending, entry.Username
            select entry).Take(20);

            ViewBag.Entries = mostRecentEntries.ToList();
            return View(mostRecentEntries.ToList());
        }
    }
}
