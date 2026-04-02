import React, { useEffect, useMemo, useState } from 'react';
import { getProjects, getUsers } from '../../api';
import CreateTeamModal from '../../components/employer/CreateTeamModal';

const apiBase = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5125/api';

function asList(payload) {
  if (Array.isArray(payload)) return payload;
  if (Array.isArray(payload?.items)) return payload.items;
  if (Array.isArray(payload?.Items)) return payload.Items;
  if (Array.isArray(payload?.data)) return payload.data;
  return [];
}

export default function EmployerTeamDirectory() {
  const [loading, setLoading] = useState(true);
  const [users, setUsers] = useState([]);
  const [projects, setProjects] = useState([]);
  const [showModal, setShowModal] = useState(false);

  const teams = useMemo(
    () =>
      projects.flatMap((project) =>
        (project.teams || []).map((team) => ({
          ...team,
          projectId: project.id,
          projectTitle: project.title
        }))
      ),
    [projects]
  );

  const loadData = async () => {
    setLoading(true);
    const token = localStorage.getItem('token');
    const [usersRes, projectsRes] = await Promise.all([getUsers(token), getProjects(token)]);
    setUsers(asList(usersRes));
    setProjects(asList(projectsRes));
    setLoading(false);
  };

  useEffect(() => {
    loadData();
  }, []);

  const assignTeamLead = async (teamId, teamLeadId) => {
    const token = localStorage.getItem('token');
    await fetch(`${apiBase}/teams/${teamId}/team-lead`, {
      method: 'PATCH',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ teamLeadId })
    });
    await loadData();
  };

  if (loading) {
    return <div className="glass-panel p-6 text-sm text-[var(--text-secondary)]">Loading team directory...</div>;
  }

  return (
    <>
      <CreateTeamModal
        open={showModal}
        onClose={() => setShowModal(false)}
        onCreated={() => {
          setShowModal(false);
          loadData();
        }}
        projects={projects}
        users={users}
      />

      <div className="flex items-center justify-between mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white">Team Directory</h1>
          <p className="text-sm text-[var(--text-secondary)] mt-1">{users.length} users · {teams.length} teams</p>
        </div>
        <button
          onClick={() => setShowModal(true)}
          className="px-4 py-2 bg-gradient-to-r from-purple-600 to-purple-500 text-white rounded-lg text-sm font-medium"
        >
          Create Team
        </button>
      </div>

      <div className="glass-panel p-4 mb-6">
        <div className="text-xs uppercase tracking-wide text-[var(--text-secondary)] mb-4">Teams</div>
        <div className="grid grid-cols-2 gap-4">
          {teams.length === 0 ? (
            <div className="text-sm text-[var(--text-secondary)]">No teams created yet.</div>
          ) : (
            teams.map((team) => (
              <div key={team.id} className="rounded-lg border border-[var(--border-subtle)] p-4 bg-[var(--bg-element)]">
                <div className="text-white font-medium">{team.name}</div>
                <div className="text-xs text-[var(--text-secondary)] mt-1">
                  {team.projectTitle} · {team.memberCount ?? 0} members
                </div>
                <select
                  className="mt-3 w-full bg-[var(--bg-surface-solid)] border border-[var(--border-subtle)] rounded-md px-3 py-2 text-sm text-white"
                  value=""
                  onChange={(event) => event.target.value && assignTeamLead(team.id, event.target.value)}
                >
                  <option value="">Assign team lead</option>
                  {users.map((user) => (
                    <option key={user.id} value={user.id}>
                      {user.fullName} · {user.role}
                    </option>
                  ))}
                </select>
              </div>
            ))
          )}
        </div>
      </div>

      <div className="grid grid-cols-3 gap-4">
        {users.map((user) => (
          <div key={user.id} className="glass-panel p-4">
            <div className="text-white font-medium">{user.fullName}</div>
            <div className="text-xs text-[var(--text-secondary)] mt-1">{user.email}</div>
            <div className="text-xs text-[var(--text-secondary)] mt-1">{user.role}</div>
            <div className="text-xs text-[var(--text-tertiary)] mt-3">
              Projects: {(user.currentProjects || []).join(', ') || 'None'}
            </div>
          </div>
        ))}
      </div>
    </>
  );
}
