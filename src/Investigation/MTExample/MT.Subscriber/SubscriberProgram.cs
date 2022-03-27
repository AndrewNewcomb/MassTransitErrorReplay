using Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Subscriber
{
    public class SubscriberProgram
    {
        // see     "MassTransitErrorReplay\src\Investigation\readme.md"

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"SUBSCRIBER");

            var (argOptions, argErrors) = ParamParser.ParseParams(args);

            if (!string.IsNullOrEmpty(argErrors))
            {
                Console.WriteLine($"{argErrors}");
                Environment.Exit(1);
            }

            DisplayParams(argOptions);
            
            State.EnableFail = true;

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    ConfigureMassTransit(services, argOptions);
                })
                .Build();
            
            await host.StartAsync();

            DisplayInfo(State.EnableFail);

            while (true)
            {
                var line = Console.ReadLine();

                if (line == "exit")
                {
                    break;
                }
                else if (line == "t")
                {
                    State.EnableFail = !State.EnableFail;
                    Console.WriteLine($"*** Failure processing is now {(State.EnableFail ? "on":"off" )}.");
                }
                else if (line == "?")
                {
                    DisplayInfo(State.EnableFail);
                }
            }

            await host.StopAsync();
        }

        private static void ConfigureMassTransit(IServiceCollection services, SubscriberParams argOptions)
        {
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(argOptions.Host, argOptions.VHost, argOptions.Name, h =>
                    {
                        h.Username(argOptions.Username);
                        h.Password(argOptions.Password);
                    });


                    cfg.UseInMemoryScheduler();
                    //cfg.UseMessageScheduler(new Uri($"rabbitmq://{argOptions.Host}/quartz"));

                    cfg.ReceiveEndpoint(argOptions.QueueName, ep =>
                    {
                        if (!argOptions.DisableQueue2LevelRetry) ep.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)));
                        if (!argOptions.DisableQueueRetry) ep.UseMessageRetry(r => r.Immediate(2));
                        if (argOptions.QueueIsExclusive) ep.ExclusiveConsumer = argOptions.QueueIsExclusive;
                        if (argOptions.SingleActiveConsumer) ep.SingleActiveConsumer = argOptions.SingleActiveConsumer;
                        if (!argOptions.DisableQueueOutbox) ep.UseInMemoryOutbox();

                        ep.Consumer<Consumers.NewDataAvailableConsumer>();
                        ep.Consumer<Consumers.InitialProcessingCompletedConsumer>();
                        ep.Consumer<Consumers.FinalProcessingCompletedConsumer>();
                    });

                    if (!argOptions.DisableNoteworthyQueue)
                    {
                        cfg.ReceiveEndpoint("noteworthy_queue", ep =>
                        {
                            ep.Consumer<Consumers.SomethingNoteworthyHappenedConsumer>();
                        });
                    }

                    if (!argOptions.DisableFaultQueue)
                    {
                        cfg.ReceiveEndpoint("fault_queue", ep =>
                        {
                            // can also declare handlers and consumer with a lambda
                            ep.Handler<Fault<NewDataAvailable>>(context =>
                            {
                                var msg = context.Message;

                                return Console.Out.WriteLineAsync($"{context.MessageId} - {context.ConversationId} - Fault<NewDataAvailable>: {msg.Message.Text}");
                            });
                        });
                    }
                });
            });
        }


        private static void DisplayParams(SubscriberParams parms) 
        {
            Console.WriteLine($"  --host                    {parms.Host}");                                  
            Console.WriteLine($"  --vhost                   {parms.VHost}");                                 
            Console.WriteLine($"  --name                    {parms.Name}");                                  
            Console.WriteLine($"  --username                {parms.Username}");                              
            Console.WriteLine($"  --password                {parms.Password}");                              
            Console.WriteLine($"  --queueName               {parms.QueueName}");                             
            Console.WriteLine($"  --disableQueueOutbox      {parms.DisableQueueOutbox}");                        
            Console.WriteLine($"  --queueIsExclusive        {parms.QueueIsExclusive}");                      
            Console.WriteLine($"  --singleActiveConsumer    {parms.SingleActiveConsumer}");                      
            Console.WriteLine($"  --disableQueueRetry       {parms.DisableQueueRetry}");                            
            Console.WriteLine($"  --disableQueue2LevelRetry {parms.DisableQueue2LevelRetry}");                      
            Console.WriteLine($"  --disableFaultQueue       {parms.DisableFaultQueue}");                     
            Console.WriteLine($"  --disableNoteworthyQueue  {parms.DisableNoteworthyQueue}");
            Console.WriteLine("---------------------------");
        }

        private static void DisplayInfo(bool enableFailState) 
        {
            Console.WriteLine("The event messages are..."); 
            Console.WriteLine("   NewDataAvailable ->");
            Console.WriteLine("     InitialProcessingCompleted ->");
            Console.WriteLine("       FinalProcessingCompleted");
            Console.WriteLine("       SomethingNoteworthyHappened");
            Console.WriteLine("---------------------------");
            Console.WriteLine("Special behaviours if received text contains...");
            Console.WriteLine("  'f' throws an exception if failure processing is on");
            Console.WriteLine("      (but if also contains 'ok' will fix after 2 retries when retries are enabled.");
            Console.WriteLine("      (but if also contains '2lr' will fix after 2 retries when second level retries are enabled.");
            Console.WriteLine("  'e' publishes an event");
            Console.WriteLine("      Try 'fe' with outbox disabled (bad) and enabled (good)");
            Console.WriteLine("---------------------------");
            Console.WriteLine($"Enter 't' to toggle the failure processing. It is currently {(enableFailState ? "on":"off")}.");
            Console.WriteLine("Enter 'exit' to exit the loop.");
            Console.WriteLine("Enter '?' to repeat this message.");
            Console.WriteLine("---------------------------");
            Console.WriteLine();
        }
    }
}
