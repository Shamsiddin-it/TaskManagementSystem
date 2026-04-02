import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { api } from "../lib/api.js";

const DEV_ROLES = ["Frontend", "Backend", "Designer", "Tester", "DevOps", "Fullstack"];

function asList(payload) {
  if (Array.isArray(payload)) return payload;
  if (Array.isArray(payload?.items)) return payload.items;
  if (Array.isArray(payload?.Items)) return payload.Items;
  if (Array.isArray(payload?.data)) return payload.data;
  return [];
}

function normalizeMember(item) {
  return {
    id: item.id ?? item.Id,
    teamId: item.teamId ?? item.TeamId,
    userId: item.userId ?? item.UserId,
    fullName: item.fullName ?? item.FullName ?? `User #${item.userId ?? item.UserId}`,
    avatarInitials: item.avatarInitials ?? item.AvatarInitials ?? "U",
    devRole: item.devRole ?? item.DevRole,
    isActive: item.isActive ?? item.IsActive,
    weeklyCapacityPct: item.weeklyCapacityPct ?? item.WeeklyCapacityPct ?? 0,
    focusScore: item.focusScore ?? item.FocusScore,
    throughputPtsPerWk: item.throughputPtsPerWk ?? item.ThroughputPtsPerWk
  };
}

function normalizeTask(item) {
  return {
    id: item.id ?? item.Id,
    title: item.title ?? item.Title,
    status: item.status ?? item.Status,
    priority: item.priority ?? item.Priority,
    deadline: item.deadline ?? item.Deadline
  };
}

export default function TeamOverviewPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = query.get("teamId") || localStorage.getItem("activeTeamId");

  const [loading, setLoading] = useState(true);
  const [members, setMembers] = useState([]);
  const [selectedMember, setSelectedMember] = useState(null);
  const [memberTasks, setMemberTasks] = useState([]);

  const loadMembers = async () => {
    if (!teamId) {
      setLoading(false);
      return;
    }

    setLoading(true);
    const res = await api.get(`/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`);
    setMembers(asList(res.data).map(normalizeMember));
    setLoading(false);
  };

  useEffect(() => {
    loadMembers();
  }, [teamId]);

  const openMember = async (member) => {
    setSelectedMember(member);
    const res = await api.get(`/api/tasks/team/${teamId}?AssigneeId=${member.userId}&Page=1&PageSize=50`);
    setMemberTasks(asList(res.data).map(normalizeTask));
  };

  const updateRole = async (member, nextRole) => {
    await api.put(`/api/team-members/${member.id}`, {
      id: member.id,
      teamId: member.teamId,
      userId: member.userId,
      devRole: nextRole
    });
    await loadMembers();
    if (selectedMember?.id === member.id) {
      setSelectedMember({ ...member, devRole: nextRole });
    }
  };

  if (!teamId) {
    return <div className="glass-panel p-6 text-sm text-[var(--text-secondary)]">No active team selected.</div>;
  }

  if (loading) {
    return <div className="glass-panel p-6 text-sm text-[var(--text-secondary)]">Loading team...</div>;
  }

  return (
    <>
      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Team Overview</h1>
          <p className="text-sm text-[var(--text-secondary)] mt-1">{members.length} members in this team</p>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        {members.map((member) => (
          <div key={member.id} className="glass-panel p-4">
            <div className="flex items-center justify-between mb-3">
              <div>
                <div className="text-white font-medium">{member.fullName}</div>
                <div className="text-xs text-[var(--text-secondary)] mt-1">{member.userId}</div>
              </div>
              <button className="tag text-[10px]" onClick={() => openMember(member)}>
                View Tasks
              </button>
            </div>

            <div className="grid grid-cols-3 gap-3 mb-3 text-xs text-[var(--text-secondary)]">
              <div>Capacity: {member.weeklyCapacityPct}%</div>
              <div>Focus: {member.focusScore ?? "-"}</div>
              <div>Throughput: {member.throughputPtsPerWk ?? "-"}</div>
            </div>

            <div className="flex items-center gap-3">
              <select
                className="bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-sm text-white"
                value={member.devRole}
                onChange={(event) => updateRole(member, event.target.value)}
              >
                {DEV_ROLES.map((role) => (
                  <option key={role} value={role}>{role}</option>
                ))}
              </select>
              <span className="text-xs text-[var(--text-secondary)]">
                {member.isActive ? "Active" : "Inactive"}
              </span>
            </div>
          </div>
        ))}
      </div>

      {selectedMember ? (
        <div className="glass-panel p-4 mt-6">
          <div className="flex items-center justify-between mb-4">
            <div>
              <h2 className="text-lg text-white">{selectedMember.fullName}</h2>
              <p className="text-xs text-[var(--text-secondary)] mt-1">Assigned tasks</p>
            </div>
            <button className="tag text-[10px]" onClick={() => setSelectedMember(null)}>
              Close
            </button>
          </div>

          <div className="grid grid-cols-2 gap-3">
            {memberTasks.length === 0 ? (
              <div className="text-sm text-[var(--text-secondary)]">No tasks assigned.</div>
            ) : (
              memberTasks.map((task) => (
                <div key={task.id} className="rounded-lg border border-[var(--border-subtle)] p-3 bg-[var(--bg-element)]">
                  <div className="text-white text-sm">{task.title}</div>
                  <div className="text-xs text-[var(--text-secondary)] mt-2">
                    {String(task.status)} · {String(task.priority)}
                  </div>
                  {task.deadline ? (
                    <div className="text-[10px] text-[var(--text-tertiary)] mt-2">
                      Due {new Date(task.deadline).toLocaleDateString()}
                    </div>
                  ) : null}
                </div>
              ))
            )}
          </div>
        </div>
      ) : null}
    </>
  );
}
