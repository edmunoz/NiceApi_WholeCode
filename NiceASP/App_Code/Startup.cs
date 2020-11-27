using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NiceASP.Startup))]
namespace NiceASP
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
