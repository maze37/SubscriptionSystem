using Serilog;
using SubscriptionService.Web.Configuration;

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ServiceName", "SubscriptionService"));

    builder.Services.ConfigureApp(builder.Configuration);

    var app = builder.Build();

    await app.ConfigureExtensions();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

namespace SubscriptionService.Web
{
    public partial class Program;
}
