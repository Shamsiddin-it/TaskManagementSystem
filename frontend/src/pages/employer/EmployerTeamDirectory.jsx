import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { createUser, getUsers } from '../../api';
import { Search, Plus, X } from 'lucide-react';

export default function EmployerTeamDirectory() {
  const [loading, setLoading] = useState(true);
  const [users, setUsers] = useState([]);
  const [query, setQuery] = useState('');
  const [openModal, setOpenModal] = useState(false);
  const [creating, setCreating] = useState(false);
  const [createError, setCreateError] = useState('');
  const [form, setForm] = useState({
    fullName: '',
    email: '',
    password: '',
    role: 2,
  });

  useEffect(() => {
    async function loadTeam() {
      const token = localStorage.getItem('token');
      try {
        const res = await getUsers(token);
        setUsers(res || []);
      } catch (err) {
        console.error("Failed fetching team", err);
      } finally {
        setLoading(false);
      }
    }
    loadTeam();
  }, []);

  const filteredUsers = users.filter((user) => {
    const haystack = [
      user.fullName,
      user.email,
      user.role,
      ...(user.skills || []),
      ...(user.currentProjects || []),
    ]
      .filter(Boolean)
      .join(' ')
      .toLowerCase();

    return haystack.includes(query.trim().toLowerCase());
  });

  const handleCreateMember = async (event) => {
    event.preventDefault();

    const token = localStorage.getItem('token');
    if (!token) {
      setCreateError('Please log in again.');
      return;
    }

    setCreating(true);
    setCreateError('');

    const createdUser = await createUser(token, {
      fullName: form.fullName.trim(),
      email: form.email.trim(),
      password: form.password,
      role: form.role,
    });

    if (!createdUser) {
      setCreateError('Failed to create member.');
      setCreating(false);
      return;
    }

    setUsers((current) => [createdUser, ...current]);
      setForm({
        fullName: '',
        email: '',
        password: '',
        role: 2,
      });
    setCreating(false);
    setOpenModal(false);
  };

  if (loading) {
    return (
      <div className="flex h-full flex-col items-center justify-center">
        <div className="w-10 h-10 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="h-full flex flex-col">
      {openModal ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 p-4">
          <div className="w-full max-w-lg rounded-2xl border border-[#1C212B] bg-[#0D1117] shadow-2xl overflow-hidden">
            <div className="flex items-center justify-between border-b border-[#1C212B] px-6 py-5">
              <div>
                <h2 className="text-xl font-bold text-white tracking-tight">Add Team Member</h2>
                <p className="text-sm text-gray-400">Create a real workspace user in the backend.</p>
              </div>
              <button onClick={() => setOpenModal(false)} className="rounded-xl bg-[#161B22] p-2 text-gray-500 hover:text-white">
                <X size={18} />
              </button>
            </div>

            <form onSubmit={handleCreateMember} className="space-y-5 px-6 py-6">
              <InputField label="Full name" value={form.fullName} onChange={(value) => setForm((current) => ({ ...current, fullName: value }))} />
              <InputField label="Email" type="email" value={form.email} onChange={(value) => setForm((current) => ({ ...current, email: value }))} />
              <InputField label="Password" type="password" value={form.password} onChange={(value) => setForm((current) => ({ ...current, password: value }))} />
              <div>
                <label className="mb-2 block text-xs font-bold uppercase tracking-widest text-gray-500">Role</label>
                <select
                  value={form.role}
                  onChange={(event) => setForm((current) => ({ ...current, role: Number(event.target.value) }))}
                  className="w-full rounded-lg border border-[#1C212B] bg-[#06080A] p-3 text-sm text-white"
                >
                  <option value={2}>Worker</option>
                  <option value={1}>Team Lead</option>
                </select>
              </div>
              {createError ? <div className="text-sm text-red-400">{createError}</div> : null}
              <div className="flex justify-end gap-3 pt-2">
                <button type="button" onClick={() => setOpenModal(false)} className="rounded-lg px-4 py-2 text-sm font-medium text-gray-400 hover:text-white">
                  Cancel
                </button>
                <button type="submit" disabled={creating} className="rounded-lg bg-gradient-to-r from-purple-600 to-purple-500 px-5 py-2 text-sm font-medium text-white disabled:opacity-50">
                  {creating ? 'Creating...' : 'Create Member'}
                </button>
              </div>
            </form>
          </div>
        </div>
      ) : null}

      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Team Directory</h1>
          <p className="text-gray-400 text-sm">{filteredUsers.length} active members across departments.</p>
        </div>
        <div className="flex items-center gap-4">
          <div className="relative">
            <input
              type="text"
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Search team members, skills..."
              className="w-64 bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] text-white rounded-xl py-2.5 pl-10 pr-4 text-sm focus:outline-none focus:border-purple-500/50 transition-colors"
            />
            <Search className="absolute left-3.5 top-3 w-4 h-4 text-gray-500" />
          </div>
          <button onClick={() => setOpenModal(true)} className="flex items-center gap-2 px-4 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)] text-white font-medium text-sm rounded-xl transition-all">
            <Plus size={16} /> Add Member
          </button>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-6">
        {filteredUsers.map(user => (
          <TeamCard key={user.id} user={user} />
        ))}
      </div>
    </motion.div>
  );
}

