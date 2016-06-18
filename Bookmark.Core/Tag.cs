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

        // might change this to ICollection, reason is existing bookmarks can be used from the UI for assigning tags
        // we have a table for bookmarks, tags, and table with composite keys BookmarkTag
        public Bookmark Bookmark { get; set; }
    }
}
