using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Alexa.NET.Request;
using Newtonsoft.Json;

namespace AlexaEnqueuer {
    public class AlexaEnqueuer {
        // Change this instantiation for your own subclass
        private readonly IntentProcessor m_intentProcessor;
        private ILogger m_logger;

        public AlexaEnqueuer(IntentProcessor intentProcessor) {
            m_intentProcessor = intentProcessor;
        }

        [FunctionName("AlexaEnqueuer")]
        public async Task<IActionResult> AlexaInput([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
                                                    [ServiceBus("%" + VariableName.queue + "%", Connection = VariableName.serviceBusUri)] IAsyncCollector<MessageDTO> queueCollector,
                                                    ILogger logger) {
            m_logger = logger;
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(await request.ReadAsStringAsync());
            if (!await ValidateRequest(request, skillRequest)) {
                m_logger.LogError("Validation failed - RequestVerification failed");
                return new BadRequestResult();
            }

            var response = m_intentProcessor.ProcessIntent(skillRequest, logger);
            await EnqueueMessage(queueCollector, response.Message);
            return response.Response;
        }

        private async Task<bool> ValidateRequest(HttpRequest request, SkillRequest skillRequest) {
            try {
                var header = request.Headers;
                var signature = header["Signature"];
                var certUrl = new Uri(header["SignatureCertChainUrl"]);
                var body = await request.ReadAsStringAsync();

                return 
                    RequestVerification.RequestTimestampWithinTolerance(skillRequest) &&
                    await RequestVerification.Verify(signature, certUrl, body);
            } catch {
                m_logger.LogError("Validation exception");
                return false;
            }            
        }

        private async Task EnqueueMessage(IAsyncCollector<MessageDTO> queueCollector, MessageDTO message) {
            if (message != null) {
                m_logger.LogInformation($"Enqueuing {message.Skill} - {message.Intent}");
                await queueCollector.AddAsync(message);
            }
        }

        [FunctionName("AutoHeater")]
        public void AutoHeater([TimerTrigger("0 */15 * * * *")] TimerInfo myTimer, ILogger log) {
            log.LogInformation($"Warming...: {DateTime.Now}");
        }
    }
}
