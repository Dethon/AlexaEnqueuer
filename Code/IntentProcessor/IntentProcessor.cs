using System.Globalization;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using AlexaEnqueuer.Code.Utils;
using AlexaEnqueuer.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ConfirmationStatus = AlexaEnqueuer.Code.Utils.ConfirmationStatus;

namespace AlexaEnqueuer.Code.IntentProcessor;

public abstract class IntentProcessor {
    private ILogger? _mLogger;

    public ProcessorResponse ProcessIntent(SkillRequest skillRequest, ILogger logger) {
        AlexaResponse.Culture = CultureInfo.GetCultureInfo(skillRequest.Request.Locale);
        _mLogger = logger;

        if (!UserAllowed(skillRequest)) {
            return new ProcessorResponse(Tell(AlexaResponse.userNotAllowed));
        } else if (skillRequest.GetRequestType() == typeof(LaunchRequest)) {
            return new ProcessorResponse(AskOrders(AlexaResponse.askAtFirst, AlexaResponse.askForActions));
        } else if (skillRequest.GetRequestType() == typeof(IntentRequest)) {
            return ProcessIntent((IntentRequest)skillRequest.Request);
        }

        _mLogger.LogInformation($"Unexpected type {skillRequest.GetRequestType().Name}");
        return new ProcessorResponse(Tell(AlexaResponse.typeError));
    }

    private ProcessorResponse ProcessIntent(IntentRequest intentRequest) {
        var intent = intentRequest.Intent.Name;
        var intentConfirmation = intentRequest.Intent.ConfirmationStatus;
        _mLogger?.LogInformation($"Processing {intent} with confirmation {intentConfirmation}");

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

    protected string? GetSetting(string name) {
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