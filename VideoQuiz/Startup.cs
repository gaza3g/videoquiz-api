using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VideoQuiz.Startup))]
namespace VideoQuiz
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
