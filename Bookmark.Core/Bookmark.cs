using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark.Core
{
    public class Bookmark
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
