using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bookmark.WebUI.Startup))]
namespace Bookmark.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
