# Nexus Task Management System

Nexus is a full-stack task management platform built around three working roles:

- Employer
- Team Lead
- Worker

The repository currently contains an active ASP.NET Core Web API backend and an active React/Vite frontend, plus older Blazor/frontend artifacts that still exist in the tree as legacy leftovers. The active application path is:

- Backend: `backend/WebAPI`
- Frontend: `frontend/src`

## What The System Does

Nexus combines organization management and day-to-day task execution in one product:

- Employers manage projects, budgets, reports, workspace settings, notifications, teams, and users
- Team Leads manage sprint boards, retros, analytics, backlog, and team oversight
- Workers manage assigned tasks, activity, schedule, notifications, focus sessions, and end-of-day summaries

The backend exposes role-protected REST APIs. The frontend provides separate UI shells for employer, team lead, workspace, and worker flows.

## Repository Layout

### Backend

The backend is organized as a layered .NET solution:

- `backend/Domain`
  Contains domain models, enums, and filters
- `backend/Application`
  Contains DTOs, service interfaces, and shared response models
- `backend/Infrastructure`
  Contains EF Core data access, entity configurations, migrations, mapping, seeders, and service implementations
- `backend/WebAPI`
  Contains API controllers, DI registration, auth setup, middleware, config, and startup logic

### Frontend

The frontend folder contains multiple generations of UI work:

- `frontend/src`
  Active React/Vite frontend
- `frontend/Pages`, `frontend/Components`, `frontend/Layout`, `frontend/Services`, `frontend/App.razor`, `frontend/Program.cs`
  Older Blazor-based frontend artifacts that are no longer the active UI runtime

The active entrypoint for the running frontend is:

- `frontend/index.html` -> `frontend/src/main.jsx` -> `frontend/src/App.jsx`

## Backend Overview

### Stack

- .NET 9
- ASP.NET Core Web API
- Entity Framework Core with PostgreSQL
- ASP.NET Core Identity
- JWT authentication
- Swagger / OpenAPI

### Backend Startup

Active startup is configured in `backend/WebAPI/Program.cs`.

Key responsibilities there:

- configure `ApplicationDbContext` with Npgsql
- register Identity for `ApplicationUser`
- configure JWT authentication
- register domain/application services
- enable Swagger in development
- optionally apply migrations at startup
- seed roles and demo data

### Authentication And Roles

Authentication is JWT-based.

Main roles in the system:

- `Employer`
- `Team Lead`
- `Worker`

Identity role names matter because authorization attributes and policies use those exact names. For example:

- employer APIs are protected with `Authorize(Roles = "Employer")`
- team lead policy uses `"Team Lead"`
- worker policy uses `"Worker"`

### Data And Persistence

The system uses PostgreSQL. Connection strings live in:

- `backend/WebAPI/appsettings.json`
- `backend/WebAPI/appsettings.Development.json`

Migration ownership is configured through the Infrastructure assembly.

### Main Backend Feature Areas

Employer-facing APIs:

- `api/projects`
- `api/projects/{projectId}/members`
- `api/projects/{projectId}/timeline`
- `api/projects/{projectId}/risks`
- `api/reports/dashboard`
- `api/budget/org`
- `api/employer/notifications`
- `api/workspace/*`
- `api/users`

Team Lead / Worker / Shared APIs:

- tasks
- subtasks
- task comments
- task tags
- sprints
- sprint retros
- retro action items
- activity logs
- notifications
- focus sessions
- schedule events
- day summaries
- user settings
- attachments
- absences
- join requests
- invitations

### Seed Data

Seed logic currently lives in:

- `backend/Infrastructure/Data/AuthSeeder.cs`
- `backend/WebAPI/Seeds/DeafultRoles.cs`

The seeder currently creates an employer, multiple team leads, workers, teams, projects, and tasks for demo purposes.

Important note:

The current seed implementation is aggressive. On first seed path it removes existing project/team/task/time-log data before reseeding. This is acceptable for local demo/dev environments but is risky for shared or persistent environments.

## Frontend Overview

### Active Frontend Stack

- React 18
- React Router
- Vite
- Framer Motion
- Lucide React
- Tailwind utility usage through CDN and utility classes

### Active UI Structure

The main active app lives in `frontend/src`.

Important files:

- `frontend/src/main.jsx`
  React entrypoint
- `frontend/src/App.jsx`
  Routing, auth redirects, role gates
- `frontend/src/api.ts`
  Employer/shared API layer
- `frontend/src/api/workspaceApi.js`
  Workspace/worker API helper layer
- `frontend/src/lib/api.js`
  Team lead API helper layer

### Frontend Role Areas

Employer area:

- overview
- projects board
- project detail
- team directory
- reports
- notifications
- settings

Team Lead area:

- sprint board
- sprint retro
- analytics
- backlog
- team overview
- notifications
- settings

Worker / workspace area:

- workspace dashboard
- today
- schedule
- end of day
- backlog
- activity
- team
- notifications
- settings
- worker-specific task detail

### Frontend Current State

The frontend has been assembled from multiple implementation passes. Some pages are fully integrated with backend APIs, while others still mix:

- real backend fetches
- partial contract assumptions
- mock display content
- legacy duplicated components

This means a page can build correctly but still be functionally incomplete or fragile at runtime.

## Running The Project

### Backend

From the repository root:

```powershell
dotnet build backend/WebAPI/WebAPI.csproj
dotnet ef database update --project backend/Infrastructure/Infrastructure.csproj --startup-project backend/WebAPI/WebAPI.csproj
dotnet run --project backend/WebAPI/WebAPI.csproj
```

### Frontend

From `frontend`:

```powershell
npm install
npm run dev
```

Production build:

```powershell
npm run build
```

## Environment Configuration

### Backend

Important configuration keys in `backend/WebAPI/appsettings.json`:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Database:ApplyMigrationsOnStartup`

### Frontend

Important environment variables:

- `VITE_API_BASE_URL`
- `VITE_TEAM_ID`
- `VITE_ACTOR_ID`

The frontend expects the backend API base URL to point to the WebAPI host, for example:

```env
VITE_API_BASE_URL=http://localhost:5125
```

## Known Architectural Realities

This codebase is functional but not yet cleanly consolidated.

Current realities:

- there are legacy Blazor frontend files still present beside the active React app
- naming inconsistencies exist in the backend such as `ProjectCOntroller`, `INterfaces`, and similar casing drift
- some frontend screens still render mock visual content while only partially using real backend data
- some backend controllers and services reflect multiple generations of feature work
- there are leftover build/debug artifact text files in the backend root

## Refactor Priorities

Recommended cleanup order:

1. Stabilize backend startup, migrations, and seeding behavior
2. Normalize backend naming and remove dead/legacy code paths
3. Consolidate frontend API contracts and remove duplicated helper layers where possible
4. Replace remaining mock UI sections with fully real backend-driven data
5. Remove inactive Blazor frontend artifacts if the React app is the permanent direction
6. Add end-to-end smoke tests for auth, employer, team lead, and worker flows

## Current Goal Of This Audit

This README is being used as the working map for an end-to-end stabilization/refactor pass.

The current effort focuses on:

- identifying backend issues that can break build, run, migrations, or role-based APIs
- identifying frontend issues caused by split implementations and contract mismatches
- improving project clarity so future work happens against the real active app instead of legacy leftovers
