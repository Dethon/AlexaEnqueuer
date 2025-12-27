using Alexa.NET.Request.Type;
using AlexaEnqueuer.Code.Utils;
using AlexaEnqueuer.Resources;

namespace AlexaEnqueuer.Code.Example;

public class ComputerIgniterIntentProcessor : IntentProcessor.IntentProcessor {

    protected override ProcessorResponse IntentBehavior(IntentRequest intentRequest) {
        var intent = intentRequest.Intent.Name;
        switch (intent) {
            case Intent.computerOn:
            case Intent.computerOff:
            case Intent.prepareForWork:
                return new ProcessorResponse(
                    Tell(AlexaResponse.orderSubmitted),
                    new MessageDto(Skill.computerControl, intent));
            case Intent.stop:
            case Intent.cancel:
            case Intent.navigateHome:
                return new ProcessorResponse(Tell(AlexaResponse.bye));
            case Intent.help:
                return new ProcessorResponse(Tell(AlexaResponse.help));
            default:
                return new ProcessorResponse(AskOrders(AlexaResponse.orderNotRecognized, AlexaResponse.askForActions));
        }
    }
}