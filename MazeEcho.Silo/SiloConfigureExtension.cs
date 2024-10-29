using Microsoft.Extensions.Hosting;
using Orleans.Configuration;

namespace MazeEcho.Silo;

public static class SiloConfigureExtension
{
    public static void ConfigureSilo(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseOrleans(siloBuilder =>
        {
            siloBuilder.UseLocalhostClustering();

            siloBuilder.AddMemoryGrainStorage("urls");
        });
    }
}