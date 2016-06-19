using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark.Core
{
    public class Tag
    {
        public int Id { get; set; }

        public string Text { get; set; }

        // we have a table for bookmarks, tags, and table with composite keys BookmarkTag
        public virtual ICollection<Bookmark> Bookmarks { get; set; }
    }
}
