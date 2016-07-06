using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bookmark.Infrastructure;
using System.Net;

namespace Bookmark.WebUI.Controllers
{
    public class BookmarksController : Controller
    {
        private BookmarkContext dataContext = new Infrastructure.BookmarkContext();

        // GET: Bookmarks
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Core.Bookmark bookmark)
        {
            if (ModelState.IsValid)
            {
                this.dataContext.Bookmarks.Add(bookmark);
                this.dataContext.SaveChanges();
                return Redirect("Index");
            }
            return View(bookmark);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookmark = this.dataContext.Bookmarks.Find(id);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
            return View(bookmark);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookmark = this.dataContext.Bookmarks.Find(id);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
            this.dataContext.Bookmarks.Remove(bookmark);
            this.dataContext.SaveChanges();
            return Redirect("Index");
        }
    }
}