import { useEffect, useMemo, useState } from "react";
import { useLocation } from "react-router-dom";
import TaskCreateModal from "../components/TaskCreateModal.jsx";
import { api } from "../lib/api.js";
import { priorityClass, priorityLabel, ticketTypeLabel } from "../lib/utils.js";

const STATUS_COLUMNS = [
  { label: "To Do", status: "Todo" },
  { label: "In Progress", status: "InProgress" },
  { label: "Review", status: "Review" },
  { label: "Done", status: "Done" }
];

function asList(payload) {
  if (Array.isArray(payload)) return payload;
  if (Array.isArray(payload?.items)) return payload.items;
  if (Array.isArray(payload?.Items)) return payload.Items;
  if (Array.isArray(payload?.data)) return payload.data;
  return [];
}

function normalizeSprint(item) {
  return {
    id: item.id ?? item.Id,
    name: item.name ?? item.Name,
    status: item.status ?? item.Status,
    startDate: item.startDate ?? item.StartDate,
    endDate: item.endDate ?? item.EndDate
  };
}

function normalizeTask(item) {
  return {
    id: item.id ?? item.Id,
    ticketCode: item.ticketCode ?? item.TicketCode,
    title: item.title ?? item.Title,
    description: item.description ?? item.Description,
    assignedToId: item.assignedToId ?? item.AssignedToId ?? item.assignedTo ?? item.AssignedTo,
    status: item.status ?? item.Status,
    priority: item.priority ?? item.Priority,
    ticketType: item.ticketType ?? item.TicketType,
    deadline: item.deadline ?? item.Deadline,
    storyPoints: item.storyPoints ?? item.StoryPoints,
    isBlocked: item.isBlocked ?? item.IsBlocked,
    blockedReason: item.blockedReason ?? item.BlockedReason
  };
}

function normalizeMember(item) {
  return {
    id: item.id ?? item.Id,
    teamId: item.teamId ?? item.TeamId,
    userId: item.userId ?? item.UserId,
    fullName: item.fullName ?? item.FullName ?? `User #${item.userId ?? item.UserId}`,
    avatarInitials: item.avatarInitials ?? item.AvatarInitials ?? "U",
    devRole: item.devRole ?? item.DevRole,
    weeklyCapacityPct: item.weeklyCapacityPct ?? item.WeeklyCapacityPct ?? 0
  };
}

