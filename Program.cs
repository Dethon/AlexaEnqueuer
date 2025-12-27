using AlexaEnqueuer;
using AlexaEnqueuer.Code.Example;
using AlexaEnqueuer.Code.IntentProcessor;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IntentProcessor, ComputerIgniterIntentProcessor>();
    })
    .Build();

host.Run();
