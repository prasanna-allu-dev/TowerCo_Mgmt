# CLAUDE.md: TowerCo Tower Management System

Single source of truth for requirements, decisions, scope and progress. Update this file whenever scope or a decision changes.

---

## 1. Context

TowerCo builds and operates telecom tower sites across several countries and leases space on towers to mobile network operators (MNOs). Practical exercise: design and build a small Tower Management System, with fictional data, and explain domain model and architecture.

Deliverables for the session:
- Working demo covering the use cases
- Short explanation of domain model and architecture (simple diagram)
- Source code ready to walk through, plus one small live change

Guiding principle: clean, well-reasoned solution over feature count.

---

## 2. Use cases (from brief)

| # | Persona | Use case | Phase |
|---|---------|----------|-------|
| UC3 | Any user | Search and filter sites (country, status, text), open a single view with everything about a site | **Phase 1 (40 min)** |
| UC2 | Commercial user | Add an operator as tenant on a tower, see whether tower has capacity for more tenants | **Phase 1 (40 min)** |
| UC1 | Operations user | Register a new site and its tower, update details over time | **Phase 1 thin slice** (register only); edit in Phase 2 |
| UC4 | Manager | Visual overview: where sites are (map) and utilisation at a glance | **Phase 1 thin slice** (KPI tiles + utilisation by country); map in Phase 3 |

Focus for this session: **commercial user (UC2) and any user browsing sites (UC3)**, plus thin slices of UC1 and UC4 so the demo touches all four use cases (brief asks for a demo "covering the use cases above").

---

## 3. Domain model (proposed)

```
Operator 1 ──── * Tenancy * ──── 1 Tower 1 ──── 1 Site
                     │ 0..1          │ 1
                     │               │
                     └──── * Equipment * ──── 1 EquipmentType
```

| Entity | Key attributes | Notes |
|--------|----------------|-------|
| **Site** | Id, SiteCode (e.g. `IN-MUM-0001`), Name, CountryCode (ISO 2), City, Latitude, Longitude, Status, CreatedAtUtc | Physical location. Status: `Planned`, `UnderConstruction`, `Active`, `Decommissioned` |
| **Tower** | Id, SiteId, TowerType, HeightMetres, MaxTenants | One tower per site (simplification). TowerType: `Monopole`, `Lattice`, `Guyed`, `Rooftop` |
| **Operator** | Id, Name, CountryCode | Mobile network operator (fictional names) |
| **Tenancy** | Id, TowerId, OperatorId, StartDate, EndDate (nullable), MonthlyRent, Currency | Commercial lease of a slot on a tower. Active when EndDate is null or in future |
| **EquipmentType** | Id, Name, Category | Configurable catalogue (data, not code). Category: `Radio`, `Transmission`, `Power`, `Cooling`, `Security`. Seed: 5G Active Antenna (AAU), 4G Remote Radio Unit, Microwave Dish, Diesel Generator, Battery Bank, Solar Array, Air Conditioner, Security Camera |
| **Equipment** | Id, TowerId, EquipmentTypeId, TenancyId (required), SerialNumber, InstalledOn, MountHeightMetres (nullable), Status | Physical asset on a tower. Always tenant-owned: `TenancyId` must point to a tenancy on the same tower (`InvalidEquipmentOwnerException`, 409). No TowerCo shared equipment. Status: `Planned`, `Installed`, `Removed` |

Equipment decisions (confirmed 2026-11-02):
- Ownership: every `Equipment` item belongs to a tenancy on the same tower; there is no TowerCo shared equipment (clarified 2026-11-02, replaces the earlier shared-infrastructure decision).
- TowerCo power kit (generator, battery, solar, shelter cooling) is not `Equipment`. User decision 2026-11-02: model it separately as TowerCo site infrastructure (proposed `SiteInfrastructure` entity on Tower: Id, TowerId, Kind, SerialNumber, InstalledOn), not leased, monitored by sensors. Not built yet; currently removed from the seed.
- Configurable: `EquipmentType` catalogue table seeded; new types added as data without a code change.
- Capacity: equipment is display only; slot-based capacity unchanged. Load-based capacity (weight, wind load per type) is a future extension.
- Phase: Phase 1 shows seeded equipment on site detail (read only). Phase 2 adds create, edit and remove.

Derived (not stored):
- `ActiveTenantCount` = active tenancies on tower
- `AvailableSlots` = MaxTenants minus ActiveTenantCount
- `UtilisationPercent` = ActiveTenantCount / MaxTenants x 100
- `HasCapacity` = AvailableSlots > 0

Business rules (Phase 1):
1. Tenant can be added only when tower has capacity, else `TowerCapacityExceededException` (HTTP 409).
2. Same operator cannot hold two active tenancies on the same tower, else `DuplicateTenancyException` (HTTP 409).
3. Tenants can be added only to `Active` sites, else `SiteNotActiveException` (HTTP 409).
4. Unknown site, tower or operator returns `NotFoundException` (HTTP 404).

Why slot-based capacity: simple, explainable, matches "room for more tenants". Real-world capacity also depends on structural load (wind load, antenna weight); noted as future extension.

---

## 4. Architecture (proposed)

```
React (Vite + TypeScript)  ──HTTP/JSON──►  ASP.NET Core Web API (.NET 10)  ──EF Core──►  SQLite (file DB, seeded)
 towerco_mgmt.client                        TowerCo_Mgmt.Server
```

Backend: basic Clean Architecture (dependencies point inward), so later controllers plug in without rework:

| Project | Contains | Depends on |
|---------|----------|------------|
| `TowerCo.Domain` | Entities, enums, domain exceptions, capacity rules | nothing |
| `TowerCo.Application` | Use-case services behind interfaces (`ISiteQueryService`, `ITenancyService`), DTOs, repository abstractions, constants | Domain |
| `TowerCo.Infrastructure` | EF Core DbContext, SQLite, repositories, seed data | Application, Domain |
| `TowerCo_Mgmt.Server` | Thin controllers, exception-to-ProblemDetails middleware, DI wiring, `appsettings.json` | Application, Infrastructure |
| `TowerCo.Tests` | xUnit tests for Domain and Application (SQLite in-memory where needed) | all |

