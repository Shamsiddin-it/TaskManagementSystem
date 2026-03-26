import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { api } from "../lib/api.js";
import { TEAM_ID } from "../lib/config.js";
import { devRoleLabel, clampPct, formatDateShort, statusLabel, ticketTypeLabel, priorityClass, priorityLabel } from "../lib/utils.js";

export default function TeamOverviewPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = Number(query.get("teamId")) || TEAM_ID;

  const [members, setMembers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedMember, setSelectedMember] = useState(null);
  const [memberTasks, setMemberTasks] = useState([]);
  const [drawerLoading, setDrawerLoading] = useState(false);
  const [search, setSearch] = useState("");

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      const res = await api.get(`/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`);
      setMembers(res.data?.Items || []);
      setLoading(false);
    };
    load();
  }, [teamId]);

  const openMember = async (member) => {
    setSelectedMember(member);
    setDrawerLoading(true);
    setMemberTasks([]);
    const res = await api.get(
      `/api/tasks/team/${teamId}?AssignedToId=${member.UserId}&Page=1&PageSize=50`
    );
    setMemberTasks(res.data?.Items || []);
    setDrawerLoading(false);
  };

  const closeDrawer = () => {
    setSelectedMember(null);
    setMemberTasks([]);
  };

  const filtered = members.filter((m) => {
    if (!search) return true;
    const q = search.toLowerCase();
    return (
      devRoleLabel(m.DevRole).toLowerCase().includes(q) ||
      String(m.UserId).includes(q)
    );
  });

  const activeMembers = members.filter((m) => m.IsActive).length;
  const avgCapacity = members.length
    ? Math.round(members.reduce((s, m) => s + (m.WeeklyCapacityPct || 0), 0) / members.length)
    : 0;
  const overloaded = members.filter((m) => (m.WeeklyCapacityPct || 0) > 80).length;

  return (
    <>
      {/* Drawer overlay */}
      {selectedMember && (
        <div
          className="fixed inset-0 z-[90] bg-black/60 backdrop-blur-sm"
          onClick={closeDrawer}
        />
      )}

      {/* Member detail drawer */}
      <div
        className="fixed top-0 right-0 h-full z-[100] w-[420px] transition-transform duration-300"
        style={{
          transform: selectedMember ? "translateX(0)" : "translateX(110%)",
          background: "rgba(10,10,18,0.97)",
          borderLeft: "1px solid var(--border-subtle)",
          boxShadow: "-20px 0 60px rgba(0,0,0,0.7)",
        }}
      >
        {selectedMember && (
          <div className="flex flex-col h-full overflow-hidden">
            {/* Drawer header */}
            <div className="p-5 border-b border-[var(--border-subtle)] flex items-start justify-between">
              <div className="flex items-center gap-3">
                <div
                  className="w-12 h-12 rounded-xl flex items-center justify-center text-white font-semibold text-[16px] shadow-[0_0_20px_rgba(138,43,226,0.3)]"
                  style={{
                    background: "linear-gradient(135deg, var(--accent-purple), #6a1b9a)",
                  }}
                >
                  U{selectedMember.UserId}
                </div>
                <div>
                  <div className="text-[15px] font-medium text-white">
                    User #{selectedMember.UserId}
                  </div>
                  <div className="text-[11px] text-[var(--text-secondary)] mt-0.5">
                    {devRoleLabel(selectedMember.DevRole)}
                    {" · "}
                    {selectedMember.IsActive ? (
                      <span className="text-[var(--color-green)]">Active</span>
                    ) : (
                      <span className="text-[var(--color-red)]">Inactive</span>
                    )}
                  </div>
                </div>
              </div>
              <button
                className="text-[var(--text-tertiary)] hover:text-white transition-colors p-1"
                onClick={closeDrawer}
              >
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <line x1="18" y1="6" x2="6" y2="18" />
                  <line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </button>
            </div>

            {/* Drawer stats */}
            <div className="grid grid-cols-3 gap-3 p-4 border-b border-[var(--border-subtle)]">
              <StatBox label="Capacity" value={`${clampPct(selectedMember.WeeklyCapacityPct || 0)}%`} />
              <StatBox label="Focus Score" value={selectedMember.FocusScore != null ? selectedMember.FocusScore : "—"} />
              <StatBox label="Throughput" value={selectedMember.ThroughputPtsPerWk != null ? `${selectedMember.ThroughputPtsPerWk} pt/w` : "—"} />
            </div>

            {/* Drawer: member tasks */}
            <div className="flex-1 overflow-y-auto p-4">
              <div className="text-[11px] font-medium uppercase tracking-wider text-[var(--text-secondary)] mb-3">
                Assigned Tasks ({memberTasks.length})
              </div>
              {drawerLoading ? (
                <div className="text-[12px] text-[var(--text-tertiary)]">Loading tasks…</div>
              ) : memberTasks.length === 0 ? (
                <div className="text-[12px] text-[var(--text-tertiary)]">No tasks assigned</div>
              ) : (
                <div className="flex flex-col gap-2">
                  {memberTasks.map((task) => (
                    <div
                      key={task.Id}
                      className="rounded-lg p-3 border border-[var(--border-subtle)] hover:border-[var(--border-highlight)] transition-colors"
                      style={{ background: "rgba(255,255,255,0.02)" }}
                    >
                      <div className="flex justify-between items-start mb-1.5">
                        <span className="tag">{task.TicketCode}</span>
                        <span className={`tag ${priorityClass(task.Priority)}`}>{priorityLabel(task.Priority)}</span>
                      </div>
                      <div className="text-[12px] text-white leading-snug mb-1.5">{task.Title}</div>
                      <div className="flex justify-between items-center">
                        <span className="text-[10px] text-[var(--text-tertiary)]">
                          {statusLabel(task.Status)} · {ticketTypeLabel(task.TicketType)}
                        </span>
                        {task.Deadline && (
                          <span className="text-[10px] text-[var(--text-tertiary)]">
                            Due {formatDateShort(task.Deadline)}
                          </span>
                        )}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Drawer footer: joined */}
            <div className="p-4 border-t border-[var(--border-subtle)] text-[11px] text-[var(--text-tertiary)]">
              Joined team: {formatDateShort(selectedMember.JoinedAt)}
            </div>
          </div>
        )}
      </div>

      {/* Page header */}
      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div>
          <h2 className="text-[15px]">Team Overview</h2>
          <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
            {loading ? "Loading…" : `${members.length} members · Team #${teamId}`}
          </div>
        </div>
        <div className="flex items-center gap-3">
          <div className="flex items-center bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-2 py-1 gap-1">
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="var(--text-tertiary)" strokeWidth="2">
              <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
            <input
              type="text"
              placeholder="Filter by role or ID…"
              className="bg-transparent border-none outline-none text-[11px] w-36 placeholder:text-[var(--text-tertiary)]"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
          </div>
        </div>
      </header>

      {/* Summary stats */}
      <div className="grid grid-cols-3 gap-4 flex-shrink-0">
        <div className="glass-panel p-4 flex flex-col gap-2">
          <div className="text-[11px] text-muted">Active Members</div>
          <div className="text-[24px] tracking-tight leading-none">{loading ? "—" : activeMembers}</div>
          <div className="status-pill"><div className="status-dot success" /> Operational</div>
        </div>
        <div className="glass-panel p-4 flex flex-col gap-2">
          <div className="text-[11px] text-muted">Avg Capacity</div>
          <div className="text-[24px] tracking-tight leading-none">
            {loading ? "—" : avgCapacity}<span className="text-muted text-[14px]">%</span>
          </div>
          <div className="status-pill">
            <div className={`status-dot ${avgCapacity > 80 ? "warning" : "success"}`} />
            {avgCapacity > 80 ? "High Load" : "Healthy"}
          </div>
        </div>
        <div className="glass-panel p-4 flex flex-col gap-2">
          <div className="text-[11px] text-muted">Overloaded (&gt;80%)</div>
          <div className="text-[24px] tracking-tight leading-none" style={{ color: overloaded > 0 ? "var(--color-amber)" : "inherit" }}>
            {loading ? "—" : overloaded}
          </div>
          <div className="status-pill">
            <div className={`status-dot ${overloaded > 0 ? "warning" : "success"}`} />
            {overloaded > 0 ? "Needs Attention" : "All Good"}
          </div>
        </div>
      </div>

      {/* Member grid */}
      <div className="flex-1 overflow-y-auto pb-4">
        {loading ? (
          <div className="text-[12px] text-[var(--text-tertiary)] px-2 py-8 text-center">Loading team members…</div>
        ) : filtered.length === 0 ? (
          <div className="text-[12px] text-[var(--text-tertiary)] px-2 py-8 text-center">No members found</div>
        ) : (
          <div className="grid grid-cols-2 gap-4 mt-1" style={{ gridTemplateColumns: "repeat(auto-fill, minmax(280px, 1fr))" }}>
            {filtered.map((member) => {
              const cap = clampPct(member.WeeklyCapacityPct || 0);
              const capColor = cap > 95 ? "cap-red" : cap >= 80 ? "cap-amber" : "cap-green";
              return (
                <button
                  key={member.Id}
                  className="glass-panel p-4 text-left flex flex-col gap-4 hover:border-[var(--border-highlight)] transition-all cursor-pointer group"
                  style={{ outline: "none" }}
                  onClick={() => openMember(member)}
                >
                  {/* Card top: avatar + info + status */}
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-3">
                      <div
                        className="w-10 h-10 rounded-lg flex items-center justify-center text-white text-[13px] font-semibold"
                        style={{ background: "linear-gradient(135deg, var(--accent-purple), #6a1b9a)" }}
                      >
                        U{member.UserId}
                      </div>
                      <div>
                        <div className="text-[13px] font-medium text-white">User #{member.UserId}</div>
                        <div className="text-[11px] text-[var(--text-secondary)]">{devRoleLabel(member.DevRole)}</div>
                      </div>
                    </div>
                    <div className="status-pill text-[10px]">
                      <div className={`status-dot ${member.IsActive ? "success" : ""}`} />
                      {member.IsActive ? "Active" : "Inactive"}
                    </div>
                  </div>

                  {/* Capacity bar */}
                  <div>
                    <div className="flex justify-between text-[10px] text-[var(--text-tertiary)] mb-1.5">
                      <span>Weekly Capacity</span>
                      <span className="font-mono">{cap}%</span>
                    </div>
                    <div className="h-1.5 capacity-bar-bg rounded-full overflow-hidden">
                      <div className={`h-full rounded-full capacity-bar-fill ${capColor}`} style={{ width: `${cap}%` }} />
                    </div>
                  </div>

                  {/* Stats row */}
                  <div className="grid grid-cols-3 gap-2">
                    <MiniStat label="Focus" value={member.FocusScore != null ? member.FocusScore : "—"} />
                    <MiniStat label="Throughput" value={member.ThroughputPtsPerWk != null ? `${member.ThroughputPtsPerWk}` : "—"} suffix="pt/w" />
                    <MiniStat label="Joined" value={formatDateShort(member.JoinedAt)} />
                  </div>

                  {/* View profile hint */}
                  <div className="text-[10px] text-[var(--text-tertiary)] group-hover:text-[var(--accent-purple-light)] transition-colors flex items-center gap-1">
                    <svg width="10" height="10" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                      <circle cx="11" cy="11" r="8" /><line x1="21" y1="21" x2="16.65" y2="16.65" />
                    </svg>
                    View tasks
                  </div>
                </button>
              );
            })}
          </div>
        )}
      </div>
    </>
  );
}

function StatBox({ label, value }) {
  return (
    <div className="rounded-lg p-3 border border-[var(--border-subtle)]" style={{ background: "rgba(255,255,255,0.02)" }}>
      <div className="text-[10px] text-[var(--text-tertiary)] mb-1">{label}</div>
      <div className="text-[14px] font-medium text-white">{value}</div>
    </div>
  );
}

function MiniStat({ label, value, suffix }) {
  return (
    <div className="flex flex-col gap-0.5">
      <span className="text-[9px] uppercase tracking-wider text-[var(--text-tertiary)]">{label}</span>
      <span className="text-[11px] text-white font-mono">{value}{suffix ? <span className="text-[9px] text-[var(--text-tertiary)] ml-0.5">{suffix}</span> : null}</span>
    </div>
  );
}
