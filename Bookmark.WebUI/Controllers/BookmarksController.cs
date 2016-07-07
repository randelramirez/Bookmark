using Bookmark.Infrastructure;
using System.Net;
using System.Web.Mvc;

namespace Bookmark.WebUI.Controllers
{
    public class BookmarksController : Controller
    {
        private BookmarkContext dataContext = new BookmarkContext();

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
                this.dataContext.Entry(bookmark).State = System.Data.Entity.EntityState.Modified;
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