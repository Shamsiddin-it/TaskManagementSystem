# Frontend folder map (Blazor WebAssembly)

This project is **TaskManager.Web** — a **Nexus / Worker Workspace**–style UI that calls your **WebAPI** over HTTP (JWT + JSON).

| Path | Purpose |
|------|---------|
| **`/`** (root) | Project file, `Program.cs`, `App.razor`, `appsettings.json` |
| **`Properties/`** | `launchSettings.json` — WASM dev URLs (**https://localhost:7081**, **http://localhost:5160**). These must stay allowed in **backend CORS**. |
| **`wwwroot/`** | Static files served to the browser |
| **`wwwroot/css/app.css`** | Base Blazor styles + imports **`nexus-theme.css`** |
| **`wwwroot/css/nexus-theme.css`** | Dark theme, purple accent, layout (sidebar, dashboard, auth) — **visual match** to your mockups |
| **`wwwroot/js/authStorage.js`** | `localStorage` helpers for JWT + role (called from C# via JS interop) |
| **`Layout/`** | Shell layouts |
| **`Layout/MainLayout.razor`** | Minimal wrapper for **login / register** |
| **`Layout/NexusShellLayout.razor`** | **Worker app chrome**: top bar + sidebar slot + main content |
| **`Pages/`** | Routable screens (`@page`) |
| **`Pages/Home.razor`** | **`/`** → sends you to **`/login`** or **`/workspace`** |
| **`Pages/Auth/`** | **Login** and **Register** (real calls to **`api/Auth/login`** & **`register`**) |
| **`Pages/Workspace/`** | Authenticated area: **dashboard** (metrics + empty state + notes + quote), plus **Schedule / Team / Activity / Notifications / Settings** placeholders |
| **`Components/Navigation/`** | **`WorkspaceSidebar.razor`** — nav links + sign out |
| **`Components/Dashboard/`** | Reusable pieces like **`MetricCard.razor`** |
| **`Components/Auth/`** | **`RedirectToLogin.razor`** — used when a page requires auth |
| **`Models/Api/`** | DTOs matching API JSON (`ApiResponse`, login/register payloads). Property **`date`** matches backend **`Response<T>.Date`**. |
| **`Configuration/`** | **`ApiOptions`** — documented defaults for API base URL |
| **`Services/Auth/`** | Token storage, **`NexusAuthStateProvider`**, **`AuthApiService`**, JWT `sub` parse |
| **`Services/Http/`** | **`AuthHeaderHandler`** — attaches **Bearer** token (skips login/register) |

## Configuration

- **`appsettings.json`** → `"Api:BaseUrl"` (default **`https://localhost:7060`**) must match your running **WebAPI** URL.
- Backend **`Program.cs`** enables CORS policy **`BlazorWasm`** for the WASM dev origins above.

## Run (two terminals)

1. **API:** `dotnet run` in `backend/WebAPI` (HTTPS profile → **7060**).
2. **UI:** `dotnet run` in `frontend` (HTTPS → **7081**).

Open **https://localhost:7081**, register or log in, then use the sidebar.

## Mockup coverage

See **`PICTURE_TO_PAGE_MAP.md`** — one route per major screen from your **13 picture groups** (worker + employer).

## Next steps (when you say)

- Wire **notifications**, **schedule**, **settings** pages to real controllers.
- Add **Task / Project / Team** pages once those APIs exist.
- Replace placeholder metrics with aggregated API data.
