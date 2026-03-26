import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router-dom";
import TaskCreateModal from "../components/TaskCreateModal.jsx";
import { api } from "../lib/api.js";
import { ACTOR_ID, TEAM_ID } from "../lib/config.js";
import {
  clampPct,
  formatDateRange,
  devRoleLabel,
  priorityClass,
  priorityLabel,
  ticketTypeLabel
} from "../lib/utils.js";

const PAGE_SIZE = 200;

const STATUS_COLUMNS = [
  { label: "To Do", status: 1 },
  { label: "In Progress", status: 2 },
  { label: "In Review", status: 3 },
  { label: "Done", status: 4 }
];

export default function SprintBoardPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = Number(query.get("teamId")) || TEAM_ID;

  const [sprints, setSprints] = useState([]);
  const [currentSprint, setCurrentSprint] = useState(null);
  const [tasks, setTasks] = useState([]);
  const [teamMembers, setTeamMembers] = useState([]);
  const [stats, setStats] = useState(null);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);

  const loadData = async () => {
    setLoading(true);
    const sprintRes = await api.get(
      `/api/sprints?TeamId=${teamId}&Page=1&PageSize=50`
    );
    const sprintItems = sprintRes.data?.Items || [];
    setSprints(sprintItems);

    const active =
      sprintItems.find((s) => s.Status === 2) || sprintItems[0] || null;
    setCurrentSprint(active);

    if (active) {
      const statsRes = await api.get(`/api/sprints/${active.Id}/stats`);
      if (statsRes.ok) setStats(statsRes.data);
    } else {
      setStats(null);
    }

    const teamRes = await api.get(
      `/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`
    );
    setTeamMembers(teamRes.data?.Items || []);

    if (active) {
      const tasksRes = await api.get(
        `/api/tasks/team/${teamId}?SprintId=${active.Id}&Page=1&PageSize=${PAGE_SIZE}`
      );
      setTasks(tasksRes.data?.Items || []);
    } else {
      setTasks([]);
    }

    setLoading(false);
  };

  useEffect(() => {
    loadData();
  }, [teamId]);

  const filteredTasks = useMemo(() => {
    if (!search) return tasks;
    const q = search.toLowerCase();
    return tasks.filter((t) => t.Title.toLowerCase().includes(q));
  }, [tasks, search]);

  const completedCount = filteredTasks.filter((t) => t.Status === 4).length;
  const blockedCount = filteredTasks.filter(
    (t) => t.IsBlocked || t.Status === 5
  ).length;
  const sprintVelocity = stats?.CompletedPoints ?? 0;
  const plannedPoints = stats?.PlannedPoints ?? 0;
  const completionRate = plannedPoints
    ? Math.round((sprintVelocity / plannedPoints) * 100)
    : 0;

  const avgUtilization = teamMembers.length
    ? Math.round(
        teamMembers.reduce((sum, m) => sum + (m.WeeklyCapacityPct || 0), 0) /
          teamMembers.length
      )
    : 0;

  const blockedTasks = filteredTasks.filter(
    (t) => t.IsBlocked || t.Status === 5
  );

  const teamCapacity = teamMembers.map((member) => {
    const activeTasks = filteredTasks.filter(
      (t) => t.AssignedToId === member.UserId && t.Status !== 4
    );
    const computedPct = Math.round((activeTasks.length / 10) * 100);
    return {
      ...member,
      activeTasks: activeTasks.length,
      capacityPct:
        member.WeeklyCapacityPct && member.WeeklyCapacityPct > 0
          ? member.WeeklyCapacityPct
          : computedPct
    };
  });

  const burndownProgress = plannedPoints
    ? clampPct(Math.round((sprintVelocity / plannedPoints) * 100))
    : 0;

  const completeSprint = async () => {
    if (!currentSprint) return;
    await api.patch(
      `/api/sprints/${currentSprint.Id}/status?status=Completed`
    );
    await loadData();
  };

  const reassignBlocked = async (task) => {
    const newUserId = window.prompt("Reassign to user id:", "1");
    if (!newUserId) return;
    await api.patch(
      `/api/tasks/${task.Id}/assign?userId=${newUserId}&actorId=${ACTOR_ID}`
    );
    await loadData();
  };

  return (
    <>
      <TaskCreateModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreated={loadData}
        teamId={teamId}
        sprintId={currentSprint?.Id ?? null}
        teamMembers={teamMembers}
      />

      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div className="flex items-center gap-4">
          <div>
            <h2 className="text-[15px]">
              Team Lead Workspace
            </h2>
            <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
              {currentSprint
                ? `${currentSprint.Name} • ${formatDateRange(
                    currentSprint.StartDate,
                    currentSprint.EndDate
                  )}`
                : "No active sprint"}
            </div>
          </div>
        </div>

        <div className="flex items-center gap-3">
          <div className="flex bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md p-[2px]">
            <button className="px-3 py-1.5 rounded-[4px] text-[11px] text-white bg-[var(--bg-element)] border border-[var(--border-highlight)] shadow-[0_2px_8px_rgba(0,0,0,0.2)]">
              My Team
            </button>
            <button className="px-3 py-1.5 rounded-[4px] text-[11px] text-[var(--text-secondary)] hover:text-white transition-colors">
              Cross-functional
            </button>
          </div>
          <button className="px-3 py-1.5 rounded-md text-[12px] bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] text-[var(--text-secondary)] hover:text-white hover:bg-[var(--bg-element)] transition-all flex items-center gap-2">
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <polygon points="22 3 2 3 10 12.46 10 19 14 21 14 12.46 22 3"></polygon>
            </svg>
            Filter
          </button>
          <button
            className="px-3 py-1.5 rounded-md text-[12px] bg-[#8A2BE2]/20 border border-[#8A2BE2]/40 text-[#B475FF] hover:bg-[#8A2BE2]/30 transition-all flex items-center gap-1.5 shadow-[0_0_15px_rgba(138,43,226,0.15)]"
            onClick={() => setModalOpen(true)}
          >
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2.5"
            >
              <line x1="12" y1="5" x2="12" y2="19"></line>
              <line x1="5" y1="12" x2="19" y2="12"></line>
            </svg>
            Add Task
          </button>
        </div>
      </header>

      <div className="grid grid-cols-4 gap-4 flex-shrink-0">
        <div className="glass-panel p-4 flex flex-col gap-3">
          <div className="text-[11px] text-muted flex justify-between items-center">
            Sprint Velocity
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"></polyline>
            </svg>
          </div>
          <div className="text-[24px] tracking-tight leading-none">
            {sprintVelocity}
            <span className="text-muted text-[14px]"> pts</span>
          </div>
          <div className="status-pill">
            <div className="status-dot success"></div> On Track
          </div>
        </div>

        <div className="glass-panel p-4 flex flex-col gap-3">
          <div className="text-[11px] text-muted flex justify-between items-center">
            Team Utilization
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
              <circle cx="9" cy="7" r="4"></circle>
              <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
            </svg>
          </div>
          <div className="text-[24px] tracking-tight leading-none">
            {avgUtilization}
            <span className="text-muted text-[14px]">%</span>
          </div>
          <div className="status-pill">
            <div className="status-dot success"></div> Healthy
          </div>
        </div>

        <div className="glass-panel p-4 flex flex-col gap-3">
          <div className="text-[11px] text-muted flex justify-between items-center">
            Tasks Completed
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <polyline points="20 6 9 17 4 12"></polyline>
            </svg>
          </div>
          <div className="text-[24px] tracking-tight leading-none">
            {completedCount}
          </div>
          <div className="status-pill border-none bg-transparent p-0 pl-1">
            <span className="text-[11px] text-[var(--text-tertiary)]">
              This Sprint
            </span>
          </div>
        </div>

        <div className="glass-panel p-4 flex flex-col gap-3 border-[var(--color-amber)]/30 pulse-warning">
          <div className="text-[11px] text-muted flex justify-between items-center text-[var(--color-amber)]">
            Blocked Items
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              strokeWidth="2"
            >
              <circle cx="12" cy="12" r="10"></circle>
              <line x1="12" y1="8" x2="12" y2="12"></line>
              <line x1="12" y1="16" x2="12.01" y2="16"></line>
            </svg>
          </div>
          <div className="text-[24px] tracking-tight leading-none text-white">
            {blockedCount}
          </div>
          <div
            className="status-pill"
            style={{
              borderColor: "rgba(255, 189, 46, 0.3)",
              background: "rgba(255, 189, 46, 0.05)"
            }}
          >
            <div className="status-dot warning"></div> Review Needed
          </div>
        </div>
      </div>

      <div className="grid grid-cols-4 gap-4 flex-shrink-0">
        <div className="glass-panel col-span-2 flex flex-col overflow-hidden">
          <div className="px-4 py-3 border-b border-[var(--border-subtle)] flex justify-between items-center">
            <h3 className="text-[12px] font-medium tracking-wide">
              Team Capacity
            </h3>
            <div className="flex gap-2">
              <span className="flex items-center gap-1 text-[10px] text-[var(--text-tertiary)]">
                <span className="w-1.5 h-1.5 rounded-full bg-[var(--color-green)]"></span>{" "}
                &lt;80%
              </span>
              <span className="flex items-center gap-1 text-[10px] text-[var(--text-tertiary)]">
                <span className="w-1.5 h-1.5 rounded-full bg-[var(--color-amber)]"></span>{" "}
                80-95%
              </span>
              <span className="flex items-center gap-1 text-[10px] text-[var(--text-tertiary)]">
                <span className="w-1.5 h-1.5 rounded-full bg-[var(--color-red)]"></span>{" "}
                &gt;95%
              </span>
            </div>
          </div>
          <div className="p-2 flex flex-col gap-1">
            {teamCapacity.length ? (
              teamCapacity.map((member) => {
                const pct = clampPct(member.capacityPct || 0);
                const colorClass =
                  pct > 95
                    ? "cap-red"
                    : pct >= 80
                    ? "cap-amber"
                    : "cap-green";
                return (
                  <div
                    key={member.Id}
                    className="flex items-center justify-between p-2 hover:bg-[var(--bg-element-hover)] rounded-lg transition-colors group"
                  >
                    <div className="flex items-center gap-3 w-[140px]">
                      <div className="avatar relative">U{member.UserId}</div>
                      <div className="flex flex-col">
                        <span className="text-[12px] text-white">
                          User #{member.UserId}
                        </span>
                        <span className="text-[10px] text-[var(--text-tertiary)]">
                          {devRoleLabel(member.DevRole)}
                        </span>
                      </div>
                    </div>
                    <div className="flex-1 px-4 flex items-center gap-3">
                      <div className="h-1.5 w-full capacity-bar-bg rounded-full overflow-hidden">
                        <div
                          className={`h-full rounded-full capacity-bar-fill ${colorClass}`}
                          style={{ width: `${pct}%` }}
                        ></div>
                      </div>
                      <span className="text-[11px] font-mono text-[var(--text-secondary)] w-[30px]">
                        {pct}%
                      </span>
                    </div>
                    <div className="w-[60px] text-right text-[11px] text-[var(--text-secondary)]">
                      {member.activeTasks} tasks
                    </div>
                  </div>
                );
              })
            ) : (
              <div className="p-4 text-[11px] text-[var(--text-tertiary)]">
                No team members yet.
              </div>
            )}
          </div>
        </div>

        <div className="glass-panel flex flex-col p-4 relative overflow-hidden">
          <div className="absolute top-0 right-0 w-32 h-32 bg-[var(--accent-purple-glow)] rounded-full blur-[50px] opacity-20 -mr-10 -mt-10 pointer-events-none"></div>
          <div className="text-[11px] text-muted mb-1">Sprint Timeline</div>
          <div className="text-[15px] font-medium mb-4">Burndown Preview</div>

          <div className="flex-1 flex flex-col justify-end">
            <svg viewBox="0 0 100 50" className="w-full h-full overflow-visible">
              <line
                x1="0"
                y1="0"
                x2="100"
                y2="0"
                stroke="var(--border-subtle)"
                strokeWidth="0.5"
              ></line>
              <line
                x1="0"
                y1="25"
                x2="100"
                y2="25"
                stroke="var(--border-subtle)"
                strokeWidth="0.5"
              ></line>
              <line
                x1="0"
                y1="50"
                x2="100"
                y2="50"
                stroke="var(--border-subtle)"
                strokeWidth="0.5"
              ></line>

              <polyline
                points="0,5 100,45"
                fill="none"
                stroke="var(--text-tertiary)"
                strokeWidth="1"
                strokeDasharray="2,2"
              ></polyline>

              {plannedPoints ? (
                <>
                  <polygon
                    points={`0,5 50,${
                      50 - burndownProgress * 0.45
                    } 65,${50 - burndownProgress * 0.45} 65,50 0,50`}
                    fill="url(#purpleGrad)"
                    opacity="0.2"
                  ></polygon>
                  <polyline
                    points={`0,5 50,${
                      50 - burndownProgress * 0.45
                    } 65,${50 - burndownProgress * 0.45}`}
                    fill="none"
                    stroke="var(--accent-purple-light)"
                    strokeWidth="2"
                  ></polyline>
                  <circle
                    cx="65"
                    cy={50 - burndownProgress * 0.45}
                    r="3"
                    fill="var(--bg-surface-solid)"
                    stroke="var(--accent-purple-light)"
                    strokeWidth="2"
                    className="pulse-border-purple"
                  ></circle>
                </>
              ) : null}

              <defs>
                <linearGradient id="purpleGrad" x1="0" y1="0" x2="0" y2="1">
                  <stop
                    offset="0%"
                    stopColor="var(--accent-purple-light)"
                  ></stop>
                  <stop offset="100%" stopColor="transparent"></stop>
                </linearGradient>
              </defs>
            </svg>
            <div className="flex justify-between text-[10px] text-[var(--text-tertiary)] mt-2 font-mono">
              <span>W1</span>
              <span>W2</span>
              <span className="text-[var(--accent-purple-light)]">
                Now ({completionRate}%)
              </span>
              <span>W4</span>
            </div>
          </div>
        </div>

        <div
          className="glass-panel flex flex-col"
          style={{ background: "var(--bg-surface-solid)", padding: 0 }}
        >
          <div className="terminal-header">
            <div className="window-controls">
              <div className="dot" style={{ background: "#FF5F56" }}></div>
              <div className="dot" style={{ background: "#FFBD2E" }}></div>
              <div className="dot" style={{ background: "#27C93F" }}></div>
            </div>
            <div className="font-mono text-[10px] color-[var(--text-tertiary)] opacity-70">
              sys_alerts ~ blockers
            </div>
          </div>
          <div className="flex-1 p-3 font-mono text-[11px] leading-relaxed text-[var(--text-tertiary)] overflow-y-auto">
            {blockedTasks.length ? (
              blockedTasks.slice(0, 3).map((task) => (
                <div key={task.Id} className="mb-2">
                  <span className="text-[var(--color-amber)]">
                    &gt; [WARN]
                  </span>{" "}
                  <span className="text-white">{task.TicketCode}</span>:{" "}
                  {task.BlockedReason || task.Title}
                  <div className="pl-3 mt-1 text-[var(--text-secondary)] opacity-80">
                    - Assigned: User #{task.AssignedToId}
                  </div>
                  <div className="pl-3 flex gap-2 mt-1">
                    <button
                      className="text-[9px] bg-[var(--bg-element)] px-1.5 py-0.5 rounded border border-[var(--border-subtle)] hover:text-white"
                      onClick={() => reassignBlocked(task)}
                    >
                      Reassign
                    </button>
                    <button className="text-[9px] bg-[var(--bg-element)] px-1.5 py-0.5 rounded border border-[var(--border-subtle)] hover:text-white">
                      Ping
                    </button>
                  </div>
                </div>
              ))
            ) : (
              <div className="text-[var(--text-secondary)]">
                &gt; No blocked items
              </div>
            )}
          </div>
        </div>
      </div>

      <div className="flex items-center justify-between">
        <div className="flex items-center gap-3">
          <div className="flex items-center bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-2 py-1 gap-1">
            <svg
              width="12"
              height="12"
              viewBox="0 0 24 24"
              fill="none"
              stroke="var(--text-tertiary)"
              strokeWidth="2"
            >
              <circle cx="11" cy="11" r="8"></circle>
              <line x1="21" y1="21" x2="16.65" y2="16.65"></line>
            </svg>
            <input
              type="text"
              placeholder="Search tasks..."
              className="bg-transparent border-none outline-none text-[11px] w-32 placeholder:text-[var(--text-tertiary)]"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
          <button
            className="px-3 py-1.5 rounded-md text-[12px] bg-[var(--bg-element)] border border-[var(--border-subtle)] text-[var(--text-primary)] hover:bg-[var(--bg-element-hover)] transition-all flex items-center gap-1.5"
            onClick={completeSprint}
            disabled={!currentSprint}
          >
            Complete Sprint
          </button>
        </div>
        <div className="text-[11px] text-[var(--text-secondary)]">
          {loading ? "Loading..." : `${filteredTasks.length} Active Tasks`}
        </div>
      </div>

      <div className="grid grid-cols-4 gap-4 flex-1 overflow-hidden pb-4">
        {STATUS_COLUMNS.map((col) => (
          <KanbanColumn
            key={col.status}
            title={col.label}
            tasks={filteredTasks.filter((t) => t.Status === col.status)}
          />
        ))}
      </div>
    </>
  );
}

