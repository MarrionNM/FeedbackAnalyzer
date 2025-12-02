# AI-Powered Feedback Analysis Platform

## 1. Overview

This project implements an end-to-end feedback analysis system.
Users submit feedback, the backend performs AI-driven analysis (summary, sentiment, priority, tags, next action), and the frontend presents a searchable, filterable, paginated view of all feedback entries.

The goal was to build something small but production-minded—showing real separation of concerns, clean architecture practices, and maintainability, while keeping the implementation straightforward for a take-home assignment.

## 2. Architecture Overview

High-Level Flow

```bash
Angular Frontend
↓
POST /api/feedback
↓
.NET 8 Web API
↓
AI Service → OpenAI GPT-4o-mini + Embeddings
↓
PostgreSQL via EF Core

```

### Layer Breakdown

- Frontend (Angular 18) – Forms, list view, detail modal, filtering, reusable components.

- Backend (.NET 8) – Controllers, services, repositories, DTOs, validation and middleware.

- Database (PostgreSQL) – Relational design using EF Core.

- AI Layer (OpenAI) – Structured JSON + vector embeddings.

- Cross-Cutting – Caching, logging with correlation ID, centralized error handling.

This structure makes the codebase easy to test, reason about, and evolve.

# 3. Backend Design (.NET 8)

#### 3.1 Architectural Approach

I followed a lightweight Clean Architecture pattern:

Controllers: Handle HTTP requests and responses.

Services: Contain business logic and AI orchestration.

Repositories: Abstract database persistence using EF Core.

DTOs: Strict input/output contracts.

Middleware: Error handling + request logging.

Validation: FluentValidation for request models.

This creates clear boundaries between concerns and makes the codebase extensible.

#### 3.2 API Endpoints

Method Endpoint Description
POST /api/feedback Creates feedback and performs AI analysis
GET /api/feedback Paginated list with sentiment/tag filters
GET /api/feedback/{id} Full record including AI analysis

Pagination, validation, filtering, and structured errors are all handled server-side.

#### 3.3 Database (PostgreSQL + EF Core)

A relational database is a natural fit because feedback entries have predictable structure, well-defined fields, and require efficient filtering + pagination.

#### 3.4 Semantic Search (Optional Feature)

I stored embeddings on the Feedback row for demo simplicity.

In production, I would:

- Use pgvector
- Create a separate feedback_vectors table
- Add approximate nearest-neighbor indexing

### 4. AI Integration

#### 4.1 Provider Choice

I selected OpenAI for:

- Strong embedding model support
- Reliable structured JSON responses
- Affordable models (GPT-4o-mini)
- Official .NET SDK that integrates cleanly

#### 4.2 Models Used

- GPT-4o-mini → full analysis (summary, sentiment, tags, nextAction, priority)
- text-embedding-3-large → high-quality semantic vector

#### 4.3 Call Pattern

Per the assignment, the analysis is done synchronously:

```pgsql
POST → Validate → AI call → Save → Return
```

### 4.4 Caching Strategy

I implemented SHA-256 hashing of the input text as a simple caching mechanism.

Why caching over retries?

- Reduces API cost dramatically
- Stateless and easy to test
- Prevents duplicate analysis for identical input
- More value for this scenario than retry logic

### 4.5 Prompting

The prompt enforces:

- Strict JSON response
- Professional, concise tone
- No echoing of PII
- 1–5 short noun tags
- Priority (P0–P3) with consistent logic

Responses are validated before use.

# 5. Frontend (Angular 18)

### 5.1 Pages Implemented

- Submit Feedback
- Feedback List
- Server-side filters (sentiment, tags)
- Pagination
- Priority badges, created date, summary
- Feedback Detail

### 5.2 Reusable Components

- Tag/Badge
- Pagination
- Loader
- Filter controls

These were designed to scale with more views or future feature additions.

### 5.3 Configuration

Backend URL is defined in:

- environment.ts

# 6. Testing Strategy

### Backend

- AI Service Tests
- Mock OpenAI client
- Validate JSON parsing
- Verify error handling for malformed AI responses
- Controller Tests
- Happy path create
- Invalid input → 400 response
- Repository Tests
- Filter logic (sentiment, tags)
- Pagination behavior

### Frontend

- Component Tests
- Submit feedback form
- Filters updating the list

Tests focus on user behavior rather than implementation details.

# 7. Scaling & Production Considerations

### 7.1 AI Processing

Synchronous processing works for this exercise, but for real production:

I would move AI analysis to an async background pipeline:

- POST stores record immediately
- Message goes to SQS or Service Bus
- Worker performs AI calls
- Clients poll or receive websocket updates

Benefits:
-Eliminates long request times

- Handles spikes / rate limits cleanly
- Allows retries without blocking UI

# 8. Diagnostics / Runbook

AI rate limits or timeouts

- Check cache hit rate
- Review OpenAI latency logs
- Reduce prompt size
- Move analysis to background worker
- Database connectivity issues
- Validate connection string

Check Postgres service availability

- Inspect EF Core connection pooling
- Use AsNoTracking() for queries
- General debugging
- Verify environment variables
- Ensure Angular points to correct backend URL
- Check request IDs in logs to trace failures
