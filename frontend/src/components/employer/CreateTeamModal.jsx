import React, { useMemo, useState } from 'react';

const apiBase = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5125/api';

export default function CreateTeamModal({ open, onClose, onCreated, projects, users }) {
  const teamLeadCandidates = useMemo(
    () => (users || []).filter((user) => user.role === "TeamLead" || user.role === "Team Lead" || user.role === "Worker"),
    [users]
  );

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [projectId, setProjectId] = useState('');
  const [teamLeadId, setTeamLeadId] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  if (!open) return null;

  const submit = async (e) => {
    e.preventDefault();
    if (!projectId) {
      setError("Please select a project.");
      return;
    }

    setLoading(true);
    setError('');
    const token = localStorage.getItem('token');

    try {
      const response = await fetch(`${apiBase}/projects/${projectId}/teams`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`
        },
        body: JSON.stringify({ name, description })
      });

      const created = await response.json();
      const team = created?.data ?? created?.Data;
      if (!response.ok || !team?.id) {
        throw new Error(created?.message || created?.description || 'Failed to create team');
      }

      if (teamLeadId) {
        await fetch(`${apiBase}/teams/${team.id}/team-lead`, {
          method: 'PATCH',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`
          },
          body: JSON.stringify({ teamLeadId })
        });
      }

      onCreated?.();
      onClose?.();
    } catch (err) {
      setError(err.message || 'Error creating team');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/70 backdrop-blur-sm">
      <div className="bg-[#0D1117] border border-[#1C212B] w-[480px] rounded-xl p-6 shadow-2xl">
        <h3 className="text-lg font-bold text-white mb-4">Create Team</h3>
        <form onSubmit={submit} className="flex flex-col gap-4">
          <select
            className="w-full bg-[#161B22] border border-[#2D3342] text-white rounded-md px-3 py-2 text-sm outline-none"
            value={projectId}
            onChange={(e) => setProjectId(e.target.value)}
            required
          >
            <option value="">Select project</option>
            {(projects || []).map((project) => (
              <option key={project.id} value={project.id}>{project.title}</option>
            ))}
          </select>

          <input
            required
            className="w-full bg-[#161B22] border border-[#2D3342] rounded-md px-4 py-2 text-sm text-white outline-none"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Team name"
          />

          <textarea
            className="w-full bg-[#161B22] border border-[#2D3342] rounded-md px-4 py-2 text-sm text-white outline-none"
            rows="3"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Description"
          />

          <select
            className="w-full bg-[#161B22] border border-[#2D3342] text-white rounded-md px-3 py-2 text-sm outline-none"
            value={teamLeadId}
            onChange={(e) => setTeamLeadId(e.target.value)}
          >
            <option value="">Assign team lead later</option>
            {teamLeadCandidates.map((user) => (
              <option key={user.id} value={user.id}>{user.fullName} · {user.role}</option>
            ))}
          </select>

          {error ? <div className="text-xs text-red-500">{error}</div> : null}

          <div className="flex items-center justify-end gap-3">
            <button type="button" onClick={onClose} className="px-4 py-2 text-xs font-bold text-gray-400 hover:text-white">
              Cancel
            </button>
            <button
              type="submit"
              disabled={loading}
              className="px-5 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 text-white rounded-lg text-xs font-bold disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Create Team'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
