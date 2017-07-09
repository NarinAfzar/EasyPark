using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EasyPark.WebUI.Startup))]

namespace EasyPark.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}