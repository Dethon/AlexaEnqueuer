namespace AlexaEnqueuer {
    static class ConfirmationStatus {
        public const string denied = "DENIED";
        public const string confirmed = "CONFIRMED";
    }

    static class VariableName {
        // Use your own setting names
        public const string queue = "QueueName";
        public const string serviceBusUri = "ServiceBusConnection";
        public const string allowedUserId = "AllowedUserId";
        public const string userRestriction = "userRestriction";
    }

    static class Skill {
        // Customize this to your heart's content
        public const string computerControl = "computerControl";
    }

    static class Intent {
        // You are forced to implement these intents for all Amazon Alexa skills
        public const string cancel = "AMAZON.CancelIntent";
        public const string stop = "AMAZON.StopIntent";
        public const string navigateHome = "AMAZON.NavigateHomeIntent";
        public const string help = "AMAZON.HelpIntent";

        // Customize this to your heart's content
        public const string computerOn = "computerOn";
        public const string computerOff = "computerOff";
        public const string prepareForWork = "prepareForWork";
    }
}
