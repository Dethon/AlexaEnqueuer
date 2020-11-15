using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using AlexaEnqueuer.Resources;

namespace AlexaEnqueuer {
    abstract class IntentProcessor {
        ILogger m_logger;

        public ProcessorResponse ProcessIntent(SkillRequest skillRequest, ILogger logger) {
            AlexaResponse.Culture = CultureInfo.GetCultureInfo(skillRequest.Request.Locale);
            m_logger = logger;

            if (!UserAllowed(skillRequest)) {
                return new ProcessorResponse(Tell(AlexaResponse.userNotAllowed));
            } else if (skillRequest.GetRequestType() == typeof(LaunchRequest)) {
                return new ProcessorResponse(AskOrders(AlexaResponse.askAtFirst, AlexaResponse.askForActions));
            } else if (skillRequest.GetRequestType() == typeof(IntentRequest)) {
                return ProcessIntent(skillRequest.Request as IntentRequest);
            }

            m_logger.LogInformation($"Unexpected type {skillRequest.GetRequestType().Name}");
            return new ProcessorResponse(Tell(AlexaResponse.typeError));
        }

        private ProcessorResponse ProcessIntent(IntentRequest intentRequest) {
            var intent = intentRequest.Intent.Name;
            var intentConfirmation = intentRequest.Intent.ConfirmationStatus;
            m_logger.LogInformation($"Processing {intent} with confirmation {intentConfirmation}");

            if (intentConfirmation == ConfirmationStatus.denied) {
                return new ProcessorResponse(AskOrders(AlexaResponse.askAfterAction, AlexaResponse.askForActions));
            }

            return IntentBehavior(intentRequest);
        }

        protected abstract ProcessorResponse IntentBehavior(IntentRequest intentRequest);

        protected bool UserAllowed(SkillRequest skillRequest) {
            return 
                GetSetting(VariableName.userRestriction) == null ||
                GetSetting(VariableName.allowedUserId) == skillRequest.Session.User.UserId; 
        }

        protected string GetSetting(string name) {
            return Environment.GetEnvironmentVariable(name);
        }

        protected Reprompt GenReprompt(string message) {
            return new Reprompt {
                OutputSpeech = new PlainTextOutputSpeech(message)
            };
        }

        protected IActionResult AskOrders(string prompt, string reprompt) {
            return new OkObjectResult(ResponseBuilder.Ask(prompt, GenReprompt(reprompt)));
        }

        protected IActionResult Tell(string message) {
            return new OkObjectResult(ResponseBuilder.Tell(message));
        }
    }
}
