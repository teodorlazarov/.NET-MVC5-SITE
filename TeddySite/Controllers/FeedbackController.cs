﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using TeddySite.Models;

namespace TeddySite.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        //
        // GET: /Feedback/

        private FeedbackContext _db = new FeedbackContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            var mostRecentEntries =
            (from entry in _db.Entries
             orderby entry.DateAdded descending, entry.Username

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
                entry.Username = User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@"));
            }
            entry.DateAdded = DateTime.Now;
            _db.Entries.Add(entry);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var entry = _db.Entries.Find(id);
            if (User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@")) == entry.Username)
            {
                return View(entry);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Edit(FeedbackEntry entry)
        {
            var editEntry = _db.Entries.Find(entry.Id);
            if (User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@")) == entry.Username)
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
            var entry = _db.Entries.Find(id);
            bool hasPermission = User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@")) == entry.Username;
            ViewData["hasPermission"] = hasPermission;
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
            var entry = _db.Entries.Find(id);
            if (User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@")) == entry.Username)
            {
                return View(entry);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(TeddySite.Models.FeedbackEntry entry)
        {
            var editEntry = _db.Entries.Find(entry.Id);
            if (User.Identity.Name.Substring(0, User.Identity.Name.IndexOf("@")) == editEntry.Username)
            {
                _db.Entries.Remove(editEntry);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

    }
}
