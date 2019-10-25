FROM rabbitmq:3-management

# Add initilisation shell script
ADD init.sh /init.sh

# Script needs permissions to be executable
RUN chmod +x /init.sh

ENTRYPOINT ["sh","/init.sh"]