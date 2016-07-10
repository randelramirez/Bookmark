using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark.Core
{
    public class Bookmark
    {
        public Bookmark()
        {
            this.DateSaved = DateTime.UtcNow;
            this.Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateSaved { get; set; }

        [Required]
        public string Description { get; set; }

        public Article Article { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
