# Bank Loan Management System

A backend API for managing bank loan applications, built with .NET 10 and Clean Architecture. The system handles customer registration, loan campaign management, credit eligibility evaluation, application processing, and officer-level approval workflows.

This project was built as a learning exercise to practice designing and implementing a real-world backend system end to end — from domain modeling and authentication to messaging, containerization, and CI/CD.

---

## Table of Contents

- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Features](#features)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Docker](#docker)
- [CI/CD](#cicd)

---

## Architecture

The project follows **Clean Architecture** principles with four layers:

```
Api  -->  Application  -->  Domain
 |            |
 +------> Infrastructure
```

- **Domain** — Entities, enums, domain events, and repository interfaces. Zero external dependencies.
- **Application** — Business services, DTOs, validators, and mapping extensions. Depends only on Domain.
- **Infrastructure** — EF Core DbContext, repository implementations, Identity configuration, MassTransit consumers/publishers, and data seeding. Depends on Domain and Application.
- **Api** — Minimal API endpoints, middleware, filters, and service registration. The composition root.

Key patterns used:

- **Repository Pattern** with specific repositories per aggregate (not a generic base) for meaningful method signatures
- **Unit of Work** for coordinating transactions across repositories
- **Domain Events** published through MassTransit over RabbitMQ
- **JWT Authentication** with refresh token rotation
- **FluentValidation** with endpoint filters for automatic request validation

---

## Technology Stack

| Category          | Technology                                  |
|-------------------|---------------------------------------------|
| Framework         | .NET 10, ASP.NET Core Minimal APIs          |
| Database          | PostgreSQL 17                               |
| ORM               | Entity Framework Core 10 (Npgsql)           |
| Authentication    | ASP.NET Core Identity, JWT Bearer Tokens    |
| Messaging         | RabbitMQ 4 with MassTransit 8               |
| Validation        | FluentValidation 12                         |
| API Documentation | Swashbuckle (Swagger/OpenAPI)               |
| Testing           | xUnit, Moq, FluentAssertions               |
| Containerization  | Docker, Docker Compose                      |
| CI/CD             | GitHub Actions                              |

---

## Project Structure

```
BankLoanManagement/
├── src/
│   ├── BankLoan.Api/
│   │   ├── Endpoints/          # Minimal API endpoint groups
│   │   ├── Filters/            # Validation endpoint filter
│   │   ├── Middleware/         # Exception handling, request logging
│   │   └── Program.cs          # Composition root
│   │
│   ├── BankLoan.Application/
│   │   ├── Common/             # Shared types (EligibilityResult)
│   │   ├── DTOs/               # Request/response data transfer objects
│   │   ├── Mappings/           # Entity-to-DTO extension methods
│   │   ├── Services/           # Business logic implementations
│   │   └── Validators/         # FluentValidation request validators
│   │
│   ├── BankLoan.Domain/
│   │   ├── Entities/           # AppUser, Customer, LoanApplication, LoanCampaign, LoanApprovalHistory
│   │   ├── Enums/              # LoanStatus, PaymentFrequency, UserRole
│   │   ├── Events/             # Domain events for MassTransit
│   │   └── Interfaces/         # Repository and service contracts
│   │
│   ├── BankLoan.Infrastructure/
│   │   ├── Data/               # EF Core configurations and seed data
│   │   ├── Messaging/          # MassTransit consumers and publishers
│   │   ├── Migrations/         # EF Core database migrations
│   │   └── Repositories/       # Repository and UnitOfWork implementations
│   │
│   └── BankLoan.Shared/        # Cross-cutting constants and extensions
│
├── tests/
│   └── BankLoan.UnitTests/     # Unit tests for business services
│
├── Dockerfile                  # Multi-stage build
├── docker-compose.yml          # API + PostgreSQL + RabbitMQ
└── .github/workflows/ci.yml   # Build, test, and Docker image verification
```

---

## Features

### Authentication and Authorization
- User registration and login with ASP.NET Core Identity
- JWT access tokens with configurable expiry
- Refresh token rotation for session continuity
- Role-based access control: Admin, LoanOfficer, Customer

### Customer Management
- Customer profile creation linked to Identity user accounts
- National ID uniqueness enforcement
- Credit score and monthly income tracking

### Loan Campaigns
- Admins can create and manage loan campaigns with configurable parameters: interest rate, credit score threshold, loan amount range, term length, and payment frequency
- Campaign activation and deactivation (soft delete)
- Date-bound campaign validity

### Loan Applications
- Customers apply for loans under active campaigns
- Eligibility engine evaluates four rules before acceptance:
  - Campaign is active and within its date range
  - Requested amount falls within campaign min/max bounds
  - Customer credit score meets campaign minimum
  - No duplicate pending application exists for the same campaign
- All rejection reasons are collected (not short-circuited)
- Monthly installment is calculated at submission time
- Credit score is snapshotted at the time of application

### Approval Workflow
- Loan officers review applications in "UnderReview" status
- Approve or reject with reason tracking
- Decision timestamp and officer ID recorded
- Approval history maintained for audit

### Event-Driven Messaging
- Domain events published to RabbitMQ via MassTransit on loan submission, approval, and rejection
- Dedicated consumers handle each event type for downstream processing (notifications, auditing)

### Cross-Cutting Concerns
- Global exception middleware with structured error responses
- Request logging middleware with response time measurement
- FluentValidation endpoint filter for automatic request body validation

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### Run with Docker Compose

This is the simplest way to start everything (API, PostgreSQL, RabbitMQ):

```bash
docker-compose up --build
```

The API will be available at `http://localhost:5000`. Swagger UI is accessible in development mode.

### Run Locally (without Docker)

1. Start PostgreSQL and RabbitMQ on your machine (or via Docker):

```bash
docker run -d --name postgres -e POSTGRES_DB=bankloandb -e POSTGRES_PASSWORD=password123 -p 5433:5432 postgres:17-alpine
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:4-management-alpine
```

2. Apply migrations and run the API:

```bash
cd src/BankLoan.Api
dotnet ef database update --project ../BankLoan.Infrastructure
dotnet run
```

The database is seeded automatically on startup with roles, sample users, customers, and loan campaigns.

### Seed Data

The seeder creates the following on first run:

- **Roles:** Admin, LoanOfficer, Customer
- **Users:** An admin account and a loan officer account
- **Customers:** 8 customers with varying credit scores and income levels
- **Campaigns:** 5 loan campaigns with different interest rates, terms, and eligibility criteria

---

## API Endpoints

### Authentication
| Method | Endpoint              | Description              | Auth     |
|--------|-----------------------|--------------------------|----------|
| POST   | /api/auth/register    | Register a new user      | Public   |
| POST   | /api/auth/login       | Login and receive tokens | Public   |
| POST   | /api/auth/refresh     | Refresh an access token  | Public   |

### Customers
| Method | Endpoint              | Description              | Auth     |
|--------|-----------------------|--------------------------|----------|
| GET    | /api/customers        | List all customers       | Required |
| GET    | /api/customers/{id}   | Get customer by ID       | Required |

### Loan Campaigns
| Method | Endpoint              | Description              | Auth     |
|--------|-----------------------|--------------------------|----------|
| GET    | /api/campaigns        | List all campaigns       | Required |
| GET    | /api/campaigns/{id}   | Get campaign by ID       | Required |
| GET    | /api/campaigns/active | List active campaigns    | Required |
| POST   | /api/campaigns        | Create a new campaign    | Admin    |

### Loan Applications
| Method | Endpoint              | Description              | Auth     |
|--------|-----------------------|--------------------------|----------|
| GET    | /api/applications     | List all applications    | Required |
| GET    | /api/applications/{id}| Get application by ID    | Required |
| POST   | /api/applications     | Submit a loan application| Required |

### Loan Approvals
| Method | Endpoint                        | Description              | Auth              |
|--------|---------------------------------|--------------------------|-------------------|
| POST   | /api/applications/{id}/approve  | Approve an application   | LoanOfficer/Admin |
| POST   | /api/applications/{id}/reject   | Reject an application    | LoanOfficer/Admin |

---

## Testing

The project includes unit tests covering the core business logic:

- **LoanEligibilityServiceTests** (14 tests) — Campaign validity, amount bounds, credit score checks, duplicate detection
- **CreditScoreServiceTests** (5 tests) — Credit score retrieval and edge cases
- **LoanApplicationServiceTests** (7 tests) — Application creation, eligibility integration, installment calculation

Run all tests:

```bash
dotnet test
```

The tests use Moq for isolating dependencies and FluentAssertions for readable assertions.

---

## Docker

### Dockerfile

The API uses a multi-stage build:

1. **Build stage** — Restores, builds, and publishes using the .NET SDK image
2. **Runtime stage** — Runs the published output on the lightweight ASP.NET runtime image

### Docker Compose Services

| Service       | Image                          | Ports        |
|---------------|--------------------------------|--------------|
| bankloan-api  | Built from Dockerfile          | 5000:8080    |
| postgres      | postgres:17-alpine             | 5433:5432    |
| rabbitmq      | rabbitmq:4-management-alpine   | 5672, 15672  |

RabbitMQ Management UI is accessible at `http://localhost:15672` (guest/guest).

---

## CI/CD

GitHub Actions runs on every push to `main`/`develop` and on pull requests to `main`.

**Pipeline stages:**

1. **Build and Test** — Restores, builds, and runs unit tests against a PostgreSQL and RabbitMQ service container
2. **Docker Build** — Builds the Docker image and tags it with the commit SHA (runs only after tests pass)

---

## Design Decisions

**Specific repositories over a generic base** — Each repository interface (ICustomerRepository, ILoanApplicationRepository, etc.) defines methods relevant to its aggregate. This makes the API surface explicit and allows business-specific query methods like `GetActiveCampaignsAsync` without type-casting or generic workarounds.

**Unit of Work for transactions** — The UnitOfWork coordinates multiple repository operations under a single database transaction, ensuring data consistency when a loan application involves updates to multiple tables.

**Extension methods for mapping** — Entity-to-DTO conversion uses static extension methods (`ToDto()`) rather than a mapping library. This keeps mappings explicit, easy to debug, and free of reflection overhead.

**Eligibility engine collects all reasons** — When a loan application fails eligibility, all failing rules are evaluated and returned rather than stopping at the first failure. This gives the applicant complete feedback in a single response.

**Credit score snapshot** — The customer's credit score is recorded on the loan application at submission time, preserving the score that was used for the eligibility decision even if the customer's score changes later.
