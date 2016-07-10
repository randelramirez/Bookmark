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
                    bookmark.Tags.Add(new Core.Tag { Text = tag });
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
        public ActionResult Edit([Bind(Exclude = "Tags")]Core.Bookmark bookmark, string tags)
        {

            if (ModelState.IsValid)
            {
                this.dataContext.Entry(bookmark).State = EntityState.Modified;
                var bookmarkTags = tags.Split(',').ToList();

                // clear tags for current bookmark
                //bookmark.Tags.Clear();

                //load tag text and id
                // existing tags on database
                //var allTags = this.dataContext.Tags.ToList().Where(b => bookmarkTags.ToList().ForEach());
                // get existing tags from the database, otherwise they will be marked as added if ID is not present, NOTE: graph objects
                var allTags = this.dataContext.Tags;
                var currentTags = this.dataContext.Bookmarks.Find(bookmark.Id).Tags; //this.dataContext.Entry(bookmark).Entity.Tags;
                //var existingTags = allTags.Where(b => bookmarkTags.ToList().Contains(b.Text));//.ForEachAsync(b => this.dataContext.Entry(b).State = EntityState.Unchanged);
                //existingTags.ForEachAsync(b => this.dataContext.Entry(b).State = EntityState.Unchanged);

                // compare current tags vs tags passed
                foreach (var tag in bookmarkTags)
                {
                    // if not yet on bookmark as a tag, add it
                    if (!currentTags.Select(t => t.Text).Contains(tag))
                    {
                        // check if the tag is already existing in the database, if not create a new tag
                        var existingTag = allTags.SingleOrDefault(b => b.Text == tag);
                        if (existingTag != null)
                        {
                            bookmark.Tags.Add(existingTag);
                        }
                        else
                        {
                            var newTag = new Tag { Text = tag };
                            bookmark.Tags.Add(newTag);
                        }
                    }
                }

                foreach (var tagToRemove in currentTags.Select(t => t.Text).Where(b => !bookmarkTags.Contains(b)))
                {
                    var tag = allTags.Single(t => t.Text == tagToRemove);
                    bookmark.Tags.Remove(tag);
                }

                #region test
                ////// get removed tags, disassociate the tags
                ////foreach (var tag in bookmarkTags)
                ////{
                ////    bookmarkTags.Contains(currentag)
                ////}

                //// add new tags
                //// for new tags create tag class and add them
                //var tagsForSaving = new List<Tag>();
                //foreach (var tag in bookmarkTags)
                //{
                //    // validate, tag text should not yet exist
                //    // validate if tag text already exist in the database
                //    if (!allTags.Select(t => t.Text).Contains(tag))
                //    {
                //        var newTag = new Tag() { Text = tag };
                //        this.dataContext.Entry(newTag).State = EntityState.Added;
                //        tagsForSaving.Add(newTag);
                //    }
                //}

                //tagsForSaving.AddRange(existingTags);
                //tagsForSaving.ForEach(b => bookmark.Tags.Add(b));
                #endregion



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