Rule: no hardcoded values; constants in `Application/Constants`, config in `appsettings.json`.

Why SQLite: zero setup, file-based, portable for demo; swap to Azure SQL via connection string later.

---

## 5. Phase 1 scope: 40-minute build

### API endpoints
| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/sites?country=&status=&search=` | List and filter sites with tower capacity summary |
| GET | `/api/sites/{id}` | Full site view: site, tower, tenancies, capacity, equipment (grouped by owning operator) |
| GET | `/api/operators` | Operators for tenant picker |
| POST | `/api/towers/{towerId}/tenancies` | Add operator as tenant (capacity rules enforced) |
| POST | `/api/sites` | UC1 thin slice: register site and its tower in one request |
| GET | `/api/portfolio/summary` | UC4 thin slice: totals, utilisation by country, counts by status, towers with free slots |

Thin-slice rules (added to business rules in section 3):
5. `SiteCode` must be unique, else `DuplicateSiteCodeException` (HTTP 409).
6. `MaxTenants` and `HeightMetres` must be positive, else `InvalidTowerSpecificationException` (HTTP 400). Limits from `appsettings.json`.
7. New site starts in `Planned` status by default (configurable constant).
8. Portfolio summary counts only non-`Decommissioned` sites; utilisation = active tenancies / total slots per country.

### UI screens
1. **Sites list**: filters (country, status, free text), table with code, name, country, status, tenants `x/y`, capacity badge.
2. **Site detail**: site info, tower info, capacity bar, tenancy table, equipment table (type, category, owner: operator name, serial, status).
3. **Add tenant form** (on site detail): operator dropdown, start date, monthly rent; disabled with message when no capacity; shows API error on rule violation.
4. **Register site form** (UC1 thin): site code, name, country, city, lat/long, tower type, height, max tenants; on success navigate to new site detail; shows 409/400 errors inline.
5. **Portfolio overview page** (UC4 thin): KPI tiles (total sites, active sites, total slots, overall utilisation %, towers with free slots), utilisation by country bar chart, sites by status counts. Map is stretch.

### Seed data
- 4 countries (fictional portfolio): IN, ZA, KE, NG
- ~12 sites mixed statuses, ~5 operators, tenancies giving mix of full, partial and empty towers
- 8 equipment types; each tenancy gets a radio plus a microwave dish, all tenant-owned; no shared equipment

### Timebox
| Minutes | Work |
|---------|------|
Revised for thin slices. Skeleton (projects, DbContext, exception handler) and mock-backed UI (sites list, site detail) already exist, which frees time.

| Minutes | Work |
|---------|------|
| 0 to 10 | Domain entities, EF config, seed; `ISiteQueryService`, `ITenancyService` |
| 10 to 16 | UC1 `ISiteRegistrationService`, UC4 `IPortfolioSummaryService` |
| 16 to 22 | Controllers; unit tests for rules 1 to 8 and site filtering |
| 22 to 34 | React: switch mock to real API, add tenant form, register site form, overview page |
| 34 to 40 | Build, run tests, end-to-end demo check |

Cut order if behind time: (1) overview chart, keep KPI tiles only; (2) register form fields beyond code, name, country, max tenants; (3) equipment table.

### Out of scope for Phase 1
Edit site or tower (UC1 update), map (UC4), auth and roles, ending a tenancy, Azure hosting, pagination, tower health (section 6.1).

### Definition of success (end of 40 minutes)

Session is a success only when every **Must** item is ticked. **Should** items are stretch.

**A. Demo script runs end to end without errors (Must)**
1. Open app: sites list loads from API with seeded data (4 countries, mixed statuses).
2. Filter by country, then by status, then free-text search: list updates correctly each time.
3. Open a site: detail view shows site, tower, tenancies, capacity bar (`x of y slots`, utilisation %) and equipment grouped by owning operator.
4. Add an operator as tenant on a tower with free slots: tenancy appears, capacity bar updates.
5. Open a full tower: add-tenant form disabled with "no capacity" message.
6. Try adding same operator twice: clear 409 error message shown in UI.
7. Register a new site and tower (UC1 thin): it appears in sites list and opens in detail; duplicate site code shows 409.
8. Open portfolio overview (UC4 thin): KPI tiles and utilisation by country match seeded data and reflect the tenant added in step 4.

**B. Technical gates (Must)**
- `dotnet build` succeeds with zero errors.
- `dotnet test` all green; tests cover business rules 1 to 8, site filtering and portfolio summary maths.
- `npm run build` for client succeeds.
- App starts with one command each (API, client) from a clean clone; DB created and seeded on first run.
- No hardcoded constants, no secrets, methods under 15 lines in new code.

**C. Explainable design (Must)**
- Can explain domain model (4 entities, why derived capacity) in under 2 minutes using section 3.
- Can explain Clean Architecture layers and dependency direction using section 4.
- Can point to where each business rule lives and its test.

**D. Live-change ready (Must)**
- Codebase small and predictable enough to make a small change live, e.g. add a filter, add a field to Site, or change capacity rule, in under 5 minutes.

**E. Stretch (Should)**
- Capacity badge colours (free, nearly full, full) on sites list.
- Leaflet map on overview page (sites coloured by utilisation).
- Simple architecture diagram exported for walkthrough.
- README with run steps.

**Not a success measure in this session:** UC1 edit, UC4 map, auth, Azure hosting, tower health, UI polish.

---

## 6. Later phases

- **Phase 2 (UC1)**: edit site and tower (create delivered as Phase 1 thin slice), status transitions, validation. Equipment management: `GET /api/equipment-types`, `POST /api/towers/{towerId}/equipment`, `PUT` and remove; rules: tenant-owned equipment needs an active tenancy on the same tower (`InvalidEquipmentOwnerException`, 409), unknown type 404.
- **Phase 3 (UC4)**: extend Phase 1 overview with Leaflet map coloured by utilisation, drill-down from country to sites, trend over time.
- **Phase 4**: role-based views (operations, commercial, manager), end tenancy, audit history, Azure App Service + Azure SQL, architecture diagram polish.

- **Phase 5 (UC5, proposed)**: Tower monitoring and health page. Plan in section 6.1. Not in 40-min scope.
- **Phase 6 (proposed)**: Work orders: lifecycle state machine, auto-open on Critical alarms, SLA escalation, assignment, live updates. See section 6.2.
- **Phase 7 (proposed)**: Azure and field rollout: IoT Hub, Functions stream processor, Service Bus, Azure SQL, Data Lake, Entra ID roles, technician PWA, operator notifications.

### 6.2 Target architecture: sensors and work orders (draft, not approved)

Visual: `docs/2026-11-02_13-15-06_target-architecture.html`, published privately at https://claude.ai/artifact/Bc4X6DbvK1VAuQ3gPatA3f

- **Flow:** tower sensors (power, battery, generator, shelter climate, door, aviation light, tilt) > site gateway (MQTT over cellular) > IoT Hub > stream processor (Functions: normalise, per-minute roll-up, alarm rules) > Service Bus `tower-alarms` > worker (dedupe, open work order, SLA escalation, verification) > API, SignalR, technician app, operator notifications. Raw readings archived to Data Lake.
- **Work order states:** Alarm raised > Open > Triaged > Assigned > In progress > Completed > Verified > Closed. Side paths: Escalated (SLA breach via scheduled message), Cancelled (from Open or Triaged, with reason), Reopened (verification fails).
- **Domain additions:** `Sensor`, `TelemetryReading` (per-minute roll-up), `AlarmRule` (config), `Alarm`, `WorkOrder` (replaces `MaintenanceTicket` in 6.1), `WorkOrderEvent` (audit), `Technician`. `WorkOrderStateMachine` in Domain; illegal move raises `InvalidWorkOrderTransitionException` (409).
- **Code placement:** modular monolith; `Monitoring/` and `WorkOrders/` folders in existing projects; new `TowerCo.Ingestion` Functions project reusing Domain alarm rules.
- **Phases 5 and 6 need no Azure:** simulator posts to API, in-process queue behind the same interface as Service Bus.
- **Built (2026-11-02):** `TowerCo.Ingestion` (Functions v4, .NET 10 isolated). `IngestReading` HTTP POST `/api/telemetry` for demo; `IngestFromIoTHub` Event Hub trigger on IoT Hub built-in endpoint (disabled locally via `AzureWebJobs.IngestFromIoTHub.Disabled`). Domain `Monitoring/`: `SensorReading`, `AlarmThreshold` (warning, critical, direction), `SensorType`, `AlarmSeverity`. Thresholds in `AlarmThresholds` config. `IAlarmPublisher` has logging stub; Service Bus publisher and roll-up still to do.
- **Sensor thresholds** shown in the visual are proposed starting values for discussion, held in configuration, not sourced from a standard.
- **Telemetry store:** ~~Azure SQL roll-ups vs Azure Data Explorer~~ Decided 2026-11-02 by user: sensor data in Azure Cosmos DB (see 6.3).
- **Open questions:** per-country data residency; build work orders in-house vs integrate an existing maintenance system; auto-open on Critical only or also repeated Warnings.

### 6.3 Enterprise view and sensor data collection (draft, not approved)

- **Context:** TowerCo platform connects to mobile operators (lease requests, outage notices), TowerCo staff, field technicians and contractors, finance ERP (rent invoicing, ground rent), analytics, regulators (aviation light compliance), and optional feeds from landlords and energy/grid providers. Phase 1 covers TowerCo staff only.
- **Bounded contexts:** Portfolio and Commercial (Phase 1), Monitoring, Field service, Partner access (API Management, Entra External ID for operator users), Insights (reads the lake, never live tables). Contexts share IDs and events (proposed names `SiteRegistered`, `TenancyStarted`, `TenancyEnded`, `AlarmRaised`, `AlarmCleared`, `WorkOrderClosed`), not tables. Modular monolith; ingestion runs separately from day one.
- **Sensor data store:** Azure Cosmos DB (NoSQL). Containers `readings` (raw), `rollups` (per minute), `alarm-state` (one active alarm per tower and sensor, ETag check). Hierarchical partition key tower > sensor > day. Autoscale throughput; time to live from configuration. Change feed copies raw readings to Data Lake for cold storage and analytics. Business data stays relational (SQLite now, Azure SQL later). Telemetry repository interface in Application, Cosmos DB implementation in Infrastructure.
- **Azure Functions (`TowerCo.Ingestion`):** `TelemetryIngest` (IoT Hub trigger via Event Hubs compatible endpoint; validate > enrich from sensor registry > store raw keyed by message ID > per-minute roll-up by patch increment > evaluate Domain alarm rules > `alarm-state` + Service Bus `tower-alarms`; invalid messages to a dead-letter container), `Archive` (Cosmos DB change feed > Data Lake), `Heartbeat` (timer; silent gateway raises "communication lost"), `Simulator` (timer; demo readings for seeded towers). Gateways enrolled via Device Provisioning Service with per-device certificates. Managed identity for all bindings. Hosting plan (Flex Consumption or Premium) decided in Phase 7.
- **Qualities:** independent scaling of sensor path; Entra ID roles, managed identities, Key Vault, private endpoints, Front Door with WAF; queue between alarm and work order, idempotent handlers, geo-replication; one correlation ID from sensor message to work order; cost control via time to live and lake storage; Bicep and GitHub Actions. No uptime, latency or retention targets set; they must come from the business.
- **Phase 8 (proposed):** enterprise integration: operator partner API, finance ERP invoicing, analytics on the lake.

### 6.1 Plan: Tower monitoring and health (draft, not approved)

**User story (proposed UC5):** As an operations user or manager, I want to see the health of each tower and the portfolio, so that I can spot towers needing attention before they affect tenants.

**Health definition (fictional, to confirm):** health is derived from three signal groups, each scored `Healthy`, `Warning`, `Critical`:
| Signal | Source | Warning | Critical |
|--------|--------|---------|----------|
| Power / uptime (rolling 30 days) | Simulated telemetry readings | below 99.5% | below 98% |
| Last structural inspection | Inspection records | older than 12 months | older than 24 months |
| Open maintenance tickets | Maintenance tickets | 1 or more open | any `High` severity open |

Overall tower health = worst signal (simple, explainable). Thresholds live in `appsettings.json` (`HealthThresholds` section), never hardcoded.

**Domain additions (TowerCo.Domain)**
- `TelemetryReading`: TowerId, RecordedAtUtc, PowerOk (bool), UptimePercent, TemperatureCelsius
- `Inspection`: TowerId, InspectedOn, Inspector, Outcome (`Pass`, `PassWithActions`, `Fail`), Notes
- `MaintenanceTicket`: TowerId, OpenedAtUtc, ClosedAtUtc (nullable), Severity (`Low`, `Medium`, `High`), Summary
- Enum `HealthStatus`; value object `TowerHealth` (Overall, per-signal statuses, reasons)
- `ITowerHealthEvaluator` + one `IHealthSignalRule` per signal (Open/Closed: add a signal = add a rule class)

**Application (TowerCo.Application)**
- `ITowerHealthQueryService`: `GetPortfolioHealthAsync(filters)`, `GetTowerHealthAsync(towerId)`
- DTOs: `PortfolioHealthSummaryDto` (counts by status, by country, worst 10 towers), `TowerHealthDto` (signals, reasons, uptime trend points, recent inspections, open tickets)
- Options class `HealthThresholdOptions` bound from config
- Repository abstractions: `ITelemetryRepository`, `IInspectionRepository`, `IMaintenanceTicketRepository`

**Infrastructure (TowerCo.Infrastructure)**
- EF configurations + tables for the three new entities; index on (TowerId, RecordedAtUtc)
- Seed: 30 days of daily readings per tower with a planted mix of healthy, warning and critical towers; inspections and tickets to match
- Telemetry is simulated (no real devices). Future: background `IHostedService` ingesting from Azure IoT Hub / Event Hubs

**API (TowerCo_Mgmt.Server)**
| Method | Route | Purpose |
|--------|-------|---------|
| GET | `/api/health/towers?country=&status=` | Portfolio health summary + tower list with overall status |
| GET | `/api/health/towers/{towerId}` | Single tower health: signals, reasons, 30-day uptime trend, inspections, tickets |
| POST | `/api/towers/{towerId}/maintenance-tickets` | Raise a ticket (optional, stretch) |

Errors: unknown tower uses existing `NotFoundException` (404) through `DomainExceptionHandler`.

**UI (towerco_mgmt.client, owned by frontend stream)**
1. **Health overview page** (`/health`): KPI tiles (Healthy / Warning / Critical counts), filter by country and status, table of towers sorted worst first with status pill and main reason.
2. **Tower health panel**: tab on existing site detail page: signal cards, 30-day uptime line chart, inspection list, open tickets.
3. **Sites list**: small health dot column linking to the panel.
4. Status shown by colour and text label (accessibility). Chart library: Recharts. Polling every 60 s on overview (no real-time push in first cut).

**Testing**
- Unit: each `IHealthSignalRule` at boundary values (just above / at / below thresholds), evaluator worst-of logic, query service filters. 100% coverage on new Domain and Application code.
- Integration: `WebApplicationFactory` tests for the two GET endpoints with SQLite in-memory seed.
- UI: render tests for status pill and empty states (frontend stream).

**Delivery slices**
| Slice | Content | Rough effort |
|-------|---------|--------------|
| 1 | Domain entities, rules, evaluator + unit tests | 1.5 h |
| 2 | EF config, seed, repositories, query service | 1.5 h |
| 3 | API endpoints + integration tests | 1 h |
| 4 | UI overview page + tower panel | 2 h |
| 5 | Polish: health dot on sites list, ticket creation (stretch) | 1 h |

**Open questions**
1. ~~Scope of "health"~~ Decided 2026-11-02: operational tower health only. API/platform `/healthz` out of scope.
2. Are the three signals and thresholds acceptable, or are there others (e.g. generator fuel, battery backup, security alarms)?
3. Simulated telemetry acceptable for demo, or should ingestion be designed now?
4. Which persona owns this page: operations, manager, or both?

---

## 7. Engineering standards (from user-level CLAUDE.md)

- Methods under 15 lines; SOLID; no code smells
- Custom exceptions, no generic ones
- No hardcoded constants; use `Constants/` or `appsettings.json`
- New backend code: 100% unit test coverage; build and tests must pass before delivery
- Indian English; no em or en dashes in docs
- No secrets in code, docs or git
- Workflow: plan, develop, test, refine

---

## 8. Assumptions (confirm or correct)

1. File name "cloud.md" in request taken as `CLAUDE.md` (auto-loaded by Claude Code each session).
2. `TowerCo_Mgmt.Server` and `towerco_mgmt.client` come from the VS "React and ASP.NET Core" template. From 2026-11-02 this stream also owns the React client (UI ownership confirmed by user).
3. .NET 10 SDK (installed: 10.0.302) and Node 24 used.
4. No authentication in Phase 1; personas are conceptual only.
5. One tower per site.
6. Capacity = configurable slot count per tower (`MaxTenants`).

## 9. Decisions (confirmed 2026-11-02)

1. Capacity model: slot count (`MaxTenants`). Structural load is a future extension.
2. Database: SQLite.
3. Phase 1 delivers UC2 + UC3 on a basic Clean Architecture skeleton; tests cover Phase 1 Domain and Application logic. Later controllers (UC1, UC4) extend the same layers.
4. Tenant equipment: one `Equipment` entity, always owned by a tenancy on the same tower (no TowerCo shared equipment, clarified 2026-11-02), with configurable `EquipmentType` catalogue, display only, read in Phase 1, managed in Phase 2 (see section 3).
5. Phase 1 adds thin slices of UC1 (register site + tower) and UC4 (KPI tiles + utilisation by country) so the demo covers all four use cases. Edit and map stay in later phases.
6. Target architecture (proposed phases): sensor data stored in Azure Cosmos DB for scale; collected by Azure Functions in `TowerCo.Ingestion` (see 6.3).

---

## 10. Progress log

| Date | Change |
|------|--------|
| 2026-11-02 | Requirements analysed, phases and 40-min scope defined, CLAUDE.md created |
| 2026-11-02 | Confirmed: slot capacity, SQLite, basic Clean Architecture for Phase 1 |
| 2026-11-02 | Visual requirements brief: `docs/2026-11-02_12-08-38_requirements-overview.html`, published at https://claude.ai/artifact/CVUiTS7Wou2epnSBfPaYK2 (private) |
| 2026-11-02 | Added definition of success for 40-min session (section 5) |
| 2026-11-02 | Drafted tower monitoring and health plan (section 6.1), awaiting approval |
| 2026-11-02 | Health page scope confirmed: tower health only, `/healthz` dropped |
| 2026-11-02 | Backend skeleton built: Domain (`Entity`, `DomainException`, `NotFoundException`, `BusinessRuleException`), Application (`AddApplication`), Infrastructure (`TowerCoDbContext`, SQLite, `AddInfrastructure`, `InitialiseDatabaseAsync`, `MissingConfigurationException`), Server (`DomainExceptionHandler` to ProblemDetails, 404/409, `Program` split). Weather sample removed. Packages pinned to 10.0.14 (fixes Microsoft.OpenApi high-severity advisory). Build OK, 7/7 tests pass. Server and client esproj are VS-generated; slnx recreated with all 6 projects |
| 2026-11-02 | Added tenant and shared equipment: `EquipmentType` catalogue + `Equipment` entity; Phase 1 read only on site detail, Phase 2 manage |
| 2026-11-02 | Phase 1 scope extended with UC1 (register site) and UC4 (overview KPIs) thin slices: 2 endpoints, 2 screens, rules 5 to 8, revised timebox with cut order, success steps 7 and 8 |
| 2026-11-02 | Tenant management UI, read phase: sites list, site detail (site, tower, capacity, tenancies), operators list, tower configuration page. Runs on typed mock API (`src/api/mock`) behind `SitesApi`/`OperatorsApi`/`TowersApi` contracts, switched by `VITE_USE_MOCK_API` in `.env`. New endpoint needed for config page: `GET /api/towers`. Writes (add tenant, edit config) deferred. `npm run build` and lint OK |
| 2026-11-02 | Super admin portfolio dashboard (`/dashboard`) on mock API behind `DashboardApi` contract: KPI tiles, utilisation by country, sites by status, tenancies by operator, monthly rent per currency (no FX), renewals due in 90 days, sellable capacity. Backend endpoint needed later: `GET /api/dashboard` returning `PortfolioDashboard`. Two mock tenancies with near end dates added for renewals demo. Build and lint OK |
| 2026-11-02 | Work order management UI (`/work-orders`, `/work-orders/:id`) on in-memory mock behind `WorkOrdersApi` contract: KPI tiles (open, overdue, due this week, unassigned), filters (text, status, priority, country), create form, detail with status state machine (`WORK_ORDER_TRANSITIONS` in `constants/workOrders.ts`). Fictional data and engineer names. Backend endpoints needed later: `GET/POST /api/work-orders`, `GET /api/work-orders/{id}`, `PATCH /api/work-orders/{id}/status` (409 on invalid transition, 400 when assigning without engineer). Build and lint OK; not yet exercised in browser |
| 2026-11-02 | Scalar API reference added (`Scalar.AspNetCore` 2.18.7) at `/scalar` (Development only), reading `/openapi/v1.json`. Title in `ServerConstants`. Seed confirmed already in place (`SeedCatalogue`, `SeedData`, `DatabaseSeeder`, run on startup). Build OK, 36/36 tests pass |
| 2026-11-02 | Backend entities and persistence: `Site` (with `SiteLocation` value object), `Tower` (slot capacity: `ActiveTenantCount`, `AvailableSlots`, `HasCapacity`, `UtilisationPercent` as of a date), `Operator`, `Tenancy` (`Money` value object, `IsActiveOn`), `EquipmentType`, `Equipment`; enums stored as text; EF configurations per entity; `Guard` + `DomainValidationException` (400), `TowerAlreadyExistsException` (409). SQLite file `TowerCo_Mgmt.Server/towerco.db` created and seeded on first run via `DatabaseSeeder` (12 sites, 12 towers, 9 operators, 17 tenancies, 8 equipment types, 35 equipment). Capacity and duplicate rules for adding tenants (UC2) not yet enforced. 34/34 tests pass, 100% line and branch coverage on Domain and Infrastructure |
| 2026-11-02 | Dashboard severity grading: shared `Severity` scale (ok green, warning amber, critical red, neutral blue) in `src/utils/severity.ts`; thresholds: utilisation 75% warning, 100% critical; renewals 30 days critical, 90 days warning. "Critical indicators" panel driven by `alerts` on `PortfolioDashboard` (full towers, empty active towers, tenants on decommissioned sites, renewals). Backend `GET /api/dashboard` must return `alerts`. Build and lint OK |
| 2026-11-02 | Equipment seed aligned with forwarded equipment spec: each tenancy now gets a radio (5G AAU or 4G RRU) plus a microwave dish (2 tenant items); active towers get shared power (Solar Array on rooftops, Diesel Generator otherwise) plus Battery Bank. Expected seed: 54 equipment rows. Stale `towerco.db` deleted so next start re-seeds. Grouped equipment in `GET /api/sites/{id}` comes with the UC3 endpoint. 36/36 tests pass, coverage 100% on Domain and Infrastructure |
| 2026-11-02 | Demo deck (15 slides, speaker notes, keyboard nav): `docs/2026-11-02_12-48-28_towerco-demo-deck.html`, published privately at https://claude.ai/artifact/XidfFmqgrVGchR52yuW8cw. Shows built vs mock vs next status, today's demo run order and the target Phase 1 script. Portfolio figures (16/32 slots, 50.0%) hand-computed from seed as of 2 Nov 2026; update the status slide as API endpoints land |
| 2026-11-02 | Target architecture drafted (section 6.2): sensors, telemetry ingestion, alarms, work order lifecycle; Phases 6 and 7 proposed; visual published privately at https://claude.ai/artifact/Bc4X6DbvK1VAuQ3gPatA3f |
| 2026-11-02 | Demo deck v2: two "Site hierarchy" slides added after the domain model (top-down diagram Country > Site > Tower > Tenancy / shared equipment > tenant equipment > Equipment type, teal tenant-owned, coral TowerCo-owned, dashed reference data; worked example uses real seeded `IN-DEL-0002` instead of the requested placeholder data so it matches app and DB). Rules 1 to 3 marked "Built in domain" (`Tower.AddTenancy`). Deck now 17 slides, same URL |
| 2026-11-02 | Sensor ingestion Functions project `TowerCo.Ingestion` added to solution (see section 6.2 "Built"). HTTP + IoT Hub triggers share `ISensorReadingProcessor`; bad IoT Hub messages logged and skipped, bad HTTP bodies return 400 ProblemDetails. Custom exceptions `InvalidSensorPayloadException`, `MissingAlarmThresholdException`. Threshold values in `local.settings.json` are fictional discussion values. Packages: Worker 2.52.0, Sdk 2.1.0, EventHubs 6.5.0, Http.AspNetCore 2.1.1; no vulnerable packages. 71/71 tests pass; 100% line and branch on new hand-written code |
| 2026-11-02 | Demo deck v3: four target architecture slides after the current architecture slide (system view in 4 lanes Field > Ingestion > Core > People with built items marked; telemetry hot, warm and cold paths; work order lifecycle with Cancelled, Escalated, Reopened side paths and `InvalidWorkOrderTransitionException` 409; seven domain additions and code placement). Every slide marked "Proposed, not approved". Worked example uses seeded `IN-DEL-0002`; no SLA or threshold figures shown. Roadmap slide now lists Phases 5 to 7 and the three open questions from section 6.2. Deck now 21 slides, same URL |
| 2026-11-02 | Requirements brief (v3) and target architecture page (v3) aligned with seed data: placeholder IN-MUM-0001 Lattice, Kestrel Mobile, Baobab Telecom replaced by seeded `IN-DEL-0002` (Monopole 30 m, 3 slots, Aurora Mobile 5G AAU + dish, Bluewave Telecom 4G RRU + dish, shared Diesel Generator + Battery Bank). Simulator operators now seeded names. Work order walkthrough now a failed generator start (Air Conditioner is not seeded) |
| 2026-11-02 | Create tower site UI flow (UC1 thin, mock): `/configuration/new` 3-step wizard (Site details, Tower specification, Review and create) from "+ New tower site" on Configuration page. Per-step validation (site code format `XX-XXX-0000`, lat/long ranges, height 5 to 120 m, 1 to 6 tenant slots; limits in `constants/domain.ts`). `SitesApi.create` added (HTTP `POST /api/sites`, mock creates Planned site + tower, 409 on duplicate code, 400 on non-positive spec). On success navigates to new site detail. Files: `components/configuration/*`, `hooks/useCreateSiteWizard.ts`, `pages/CreateTowerSitePage.tsx`, button/form/wizard styles in `App.css`. Own files type-check clean; full `npm run build` currently blocked by in-progress work orders and site equipment changes from another stream. Not yet checked in a browser. Backend `POST /api/sites` still to build |

| 2026-11-02 | Tenants and equipment mock screens: `/tenants` (KPIs, filters by operator, country, state, table), `/equipment` (register with filters by category, owner, status, country, plus type catalogue), equipment card on site detail (tenant-owned vs TowerCo shared). Behind `TenantsApi` and `EquipmentApi` contracts, mock in `api/mock/mockEquipment.ts` and `mockTenants.ts`. Backend endpoints needed: `GET /api/tenancies`, `GET /api/equipment?siteId=`, `GET /api/equipment-types`. `npm run build` OK || 2026-11-02 | Demo deck v4 and section 6.3: bigger-picture architecture (enterprise context, six bounded contexts, cross-cutting qualities), sensor collection Azure Functions slide, sensor data moved to Azure Cosmos DB (user decision) across telemetry, system view and domain slides; roadmap adds Phase 8 enterprise integration. All new content "Proposed, not approved". Deck now 26 slides, same URL |

| 2026-11-02 | Demo deck: "Five goals and how we are meeting each" slide added after "Four people, four questions" (4 use cases + explainable, changeable code; approach and status per goal from this log). Added to local deck file and published to same URL on top of the then-live version, so the Cosmos DB and bigger-picture edits stay as the other stream left them, ready for its next publish |
| 2026-11-02 | Phase 1 controllers added (Sites, Operators, Tenancies) reading DbContext directly (demo shortcut; Application services exist but are not used by controllers yet). Repositories + UnitOfWork registered so API starts |
| 2026-11-02 | Work orders added: `WorkOrder` entity on Tower (priority Low/Medium/High; status Open > InProgress > Completed or Cancelled, invalid move 409), seed, `api/work-orders` GET list (towerId/status/priority filters), GET by id, POST, PATCH `{id}/status`. Delete `towerco.db` once to pick up new table |
| 2026-11-02 | Backend completed and wired to every UI feature. Controllers now delegate to Application services (Clean Architecture restored). Endpoints: sites (list, get, POST create with 409 on duplicate code), operators, towers, dashboard (options in `appsettings.json` `Dashboard`), tenancies register (`GET /api/tenancies`) and POST tenant, equipment + equipment-types, work orders (site based; statuses Open, Assigned, InProgress, OnHold, Completed, Cancelled; assign needs engineer 400; invalid move 409). 69 tests green; every endpoint and every UI page checked against the real API. Client still needs `VITE_USE_MOCK_API=false` in `.env` (edit blocked for Claude by deny rule). Delete `towerco.db` once for new schema |
| 2026-11-02 | Endpoint hardening: all routes in `Server/ApiRoutes.cs` (no literals in controllers); every query filter and request validated by DataAnnotations (search length, ISO country code, `state` active/ended, `owner` tenant/shared, ids >= 1) returning 400 ProblemDetails; rule values in `Application/Constants/ValidationRules.cs` and `FilterValues.cs`. Work order custom exceptions `InvalidWorkOrderTitleException`, `WorkOrderAssigneeRequiredException` (400) replace generic validation. New Application tests on seeded in-memory SQLite (tenants, equipment, site create, work orders, validation rules). 97/97 tests pass |
| 2026-11-02 | Tower health (plan 6.1) built end to end. Signals: Telemetry (latest reading per sensor vs `AlarmThresholds`, same config shape as ingestion), Inspection (age 12 months warning, 24 critical, Fail critical; `Health` section), Work orders (any open warning, open Critical priority critical; High kept as warning because the Critical priority level was added after the plan). Overall = worst signal. Stored `SensorReadings` and `Inspections` tables, seeded 30 days of simulated telemetry for Active sites with planted drift. API `GET /api/health/sites?country=&status=`, `GET /api/health/sites/{siteId}`. UI: `/health` page (KPIs, filters, worst-first table) and Tower health card with 30-day sparklines on site detail. Deviations from plan: site based (not tower), work orders reused instead of a new MaintenanceTicket entity, sensor thresholds instead of uptime %. Ingestion function still does not write readings to the DB (next step). 108 tests green |
| 2026-11-02 | Git documentation added: root `README.md` (overview, feature status, run steps, configuration, docs index), `docs/architecture/2026-11-02_19-06-37_backend-architecture.md` (layers, ER and aggregate diagrams, business rules, work order and health flows, API reference, persistence, ingestion, gaps) and `docs/architecture/2026-11-02_19-06-37_ui-architecture.md` (layers, routes, data access and mock switch, state, flows, conventions, gaps). Mermaid diagrams for GitHub. Facts checked against code; 108 tests green |
| 2026-11-02 | Auth marked out of scope: `// AUTH (out of scope for this demo)` comments on every controller (intended policy per persona: Operations, Commercial, Manager) and in `Program.cs` where AddAuthentication and UseAuthentication/UseAuthorization belong. Production readiness review: not production ready (see chat summary); main gaps auth, EF migrations, Azure SQL, ingestion persistence, health checks, CORS, logging/telemetry, concurrency, pagination, CI |
| 2026-11-02 | UI personas (Phase 1, no auth): sidebar persona switcher for Any user (UC3), Commercial (UC2), Operations (UC1), Manager (UC4). Filters nav via optional `personas` tag on `NavItem` and redirects unknown or root URLs to the persona landing route (viewer `/sites`, commercial `/tenants`, operations `/sites`, manager `/dashboard`). Choice kept in localStorage. Routes not guarded, actions not gated (Phase 4). Files in `src/app/`: `personas.ts`, `personaNav.ts`, `personaContext.ts`, `PersonaProvider.tsx`, `usePersona.ts`, `layout/PersonaSwitcher.tsx`, `layout/LandingRedirect.tsx`. `npm run build` OK; not yet checked in a browser; no client unit tests (no runner) |
| 2026-11-02 | Personas revised: Admin persona added (sees every nav item, lands on `/dashboard`); Manager now also sees Work orders; UC codes removed from dropdown labels (`useCase` field dropped) |
| 2026-11-02 | Demo deck v6 synced to latest code: UI wired to real API on every page, 108 tests, eight controllers, rules 1 to 7 built (rule 8 not applied: dashboard totals include decommissioned KE-KSM-0003, so portfolio shows 16/35, 45.7%), work order lifecycle slide now shows the built six-state machine (extensions proposed), monitoring slide shows built SensorReading, AlarmThreshold, Inspection, health rules, ingestion Functions first slice. Equipment presented as tenant-owned only (user decision). **Gap:** code still models and seeds TowerCo shared equipment (`Equipment.TenancyId` nullable, `SeedData.InstallSharedPower`, owner filter `shared`, `SharedEquipmentOwner` label, related tests) |
| 2026-11-02 | Git: repo already initialised on `main` by another stream (3 commits). Root `.gitignore` extended (superset of previous rules): .NET and VS output, test and coverage output, SQLite files, `local.settings.json`, `.env*` (except `.env.example`), certificates and keys, Node and Vite output, tooling scratch. `TowerCo.Ingestion/local.settings.json` and `towerco_mgmt.client/.env` are already tracked, so the new rules do not affect them; untracking awaits user decision. Nothing committed |
| 2026-11-02 | Client-side pagination on all list tables (sites, site tenancies, tenants, equipment, equipment types, work orders, health, operators, tower configuration): `shared/hooks/usePagination.ts`, `shared/components/Pagination.tsx` (range label, rows 10/25/50, Previous/Next, hidden at 10 rows or fewer), sizes in `shared/constants/pagination.ts`. Resets to page 1 when rows change (filters). Dashboard tables and KPIs unchanged. No API change. `npm run build` and lint OK; not yet checked in browser |
| 2026-11-02 | Dashboard fixes: alert severity came back as "Critical" (global `JsonStringEnumConverter` in `Program.cs` overrode the camelCase converter on the enum type), so Critical alerts KPI showed 0 and alert rows had no pill or tint. Converter moved to the `DashboardAlertDto.Severity` property; regression test `DashboardAlertSerialisationTests`. CSS: 6 KPI tiles fit one row, `grid-2` cards no longer stretch, fixed bar value column, table links primary colour and site codes no wrap. 144 tests green, `npm run build` OK |
| 2026-11-02 | Rich domain (core set 1 to 5). Money: non-negative, ISO currency normalised, Add/Times/Annualised, CurrencyMismatchException. SiteLocation: validated country, city, lat/long. Site: lifecycle Planned > UnderConstruction > Active > Decommissioned (no going back; Active needs a tower; Decommission blocked while tenancies active or future), Rename. Tenancy: IsActiveOn respects start date (future leases), IsCommittedFrom, Overlaps, DaysToExpiry, IsExpiringWithin, AnnualValue; End/Renew/ReviewRent only via Tower. Tower: capacity checked over the whole lease period (future-dated leases cannot be oversold), same-operator overlap is a duplicate, RenewTenancy re-checks capacity, ChangeCapacity cannot drop below committed tenancies, MonthlyRevenue per currency. 34 new domain tests; 142 green. Not yet exposed through API/UI |
| 2026-11-02 | Layout: sidebar is now a 72px icon rail (labels as tooltips and aria-label, groups split by dividers, no persona switcher). New top bar (`app/layout/TopBar.tsx`) on the right holds the signed-in user: photo from `public/images/user-photo.jpg` with initials fallback (`UserAvatar`), name from `app/currentUser.ts`, and "Viewing as" persona switcher as dropdown. Auth still out of scope, user is fixed. Free slots KPI tile removed (shown in Utilisation hint); default page size 5; sidebar sticky. Build and lint OK |
| 2026-11-02 | Sidebar expand/collapse: » / « toggle under the logo (`useSidebarExpanded`, choice remembered in localStorage, safe when storage is blocked). Collapsed: 72px icon rail; expanded: 232px with brand, group labels and page names. Toggle hidden on screens 900px or narrower (top row layout). Build and lint OK |
| 2026-11-02 | Equipment is tenant-owned only (user decision). Backend already changed by another stream (`Equipment.TenancyId` required, `InvalidEquipmentOwnerException`, no shared seed, owner filter removed). This stream finished the client: removed `OWNER_TENANT`, `OWNER_SHARED`, `SHARED_OWNER_LABEL`, `OWNER_FILTERS`; `EquipmentItem.tenancyId`/`ownerName` non-null; owner filter removed from Equipment page; site detail equipment grouped by operator; mock equipment no longer adds shared power kit. `npm run build` and lint OK; 153/153 backend tests pass (run via `--artifacts-path` because the running API locked `bin`). Restart the API and delete `towerco.db` once so old shared rows are gone. Deck v7 updated (153 tests, gap removed) |
| 2026-11-02 | Rich domain exposed. API: `PATCH /api/sites/{id}/status`, `PATCH /api/sites/{id}/name`, `PATCH /api/towers/{towerId}/capacity`, `POST /api/towers/{towerId}/tenancies/{tenancyId}/end`, `/renewal`, `/rent-review`; POST tenancy now accepts optional `endDate` (fixed term). Rule breaches return 409 from the domain. UI (site detail): lifecycle buttons and capacity editor for Operations/Admin personas, per-tenancy End/Renew/Review rent for Commercial/Admin (UI gating only; backend auth still out of scope, AUTH comments updated). 11 new service tests; 153 green; client build OK; checked in browser |
| 2026-11-02 | TowerCo site infrastructure (user decision "Separate site infra"): new `SiteInfrastructure` entity on Tower (Kind `Generator`, `Battery`, `Solar`, `Cooling`; SerialNumber unique; InstalledOn), added via `Tower.InstallInfrastructure`, TowerCo-owned, not leased, no capacity impact. Seed: every Active tower gets Solar (rooftop) or Generator (others) plus Battery, serials `{SiteCode}-INF-NNN` (18 rows). `GET /api/sites/{id}` returns `infrastructure`; site detail shows "TowerCo site infrastructure" card separate from tenant equipment. Sensors and health signals not yet linked to it. 156 tests green, client build and lint OK, checked in browser. DB recreated |
| 2026-11-02 | Docs aligned with tenant-only equipment: README overview, backend architecture (ER `TENANCY ||--o{ EQUIPMENT` required, aggregate diagram, entity catalogue, rule 13 `InvalidEquipmentOwnerException` 409, exception hierarchy, equipment filters, seed), UI architecture (site detail groups equipment by operator), CLAUDE.md sections 3, 5 and decision 4. Fixed existing bug: `TowerRepository` read queries used `AsNoTracking`, so `Equipment.Tenancy` was never linked and all items showed as "TowerCo shared"; now `AsNoTrackingWithIdentityResolution`. 153 tests green, client build and lint OK. Delete `towerco.db` and restart API to re-seed |
| 2026-11-02 | Target architecture page v4: sensor data in Azure Cosmos DB (readings, rollups, alarm-state; change feed to Data Lake; Functions TelemetryIngest, Archive, Heartbeat, Simulator); telemetry store moved from open question to decision; data residency added as open question |
| 2026-11-02 | User decision: TowerCo power kit modelled as separate site infrastructure (section 3), not tenant equipment. Requirements brief and target architecture pages updated for tenant-only equipment; generator alarm example now refers to site infrastructure. Backend entity not built yet (forwarded to Backend session) |
