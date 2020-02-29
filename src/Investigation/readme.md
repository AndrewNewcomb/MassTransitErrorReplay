# Setting up the container

From the `/docker` folder build the image  
`docker build -t rabbitmq_investigation .`

Run the container  
`docker run -d --name rabbitmq_inv -p 5672:5672 -p 15672:15672 rabbitmq_investigation --restart unless-stopped`

You can then stop it with `docker stop rabbitmq_inv`, and start it again with `docker start rabbitmq_inv`

The Rabbit MQ admin server is available at `http://localhost:15672`

# Project configuration
There are three projects
- MT.Publisher
- MT.Subscriber
- MT.Scheduler

## MT.Publisher configuration
This project is configured via command line parameters. The following defaults are provided.
```
  --host                    localhost
  --vhost                   vh1
  --name                    invPublisher
  --username                RmqSvcUser
  --password                PasswordRmq
  --queueName               test_queue
```

## MT.Subscriber configuration
This project is configured via command line parameters. The following defaults are provided.
```
  --host                    localhost
  --vhost                   vh1
  --name                    invSubscriber
  --username                RmqSvcUser
  --password                PasswordRmq
  --queueName               test_queue
  --disableQueueOutbox      False
  --queueIsExclusive        False
  --disableQueueRetry       False
  --disableQueue2LevelRetry False
  --disableFaultQueue       False
  --disableNoteworthyQueue  False
```

## MT.Scheduler initial setup and configuration
The scheduler was taken from the [MassTransit Sample-Quartz](https://github.com/MassTransit/Sample-Quartz). It uses SQL Server for persistence, but [other databases are supported](https://github.com/quartznet/quartznet/tree/master/database/tables).

The appsettings.json file contains a connection string to the `mt-scheduler` database running on the `(LocalDb)\\MSSQLLocalDB` instance.
You will need to create the database, and then add tables using the [create tables script](https://github.com/MassTransit/Sample-Quartz/blob/master/create_quartz_tables.sql). 

The appsettings.json file also contains the queue configuration values. The default values match the defaults for the publisher and subscriber.

# Running the apps
```
cd src\Investigation\MTExample

start powershell "dotnet run --project .\MT.Publisher\MT.Publisher.csproj"

start powershell "dotnet run --project .\MT.Subscriber\MT.Subscriber.csproj"

start powershell "dotnet run --project .\MT.Subscriber\MT.Scheduler.csproj"
```

You can have multiple subscribers running at the same time. This subscriber will share the same queues as the previous subscriber  
`start powershell "dotnet run --project .\MT.Subscriber\MT.Subscriber.csproj --name invSubscriber2 --disableQueue2LevelRetry"`

This will use a different queue (but share the fault and notification queues)  
`start powershell "dotnet run --project .\MT.Subscriber\MT.Subscriber.csproj --name invSubscriber3 --queueName test_queue2"`


## Publish a message
The text that you enter in the publisher will be received by the subscriber(s). What happens to the message depends on its content.

The normal message flow is   
-- Publisher publishes `NewDataVailable`.   
-- The subscriber receives this and publishes `InitialProcessingCompleted`.   
-- The subscriber receives this and publishes `FinalProcessingCompleted`.  

This normal flow can be amended via the message content. 

If the received text contains the character 'f' it throws an exception. If the text also contains 'ok' it will fix after 2 retries. If the text does not contain 'ok' but does contain '2lr' it will fix after 2 second level retries via the quartz scheduler. This retry behaviour can be suppressed via the command line parameters `disableQueueRetry` and `disableQueue2LevelRetry` respectively. If none of the retries fix the message then it will appear in the error queue, and a fault event will be published which will go to the fault queue. Subscribers can be configured to not consume these fault events via the `disableFaultQueue` command line parameter. 

If the receieved text contains the character 'e' then a `SomethingNoteworthyHappened` event is published. Subscribers can be configured to not consume these events via the `disableNoteworthyQueue` command line parameter. 
 
## Queues
You can see the queue activity via the Rabbit MQ admin server, which is available at `http://localhost:15672`

The command line parameter `--queueName` sets the name of the main queue that is used. The default of `test_queue` gives the queue `test_queue` and its `test_queue_error`.    
The `quartz` queue is used by the quartz scheduler for second level retries.  
The `noteworthy_queue` is published to if the message contains the character 'e'.  
The `fault_queue` is published to if a message fails.



