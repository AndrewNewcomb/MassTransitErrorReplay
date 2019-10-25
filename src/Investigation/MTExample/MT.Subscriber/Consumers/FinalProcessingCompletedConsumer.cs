using Common;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Subscriber.Consumers
{
    public class FinalProcessingCompletedConsumer : IConsumer<FinalProcessingCompleted>
    {
        public async Task Consume(ConsumeContext<FinalProcessingCompleted> context)
        {
            var msg = context.Message;

            await Console.Out.WriteLineAsync($"{context.MessageId} - Received FinalProcessingCompleted: {msg.Text}");
        }
    }
}
