#Setting up the container

##From the `/docker` folder build the image
```
docker build -t rabbitmq_investigation .
```

##Run the container
```
docker run -d --name rabbitmq_inv -p 5672:5672 -p 15672:15672 rabbitmq_investigation --restart unless-stopped
```

You can then stop it with `docker stop rabbitmq_inv`, and start it again with `docker start rabbitmq_inv`

#Rabbit MQ admin server is
`http://localhost:15672`

# Publisher and subscriber
## publisher command line parameters
  --host                    localhost
  --vhost                   vh1
  --name                    invPublisher
  --username                RmqSvcUser
  --password                PasswordRmq
  --queueName               test_queue

## subscriber command line parameters
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

## start publisher and subscribers

cd C:\Projects\Playpit\MassTransit\MassTransitErrorReplay\src\Investigation\MTExample

Start powershell ".\MT.Publisher\bin\debug\netcoreapp3.0\MT.Publisher"

** these two share the same main queue **
Start powershell ".\MT.Subscriber\bin\debug\netcoreapp3.0\MT.Subscriber --name invSubscriber1"
Start powershell ".\MT.Subscriber\bin\debug\netcoreapp3.0\MT.Subscriber --name invSubscriber2 --disableQueue2LevelRetry"

** this is a different main queue **
Start powershell ".\MT.Subscriber\bin\debug\netcoreapp3.0\MT.Subscriber --name invSubscriber3 --queueName test_queue3"
