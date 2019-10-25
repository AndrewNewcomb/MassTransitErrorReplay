using Common;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Subscriber.Consumers
{
    public class SomethingNoteworthyHappenedConsumer : IConsumer<SomethingNoteworthyHappened>
    {
        public async Task Consume(ConsumeContext<SomethingNoteworthyHappened> context)
        {
            var msg = context.Message;

            await Console.Out.WriteLineAsync($"{context.MessageId} - Received SomethingNoteworthyHappened: {msg.Text}");
        }
    }
}
