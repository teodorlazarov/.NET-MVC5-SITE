using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using TeddySite.Models;

namespace TeddySite.Controllers
{
    public class FeedbackController : Controller
    {
        //
        // GET: /Feedback/

        private FeedbackContext _db = new FeedbackContext();

        public ActionResult Index()
        {
            var mostRecentEntries =
            (from entry in _db.Entries
             orderby entry.DateAdded descending, entry.FirstName

             select entry).Take(20);

            ViewBag.Entries = mostRecentEntries.ToList();
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FeedbackEntry entry)
        {
            if (User.Identity.IsAuthenticated)
            {
                entry.FirstName = User.Identity.Name;
                entry.LastName = "";
            }
            entry.DateAdded = DateTime.Now;
            _db.Entries.Add(entry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var entry = _db.Entries.Find(id);
            if (User.Identity.Name == entry.FirstName)
            {
                return View(entry);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(FeedbackEntry entry)
        {
            var editEntry = _db.Entries.Find(entry.Id);
            editEntry.Message = entry.Message;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
