using Common;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Subscriber.Consumers
{
    public class InitialProcessingCompletedConsumer : IConsumer<InitialProcessingCompleted>
    {
        public async Task Consume(ConsumeContext<InitialProcessingCompleted> context)
        {
            var msg = context.Message;

            await Console.Out.WriteLineAsync($"{context.MessageId} - {context.ConversationId} - Received InitialProcessingCompleted: {msg.Text}.");

            await context.Publish<FinalProcessingCompleted>(new { Text = msg.Text });
        }
    }
}
