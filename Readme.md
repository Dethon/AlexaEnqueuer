# Alexa Equeuer

A simple Azure Function to enqueue commands sent by an Alexa Skill.

## Objective

A rather straightfoward way to use Alexa skills to control programmable devices. 
This receives intents sent by Alexa and pushes them into an Azure Bus queue from which they can be read by another program and used to launch custom actions.

Implement the `IntentBehavior()` method from `IntentProcessor` in a subclass to add logic to your skill's intents.
