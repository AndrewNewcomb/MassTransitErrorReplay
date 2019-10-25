using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class BaseParams
    {
        public string Host { get; set; } = "localhost";
        public string VHost { get; set; } = "vh1";
        public string Username { get; set; } = "RmqSvcUser";
        public string Password { get; set; } = "PasswordRmq";
        public string QueueName { get; set; } = "test_queue";
    }
}
