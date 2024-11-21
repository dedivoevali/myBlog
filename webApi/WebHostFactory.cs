using NLog.Web;

namespace API
{
    public static class WebHostFactory
    {
        public static IHostBuilder CreateHostBuilderUsingStartupAndLogging()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}