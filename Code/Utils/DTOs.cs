using Microsoft.AspNetCore.Mvc;

namespace AlexaEnqueuer.Code.Utils;

[Serializable]
public class MessageDto {
    // Customize this to your heart's content
    public string Skill { get; set; }
    public string Intent { get; set; }

    public MessageDto(string skillValue, string intentValue) {
        Skill = skillValue;
        Intent = intentValue;
    }
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