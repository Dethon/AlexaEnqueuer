using AlexaEnqueuer;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(AlexaEnqueuerExample.Startup))]
namespace AlexaEnqueuerExample {
    public class Startup : FunctionsStartup {
        public override void Configure(IFunctionsHostBuilder builder) {
            builder.Services.AddSingleton<IntentProcessor, ComputerIgniterIntentProcessor>();
        }
    }
}
