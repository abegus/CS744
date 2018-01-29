using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_744Project.Startup))]
namespace _744Project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
