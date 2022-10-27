using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Photo.Album.Console;
using Photo.Album.Console.Console;
using Photo.Album.Data.Models;
using Photo.Album.Engine.AlbumApiClient;
using Polly;
using Polly.Extensions.Http;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<App>().RunAsync(args);
}
catch (Exception ex)
{

    Console.WriteLine(ex.Message);
}

static IHostBuilder CreateHostBuilder(string[] args)
{
    var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddEnvironmentVariables()
        .Build();

    var settings = config.GetRequiredSection("Settings").Get<Settings>();

    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddHttpClient<IAlbumApiClient, AlbumApiClient>(client =>
            { 
                client.BaseAddress = new Uri(settings.BaseAlbumUrl);
            })
            .AddPolicyHandler(GetRetryPolicy(settings.ApiRetryCount));

            services.AddSingleton<App>();

            services.AddSingleton<IConsoleManager, ConsoleManager>();

            //removing info logs from console.  Only show Warning and Above to not interfere with output
            services.AddLogging(builder => builder.AddFilter("System", LogLevel.Warning)
                .AddFilter("System", LogLevel.Error)
                .AddFilter("System", LogLevel.Critical).AddConsole());
        });
}

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(string apiRetryCount)
{
    var jitterer = new Random();
    var success = int.TryParse(apiRetryCount, out int retryCount);
    if (!success) retryCount = 5;
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => !response.IsSuccessStatusCode)
        .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)));
}