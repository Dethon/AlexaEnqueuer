using AlexaEnqueuer;
using AlexaEnqueuer.Code.Example;
using AlexaEnqueuer.Code.IntentProcessor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IntentProcessor, ComputerIgniterIntentProcessor>();
    })
    .Build();

host.Run();
