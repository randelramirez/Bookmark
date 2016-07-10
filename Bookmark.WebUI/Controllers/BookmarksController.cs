using Bookmark.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Linq;

namespace Bookmark.WebUI.Controllers
{
    public class BookmarksController : Controller
    {
        private BookmarkContext dataContext = new BookmarkContext();

        // for debugging and testing of tagit plugin
        public JsonResult GetTags(string term)
        {
            var tags = new List<string>();
            tags.Add("C#");
            tags.Add("F#");
            tags.Add("JavaScript");
            tags.Add("Java");
            tags.Add("Swift");

            //return Json(tags.Select(b => new { label = b, value = b + " val" }).Where(t => t.label.Contains(term)), JsonRequestBehavior.AllowGet)
            return Json(tags.Where(t => t.Contains(term)), JsonRequestBehavior.AllowGet);
        }

        // for debugging and testing of tagit plugin
        public JsonResult GetTags2(string term)
        {
            var tags = new List<string>();
            tags.Add("C#");
            tags.Add("F#");
            tags.Add("JavaScript");
            tags.Add("Java");
            tags.Add("Swift");

            return Json(tags.Select(b => new { label = b, value = b + " val" }).Where(t => t.label.Contains(term)), JsonRequestBehavior.AllowGet);
        }

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
        public ActionResult Create(Core.Bookmark bookmark, string tags)
        {
            if (ModelState.IsValid)
            {
                this.dataContext.Bookmarks.Add(bookmark);

                // add tags logic, *refactor*, chain of responsibility?, 
                foreach (var tag in tags.Split(','))
                {
                    bookmark.Tags.Add(new Core.Tag { Text = tag });
                }

                this.dataContext.SaveChanges();
                return Redirect("Index");
            }
            return View(bookmark);
        }

        public ActionResult Edit(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Core.Bookmark bookmark)
        {
            if (ModelState.IsValid)
            {
                this.dataContext.Entry(bookmark).State = EntityState.Modified;
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