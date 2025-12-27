using Alexa.NET.Request;
using Newtonsoft.Json;
using AlexaEnqueuer.Code.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AlexaEnqueuer.Code;

public class AlexaEnqueuer(IntentProcessor.IntentProcessor intentProcessor, ILogger<AlexaEnqueuer> logger)
{
    // Change this instantiation for your own subclass

    [Function("AlexaEnqueuer")]
    public async Task<AlexaEnqueuerOutput> AlexaInput(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request) {
        using var reader = new StreamReader(request.Body);
        var body = await reader.ReadToEndAsync();
        var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(body);
        if (skillRequest == null || !await ValidateRequest(request, skillRequest, body)) {
            logger.LogError("Validation failed - RequestVerification failed");
            return new AlexaEnqueuerOutput { HttpResponse = new BadRequestResult() };
        }

        var response = intentProcessor.ProcessIntent(skillRequest, logger);
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
            logger.LogError("Validation exception");
            return false;
        }            
    }

    [Function("AutoHeater")]
    public void AutoHeater([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer) {
        logger.LogInformation($"Warming...: {DateTime.Now}");
    }
}

public class AlexaEnqueuerOutput {
    [HttpResult]
    public IActionResult? HttpResponse { get; set; }

    [ServiceBusOutput("%" + VariableName.queue + "%", Connection = VariableName.serviceBusUri)]
    public MessageDto? Message { get; set; }
}