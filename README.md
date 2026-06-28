
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

# Mansari Store

A modern microservices-based backend built with **ASP.NET Core 8**, demonstrating clean architecture, distributed system patterns, and service orchestration.

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Architecture](https://img.shields.io/badge/Microservices-Clean%20Architecture-orange)
![Messaging](https://img.shields.io/badge/RabbitMQ-Event%20Driven-red)
![Gateway](https://img.shields.io/badge/API-Gateway-blue)
![gRPC](https://img.shields.io/badge/Internal-gRPC-green)
![Database](https://img.shields.io/badge/PostgreSQL-blue)
![Cache](https://img.shields.io/badge/Redis-success)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED)

---

## Overview

Mansari Store is a learning-focused microservices project designed to explore modern backend architecture using .NET.

The solution follows a service-oriented architecture where each microservice owns its own domain, persistence, and business logic while communicating through asynchronous messaging and gRPC.

Current services:

- Catalog Service
- Ordering Service
- Basket Service
- User Service
- API Gateway
- Shared Contracts

---

## Architecture

```
                Client
                   │
             REST / HTTP
                   │
            ┌──────────────┐
            │ API Gateway  │
            └──────┬───────┘
                   │
          Internal gRPC Calls
                   │
    ┌────────┬─────────┬────────┬────────┐
    │Catalog │Ordering │Basket  │ User   │
    └────────┴─────────┴────────┴────────┘
                   │
        RabbitMQ Integration Events
                   │
            Event Driven Architecture
```

### Design Principles

- Clean Architecture
- CQRS with MediatR
- Domain-Driven Design (lightweight)
- API Gateway Aggregation
- Internal gRPC Communication
- Event-Driven Messaging
- Eventual Consistency
- Choreography Saga
- Outbox / Inbox Pattern
- Redis Caching
- Polly Resilience Policies

---

## Solution Structure

```
src/

├── Mansari.Store.Catalog/
├── Mansari.Store.Ordering/
├── Mansari.Store.Basket/
├── Mansari.Store.User/
├── Mansari.Store.Gateway/
└── Mansari.Store.Contracts/
```

Each microservice follows the same layered architecture:

```
API
Application
Domain
Infrastructure
```

---

## API Gateway

The Gateway acts as an **Aggregation Layer**, not merely a reverse proxy.

Responsibilities:

- HTTP entry point
- Service orchestration
- gRPC communication
- Response aggregation
- Error translation
- Resiliency

The Gateway intentionally contains **no business logic**.

---

## Communication

External clients communicate using REST.

Internal service-to-service communication uses:

- gRPC
- Protocol Buffers

Asynchronous workflows use:

- RabbitMQ

---

## Technology Stack

- ASP.NET Core 8
- Entity Framework Core
- PostgreSQL
- Redis
- RabbitMQ
- MediatR
- gRPC
- Polly
- Docker Compose

---

## Running the Project

```bash
docker compose up --build
```

Stop:

```bash
docker compose down
```

Remove volumes:

```bash
docker compose down -v
```

---

## Current Status

Implemented:

- Catalog Service
- Ordering Service
- Basket Service
- User Service
- Shared Contracts
- API Gateway (Aggregation)
- Redis Cache
- RabbitMQ Messaging
- CQRS
- Clean Architecture
- Docker Compose

In Progress:

- Gateway Aggregations
- Gateway Resilience
- Observability
- Integration Tests

---

## Project Goals

This project is primarily built to explore practical software architecture concepts, including:

- scalable microservices
- clean boundaries
- distributed communication
- resilient systems
- maintainable backend design

---

## License

MIT 

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
