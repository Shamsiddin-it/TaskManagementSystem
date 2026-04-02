import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { api } from "../lib/api.js";
import { formatDateShort, statusLabel, priorityClass, priorityLabel, ticketTypeLabel } from "../lib/utils.js";

const ACTION_TYPE_LABELS = {
  0: "Action",
  1: "Task Moved",
  2: "Task Created",
  3: "Task Assigned",
  4: "Commented",
  5: "Sprint Started",
  6: "Sprint Completed",
  7: "Member Joined",
  8: "Member Removed",
};

const ACTION_ICONS = {
  1: (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <polyline points="13 17 18 12 13 7" /><line x1="6" y1="12" x2="18" y2="12" />
    </svg>
  ),
  2: (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <line x1="12" y1="5" x2="12" y2="19" /><line x1="5" y1="12" x2="19" y2="12" />
    </svg>
  ),
  3: (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
      <polyline points="23 11 20 8 17 11" /><line x1="20" y1="8" x2="20" y2="16" />
    </svg>
  ),
  5: (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <polygon points="5 3 19 12 5 21 5 3" />
    </svg>
  ),
  6: (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <polyline points="20 6 9 17 4 12" />
    </svg>
  ),
};

function actionIcon(type) {
  return ACTION_ICONS[type] || (
    <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
    </svg>
  );
}

function actionColor(type) {
  switch (type) {
    case 2: return "var(--accent-purple-light)";
    case 3: return "var(--color-blue)";
    case 5: return "var(--color-green)";
    case 6: return "var(--color-green)";
    case 7: return "var(--color-green)";
    case 8: return "var(--color-red)";
    default: return "var(--text-secondary)";
  }
}

