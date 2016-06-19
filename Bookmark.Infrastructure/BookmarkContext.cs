using Bookmark.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark.Infrastructure
{
    public class BookmarkContext : DbContext
    {
        public BookmarkContext() : base ("BookmarkContext")
        {
        }

        public DbSet<Core.Bookmark> Bookmarks { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}
