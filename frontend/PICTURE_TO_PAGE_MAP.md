# Your mockups → Blazor routes (13 picture groups)

Static UI only (no new API wiring in this pass). Login first, then use the **sidebar**.

| # | What you showed (short) | Route |
|---|-------------------------|-------|
| 1 | Worker **empty** dashboard + metrics + quick notes + quote | `/workspace` |
| 2 | **Today’s focus** (task cards, templates, deadlines, activity) | `/workspace/today` |
| 3 | **Task detail** (timer, subtasks, files, comments, attributes) | `/workspace/tasks/1` |
| 4 | **Weekly schedule** + today’s agenda | `/workspace/schedule` |
| 5 | **Team directory** (cards, workload, skills, feed) | `/workspace/team` |
| 6 | **Activity** timeline | `/workspace/activity` |
| 7 | **End of day** wrap-up + productivity sidebar | `/workspace/end-of-day` |
| 8 | **Notifications** center (filters + cards) | `/workspace/notifications` |
| 9 | **Settings** (profile + focus + integrations tabs) | `/workspace/settings` |
| 10 | **Backlog** columns + drop zone | `/workspace/backlog` |
| 11 | **Import / Jira** form | `/workspace/import` |
| 12 | **Employer** dashboard (budget / runway) | `/employer/dashboard` |
| 13 | **Employer** project detail (Gantt, risks, checklist, discussion) | `/employer/project` |

**Default after login:** `/workspace/today` (main “working” hub).

**Employer shell** uses a gold-accent sidebar; **worker** uses purple Nexus shell.