export default function NotificationsPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = query.get("teamId") || localStorage.getItem("activeTeamId");

  const [logs, setLogs] = useState([]);
  const [blockedTasks, setBlockedTasks] = useState([]);
  const [logsLoading, setLogsLoading] = useState(true);
  const [blockedLoading, setBlockedLoading] = useState(true);
  const [readIds, setReadIds] = useState(() => {
    try { return new Set(JSON.parse(localStorage.getItem("nexus_read_logs") || "[]")); }
    catch { return new Set(); }
  });
  const [activeTab, setActiveTab] = useState("all"); // all | unread | blocked

  const loadData = async () => {
    setLogsLoading(true);
    setBlockedLoading(true);

    const [logRes, blockedRes] = await Promise.all([
      api.get(`/api/activity-logs/team/${teamId}?Limit=60&Offset=0`),
      api.get(`/api/tasks/team/${teamId}/blocked?Page=1&PageSize=50`),
    ]);

    setLogs(logRes.data?.Data?.Items ?? logRes.data?.Items ?? logRes.data ?? []);
    setBlockedTasks(blockedRes.data?.Data?.Items ?? blockedRes.data?.Items ?? []);
    setLogsLoading(false);
    setBlockedLoading(false);
  };

  useEffect(() => {
    loadData();
  }, [teamId]);

  const markRead = (id) => {
    const next = new Set(readIds);
    next.add(id);
    setReadIds(next);
    localStorage.setItem("nexus_read_logs", JSON.stringify([...next]));
  };

  const markAllRead = () => {
    const next = new Set(logs.map((l) => l.Id));
    setReadIds(next);
    localStorage.setItem("nexus_read_logs", JSON.stringify([...next]));
  };

  const unreadCount = logs.filter((l) => !readIds.has(l.Id)).length;

  const displayLogs =
    activeTab === "unread" ? logs.filter((l) => !readIds.has(l.Id)) :
    activeTab === "blocked" ? [] : logs;

  const showBlocked = activeTab === "all" || activeTab === "blocked";

  return (
    <>
      {/* Page header */}
      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div>
          <h2 className="text-[15px] flex items-center gap-2">
            Notifications Center
            {unreadCount > 0 && (
              <span className="text-[10px] bg-[var(--accent-purple)]/30 border border-[var(--accent-purple)]/50 text-[var(--accent-purple-light)] px-1.5 py-0.5 rounded-full">
                {unreadCount} new
              </span>
            )}
          </h2>
          <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
            Activity feed · Team #{teamId}
          </div>
        </div>
        <div className="flex items-center gap-2">
          <button
            className="px-3 py-1.5 rounded-md text-[11px] bg-[var(--bg-element)] border border-[var(--border-subtle)] hover:text-white text-[var(--text-secondary)] transition-colors"
            onClick={loadData}
          >
            <svg width="11" height="11" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" className="inline mr-1">
              <polyline points="23 4 23 10 17 10" /><polyline points="1 20 1 14 7 14" />
              <path d="M3.51 9a9 9 0 0 1 14.85-3.36L23 10M1 14l4.64 4.36A9 9 0 0 0 20.49 15" />
            </svg>
            Refresh
          </button>
          {unreadCount > 0 && (
            <button
              className="px-3 py-1.5 rounded-md text-[11px] bg-[var(--bg-element)] border border-[var(--border-subtle)] hover:text-white text-[var(--text-secondary)] transition-colors"
              onClick={markAllRead}
            >
              Mark all read
            </button>
          )}
        </div>
      </header>

      {/* Tabs */}
      <div className="flex gap-1 flex-shrink-0">
        {[
          { id: "all", label: "All Activity" },
          { id: "unread", label: `Unread${unreadCount > 0 ? ` (${unreadCount})` : ""}` },
          { id: "blocked", label: `Blocked${blockedTasks.length > 0 ? ` (${blockedTasks.length})` : ""}` },
        ].map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id)}
            className={`px-4 py-2 rounded-md text-[12px] transition-all border ${
              activeTab === tab.id
                ? "bg-[var(--bg-element)] border-[var(--border-highlight)] text-white"
                : "border-transparent text-[var(--text-secondary)] hover:text-white"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {/* Blocked tasks alert */}
      {showBlocked && !blockedLoading && blockedTasks.length > 0 && (
        <div className="glass-panel p-4 flex-shrink-0 border-[var(--color-amber)]/30">
          <div className="flex items-center gap-2 mb-3">
            <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="var(--color-amber)" strokeWidth="2">
              <circle cx="12" cy="12" r="10" /><line x1="12" y1="8" x2="12" y2="12" /><line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <span className="text-[12px] font-medium text-[var(--color-amber)]">
              {blockedTasks.length} Blocked Task{blockedTasks.length !== 1 ? "s" : ""} — Require Attention
            </span>
          </div>
          <div className="flex flex-col gap-2">
            {blockedTasks.map((task) => (
              <div
                key={task.Id}
                className="flex items-center justify-between p-3 rounded-lg border border-[var(--color-amber)]/20"
                style={{ background: "rgba(255,189,46,0.04)" }}
              >
                <div className="flex items-center gap-3">
                  <span className="tag">{task.TicketCode}</span>
                  <div>
                    <div className="text-[12px] text-white">{task.Title}</div>
                    {task.BlockedReason && (
                      <div className="text-[10px] text-[var(--color-amber)] mt-0.5">{task.BlockedReason}</div>
                    )}
                  </div>
                </div>
                <div className="flex items-center gap-2">
                  <span className={`tag ${priorityClass(task.Priority)}`}>{priorityLabel(task.Priority)}</span>
                  <span className="text-[10px] text-[var(--text-tertiary)]">User #{task.AssignedToId}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Activity Log */}
      <div className="glass-panel flex-1 flex flex-col overflow-hidden">
        <div className="px-4 py-3 border-b border-[var(--border-subtle)] flex items-center justify-between">
          <h3 className="text-[12px] font-medium tracking-wide">
            {activeTab === "blocked" ? "Blocked Tasks" : "Activity Log"}
          </h3>
          {!logsLoading && activeTab !== "blocked" && (
            <span className="text-[10px] text-[var(--text-tertiary)]">{displayLogs.length} entries</span>
          )}
        </div>

        <div className="flex-1 overflow-y-auto">
          {activeTab === "blocked" ? (
            blockedLoading ? (
              <div className="p-6 text-[12px] text-[var(--text-tertiary)] text-center">Loading…</div>
            ) : blockedTasks.length === 0 ? (
              <EmptyState icon="✓" title="No blocked tasks" subtitle="All tasks are running smoothly" />
            ) : (
              <div className="p-4 flex flex-col gap-3">
                {blockedTasks.map((task) => (
                  <BlockedTaskRow key={task.Id} task={task} />
                ))}
              </div>
            )
          ) : logsLoading ? (
            <div className="p-6 text-[12px] text-[var(--text-tertiary)] text-center">Loading activity…</div>
          ) : displayLogs.length === 0 ? (
            <EmptyState icon="🔔" title="All caught up" subtitle={activeTab === "unread" ? "No unread notifications" : "No activity recorded yet"} />
          ) : (
            <div className="divide-y divide-[var(--border-subtle)]">
              {displayLogs.map((log) => {
                const isUnread = !readIds.has(log.Id);
                return (
                  <div
                    key={log.Id}
                    className={`flex items-start gap-3 px-4 py-3 hover:bg-[var(--bg-element)] transition-colors cursor-pointer ${isUnread ? "bg-[rgba(138,43,226,0.03)]" : ""}`}
                    onClick={() => markRead(log.Id)}
                  >
                    {/* Icon */}
                    <div
                      className="w-7 h-7 rounded-lg flex items-center justify-center flex-shrink-0 mt-0.5"
                      style={{ color: actionColor(log.ActionType), background: `${actionColor(log.ActionType)}18`, border: `1px solid ${actionColor(log.ActionType)}30` }}
                    >
                      {actionIcon(log.ActionType)}
                    </div>

                    {/* Content */}
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center justify-between gap-2">
                        <span className="text-[11px] font-medium text-[var(--text-secondary)] uppercase tracking-wide">
                          {ACTION_TYPE_LABELS[log.ActionType] || "Activity"}
                        </span>
                        <div className="flex items-center gap-2 flex-shrink-0">
                          {isUnread && (
                            <div className="w-1.5 h-1.5 rounded-full bg-[var(--accent-purple-light)]" />
                          )}
                          <span className="text-[10px] text-[var(--text-tertiary)]">
                            {formatDateShort(log.CreatedAt)}
                          </span>
                        </div>
                      </div>
                      <div className="text-[12px] text-white mt-0.5 leading-snug">
                        {log.Description || `${log.EntityType} #${log.EntityId} was updated`}
                      </div>
                      <div className="text-[10px] text-[var(--text-tertiary)] mt-0.5">
                        By User #{log.ActorId} · {log.EntityType} #{log.EntityId}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </>
  );
}

