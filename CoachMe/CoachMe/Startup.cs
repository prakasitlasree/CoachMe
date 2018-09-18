using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CoachMe.Startup))]
namespace CoachMe
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
