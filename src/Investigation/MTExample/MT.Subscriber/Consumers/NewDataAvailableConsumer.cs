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

            await Console.Out.WriteLineAsync($"\r\n{context.MessageId} - Received NewDataAvailable: {msg.Text}");

            if (msg.Text.Contains('e'))
            {
                await context.Publish<SomethingNoteworthyHappened>(
                    new { Text = $"There is an 'e' in: {msg.Text}" });
            }

            if (State.EnableFail && msg.Text.Contains('f'))
            {
                var retryAttempt = context.GetRetryAttempt();
                if (msg.Text.Contains("ok") && retryAttempt > 1)
                {
                    Console.Out.WriteLine($"\t{context.MessageId} - Retry number {retryAttempt}, will treat as ok....");
                }
                else
                {
                    Console.Out.WriteLine($"\t{context.MessageId} - Retry number {retryAttempt}, will THROW for text: {msg.Text}.");
                    throw new ApplicationException($"Subscriber throws for text: {msg.Text}");
                }
            }

            await context.Publish<InitialProcessingCompleted>(new { Text = msg.Text });
        }
    }
}
