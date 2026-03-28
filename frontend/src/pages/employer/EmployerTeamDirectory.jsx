import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { getUsers } from '../../api';
import { Search, Plus } from 'lucide-react';

export default function EmployerTeamDirectory() {
  const [loading, setLoading] = useState(true);
  const [users, setUsers] = useState([]);

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

  if (loading) {
    return (
      <div className="flex h-full flex-col items-center justify-center">
        <div className="w-10 h-10 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="h-full flex flex-col">
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Team Directory</h1>
          <p className="text-gray-400 text-sm">{users.length} Active members across departments.</p>
        </div>
        <div className="flex items-center gap-4">
          <div className="relative">
            <input type="text" placeholder="Search team members, skills..." className="w-64 bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] text-white rounded-xl py-2.5 pl-10 pr-4 text-sm focus:outline-none focus:border-purple-500/50 transition-colors" />
            <Search className="absolute left-3.5 top-3 w-4 h-4 text-gray-500" />
          </div>
          <button className="flex items-center gap-2 px-4 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)] text-white font-medium text-sm rounded-xl transition-all">
            <Plus size={16} /> Add Member
          </button>
        </div>
      </div>

      <div className="grid grid-cols-3 gap-6">
        {users.map(user => (
          <TeamCard key={user.id} user={user} />
        ))}
      </div>
    </motion.div>
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
