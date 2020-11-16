namespace AlexaEnqueuerExample {
    public static class Skill {
        public const string computerControl = "computerControl";
    }

    public static class Intent {
        // You are forced to implement these intents for all Amazon Alexa skills
        public const string cancel = "AMAZON.CancelIntent";
        public const string stop = "AMAZON.StopIntent";
        public const string navigateHome = "AMAZON.NavigateHomeIntent";
        public const string help = "AMAZON.HelpIntent";

        //These are custom intents
        public const string computerOn = "computerOn";
        public const string computerOff = "computerOff";
        public const string prepareForWork = "prepareForWork";
    }
}
