using Bookmark.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using Bookmark.Core;
using Bookmark.WebUI.Services;

namespace Bookmark.WebUI.Controllers
{
    public class BookmarksController : Controller
    {
        private BookmarkContext dataContext = new BookmarkContext();
        private BookmarkService service = new BookmarkService(new BookmarkContext());

        // for debugging and testing of tagit plugin
        // possibly move to Web API or other miscellaneous/utitliy/validation controller
        public JsonResult GetTags(string term)
        {
            var tags = this.dataContext.Tags.Select(t => t.Text);
            return Json(tags.Where(t => t.Contains(term)), JsonRequestBehavior.AllowGet);
        }

        // GET: Bookmarks
        public ActionResult Index(string search)
        {
            var bookmarks = this.service.GetAllBookmarks();
            if (!string.IsNullOrEmpty(search))
            {
                // filtering is done in application
                // todo move the filtering of the data to the database
                bookmarks = bookmarks.Where(b => b.Article.Title.Contains(search) || b.Tags.Select(t => t.Text).Contains(search));
            }
            return View(bookmarks);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude = "Tags")]Core.Bookmark bookmark, string tags)
        {
            if (ModelState.IsValid)
            {
                this.service.Add(bookmark, tags);
                this.service.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(bookmark);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookmark = this.service.GetOne(id.Value);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
            return View(bookmark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "Article.URL,Tags")]Core.Bookmark postedBookmark, string tags)
        {
            var bookmark = this.service.GetOne(postedBookmark.Id);
            if (ModelState.IsValid)
            {
                this.service.Update(postedBookmark, tags);
                this.service.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bookmark);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookmark = this.service.GetOne(id.Value);
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
            var bookmark = this.service.GetOne(id.Value);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
            this.service.Delete(bookmark);
            this.service.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}