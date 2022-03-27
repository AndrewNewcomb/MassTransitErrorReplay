using Common;
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Publisher
{
    public class PublisherProgram
    {
        // see     "MassTransitErrorReplay\src\Investigation\readme.md"
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"PUBLISHER");

            var (argOptions, argErrors) = ParamParser.ParseParams(args);
            if (!string.IsNullOrEmpty(argErrors))
            {
                Console.WriteLine($"{argErrors}");
                Environment.Exit(1);
            }

            DisplayParams(argOptions);

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    ConfigureMassTransit(services, argOptions);
                })
                .Build();

            await host.StartAsync();

            DisplayInfo();

            var publisherEndpoint = host.Services.GetRequiredService<IPublishEndpoint>();

            Console.WriteLine();
            while (true) 
            {
                var line = Console.ReadLine();

                if (line == "exit") 
                {
                    break;
                }
                else if (line == "?")
                {
                    DisplayInfo();
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    await publisherEndpoint.Publish<NewDataAvailable>(new { Text = line });
                }
            }

            publisherEndpoint = null;

            await host.StopAsync();
        }

        private static IServiceCollection ConfigureMassTransit(IServiceCollection services, PublisherParams argOptions)
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
                });
            });

            return services;
        }

        private static void DisplayParams(PublisherParams parms)
        {
            Console.WriteLine($"  --host                   {parms.Host}");
            Console.WriteLine($"  --vhost                  {parms.VHost}");
            Console.WriteLine($"  --name                   {parms.Name}");
            Console.WriteLine($"  --username               {parms.Username}");
            Console.WriteLine($"  --password               {parms.Password}");
            Console.WriteLine($"  --queueName              {parms.QueueName}");
            Console.WriteLine("---------------------------");
        }

        private static void DisplayInfo()
        {
            Console.WriteLine("Enter 'exit' to exit the loop.");
            Console.WriteLine("Enter '?' to repeat this message.");
            Console.WriteLine("---------------------------");
            Console.WriteLine();
        }
    }
}
