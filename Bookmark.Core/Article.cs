using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark.Core
{
    public class Article
    {
        [Required]
        public string Title { get; set; }

        [Required]
        [Url]
        public string URL { get; set; }
    }
}
