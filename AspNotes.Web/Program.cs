using Serilog;

namespace AspNotes.Web;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args);

        host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        host.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });

        return host;
    }
}
