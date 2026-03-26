import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import TaskCreateModal from "../components/TaskCreateModal.jsx";
import { api } from "../lib/api.js";
import { TEAM_ID } from "../lib/config.js";
import { priorityClass } from "../lib/utils.js";

export default function BacklogPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = Number(query.get("teamId")) || TEAM_ID;

  const [backlog, setBacklog] = useState([]);
  const [teamMembers, setTeamMembers] = useState([]);
  const [nextSprint, setNextSprint] = useState(null);
  const [modalOpen, setModalOpen] = useState(false);

  const loadData = async () => {
    const backlogRes = await api.get(
      `/api/tasks/team/${teamId}/backlog?Page=1&PageSize=100`
    );
    setBacklog(backlogRes.data?.Items || []);

    const teamRes = await api.get(
      `/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`
    );
    setTeamMembers(teamRes.data?.Items || []);

    const sprintRes = await api.get(
      `/api/sprints?TeamId=${teamId}&Page=1&PageSize=50`
    );
    const sprints = sprintRes.data?.Items || [];
    const planning = sprints.find((s) => s.Status === 1) || sprints[0] || null;
    setNextSprint(planning);
  };

  useEffect(() => {
    loadData();
  }, [teamId]);

  const totalPoints = backlog.reduce((sum, t) => sum + (t.StoryPoints ?? 0), 0);
  const capacity = nextSprint?.CapacityPoints ?? 40;
  const fillPct = capacity ? Math.round((totalPoints / capacity) * 100) : 0;

  return (
    <>
      <TaskCreateModal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        onCreated={loadData}
        teamId={teamId}
        sprintId={null}
        teamMembers={teamMembers}
      />

      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div>
          <h2 className="text-[15px]">Backlog Management</h2>
          <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
            {backlog.length} Items • Q4 Roadmap
          </div>
        </div>
        <div className="flex items-center gap-3">
          <div className="flex items-center gap-2 bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-1.5">
            <span className="text-[11px] text-[var(--text-secondary)]">
              Assignee:
            </span>
            <div className="flex -space-x-1.5">
              {teamMembers.slice(0, 3).map((m) => (
                <div
                  key={m.Id}
                  className="avatar w-5 h-5 text-[8px] ring-2 ring-[var(--bg-surface-solid)]"
                >
                  U{m.UserId}
                </div>
              ))}
              <div className="avatar w-5 h-5 text-[8px] ring-2 ring-[var(--bg-surface-solid)] flex items-center justify-center bg-[var(--bg-element)] text-[var(--text-tertiary)] border-dashed">
                +
              </div>
            </div>
          </div>
          <button
            className="px-3 py-1.5 rounded-md text-[12px] bg-[#8A2BE2]/20 border border-[#8A2BE2]/40 text-[#B475FF] hover:bg-[#8A2BE2]/30 transition-all flex items-center gap-1.5"
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
            New Task
          </button>
        </div>
      </header>

      <div className="grid grid-cols-[1fr_320px] gap-4 flex-1 overflow-hidden">
        {backlog.length === 0 ? (
          <div className="glass-panel flex flex-col items-center justify-center text-center p-12 overflow-hidden">
            <div className="empty-illustration">
              <svg
                className="ghost-icon"
                width="80"
                height="80"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="1.5"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <path d="M9 10h.01M15 10h.01"></path>
                <path d="M12 2a8 8 0 0 0-8 8v12l3-3 2.5 2.5L12 19l2.5 2.5L17 19l3 3V10a8 8 0 0 0-8-8z"></path>
              </svg>
              <div className="absolute inset-0 bg-purple-500/5 blur-3xl rounded-full"></div>
            </div>

            <h2 className="text-xl font-medium mb-2 tracking-tight">
              No tasks yet
            </h2>
            <p className="text-[var(--text-secondary)] max-w-xs mb-8 leading-relaxed">
              Your backlog is empty. Start mapping out your roadmap by creating
              the first task.
            </p>

            <button
              className="cta-button flex items-center gap-2"
              onClick={() => setModalOpen(true)}
            >
              <svg
                width="16"
                height="16"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2.5"
              >
                <line x1="12" y1="5" x2="12" y2="19"></line>
                <line x1="5" y1="12" x2="19" y2="12"></line>
              </svg>
              New Task
            </button>
          </div>
        ) : (
          <div className="glass-panel flex flex-col overflow-hidden">
            <div className="grid grid-cols-[40px_1fr_100px_100px_100px_100px] gap-4 px-6 py-3 border-b border-[var(--border-subtle)] label-caps">
              <div className="flex items-center justify-center">
                <input type="checkbox" className="accent-[var(--accent-purple)]" />
              </div>
              <div className="flex items-center gap-1 cursor-pointer hover:text-white transition-colors">
                Task
              </div>
              <div className="flex items-center gap-1 cursor-pointer hover:text-white transition-colors">
                ID
              </div>
              <div className="flex items-center gap-1 cursor-pointer hover:text-white transition-colors">
                Priority
              </div>
              <div className="flex items-center gap-1 cursor-pointer hover:text-white transition-colors">
                Estimate
              </div>
              <div className="flex items-center gap-1 cursor-pointer hover:text-white transition-colors">
                Assignee
              </div>
            </div>

            <div className="flex-1 overflow-y-auto">
              {backlog.map((task) => (
                <div
                  key={task.Id}
                  className="backlog-row grid grid-cols-[40px_1fr_100px_100px_100px_100px] gap-4 px-6 py-3.5 border-b border-[var(--border-subtle)] items-center"
                >
                  <div className="flex items-center justify-center text-[var(--text-tertiary)]">
                    <svg
                      width="14"
                      height="14"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="2"
                    >
                      <path d="M8 9h8M8 15h8"></path>
                    </svg>
                  </div>
                  <div className="text-[13px] text-white truncate">
                    {task.Title}
                  </div>
                  <div className="tag w-fit">{task.TicketCode}</div>
                  <div className={`tag ${priorityClass(task.Priority)} w-fit`}>
                    {task.Priority === 4
                      ? "P0 High"
                      : task.Priority === 3
                      ? "P1 Medium"
                      : "P2 Low"}
                  </div>
                  <div className="text-[12px] text-[var(--text-secondary)] font-mono">
                    {task.EstimatedHours ? `${task.EstimatedHours}h` : "—"}
                  </div>
                  <div className="avatar">U{task.AssignedToId}</div>
                </div>
              ))}
            </div>
          </div>
        )}

        <div className="glass-panel flex flex-col p-4 gap-4">
          <div className="label-caps mb-1">Sprint Planning</div>
          <div className="p-4 rounded-xl bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] flex flex-col gap-3">
            <div className="flex justify-between items-center">
              <span className="text-[14px] font-medium">
                {nextSprint?.Name || "Next Sprint"}
              </span>
              <span className="text-[10px] bg-[var(--color-green)]/10 text-[var(--color-green)] px-1.5 py-0.5 rounded border border-[var(--color-green)]/20">
                Next
              </span>
            </div>
            <div className="flex flex-col gap-2">
              <div className="flex justify-between text-[11px] text-[var(--text-secondary)]">
                <span>Total Points:</span>
                <span className="text-white font-mono">
                  {totalPoints} / {capacity}
                </span>
              </div>
              <div className="h-1.5 w-full bg-[var(--bg-element)] rounded-full overflow-hidden">
                <div
                  className="h-full bg-[var(--accent-purple-light)]"
                  style={{ width: `${Math.min(100, fillPct)}%` }}
                ></div>
              </div>
            </div>
            <div className="drop-zone h-32 rounded-lg flex flex-col items-center justify-center gap-2 text-[var(--text-tertiary)]">
              <span className="text-[10px] text-center px-4 leading-tight">
                Drag tasks here to assign to {nextSprint?.Name || "the sprint"}
              </span>
            </div>
          </div>

          <div className="mt-auto pt-4 border-t border-[var(--border-subtle)]">
            <div className="flex flex-col gap-3">
              <div className="label-caps">Capacity Insights</div>
              <div className="flex items-center justify-between text-[11px]">
                <span className="text-[var(--text-secondary)]">
                  FE Availability
                </span>
                <span className="text-[var(--color-amber)]">85% full</span>
              </div>
              <div className="flex items-center justify-between text-[11px]">
                <span className="text-[var(--text-secondary)]">
                  BE Availability
                </span>
                <span className="text-[var(--color-green)]">42% full</span>
              </div>
              <button className="w-full mt-2 py-2 rounded-lg bg-[var(--bg-element)] border border-[var(--border-subtle)] text-[12px] hover:bg-[var(--bg-element-hover)] transition-all">
                Auto-assign by Priority
              </button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
