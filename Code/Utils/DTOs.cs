using Microsoft.AspNetCore.Mvc;

namespace AlexaEnqueuer {
    public class MessageDTO {
        public string Skill { get; set; }
        public string Intent { get; set; }

        public MessageDTO(string skillValue, string intentValue) {
            Skill = skillValue;
            Intent = intentValue;
        }
    }

    public class ProcessorResponse {
        public MessageDTO Message { get; set; }
        public IActionResult Response { get; set; }

        public ProcessorResponse(IActionResult responseValue) {
            Response = responseValue;
        }

        public ProcessorResponse(IActionResult responseValue, MessageDTO messageValue) {
            Response = responseValue;
            Message = messageValue;
        }
    }
}
