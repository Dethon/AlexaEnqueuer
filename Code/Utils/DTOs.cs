using Microsoft.AspNetCore.Mvc;

namespace AlexaEnqueuer.Code.Utils;

[Serializable]
public class MessageDto(string skillValue, string intentValue)
{
    // Customize this to your heart's content
    public string Skill { get; set; } = skillValue;
    public string Intent { get; set; } = intentValue;
}

public class ProcessorResponse {
    public MessageDto? Message { get; set; }
    public IActionResult Response { get; set; }

    public ProcessorResponse(IActionResult responseValue) {
        Response = responseValue;
    }

    public ProcessorResponse(IActionResult responseValue, MessageDto messageValue) {
        Response = responseValue;
        Message = messageValue;
    }
}