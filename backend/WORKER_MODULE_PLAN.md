# Worker Module Plan

This file explains what is left for Zulhija's part of the backend and what each task means.

## Scope

This module is for the `Worker` side only.

Main tables for this module:

- `Invitation`
- `JoinRequest`
- `TaskComment`
- `Absence`
- `Notification`
- `DaySummary`
- `ScheduleEvent`

Main interfaces for this module:

- `IInvitationService`
- `IJoinRequestService`
- `ITaskCommentService`
- `IAbsenceService`
- `INotificationService`
- `IDaySummaryService`
- `IScheduleEventService`

Auth part like `register`, `login`, `JWT`, `Authorize`, role seeding is not this module's responsibility if Umar owns common auth.

## What "create worker controllers" means

Controller = the API file that receives HTTP requests from frontend/Postman and calls the service.

Example:

- frontend sends `GET /api/worker/invitations`
- controller receives request
- controller calls service
- service reads data from database
- controller returns result

For this module you will likely need controllers like:

- `InvitationController`
- `JoinRequestController`
- `TaskCommentController`
- `AbsenceController`
- `NotificationController`
- `DaySummaryController`
- `ScheduleEventController`

You can also group some worker endpoints into one controller later, but for now one controller per table is the cleanest.

## What "write service methods" means

Service = business logic layer.

Controller should stay thin.
Service should do the real work.

For every service, these basic methods were mentioned:

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`

These are generic CRUD methods.

Meaning:

- `AddAsync` = create new record
- `UpdateAsync` = edit existing record
- `DeleteAsync` = remove record
- `GetAllAsync` = return list
- `GetByIdAsync` = return one item by id

## What "add worker-specific logic" means

This means not only basic CRUD.
It means real worker actions.

### 1. Accept/Reject invitation

Worker gets invitation to team.

Needed logic:

- find invitation by id
- check invitation belongs to current worker
- if accepted:
  - set invitation status to accepted
  - add worker to `TeamMember`
- if rejected:
  - set invitation status to rejected

Suggested methods:

- `AcceptInvitationAsync(int invitationId, int currentUserId)`
- `RejectInvitationAsync(int invitationId, int currentUserId)`

### 2. Apply to team

Worker wants to join a team.

Needed logic:

- create new `JoinRequest`
- save `TeamId`
- save `UserId`
- save `CoverMessage`
- default status should be pending

Suggested method:

- `ApplyToTeamAsync(CreateJoinRequestDto dto)`

### 3. Request absence

Worker wants temporary leave.

Needed logic:

- create `Absence`
- save reason
- save from/to dates
- status = pending

Suggested method:

- `RequestAbsenceAsync(CreateAbsenceDto dto)`

### 4. Add comment

Worker leaves comment on task.

Needed logic:

- create `TaskComment`
- save task id
- save author id
- save message
- save type

Suggested method:

- `AddCommentAsync(CreateTaskCommentDto dto)`

Special case:

If worker says "I can't do this task", comment type can be `CantComplete`.

### 5. Mark notification as read

Worker opens notification.

Needed logic:

- find notification by id
- check notification belongs to current user
- set `IsRead = true`

Suggested method:

- `MarkAsReadAsync(int notificationId, int currentUserId)`

### 6. Manage day summary

Worker daily report / productivity summary.

Needed logic:

- create day summary
- update day summary
- get all summaries for worker
- get one summary
- optionally delete summary

Suggested methods:

- `AddAsync(CreateDaySummaryDto dto)`
- `UpdateAsync(int id, UpdateDaySummaryDto dto)`
- `DeleteAsync(int id)`
- `GetAllAsync()`
- `GetByIdAsync(int id)`

### 7. Manage schedule events

Worker manages personal schedule / task schedule blocks.

Needed logic:

- create event
- update event
- delete event
- get all events
- get event by id

Suggested methods:

- `AddAsync(CreateScheduleEventDto dto)`
- `UpdateAsync(int id, UpdateScheduleEventDto dto)`
- `DeleteAsync(int id)`
- `GetAllAsync()`
- `GetByIdAsync(int id)`

## What "connect services to DI" means

DI = Dependency Injection.

This means registering services in `Program.cs` so controllers can use them.

Example:

```csharp
builder.Services.AddScoped<IInvitationService, InvitationService>();
builder.Services.AddScoped<IJoinRequestService, JoinRequestService>();
builder.Services.AddScoped<ITaskCommentService, TaskCommentService>();
builder.Services.AddScoped<IAbsenceService, AbsenceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IDaySummaryService, DaySummaryService>();
builder.Services.AddScoped<IScheduleEventService, ScheduleEventService>();
```

Without this, controllers cannot receive services in constructor.

## What "later test endpoints" means

After controllers and services are ready, you test with Postman or Swagger.

Examples:

- create invitation
- accept invitation
- reject invitation
- apply to team
- create absence request
- create comment
- mark notification read
- create day summary
- create schedule event

You test:

- correct success response
- wrong id
- duplicate request
- unauthorized access
- data saved correctly in database

## Recommended order

Do work in this order so it is easier:

1. Implement services
2. Register services in DI
3. Create controllers
4. Test endpoints

## Service implementation checklist

### Invitation

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`
- `AcceptInvitationAsync`
- `RejectInvitationAsync`

### JoinRequest

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`
- `ApplyToTeamAsync`

### TaskComment

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`
- `AddCommentAsync`

### Absence

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`
- `RequestAbsenceAsync`

### Notification

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`
- `MarkAsReadAsync`

### DaySummary

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`

### ScheduleEvent

- `AddAsync`
- `UpdateAsync`
- `DeleteAsync`
- `GetAllAsync`
- `GetByIdAsync`

## Controller endpoint idea

Example route ideas:

- `GET /api/invitations`
- `GET /api/invitations/{id}`
- `POST /api/invitations`
- `PUT /api/invitations/{id}`
- `DELETE /api/invitations/{id}`
- `PATCH /api/invitations/{id}/accept`
- `PATCH /api/invitations/{id}/reject`

- `POST /api/joinrequests/apply`

- `POST /api/taskcomments`

- `POST /api/absences/request`

- `PATCH /api/notifications/{id}/read`

- `GET /api/daysummaries`
- `POST /api/daysummaries`

- `GET /api/scheduleevents`
- `POST /api/scheduleevents`

## Short summary

What is left for this module:

- implement 7 services
- add special worker actions
- create 7 controllers
- register services in DI
- test all endpoints

If you want to move faster, start with:

1. `Invitation`
2. `JoinRequest`
3. `TaskComment`
4. `Absence`
5. `Notification`
6. `DaySummary`
7. `ScheduleEvent`

That is the cleanest order for this worker module.
