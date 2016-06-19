using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookmark.Infrastructure
{
    public class DataInitializer : DropCreateDatabaseIfModelChanges<BookmarkContext>
    {
        protected override void Seed(BookmarkContext context)
        {

        }
    }
}
