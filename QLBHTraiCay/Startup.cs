using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QLBHTraiCay.Startup))]
namespace QLBHTraiCay
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
