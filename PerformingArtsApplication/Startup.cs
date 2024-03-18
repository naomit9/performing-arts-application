using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PerformingArtsApplication.Startup))]
namespace PerformingArtsApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
