# Alexa Equeuer

A simple Azure Function to enqueue commands sent by an Alexa skill.

## Objective

A rather straightfoward way to use Alexa skills to control programmable devices. 
This receives intents sent by Alexa and pushes them into an Azure Bus queue from which they can be read by another program and used to launch custom actions.

To add logic to your skill's intents just implement the `IntentBehavior()` method from `IntentProcessor` in a subclass and inject it into `AlexaEnqueuer`. You can see an example in the `Code/Example` folder.

## Settings
The function uses some settings that must be configured for your use case:
    `QueueName`: The name of your Azure Service Bus queue.
    `ServiceBusConnection`: The connection string to your Azure Service Bus namespace.
    `UserRestriction`: If this is defined, the use of this skill will be limited to the user referenced in `AllowedUserId`.
    `AllowedUserId`: The id of the only amazon user that wil be able to use the skill.

These setting names are defined as constants within `VariableName`.
