using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EGSW.Web.Startup))]
namespace EGSW.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
