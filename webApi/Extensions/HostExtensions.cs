using DAL;
using DAL.DataSeed;

namespace API.Extensions;

public static class HostExtensions
{
    public static async Task<IHost> SeedData(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<BlogDbContext>();
            await SeedFacade.SeedData(context);
            return host;
        }
    }
}