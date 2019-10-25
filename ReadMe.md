# WIP - Mass Transit Error Replay

Want to investigate how to handle retrying of Mass Transit messages that end up in RabbitMQ error queues.

First step is to build a PubSub app so that errored messages can be generated. See `./src/Investigation/readme.md`