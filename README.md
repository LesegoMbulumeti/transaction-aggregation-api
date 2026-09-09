# Transaction Aggregation API

A microservice-based system that ingests customer financial transaction data
from multiple external sources, normalizes and categorizes it, and exposes a
rich API for querying and aggregating the results.

Built for the Capitec Software Engineer II project brief.

---

## Architecture

The system is composed of two independently deployable ASP.NET Core services
that communicate over HTTP:
┌──────────────────────┐ HTTP ┌───────────────────────┐
│ MockSources.Service │ <───────────────── │ Aggregation.Api │
│ │ │ │
│ Simulates 3 raw │ │ Pulls from all 3 │
│ external sources: │ │ sources, normalizes, │
│ - Bank Feed │ │ categorizes, stores │
│ - Card Provider │ │ in-memory, exposes │
│ - EFT Transfers │ │ query/aggregation │
│ │ │ endpoints │
└──────────────────────┘ └───────────────────────┘


**Why two services, not one monolith:** this demonstrates genuine service
boundaries and inter-service HTTP communication — closer to how this would
look in a real microservice environment — while staying simple enough to
run and review easily (no message broker required for this scope).

**MockSources.Service** simulates three real-world data sources, each with a
deliberately different raw JSON shape: different field names, different
date/amount formats (e.g. Unix epoch milliseconds vs. `DateTime`, cents vs.
decimal), and different direction conventions (`DR`/`CR` vs `CREDIT`/`DEBIT`).
This mirrors how real bank feed, card processor, and EFT integrations
genuinely differ from one another in practice.

**Aggregation.Api** is the system's core, structured in layers with
dependencies pointing inward (Controllers → Application → Domain).
Infrastructure implements Application's interfaces, but Application never
references Infrastructure directly — this Dependency Inversion is what lets
the core business logic (categorization, aggregation math) be unit tested
without a database or HTTP server:

Controllers/ thin HTTP layer — translates requests to service calls
Application/
Interfaces/ contracts: ITransactionRepository, ISourceClient,
ICategorizationService, IAggregationService
Services/ business logic: CategorizationService, AggregationService
DTOs/ request/response shapes, query option objects
Domain/
Entities/ Transaction — the normalized model all 3 sources map into
Enums/ TransactionCategory
Infrastructure/
SourceClients/ 3 adapters (BankFeed, CardProvider, Eft), each mapping
its source's raw shape into the canonical Transaction
Persistence/ EF Core DbContext (InMemory provider) + repository
BackgroundServices/ IngestionHostedService — runs the full pipeline once
on startup: fetch → categorize → persist

**On startup**, `IngestionHostedService` calls all three source clients,
maps every raw transaction into the canonical `Transaction` entity (Adapter
pattern), runs each one through `CategorizationService` (simple
keyword-matching, first-match-wins), and saves everything via
`ITransactionRepository` into an EF Core InMemory store. One source failing
doesn't stop the others — each is wrapped in its own try/catch.

### Key design decisions worth knowing

- **In-memory storage** — appropriate for this exercise's scope. The
  repository sits behind `ITransactionRepository`, so swapping to a real
  database (SQL Server/Postgres) later is a configuration change, not a
  rewrite.
- **Keyword-based categorization** — fast to build, easy to reason about
  and test. Known limitation: rule order matters if keywords overlap across
  categories. A more advanced version could score matches or use ML-based
  classification.
- **Case-insensitive merchant grouping** (`top-merchants` endpoint) — the
  mock bank feed source uppercases its narratives while the other two don't;
  without case-insensitive grouping, the same real-world merchant would be
  double-counted as two separate entries. This mirrors genuine real-world
  inconsistency between integrated systems.
- **Soft deletes via `DeletedAt`** — no delete endpoint currently exists,
  but the audit fields (`CreatedAt`, `UpdatedAt`, `DeletedAt`) and repository
  filtering already support it without a future schema change.
- **Container networking** — `Aggregation.Api` reaches `MockSources.Service`
  via `http://mock-sources:8080` inside Docker (Docker Compose's built-in
  service-name DNS), but via `http://localhost:5016` when both run locally
  with the .NET CLI. This is handled by an environment variable override in
  `docker-compose.yml` (`MockSources__BaseUrl`), so no code or config file
  changes are needed between the two environments.

