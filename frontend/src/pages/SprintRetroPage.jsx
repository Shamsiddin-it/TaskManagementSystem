import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router-dom";
import { api } from "../lib/api.js";
import { ACTOR_ID, TEAM_ID } from "../lib/config.js";
import { formatDateRange } from "../lib/utils.js";

const PAGE_SIZE = 200;

export default function SprintRetroPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = Number(query.get("teamId")) || TEAM_ID;

  const [sprints, setSprints] = useState([]);
  const [sprint, setSprint] = useState(null);
  const [stats, setStats] = useState(null);
  const [tasks, setTasks] = useState([]);
  const [retro, setRetro] = useState(null);
  const [actionItems, setActionItems] = useState([]);
  const [notes, setNotes] = useState("");
  const [saving, setSaving] = useState(false);

  const loadData = async () => {
    const sprintRes = await api.get(
      `/api/sprints?TeamId=${teamId}&Page=1&PageSize=50`
    );
    const sprintItems = sprintRes.data?.Items || [];
    const completed =
      sprintItems.find((s) => s.Status === 3) || sprintItems[0] || null;
    setSprints(sprintItems);
    setSprint(completed);

    if (!completed) {
      setStats(null);
      setTasks([]);
      setRetro(null);
      setActionItems([]);
      setNotes("");
      return;
    }

    const statsRes = await api.get(`/api/sprints/${completed.Id}/stats`);
    if (statsRes.ok) setStats(statsRes.data);

    const tasksRes = await api.get(
      `/api/tasks/team/${teamId}?SprintId=${completed.Id}&Page=1&PageSize=${PAGE_SIZE}`
    );
    setTasks(tasksRes.data?.Items || []);

    const retroRes = await api.get(
      `/api/sprint-retros/sprint/${completed.Id}`
    );
    if (retroRes.ok) {
      setRetro(retroRes.data);
      setNotes(retroRes.data?.Notes || "");
      const actionsRes = await api.get(
        `/api/retro-action-items?RetroId=${retroRes.data.Id}&Page=1&PageSize=100`
      );
      setActionItems(actionsRes.data?.Items || actionsRes.data || []);
    } else {
      setRetro(null);
      setActionItems([]);
      setNotes("");
    }
  };

  useEffect(() => {
    loadData();
  }, [teamId]);

  const plannedPoints = stats?.PlannedPoints ?? 0;
  const completedPoints = stats?.CompletedPoints ?? 0;
  const spillover = stats?.SpilloverPoints ?? Math.max(0, plannedPoints - completedPoints);
  const completionRate = plannedPoints
    ? Math.round((completedPoints / plannedPoints) * 1000) / 10
    : 0;

  const contributions = useMemo(() => {
    const map = new Map();
    tasks.forEach((task) => {
      if (task.Status !== 4) return;
      const points = task.StoryPoints ?? 0;
      const prev = map.get(task.AssignedToId) || 0;
      map.set(task.AssignedToId, prev + points);
    });
    const entries = Array.from(map.entries()).map(([userId, points]) => ({
      userId,
      points
    }));
    entries.sort((a, b) => b.points - a.points);
    return entries;
  }, [tasks]);

  const maxPoints = Math.max(1, ...contributions.map((c) => c.points));

  const saveActionPlan = async () => {
    if (!sprint) return;
    setSaving(true);

    if (!retro) {
      const createRes = await api.post("/api/sprint-retros", {
        SprintId: sprint.Id,
        CreatedById: ACTOR_ID,
        WentWell: retro?.WentWell || null,
        BlockedSummary: retro?.BlockedSummary || null,
        Notes: notes || null
      });
      if (createRes.ok) setRetro(createRes.data);
    } else {
      await api.put(`/api/sprint-retros/${retro.Id}`, {
        Id: retro.Id,
        SprintId: retro.SprintId,
        CreatedById: retro.CreatedById,
        WentWell: retro.WentWell,
        BlockedSummary: retro.BlockedSummary,
        Notes: notes || null
      });
    }

    setSaving(false);
    await loadData();
  };

  return (
    <>
      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div>
          <h2 className="text-[15px]">
            {sprint ? `${sprint.Name} Retrospective` : "Sprint Retrospective"}
          </h2>
          <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
            {sprint
              ? `${formatDateRange(sprint.StartDate, sprint.EndDate)} • ${
                  sprint.Status === 3 ? "Complete" : "In Progress"
                }`
              : "No sprint data"}
          </div>
        </div>
        <div className="flex gap-2">
          <button className="px-3 py-1.5 rounded-md text-[12px] bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] text-[var(--text-secondary)] hover:text-white transition-all flex items-center gap-2">
            <svg
              width="14"
              height="14"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
              <polyline points="7 10 12 15 17 10"></polyline>
              <line x1="12" y1="15" x2="12" y2="3"></line>
            </svg>
            Export PDF
          </button>
          <button className="px-3 py-1.5 rounded-md text-[12px] bg-[#8A2BE2]/20 border border-[#8A2BE2]/40 text-[#B475FF]">
            Share with Stakeholders
          </button>
        </div>
      </header>

      <div className="grid grid-cols-3 gap-4 h-full overflow-hidden">
        <div className="col-span-2 flex flex-col gap-4 overflow-y-auto pr-1">
          <div className="glass-panel p-5 grid grid-cols-3 gap-8">
            <div className="col-span-2">
              <span className="label-caps mb-4 block">Velocity Comparison</span>
              <div className="flex items-end gap-6 h-32">
                <div className="flex flex-col items-center gap-2 flex-1 group">
                  <div
                    className="w-full bg-[var(--bg-element)] border border-[var(--border-subtle)] rounded-t-lg relative"
                    style={{ height: "100%" }}
                  >
                    <div
                      className="absolute bottom-0 left-0 right-0 bg-white/5 rounded-t-lg"
                      style={{
                        height: plannedPoints
                          ? `${completionRate}%`
                          : "0%"
                      }}
                    ></div>
                    <span className="absolute -top-6 left-1/2 -translate-x-1/2 text-[10px] text-muted">
                      Planned
                    </span>
                  </div>
                  <span className="text-[14px] font-mono">
                    {plannedPoints} pts
                  </span>
                </div>
                <div className="flex flex-col items-center gap-2 flex-1">
                  <div
                    className="w-full bg-[var(--accent-purple)]/20 border border-[var(--border-purple)] rounded-t-lg relative shadow-[0_0_20px_rgba(138,43,226,0.1)]"
                    style={{ height: `${completionRate || 0}%` }}
                  >
                    <div className="absolute bottom-0 left-0 right-0 bg-[var(--accent-purple)]/30 h-full rounded-t-lg"></div>
                    <span className="absolute -top-6 left-1/2 -translate-x-1/2 text-[10px] text-[var(--accent-purple-light)]">
                      Completed
                    </span>
                  </div>
                  <span className="text-[14px] font-mono text-[var(--accent-purple-light)]">
                    {completedPoints} pts
                  </span>
                </div>
              </div>
            </div>
            <div className="flex flex-col justify-center border-l border-[var(--border-subtle)] pl-8">
              <div className="mb-4">
                <span className="text-[11px] text-muted block mb-1">
                  Completion Rate
                </span>
                <span className="text-2xl font-medium">
                  {completionRate}%
                </span>
              </div>
              <div>
                <span className="text-[11px] text-muted block mb-1">
                  Spillover
                </span>
                <span className="text-2xl font-medium text-[var(--color-amber)]">
                  {spillover} pts
                </span>
              </div>
            </div>
          </div>

          <div className="glass-panel p-5">
            <span className="label-caps mb-4 block">Individual Contributions</span>
            <div className="grid grid-cols-1 gap-1">
              {contributions.length ? (
                contributions.map((item) => (
                  <div
                    key={item.userId}
                    className="flex items-center justify-between p-2 hover:bg-[var(--bg-element-hover)] rounded-lg"
                  >
                    <div className="flex items-center gap-3 w-40">
                      <div className="avatar">U{item.userId}</div>
                      <span className="text-[12px]">User #{item.userId}</span>
                    </div>
                    <div className="flex-1 px-4 flex items-center gap-4">
                      <div className="velocity-bar flex-1">
                        <div
                          className="h-full bg-[var(--accent-purple)]"
                          style={{
                            width: `${Math.round(
                              (item.points / maxPoints) * 100
                            )}%`
                          }}
                        ></div>
                      </div>
                      <span className="text-[11px] font-mono w-12 text-right">
                        {item.points} pts
                      </span>
                    </div>
                  </div>
                ))
              ) : (
                <div className="text-[11px] text-[var(--text-tertiary)] px-2">
                  No completed tasks with story points.
                </div>
              )}
            </div>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="glass-panel flex flex-col">
              <div className="p-3 border-b border-[var(--border-subtle)] flex items-center gap-2">
                <div className="status-dot success"></div>
                <span className="label-caps">What Went Well</span>
              </div>
              <div className="p-4 flex flex-col gap-3">
                <div className="terminal-block p-3 text-[var(--text-secondary)]">
                  {retro?.WentWell ? (
                    retro.WentWell.split("\n").map((line, idx) => (
                      <div key={idx} className="mb-2">
                        <span className="text-[var(--color-green)]">+</span>{" "}
                        {line}
                      </div>
                    ))
                  ) : (
                    <div className="text-[11px] text-[var(--text-tertiary)]">
                      No highlights recorded.
                    </div>
                  )}
                </div>
              </div>
            </div>
            <div className="glass-panel flex flex-col">
              <div className="p-3 border-b border-[var(--border-subtle)] flex items-center gap-2">
                <div className="status-dot warning"></div>
                <span className="label-caps">Blocked Items Summary</span>
              </div>
              <div className="p-4 flex flex-col gap-3">
                <div className="terminal-block p-3 text-[var(--text-secondary)]">
                  {retro?.BlockedSummary ? (
                    retro.BlockedSummary.split("\n").map((line, idx) => (
                      <div key={idx} className="mb-2">
                        <span className="text-[var(--color-red)]">-</span>{" "}
                        {line}
                      </div>
                    ))
                  ) : (
                    <div className="text-[11px] text-[var(--text-tertiary)]">
                      No blockers recorded.
                    </div>
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>

        <div className="glass-panel flex flex-col overflow-hidden">
          <div className="p-4 border-b border-[var(--border-subtle)] bg-white/5">
            <span className="label-caps">Lead Action Items</span>
          </div>
          <div className="flex-1 p-4 overflow-y-auto">
            {actionItems.length ? (
              actionItems.map((item) => (
                <div key={item.Id} className="action-item">
                  <div className="text-[12px] text-white font-medium mb-1">
                    {item.Title}
                  </div>
                  <div className="text-[11px] text-muted">
                    {item.Description || "No description"}
                  </div>
                  <div className="mt-2 flex justify-between items-center">
                    <span className="text-[9px] px-1.5 py-0.5 bg-white/5 rounded border border-white/10 text-muted uppercase tracking-wider">
                      {item.Priority === 3
                        ? "High Priority"
                        : item.Priority === 2
                        ? "Medium Priority"
                        : "Low Priority"}
                    </span>
                    <span className="text-[10px] text-[var(--accent-purple-light)]">
                      {item.DueDate ? new Date(item.DueDate).toLocaleDateString() : "No date"}
                    </span>
                  </div>
                </div>
              ))
            ) : (
              <div className="text-[11px] text-[var(--text-tertiary)]">
                No action items yet.
              </div>
            )}

            <div className="mt-6">
              <span className="label-caps block mb-3">Retro Notes</span>
              <textarea
                className="w-full h-40 bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md p-3 text-[11px] text-[var(--text-secondary)] resize-none focus:outline-none focus:border-[var(--accent-purple)] transition-colors"
                placeholder="Add sprint reflections..."
                value={notes}
                onChange={(e) => setNotes(e.target.value)}
              ></textarea>
            </div>
          </div>
          <div className="p-4 border-t border-[var(--border-subtle)]">
            <button
              className="w-full py-2 bg-[var(--bg-element)] hover:bg-[var(--bg-element-hover)] border border-[var(--border-subtle)] rounded-md text-[11px] font-medium transition-colors"
              onClick={saveActionPlan}
              disabled={saving}
            >
              {saving ? "Saving..." : "Save Action Plan"}
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
