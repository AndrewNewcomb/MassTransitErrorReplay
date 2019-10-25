#!/bin/sh
( sleep 20 ; \
rabbitmq-plugins enable rabbitmq_shovel rabbitmq_shovel_management ; \
rabbitmqctl add_user RmqSvcUser PasswordRmq ; \
#rabbitmqctl delete_vhost / ; \
rabbitmqctl add_vhost vh1 ; \
rabbitmqctl add_vhost vh2 ; \
#rabbitmqctl set_permissions RmqSvcUser ".*" ".*" ".*" ; \
rabbitmqctl set_permissions -p vh1 RmqSvcUser ".*" ".*" ".*" ; \
rabbitmqctl set_permissions -p vh2 RmqSvcUser ".*" ".*" ".*" ; \
) &
exec docker-entrypoint.sh rabbitmq-server $@