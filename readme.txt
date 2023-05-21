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
ApiGateway: An API Gateway based on Ocelot package which handler both authentication and authorization*
UserWebAPI: An API for managing users (CRUD) and provide authentication capabilities.
DoorWebAPI: An API for managing doors (CRUD) and permission for doors (CRUD) and provide Unlock functionality (Main Goal)
HistoryWebAPI: An API for persist history of door events and provide an endpoint for querying history
LockHandlerWebAPI: An API which simulate the door or API of the door 

DATABASE
For DBMS I'm using MariaDB for all microservices in this project

MESSAGE BROCKER
I'm using RabbitMQ as message brocker.

ORCHESTRATOR
I'm using docker compose for orchestration but I'm also started using Kubernetes but there was not enough time to finish.
Some of Kubernetes files are encluded in Deploy folder.

LOGGING
For logging I used Serilog to write directly in Elasticsearch which is searchable via Kibana.
But to comply 12 Factorys there are another option which is configurable using environment variable to write
on STDOUT and the collect it using different solution.
