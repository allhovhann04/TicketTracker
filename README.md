# TicketTracker

A small Kanban-style ticket tracker: React + TypeScript SPA, ASP.NET Core 8 Web API, PostgreSQL.

## Structure

- `backend/` — ASP.NET Core 8 Web API (`TicketTracker.Api`) + xUnit tests (`TicketTracker.Api.Tests`)
- `frontend/` — React + TypeScript (Vite) SPA
- `docker-compose.yml` — orchestrates backend, frontend, and database containers
- `.env.example` — template for local environment configuration

## Prerequisites

- Docker and Docker Compose. That's it — no host-installed .NET SDK, Node.js, or PostgreSQL is required to run the application.
- (Optional, only for local development outside Docker) [.NET 8 SDK](https://dotnet.microsoft.com/download) and [Node.js 20+](https://nodejs.org).

## Running the application

1. Copy the environment template and fill in real values (at minimum, set a real `Jwt__Secret` and SMTP credentials if you want verification emails to actually send):
   ```
   cp .env.example .env
   ```
2. From the repository root:
   ```
   docker compose up --build
   ```
   This is the **only** command needed from a clean checkout. It builds and starts three containers:
   - `db` — PostgreSQL 16
   - `backend` — the API, listening on `http://localhost:8000` (port configurable via `BACKEND_PORT`). Database migrations are applied automatically on startup — no manual `dotnet ef database update` step is needed.
   - `frontend` — the built SPA served by nginx, on `http://localhost:3000` (port configurable via `FRONTEND_PORT`)
3. Open `http://localhost:3000` in a browser.

A fresh database starts empty (schema + migration history only) — create teams, epics, tickets, and users through the application itself.

### Configuration reference (`.env`)

| Variable | Purpose |
| --- | --- |
| `BACKEND_PORT`, `FRONTEND_PORT`, `DB_PORT` | Host ports the containers are published on |
| `DB_USER`, `DB_PASSWORD`, `DB_NAME` | PostgreSQL credentials (used by both the `db` container and the backend's connection string) |
| `ConnectionStrings__DefaultConnection` | Full Postgres connection string used by the backend (`Host=db;...` — `db` is the Docker Compose service name) |
| `Jwt__Secret`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpiryMinutes` | JWT signing configuration — **set a real, random `Jwt__Secret` outside of local demos** |
| `Smtp__*` | SMTP relay used to send email-verification messages |
| `App__PublicUrl` | Base URL used to build links inside verification emails — must be reachable from wherever the user reads their email (host-published backend URL, not the internal Docker service name) |
| `VITE_API_BASE_URL` | The backend URL the frontend calls. Baked into the compiled JS bundle at **build time** (Vite env vars aren't runtime-configurable) — must be a browser-reachable URL, not the internal `backend` service name |

No secrets are committed — `.env` is gitignored, and `.env.example` only contains placeholder values.

## Local development (without Docker)

Backend:
```
cd backend/TicketTracker.Api
dotnet run
```
Uses `appsettings.Development.json` (a local Postgres connection string and a clearly-marked dev-only JWT secret). Requires a Postgres instance reachable at `localhost:5432` — `docker compose up db` starts just the database container if you don't want to install Postgres locally.

Frontend:
```
cd frontend
cp .env.example .env   # adjust VITE_API_BASE_URL if the backend isn't on localhost:8000
npm install
npm run dev
```

## Tests

Backend:
```
cd backend
dotnet test
```

Frontend:
```
cd frontend
npm run test
```
