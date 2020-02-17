# Setting up the container

From the `/docker` folder build the image
```
docker build -t rabbitmq_investigation .
```

## Run the container
```
docker run -d --name rabbitmq_inv -p 5672:5672 -p 15672:15672 rabbitmq_investigation --restart unless-stopped
```

You can then stop it with `docker stop rabbitmq_inv`, and start it again with `docker start rabbitmq_inv`

# Rabbit MQ admin server is
`http://localhost:15672`

# Publisher and subscriber

Messages can be published and received via the MT.Publisher and MT.Subscriber projects.

## MT.Publisher command line parameters
  --host                    localhost
  --vhost                   vh1
  --name                    invPublisher
  --username                RmqSvcUser
  --password                PasswordRmq
  --queueName               test_queue

## MT.Subscriber command line parameters
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

## start the publisher and subscriber(s)
cd src\Investigation\MTExample

start powershell "dotnet run --project .\MT.Publisher\MT.Publisher.csproj"

** these two share the same main queue **
start powershell "dotnet run --project .\MT.Subscriber\MT.Subscriber.csproj --name invSubscriber1"
start powershell "dotnet run --project .\MT.Subscriber\MT.Subscriber.csproj --name invSubscriber2 --disableQueue2LevelRetry"

** this is a different main queue **
start powershell "dotnet run --project .\MT.Subscriber\MT.Subscriber.csproj --name invSubscriber3 --queueName test_queue3"
