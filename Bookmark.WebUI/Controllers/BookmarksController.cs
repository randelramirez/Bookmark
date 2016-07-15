using Bookmark.Infrastructure;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using System.Linq;
using Bookmark.Core;

namespace Bookmark.WebUI.Controllers
{
    public class BookmarksController : Controller
    {
        private BookmarkContext dataContext = new BookmarkContext();

        // for debugging and testing of tagit plugin
        // possibly move to Web API or other miscellaneous/utitliy/validation controller
        public JsonResult GetTags(string term)
        {
            var tags = this.dataContext.Tags.Select(t => t.Text);
            return Json(tags.Where(t => t.Contains(term)), JsonRequestBehavior.AllowGet);
        }

        // GET: Bookmarks
        public ActionResult Index()
        {

            return View(this.dataContext.Bookmarks);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Exclude ="Tags")]Core.Bookmark bookmark, string tags)
        {
            if (ModelState.IsValid)
            {
                this.dataContext.Bookmarks.Add(bookmark);

                // add tags logic, *refactor*, chain of responsibility?, 
                foreach (var tag in tags.Split(','))
                {

                    // validate if the tag already exist in the database
                    var existingTag = this.dataContext.Tags.SingleOrDefault(t => t.Text == tag);
                    if (existingTag != null)
                    {
                        bookmark.Tags.Add(existingTag);
                    }
                    else
                    {
                        bookmark.Tags.Add(new Core.Tag { Text = tag });
                    }
                }

                this.dataContext.SaveChanges();
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
            var bookmark = this.dataContext.Bookmarks.Find(id);
            if (bookmark == null)
            {
                return HttpNotFound();
            }
            return View(bookmark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Exclude = "Tags")]Core.Bookmark postedBookmark, string tags)
        {
            var bookmark = this.dataContext.Bookmarks.Single(b => b.Id == postedBookmark.Id);
            var tagsCollection = this.dataContext.Tags.AsNoTracking();
            var bookmarkCollection = this.dataContext.Bookmarks;
            if (ModelState.IsValid)
            {
                //this.dataContext.Entry(bookmark).State = EntityState.Modified;
                var bookmarkTags = tags.Split(',').ToList();
                if (bookmarkTags == null  ) //if(bookmarkTags.Count > 0 ) //test which one is working
                {
                   
                }
       
                foreach (var tag in bookmarkTags.ToList())
                {
                    var currentTagsOfBookmark = this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.ToList();
                    if (!currentTagsOfBookmark.Select(t => t.Text).Contains(tag))
                    {
                        // check if the tag is new or already exists in the database
                        var existingTag = this.dataContext.Tags.SingleOrDefault(t => t.Text == tag);
                        if (existingTag != null)
                        {
                            this.dataContext.Tags.Attach(existingTag);
                            this.dataContext.Entry(existingTag).State = EntityState.Unchanged;
                            bookmark.Tags.Add(existingTag);
                        }
                        else
                        {
                            var newTag = new Tag { Text = tag };
                            this.dataContext.Tags.Attach(newTag);
                            this.dataContext.Entry(newTag).State = EntityState.Added;
                            bookmark.Tags.Add(newTag);
                        }
                    }
                }

                // for removing
                // cannot remove on the same collection while iterating through it(), this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.Remove, perhpas put inside a list and then remove range, or assign Bookmark.Tags = new HashSet or List
                // check if I can assign List to an ICollection
                var tagsToRemove = new List<Tag>();
                foreach (var tag in this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.Select(t => t.Text))
                {
                    if (!bookmarkTags.Contains(tag))
                    {
                        var tagToRemove = this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.SingleOrDefault(t => t.Text == tag);
                        
                        if (tagToRemove != null)
                        {
                            //this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.Remove(tagToRemove);
                            this.dataContext.Tags.Attach(tagToRemove);
                            this.dataContext.Entry(tagToRemove).State = EntityState.Unchanged;
                            tagsToRemove.Add(tagToRemove);
                        }
                    }
                }
                tagsToRemove.ForEach(t => bookmark.Tags.Remove(t));
                this.dataContext.Bookmarks.Attach(bookmark);
                this.dataContext.Entry(bookmark).State = EntityState.Modified;
                this.dataContext.SaveChanges();
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