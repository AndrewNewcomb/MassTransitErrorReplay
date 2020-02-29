using Common;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Subscriber.Consumers
{
    public class NewDataAvailableConsumer : IConsumer<NewDataAvailable>
    {
        public async Task Consume(ConsumeContext<NewDataAvailable> context)
        {
            var msg = context.Message;

            await Console.Out.WriteLineAsync($"\r\n{context.MessageId} - {context.ConversationId} - Received NewDataAvailable: {msg.Text}");

            if (msg.Text.Contains('e'))
            {
                await context.Publish<SomethingNoteworthyHappened>(
                    new { Text = $"There is an 'e' in: {msg.Text}" });
            }

            if (State.EnableFail && msg.Text.Contains('f'))
            {
                var redeliveryAttempt = context.Headers.Get<int>("MT-Redelivery-Count", 0).Value;

                var retryAttempt = context.GetRetryAttempt();
                if (msg.Text.Contains("ok") && retryAttempt > 1)
                {
                    Console.Out.WriteLine($"{context.MessageId} - {context.ConversationId} --- Retry {retryAttempt}, 2LR {redeliveryAttempt}, will treat as ok.");
                }
                else if (msg.Text.Contains("2lr") && redeliveryAttempt > 1 && retryAttempt > 1)
                {
                    Console.Out.WriteLine($"{context.MessageId} - {context.ConversationId} --- Retry {retryAttempt}, 2LR {redeliveryAttempt}, will treat as ok.");
                }
                else
                {
                    Console.Out.WriteLine($"{context.MessageId} - {context.ConversationId} --- Retry {retryAttempt}, 2LR {redeliveryAttempt}, will THROW.");
                    throw new ApplicationException($"Subscriber throws for text: {msg.Text}");
                }
            }

            await context.Publish<InitialProcessingCompleted>(new { Text = msg.Text });
        }
    }
}