export default function SprintBoardPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const storedUser = (() => {
    try {
      return JSON.parse(localStorage.getItem("user") || "null");
    } catch {
      return null;
    }
  })();

  const teamId =
    query.get("teamId") ||
    localStorage.getItem("activeTeamId") ||
    storedUser?.teamId ||
    null;

  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [search, setSearch] = useState("");
  const [sprints, setSprints] = useState([]);
  const [currentSprint, setCurrentSprint] = useState(null);
  const [tasks, setTasks] = useState([]);
  const [teamMembers, setTeamMembers] = useState([]);

  const currentUser = (() => {
    try {
      return JSON.parse(localStorage.getItem("user") || "null");
    } catch {
      return null;
    }
  })();

  const loadData = async () => {
    if (!teamId) {
      setLoading(false);
      return;
    }

    setLoading(true);

    const [sprintRes, membersRes] = await Promise.all([
      api.get(`/api/sprints?TeamId=${teamId}&Page=1&PageSize=20`),
      api.get(`/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`)
    ]);

    const sprintItems = asList(sprintRes.data).map(normalizeSprint);
    const activeSprint =
      sprintItems.find((item) => item.status === "Active") ||
      sprintItems[0] ||
      null;

    setSprints(sprintItems);
    setCurrentSprint(activeSprint);
    setTeamMembers(asList(membersRes.data).map(normalizeMember));

    if (activeSprint) {
      const tasksRes = await api.get(
        `/api/tasks/team/${teamId}?SprintId=${activeSprint.id}&Page=1&PageSize=200`
      );
      setTasks(asList(tasksRes.data).map(normalizeTask));
    } else {
      setTasks([]);
    }

    setLoading(false);
  };

  useEffect(() => {
    loadData();
  }, [teamId]);

  const filteredTasks = useMemo(() => {
    const q = search.trim().toLowerCase();
    if (!q) return tasks;
    return tasks.filter((task) =>
      `${task.ticketCode} ${task.title} ${task.description ?? ""}`.toLowerCase().includes(q)
    );
  }, [search, tasks]);

  const reassignTask = async (task) => {
    const candidate = window.prompt("Enter assignee user id", task.assignedToId ?? teamMembers[0]?.userId ?? "");
    if (!candidate) return;
    await api.patch(
      `/api/tasks/${task.id}/assign?userId=${encodeURIComponent(candidate)}&actorId=${encodeURIComponent(currentUser?.userId || currentUser?.id || "")}`
    );
    await loadData();
  };

  const setTaskPriority = async (task) => {
    const nextPriority = window.prompt("Priority: Low / Medium / High / Critical", String(task.priority ?? "Medium"));
    if (!nextPriority) return;
    await api.patch(`/api/tasks/${task.id}/priority?priority=${encodeURIComponent(nextPriority)}`);
    await loadData();
  };

  const setTaskDeadline = async (task) => {
    const value = window.prompt("Deadline YYYY-MM-DD", task.deadline ? String(task.deadline).slice(0, 10) : "");
    if (value === null) return;
    const url = value
      ? `/api/tasks/${task.id}/deadline?deadline=${encodeURIComponent(new Date(value).toISOString())}`
      : `/api/tasks/${task.id}/deadline`;
    await api.patch(url);
    await loadData();
  };

  if (!teamId) {
    return (
      <div className="glass-panel p-6 text-sm text-[var(--text-secondary)]">
        No team selected for this Team Lead account. Sign in after assigning a team lead or set `activeTeamId`.
      </div>
    );
  }

  if (loading) {
    return <div className="glass-panel p-6 text-sm text-[var(--text-secondary)]">Loading sprint board...</div>;
  }

  return (
    <>
      <TaskCreateModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreated={loadData}
        teamId={teamId}
        sprintId={currentSprint?.id ?? null}
        teamMembers={teamMembers}
      />

      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Sprint Board</h1>
          <p className="text-sm text-[var(--text-secondary)] mt-1">
            {currentSprint ? `${currentSprint.name} · ${filteredTasks.length} tasks` : "No active sprint"}
          </p>
        </div>
        <div className="flex items-center gap-3">
          <input
            value={search}
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Search tasks"
            className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-sm text-white"
          />
          <button
            onClick={() => setModalOpen(true)}
            className="px-4 py-2 rounded-md bg-[#8A2BE2]/20 border border-[#8A2BE2]/40 text-[#B475FF]"
          >
            Add Task
          </button>
        </div>
      </div>

      <div className="glass-panel p-4 mb-6">
        <div className="text-xs uppercase tracking-wide text-[var(--text-secondary)] mb-3">Team Members</div>
        <div className="grid grid-cols-3 gap-3">
          {teamMembers.map((member) => (
            <div key={member.id} className="rounded-lg border border-[var(--border-subtle)] p-3 bg-[var(--bg-element)]">
              <div className="text-sm text-white">{member.fullName}</div>
              <div className="text-xs text-[var(--text-secondary)] mt-1">
                {member.devRole} · Capacity {member.weeklyCapacityPct}%
              </div>
              <div className="text-[10px] text-[var(--text-tertiary)] mt-1">{member.userId}</div>
            </div>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-4 gap-4">
        {STATUS_COLUMNS.map((column) => {
          const columnTasks = filteredTasks.filter((task) => task.status === column.status);
          return (
            <div key={column.status} className="glass-panel p-4 flex flex-col gap-3">
              <div className="flex items-center justify-between">
                <h2 className="text-sm text-white">{column.label}</h2>
                <span className="text-xs text-[var(--text-secondary)]">{columnTasks.length}</span>
              </div>

              <div className="flex flex-col gap-3">
                {columnTasks.length === 0 ? (
                  <div className="text-xs text-[var(--text-tertiary)]">No tasks</div>
                ) : (
                  columnTasks.map((task) => {
                    const assignee = teamMembers.find((member) => member.userId === task.assignedToId);
                    return (
                      <div key={task.id} className="rounded-lg border border-[var(--border-subtle)] p-3 bg-[var(--bg-element)]">
                        <div className="flex items-center justify-between mb-2">
                          <span className="tag">{task.ticketCode}</span>
                          <span className={`tag ${priorityClass(task.priority)}`}>{priorityLabel(task.priority)}</span>
                        </div>
                        <div className="text-sm text-white mb-2">{task.title}</div>
                        <div className="text-xs text-[var(--text-secondary)] mb-3">
                          {ticketTypeLabel(task.ticketType)} · {assignee?.fullName || task.assignedToId}
                        </div>
                        <div className="flex flex-wrap gap-2">
                          <button className="tag text-[10px]" onClick={() => reassignTask(task)}>Assign</button>
                          <button className="tag text-[10px]" onClick={() => setTaskPriority(task)}>Priority</button>
                          <button className="tag text-[10px]" onClick={() => setTaskDeadline(task)}>Deadline</button>
                        </div>
                        {task.deadline ? (
                          <div className="text-[10px] text-[var(--text-tertiary)] mt-2">
                            Due {new Date(task.deadline).toLocaleDateString()}
                          </div>
                        ) : null}
                        {task.isBlocked ? (
                          <div className="text-[10px] text-[var(--color-amber)] mt-2">
                            Blocked: {task.blockedReason || "No reason"}
                          </div>
                        ) : null}
                      </div>
                    );
                  })
                )}
              </div>
            </div>
          );
        })}
      </div>
    </>
  );
}
