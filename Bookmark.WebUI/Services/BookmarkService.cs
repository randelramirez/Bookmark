using Bookmark.Core;
using Bookmark.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Bookmark.WebUI.Services
{
    public class BookmarkService
    {
        private BookmarkContext dataContext;

        public BookmarkService(BookmarkContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<Bookmark.Core.Bookmark> GetAllBookmarks()
        {
            return this.dataContext.Bookmarks.OrderBy(b => b.Article.Title);
        }

        public Bookmark.Core.Bookmark GetOne(int id)
        {
            return this.dataContext.Bookmarks.Find(id);
        }

        public void Add(Bookmark.Core.Bookmark bookmark, string tags)
        {
            var existingBookmark = this.dataContext.Bookmarks.SingleOrDefault(b => b.Article.URL == bookmark.Article.URL);
            if (existingBookmark == null)
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
            }
            else
            {
                // to do: create exception (Custom exception, or use invalid operation exception, bookmark with same url already exist)
                // or move the validation logic to another service, BookmarkDuplicateValidator???
                // throw error message in front-end


            }
        }



        public void Update(Core.Bookmark postedBookmark, string tags)
        {
            var bookmark = this.GetOne(postedBookmark.Id);
            bookmark.Article.Title = postedBookmark.Article.Title;
            bookmark.Description = postedBookmark.Description;

            var bookmarkTags = string.IsNullOrEmpty(tags) ? null : tags.Split(',').ToList();
            if (bookmarkTags != null)
            {
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
            }

            // if there are no tags posted, then delete all tags associated with the bookmark
            if (bookmarkTags == null)
            {
                // find a way to simplify, bookmark.Tags  = null; perhaps or something equivalent?
                // I think the tags are being lazy loaded, accessed by the enumerator
                // load all related tags during load of the bookmark, reasearch how to load related entities ===>  var bookmark = this.dataContext.Bookmarks.Include(t => t.Tags).Single(b => b.Id == postedBookmark.Id);
                var tagsToRemove = new List<Tag>();
                foreach (var tag in bookmark.Tags)
                {
                    tagsToRemove.Add(tag);
                }
                tagsToRemove.ForEach(t => bookmark.Tags.Remove(t));
            }
            else
            {
                // cannot remove on the same collection while iterating through it(), this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.Remove, perhpas put inside a list and then remove range, or assign Bookmark.Tags = new HashSet or List
                // check if I can assign List to an ICollection

                // to do: from the comments above, change to: var tags = this.dataContext.Bookmarks.Single(b => b.Id == bookmark.Id).Tags.Select(t => t.Text).ToList()
                // iterate on that collection, remove  var tagsToRemove, instead do something like => context.tags.remove(tag)
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
            }

            this.dataContext.Bookmarks.Attach(bookmark);
            this.dataContext.Entry(bookmark).State = EntityState.Modified;
        }

        public void Delete(Bookmark.Core.Bookmark bookmark)
        {
            this.dataContext.Bookmarks.Remove(bookmark);
        }

        public void SaveChanges()
        {
            this.dataContext.SaveChanges();
        }
    }
}