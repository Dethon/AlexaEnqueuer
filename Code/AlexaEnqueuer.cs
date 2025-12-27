using System.Text.Json;
using Alexa.NET.Request;
using AlexaEnqueuer.Code.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AlexaEnqueuer.Code;

public class AlexaEnqueuer {
    // Change this instantiation for your own subclass
    private readonly IntentProcessor.IntentProcessor _mIntentProcessor;
    private readonly ILogger<AlexaEnqueuer> _mLogger;

    public AlexaEnqueuer(IntentProcessor.IntentProcessor intentProcessor, ILogger<AlexaEnqueuer> logger) {
        _mIntentProcessor = intentProcessor;
        _mLogger = logger;
    }

    [Function("AlexaEnqueuer")]
    public async Task<AlexaEnqueuerOutput> AlexaInput(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request) {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();
        var skillRequest = JsonSerializer.Deserialize<SkillRequest>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (skillRequest == null || !await ValidateRequest(request, skillRequest, body)) {
            _mLogger.LogError("Validation failed - RequestVerification failed");
            return new AlexaEnqueuerOutput { HttpResponse = new BadRequestResult() };
        }

        var response = _mIntentProcessor.ProcessIntent(skillRequest, _mLogger);
        return new AlexaEnqueuerOutput {
            HttpResponse = response.Response,
            Message = response.Message
        };
    }

    private async Task<bool> ValidateRequest(HttpRequest request, SkillRequest skillRequest, string body) {
        try {
            var header = request.Headers;
            var signature = header["Signature"].ToString();
            var certUrl = new Uri(header["SignatureCertChainUrl"].ToString());

            return 
                RequestVerification.RequestTimestampWithinTolerance(skillRequest) &&
                await RequestVerification.Verify(signature, certUrl, body);
        } catch {
            _mLogger.LogError("Validation exception");
            return false;
        }            
    }

    [Function("AutoHeater")]
    public void AutoHeater([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer) {
        _mLogger.LogInformation($"Warming...: {DateTime.Now}");
    }
}

public class AlexaEnqueuerOutput {
    [HttpResult]
    public IActionResult? HttpResponse { get; set; }

    [ServiceBusOutput("%" + VariableName.queue + "%", Connection = VariableName.serviceBusUri)]
    public MessageDto? Message { get; set; }
}