function InputField({ label, value, onChange, type = 'text' }) {
  return (
    <div>
      <label className="mb-2 block text-xs font-bold uppercase tracking-widest text-gray-500">{label}</label>
      <input
        type={type}
        value={value}
        onChange={(event) => onChange(event.target.value)}
        className="w-full rounded-lg border border-[#1C212B] bg-[#06080A] p-3 text-sm text-white"
        required
      />
    </div>
  );
}

function TeamCard({ user }) {
  const isDoNotDisturb = user.onlineStatus === 'Do Not Disturb';
  const isAway = user.onlineStatus === 'Away';
  const isMeeting = user.onlineStatus === 'In Meeting';
  
  const statusColor = isDoNotDisturb ? 'bg-red-500 shadow-[0_0_8px_rgba(239,68,68,0.5)]' : 
                      isAway ? 'bg-amber-500 shadow-[0_0_8px_rgba(245,158,11,0.5)]' :
                      isMeeting ? 'bg-blue-500 shadow-[0_0_8px_rgba(59,130,246,0.5)]' :
                      'bg-green-500 shadow-[0_0_8px_rgba(34,197,94,0.5)]';
                      
  const statusText = user.onlineStatus || 'Online';

  const avatarColor = user.avatarColor || '#2a2238';
  const initials = user.avatarInitials || user.fullName?.slice(0, 2).toUpperCase() || '??';
  
  const workload = Math.min(Math.max(user.workloadPercent || Math.floor(Math.random() * 100), 0), 100);
  let barColor = "bg-purple-500 shadow-[0_0_10px_rgba(168,85,247,0.5)]";
  if (workload < 30) barColor = "bg-green-500 shadow-[0_0_10px_rgba(34,197,94,0.5)]";
  else if (workload > 85) barColor = "bg-red-500 shadow-[0_0_10px_rgba(239,68,68,0.5)]";
  
  // Faking projects & skills arrays for visuals
  const projects = user.currentProjects?.length > 0 ? user.currentProjects : ["Alpha", "Beta"];
  const skills = user.skills?.length > 0 ? user.skills : ["React", "TypeScript", "WASM"];

  return (
    <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl p-6 relative group overflow-hidden hover:border-[#2D3342] transition-colors">
      <div className="flex items-center justify-between mb-4">
        <div 
          className="w-12 h-12 rounded-xl flex flex-col items-center justify-center font-bold text-lg" 
          style={{ backgroundColor: `${avatarColor}22`, color: avatarColor }}
        >
          {initials}
        </div>
        <div className="px-2.5 py-1 text-xs font-medium rounded-full border border-[#1C212B] bg-[#0A0D14]/80 flex items-center gap-1.5 text-gray-400">
          <span className={`w-1.5 h-1.5 rounded-full ${statusColor}`} />
          {statusText}
        </div>
      </div>

      <div className="mb-6">
        <h3 className="text-lg font-bold text-white tracking-tight">{user.fullName}</h3>
        <span className="text-[10px] font-bold text-green-400 uppercase tracking-widest border border-green-500/20 bg-green-500/10 px-2 py-0.5 rounded">
          {user.role}
        </span>
      </div>

      <div className="mb-6">
        <h4 className="text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2">Current Projects</h4>
        <div className="flex gap-2">
          {projects.map(p => (
            <span key={p} className="px-3 py-1 text-xs text-gray-300 bg-[#161B22] border border-[#2D3342] rounded-md">
              {p}
            </span>
          ))}
        </div>
      </div>

      <div className="mb-4">
        <div className="flex justify-between items-center text-xs font-semibold text-gray-400 mb-2">
           <span>Workload</span>
           <span>{workload}%</span>
        </div>
        <div className="h-1 w-full bg-[#161B22] rounded-full overflow-hidden">
           <div className={`h-full ${barColor} rounded-full`} style={{ width: `${workload}%` }} />
        </div>
      </div>

      <div className="flex flex-wrap gap-2 mt-4 pt-4 border-t border-[#1C212B]/50">
        {skills.map(s => (
          <span key={s} className="text-xs text-gray-500 font-medium px-2 py-1 rounded-md bg-[#0A0D14]">
            {s}
          </span>
        ))}
      </div>
    </div>
  );
}
