using Common;
using System;

namespace Publisher
{
    public class ParamParser 
    {
        public static (PublisherParams Options, string Errors) ParseParams(string[] args)
        {
            try
            {
                var spar = new PublisherParams();

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
