using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TeddySite.Startup))]
namespace TeddySite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