### Known limitations / future improvements

- `GetTotalsByMonthAsync` and `GetTopMerchantsAsync` pull rows into memory
  before grouping, since EF Core's InMemory provider can't translate
  `DateTime` component grouping or case-insensitive string grouping into its
  query engine. A real SQL-backed provider could group directly in the
  database query instead.
- `depends_on` in `docker-compose.yml` waits for the container to *start*,
  not for the app inside it to be *ready* — fine here since ingestion has
  per-source error handling, but a stricter setup would add healthcheck-based
  startup ordering.
- No authentication/authorization or persistent storage — both out of scope
  for this exercise.

---

## API Reference

### Transactions — `Aggregation.Api`

| Method | Route | Description |
|---|---|---|
| GET | `/api/transactions` | List transactions. Query params: `category`, `fromDate`, `toDate`, `accountId`, `sourceSystem`, `page`, `pageSize` |
| GET | `/api/transactions/{id}` | Get a single transaction by ID |

### Aggregations — `Aggregation.Api`

| Method | Route | Description |
|---|---|---|
| GET | `/api/aggregations/summary` | Total transactions, total spend, total income, net, average |
| GET | `/api/aggregations/by-category` | Totals grouped by category |
| GET | `/api/aggregations/by-month` | Totals grouped by year/month, for trend analysis |
| GET | `/api/aggregations/top-merchants?top=10` | Top N merchants by total spend |

### Mock Sources — `MockSources.Service`

| Method | Route | Description |
|---|---|---|
| GET | `/mock/bank-feed` | Raw bank feed transactions |
| GET | `/mock/card-provider` | Raw card provider transactions |
| GET | `/mock/eft-transfers` | Raw EFT transfer transactions |

Both services expose Swagger/OpenAPI docs at `/openapi/v1.json` in
Development mode.

---

## Getting Started

### Prerequisites

- **.NET 10 SDK** (for local CLI option) — verify with `dotnet --version`
- **Docker Desktop** (for Docker option) — must be running before any
  `docker` command

### Option A — Docker Compose (recommended, matches production topology)

Build and run both services together:

```bash
docker compose up --build
```

Once up, `Aggregation.Api` automatically ingests from `MockSources.Service`
on startup — watch the logs for `Ingestion complete. Total transactions
ingested: 100`.

- `Aggregation.Api` → http://localhost:5279
- `MockSources.Service` → http://localhost:5016

Try it:

```bash
curl http://localhost:5279/api/aggregations/summary
```

Stop everything:

```bash
# Ctrl+C, then:
docker compose down
```

### Option B — Running locally with the .NET CLI

**Build the whole solution:**

```bash
dotnet build
```

**Run** — in **one terminal**, start the mock sources service:

```bash
dotnet run --project src/MockSources.Service
```

In a **second terminal**, start the aggregation API:

```bash
dotnet run --project src/Aggregation.Api
```

You'll see the same ingestion log lines, then:
- `MockSources.Service` → http://localhost:5016
- `Aggregation.Api` → http://localhost:5279

(`Aggregation.Api` reads `MockSources:BaseUrl` from
`src/Aggregation.Api/appsettings.json`, defaulting to
`http://localhost:5016` — adjust if `MockSources.Service` starts on a
different port for you.)

### Running tests

```bash
dotnet test
```

Currently 16 tests covering:
- `CategorizationService` — keyword matching, case-insensitivity, fallback behavior
- `AggregationService` — summary math, category grouping, case-insensitive merchant
  deduplication, top-N limiting

Tests use EF Core's InMemory provider with a uniquely-named database per test,
so there's no shared state between test runs.

---

## Troubleshooting

- **First-time `docker compose up --build` is slow** — it downloads the
  .NET SDK and ASP.NET Core runtime base images (~700MB total). On a slow
  or unstable connection, a pull may need retrying; once cached locally,
  subsequent builds take seconds.
- **"Port is already allocated"** — check for a leftover container from a
  previous run with `docker ps`, then `docker stop`/`docker rm` it, or run
  `docker compose down` before starting again.