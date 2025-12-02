AI-Powered Feedback Analysis Platform

Full-Stack Implementation Using .NET 8, Angular 18, PostgreSQL, and OpenAI GPT-4o-mini

1. Overview

This project implements a complete end-to-end feedback analysis system.
Users submit feedback, the backend performs AI-driven analysis (summary, sentiment, tags, priority, next action), and the frontend displays the feedback list with filtering, pagination, and detail views.

The solution is intentionally production-minded, focusing on clean architecture, testability, modularity, and resilience while remaining simple enough for a take-home exercise.

2. Architecture Overview
   High-Level Flow
   Frontend (Angular)
   ↓
   POST /api/feedback
   ↓
   Backend (.NET 8 API)
   ↓
   AI Service → OpenAI GPT-4o-mini + Embeddings
   ↓
   PostgreSQL (EF Core)

Layers

Presentation: Angular 18 application

Business Logic: .NET 8 Web API with clean separation of concerns

Data Access: PostgreSQL using EF Core

AI Analysis: OpenAI GPT-4o-mini + Embedding model

Cross-cutting Concerns: Validation, error middleware, logging, caching

3. Backend Design (.NET 8)
   3.1 Architectural Approach

I used a Clean Architecture style for separation of concerns:

Controllers: HTTP input/output

Services: Business logic + AI integration

Repositories: DB access

DTOs & Payloads: Strong typing for requests/responses

Validation: FluentValidation

Error Middleware: Consistent error shape

Logging: Correlation ID + request metadata

3.2 Endpoints Implemented
Method Endpoint Description
POST /api/feedback Create feedback + perform synchronous AI analysis
GET /api/feedback Paginated list with sentiment + tag filters
GET /api/feedback/{id} Full record including AI analysis
3.3 Database (PostgreSQL + EF Core)

I used a relational model because feedback entries have structured, predictable fields and fit well with SQL querying, filtering, and pagination.

Core Model:

3.4 Semantic Search (Optional Enhancement)

For demonstration, I generated embeddings using text-embedding-3-large and stored them directly on the feedback row.

In a full production system, I would move embeddings to a separate vector table or a vector DB (like pgvector) to support scalable ANN search.

4. AI Integration
   4.1 Provider

I selected OpenAI due to:

Mature embeddings support

Reliable JSON responses

Cost-effective models (GPT-4o-mini)

Clean .NET SDK support

4.2 AI Models

GPT-4o-mini → structured analysis (summary, tags, sentiment, nextAction, priority)

text-embedding-3-large → semantic vector generation

4.3 Call Pattern

As required, analysis is done synchronously inside the POST request:

POST /api/feedback → AI call → Save → Return

4.4 Caching

I implemented SHA-256 text hashing to cache repeat analyses.

Rationale:

Simple, stateless

More production-minded than retries

Reduces API costs significantly

Easy to maintain

4.5 Prompt Design

Prompts instruct the model to return strict JSON with fields.
Ask model not to echo PII

Force concise and professional language

Validate & sanitize the resulting JSON

5. Frontend (Angular 18)
   5.1 Pages

Submit Feedback Page
Feedback List Page
Pagination
Server-side filters: sentiment, tags
Summaries, created date, priority badges
Feedback Detail View

5.2 Reusable Components

Tag/Badge Component

Loader Component

Filter Component

5.3 Config

Angular reads backend URL from environment:

environment.ts
environment.prod.ts

5.4 State Management

Component-level state (signals) + service caching.

6. Testing Strategy
   Backend

AI Service Tests:
Mock OpenAI client → validate JSON parsing, error handling

Controller Tests:
Successful create + failing validation path

Repository Test:
Query behavior with filters

Frontend

Component test:
Submit Feedback form
Validate loading → success → error flow

Filter component test:
Tag + sentiment updates list

7. Scalability & Production Considerations
   7.1 AI Processing

For production I would move AI analysis to an async background worker:

Use AWS SQS (simple, scalable, inexpensive)

POST request writes record first

Worker performs analysis & updates row

Clients poll or receive websockets updates

This prevents long request times and avoids hitting rate limits.

7.2 Semantic Search

For production:

Move vectors to separate pgvector table

My demo stores vectors in the row for simplicity.

8. Diagnostics & Runbook
   AI Rate Limit / Timeout

Check cache hit rate

Reduce prompt size

Log model latency

Implement SQS worker (future improvement)

Database Issues

Check EF Core connection pooling

Ensure proper AsNoTracking() usage

Validate connection string

Restart Postgres container

General Connectivity

Confirm environment variables

Check backend URL in Angular

Validate Postgres network availability

9. Trade-Off Decisions
   Decision Rationale
   .NET 8 + Clean Architecture Clear separation, testability, maintainability
   PostgreSQL Predictable schema, strong filtering & pagination
   Synchronous AI processing Required by assignment
   Caching instead of retries Simpler, more cost-effective
   Embeddings stored inline Demo simplicity; would use vector DB in prod
   Angular 18 Reusable UI, TypeScript, strong component ecosystem
