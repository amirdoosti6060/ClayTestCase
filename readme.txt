Clay Test Case Project

BACKGROUND
In our office we have two doors: the main entrance and the storage room. Every employee should
have access to the main entrance, but only a few people, like the director or the office manager,
should have access to the storage room where all sorts of valuable things are stored.
Currently, the doors can only be opened if you present a physical object (that we call a tag) to the lock.
However, we want to let users open the doors remotely. Maybe the office manager is sick and she
wants to let someone into the storage room, or an employee forgot their tag and they need to get into
the office.
We would also like certain users to be able to view the history of events: when did someone enter? Or
failed to enter? This feature should not be available to everyone.

TAKE INTO ACCOUNT
We have described just one office with 2 doors, but there may be offices with more doors and a
different number of users. The back-end should therefore be built with scalability in mind. It should also
be ready to support both mobile and web consumers. We strive to use continuous delivery, so the
codebase should be ready to support integration in such a tool.

SOLUTION
-----------------------------
ARCHITECTURE
This project is designed based on microservice architecte and contains following microservices:
ApiGateway: An API Gateway based on Ocelot package which handles both authentication and authorization*
UserWebAPI: An API for managing users and provide authentication capabilities.
DoorWebAPI: An API for managing doors and permission for doors and provide Unlock functionality (Main Goal)
HistoryWebAPI: An API for persisting history of door events and provide an endpoint for querying history
LockHandlerWebAPI: An API which simulates the door or API of the door 

DATABASE
For keeping simplicity MariaDB has been used for all microservices but different databases

MESSAGE BROCKER
RabbitMQ has been used as message brocker.

ORCHESTRATOR
Docker compose has been used for orchestration but Kubernetes was in progress to be used.
Some of Kubernetes files prepared so far are included in Deploy folder. 
If it's needed, it can be accomplished.

LOGGING
For logging I used Serilog and provide options to write directly in Elasticsearch which is searchable via Kibana or
writing directly to STDOUT and then collect it and visualize it using Loki/Graffana.
These options are configurable via environment variables of each microservices.