function KanbanColumn({ title, tasks }) {
  return (
    <div className="flex flex-col gap-2 overflow-hidden">
      <div className="column-header">
        <div className="flex items-center gap-2">
          <span className="label-caps">{title}</span>
          <span className="text-[10px] text-[var(--text-tertiary)] bg-[var(--bg-element)] px-1.5 rounded">
            {tasks.length}
          </span>
        </div>
        <svg
          width="12"
          height="12"
          viewBox="0 0 24 24"
          fill="none"
          stroke="var(--text-tertiary)"
          strokeWidth="2"
        >
          <circle cx="12" cy="12" r="1"></circle>
          <circle cx="19" cy="12" r="1"></circle>
          <circle cx="5" cy="12" r="1"></circle>
        </svg>
      </div>
      <div className="flex-1 flex flex-col gap-3 overflow-y-auto pr-1">
        {tasks.length ? (
          tasks.map((task) => (
            <div key={task.Id} className="kanban-card flex flex-col gap-3">
              <div className="flex justify-between items-start">
                <span className="tag">{task.TicketCode}</span>
                <span className="point-badge">{task.StoryPoints ?? 0}</span>
              </div>
              <div className="text-[13px] leading-snug">{task.Title}</div>
              <div className="flex justify-between items-center">
                <span className={`tag ${priorityClass(task.Priority)}`}>
                  {priorityLabel(task.Priority)}
                </span>
                <div className="flex items-center gap-2">
                  <div className="text-[9px] text-[var(--text-tertiary)]">
                    {ticketTypeLabel(task.TicketType)}
                  </div>
                  <div className="avatar">U{task.AssignedToId}</div>
                </div>
              </div>
            </div>
          ))
        ) : (
          <div className="text-[11px] text-[var(--text-tertiary)] px-2">
            No tasks
          </div>
        )}
      </div>
    </div>
  );
}
