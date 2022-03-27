using Common;
using System;

namespace Subscriber
{
    public class ParamParser 
    {
        public static (SubscriberParams Options, string Errors) ParseParams(string[] args)
        {
            try
            {
                var spar = new SubscriberParams();

                var ph = new ParamHelpers(args);

                var i = 0;
                while (i < args.Length)
                {
                    var arg = args[i];
                    var lowerArg = args[i].ToLowerInvariant();

                    i += 1;

                    if (lowerArg == "--host")
                    {
                        ph.CheckArgs(arg, i, 1);
                        (i, spar.Host) = ph.ReadString(arg, i);
                    }
                    else if (lowerArg == "--vhost")
                    {
                        ph.CheckArgs(arg, i, 1);
                        (i, spar.VHost) = ph.ReadString(arg, i);
                    }
                    else if (lowerArg == "--name")
                    {
                        ph.CheckArgs(arg, i, 1);
                        (i, spar.Name) = ph.ReadString(arg, i);
                    }
                    else if (lowerArg == "--username")
                    {
                        ph.CheckArgs(arg, i, 1);
                        (i, spar.Username) = ph.ReadString(arg, i);
                    }
                    else if (lowerArg == "--password")
                    {
                        ph.CheckArgs(arg, i, 1);
                        (i, spar.Password) = ph.ReadString(arg, i);
                    }
                    else if (lowerArg == "--queuename")
                    {
                        ph.CheckArgs(arg, i, 1);
                        (i, spar.QueueName) = ph.ReadString(arg, i);
                    }
                    else if (lowerArg == "--disablequeueoutbox")
                    {
                        (i, spar.DisableQueueOutbox) = ph.SetSwitch(i);
                    }
                    else if (lowerArg == "--queueisexclusive")
                    {
                        (i, spar.QueueIsExclusive) = ph.SetSwitch(i);
                    }
                    else if (lowerArg == "--singleactiveconsumer")
                    {
                        (i, spar.SingleActiveConsumer) = ph.SetSwitch(i);
                    }
                    else if (lowerArg == "--disablequeueretry")
                    {
                        (i, spar.DisableQueueRetry) = ph.SetSwitch(i);
                    }
                    else if (lowerArg == "--disablequeue2levelretry")
                    {
                        (i, spar.DisableQueue2LevelRetry) = ph.SetSwitch(i);
                    }
                    else if (lowerArg == "--disablefaultqueue")
                    {
                        (i, spar.DisableFaultQueue) = ph.SetSwitch(i);
                    }
                    else if (lowerArg == "--disablenoteworthyqueue")
                    {
                        (i, spar.DisableNoteworthyQueue) = ph.SetSwitch(i);
                    }
                    else
                    {
                        throw new ArgumentException($"Unknown argument '{arg}'");
                    }
                }

                return (spar, string.Empty);
            }
            catch (ArgumentException ex)
            {
                return (null, ex.Message);
            }
        }
    }
}
