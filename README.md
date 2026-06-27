
# Mansari Store - Microservices Book Ordering System

A sample **microservices-based store backend** built with ASP.NET Core and modern distributed system patterns.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-WebAPI-blue)
![Architecture](https://img.shields.io/badge/Architecture-Microservices-orange)
![Messaging](https://img.shields.io/badge/Messaging-RabbitMQ-red)
![Cache](https://img.shields.io/badge/Cache-Redis-green)
![Database](https://img.shields.io/badge/Database-PostgreSQL-blue)
![License](https://img.shields.io/badge/License-MIT-yellow)
![Docker](https://img.shields.io/badge/Container-Docker-blue)
![CQRS](https://img.shields.io/badge/Pattern-CQRS-lightgrey)

# Mansari Store - Microservices Book Ordering System

Mansari Store is a simple book ordering system implemented using a microservices architecture.

The solution contains two business services:

- Catalog Service (manages books and stock)
- Ordering Service (manages orders)

The project demonstrates:

- Clean Architecture
- CQRS with MediatR
- Redis caching
- RabbitMQ asynchronous messaging
- Eventual Consistency
- Choreography-based Saga
- Outbox / Inbox pattern
- Polly retry and resiliency
- Docker Compose orchestration
- GitHub Actions CI

------------------------------------------------------------
SOLUTION STRUCTURE
------------------------------------------------------------

src/
  Mansari.Store.Catalog/
    Mansari.Store.Catalog.Api/
    Mansari.Store.Catalog.Application/
    Mansari.Store.Catalog.Domain/
    Mansari.Store.Catalog.Infrastructure/

  Mansari.Store.Ordering/
    Mansari.Store.Ordering.Api/
    Mansari.Store.Ordering.Application/
    Mansari.Store.Ordering.Domain/
    Mansari.Store.Ordering.Infrastructure/

  Mansari.Store.Contracts/
    Catalog/
    Order/

  Mansari.Store.Gateway/

------------------------------------------------------------
1) CATALOG SERVICE WITH REDIS CACHE
------------------------------------------------------------

The Catalog Service provides full CRUD operations for Book entity:

- Id
- Title
- Author
- Stock
- Price

GetBookById flow:

1. Check Redis cache first.
2. If cache miss occurs, read from database.
3. Store result in Redis for 5 minutes.
4. On update or delete, invalidate cache.

Redis implementation file:
src/Mansari.Store.Catalog/Mansari.Store.Catalog.Infrastructure/Caching/RedisBookCacheService.cs

Cache invalidation is performed after update/delete:

await _cache.RemoveAsync(book.Id, cancellationToken);

Redis is configured in docker-compose.

------------------------------------------------------------
2) ORDER CREATION WITH EVENTUAL CONSISTENCY
------------------------------------------------------------

Order flow:

1. Client sends BookId and Quantity to Ordering Service.
2. Ordering Service creates order with status = Pending.
3. Ordering Service publishes OrderCreatedEvent.
4. Catalog Service consumes OrderCreatedEvent.
5. Catalog checks stock.
6. If stock is sufficient:
      - decrease stock
      - publish StockReservedEvent
7. If stock is insufficient:
      - publish StockFailedEvent
8. Ordering Service consumes result event:
      - StockReservedEvent => status = Confirmed
      - StockFailedEvent => status = Failed

This implements a choreography-based saga using RabbitMQ.

Contracts are defined in:

src/Mansari.Store.Contracts/

------------------------------------------------------------
3) RESILIENCY
------------------------------------------------------------

The system implements multiple resiliency techniques to ensure reliable message processing.

Durable Messaging
RabbitMQ exchanges and queues are declared as durable, ensuring messages survive broker restarts.

Messages are also published as persistent.

Relevant implementation:

src/Mansari.Store.Catalog/Mansari.Store.Catalog.Infrastructure/Messaging/Consumers/OrderCreatedEventConsumer.cs

The consumer uses manual acknowledgements:

autoAck = false
BasicAck only after successful processing
BasicNack(requeue: true) on transient failures
This ensures that if the Catalog service is down, RabbitMQ keeps the message and delivers it again when the service becomes available.

Inbox Pattern (Idempotency)
Consumers use an Inbox table to guarantee idempotent message processing.

Example:

src/Mansari.Store.Catalog/Mansari.Store.Catalog.Infrastructure/Persistence/Inbox/InboxMessage.cs

Each incoming event is recorded and processed only once.

Outbox Pattern (Reliable Event Publishing)
Both services implement the Outbox Pattern to guarantee reliable event publishing.

Events are stored in the database and later published by a background processor.

Implementation:

src/Mansari.Store.Catalog/Mansari.Store.Catalog.Infrastructure/Persistence/Outbox/OutboxProcessor.cs

Retry and Fault Handling with Polly
The OutboxProcessor uses Polly to handle transient failures when publishing messages.

Implemented strategies:

Retry with exponential backoff
Circuit Breaker
Failed messages are retried automatically based on NextRetryOnUtc.

This ensures reliable message delivery even during temporary infrastructure failures.

------------------------------------------------------------
4) MEDIATR
------------------------------------------------------------

MediatR is used for:

- CreateBookCommand
- CreateOrderCommand
- Queries (GetBookById, GetAllBooks)

Handlers are located in Application layer under Commands and Queries folders.

------------------------------------------------------------
5) CLEAN ARCHITECTURE
------------------------------------------------------------

Each microservice contains:

- API layer
- Application layer
- Domain layer
- Infrastructure layer

API:
Controllers and startup configuration.

Application:
Use cases, commands, queries, DTOs, interfaces.

Domain:
Entities, value objects, business rules.

Infrastructure:
Database, Redis, RabbitMQ, Outbox, Inbox implementations.

------------------------------------------------------------
6) DOCKERIZATION
------------------------------------------------------------

The root contains docker-compose.yml.

Services included:

- catalog-api
- ordering-api
- gateway
- catalog-db (PostgreSQL)
- ordering-db (PostgreSQL)
- redis
- rabbitmq

Run the system:

docker compose up --build

Stop the system:

docker compose down

Remove volumes:

docker compose down -v

------------------------------------------------------------
DEFAULT PORTS
------------------------------------------------------------

Gateway:            http://localhost:5000
Catalog API:        http://localhost:5001
Ordering API:       http://localhost:5002
RabbitMQ UI:        http://localhost:15672

RabbitMQ credentials:
username: guest
password: guest

PostgreSQL credentials:
username: postgres
password: postgres

------------------------------------------------------------
EXAMPLE FLOW
------------------------------------------------------------

1) Create Book

POST http://localhost:5001/api/books

{
  "title": "Clean Architecture",
  "author": "Robert C. Martin",
  "price": 45.50,
  "currency": "USD",
  "stock": 10
}

2) Get Book

GET http://localhost:5001/api/books/{bookId}

First request loads from database and caches result.
Subsequent requests are served from Redis.

3) Create Order

POST http://localhost:5002/api/orders

{
  "bookId": "{bookId}",
  "quantity": 2
}

Order is created as Pending.
Events are exchanged via RabbitMQ.
Final status becomes Confirmed or Failed.

------------------------------------------------------------
CONSISTENCY MODEL
------------------------------------------------------------

The system uses Eventual Consistency.

Ordering Service does not call Catalog synchronously.
All communication is done through integration events.

------------------------------------------------------------
CI
------------------------------------------------------------

GitHub Actions workflow:

.github/workflows/ci.yml

Pipeline steps:

- dotnet restore
- dotnet build
- dotnet test

------------------------------------------------------------
NOTES
------------------------------------------------------------

Authentication and Authorization are intentionally not implemented because they are outside the scope of the technical task.

No frontend is included.
