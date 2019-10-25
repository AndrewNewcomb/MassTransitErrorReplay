using Common;
using MassTransit;
using System;

namespace Publisher
{
    public class Publisher
    {
        // see     "MassTransitErrorReplay\src\Investigation\readme.md"

        public static void Main(string[] args)
        {
            Console.WriteLine($"PUBLISHER");

            var (argOptions, argErrors) = ParamParser.ParseParams(args);
            if (!string.IsNullOrEmpty(argErrors))
            {
                Console.WriteLine($"{argErrors}");
                Environment.Exit(1);
            }

            DisplayParams(argOptions);

            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(argOptions.Host, argOptions.VHost, argOptions.Name, h =>
                {
                    h.Username(argOptions.Username);
                    h.Password(argOptions.Password);
                });
            });

            bus.Start();

            DisplayInfo();

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
                    bus.Publish<NewDataAvailable>(new { Text = line });
                }
            }

            bus.Stop();
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
