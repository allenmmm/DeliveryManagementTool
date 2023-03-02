using Owin;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(DMT.Web.Startup))]
namespace DMT.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}