using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Webcam.Startup))]
namespace Webcam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