function BlockedTaskRow({ task }) {
  return (
    <div
      className="rounded-lg p-4 border border-[var(--color-amber)]/25"
      style={{ background: "rgba(255,189,46,0.03)" }}
    >
      <div className="flex justify-between items-start mb-2">
        <div className="flex items-center gap-2">
          <span className="tag">{task.TicketCode}</span>
          <span className={`tag ${priorityClass(task.Priority)}`}>{priorityLabel(task.Priority)}</span>
        </div>
        <span className="text-[10px] text-[var(--text-tertiary)]">
          {ticketTypeLabel(task.TicketType)} · User #{task.AssignedToId}
        </span>
      </div>
      <div className="text-[13px] text-white mb-1">{task.Title}</div>
      {task.BlockedReason && (
        <div className="text-[11px] text-[var(--color-amber)]">
          Reason: {task.BlockedReason}
        </div>
      )}
      {task.Deadline && (
        <div className="text-[10px] text-[var(--text-tertiary)] mt-1">
          Due {formatDateShort(task.Deadline)}
        </div>
      )}
    </div>
  );
}

function EmptyState({ icon, title, subtitle }) {
  return (
    <div className="flex flex-col items-center justify-center py-16 gap-3">
      <div className="text-4xl">{icon}</div>
      <div className="text-[14px] text-white font-medium">{title}</div>
      <div className="text-[12px] text-[var(--text-tertiary)]">{subtitle}</div>
    </div>
  );
}
