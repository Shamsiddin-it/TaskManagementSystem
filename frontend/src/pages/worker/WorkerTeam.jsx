import React, { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

export default function WorkerTeam() {
  const [loading, setLoading] = useState(true);
  const [showProgress, setShowProgress] = useState(false);

  useEffect(() => {
    async function loadTeam() {
      const token = localStorage.getItem('token');
      try {
        const res = await fetch(`${API_BASE_URL}/api/teams`, { headers: { 'Authorization': `Bearer ${token}` }});
      } catch (err) {}
      setLoading(false);
    }
    loadTeam();
  }, []);

  if (loading) return null;

  const squad = [
     {
        id: 1, name: "Sarah Chen", role: "Senior Frontend Engineer", initials: "SC",
        statusColor: "bg-green-500", workload: 85, workloadColor: "bg-purple-500 shadow-[0_0_10px_rgba(168,85,247,0.5)]",
        activeTask: { code: "REF-402", title: "Refactoring UI Component Library" },
        commits: ["Pushed 4 commits to origin/main", "Commented on \"Global Navigation\" design"],
        focus: "5.2h", currentState: "CURRENTLY WORKING", stateColor: "text-purple-400"
     },
     {
        id: 2, name: "Marcus King", role: "Product Designer", initials: "MK",
        statusColor: "bg-amber-500", workload: 40, workloadColor: "bg-blue-500 shadow-[0_0_10px_rgba(59,130,246,0.5)]",
        activeTask: { code: "DES-119", title: "Mobile App Dark Mode Specs" },
        commits: ["Uploaded 3 files to Figma", "Updated task status to \"In Review\""],
        focus: "2.8h", currentState: "MEETING", stateColor: "text-gray-400"
     },
     {
        id: 3, name: "Alex Miller", role: "Backend Developer", initials: "AM",
        statusColor: "bg-amber-500", workload: 95, workloadColor: "bg-red-500 shadow-[0_0_10px_rgba(239,68,68,0.5)]",
        activeTask: { code: "API-881", title: "Optimize DB Indexing queries" },
        commits: ["Merged PR #891 into staging", "Resolved 3 merge conflicts"],
        focus: "4.0h", currentState: "PAUSED TASK", stateColor: "text-gray-400"
     },
     {
        id: 4, name: "Jamie Tan", role: "QA Engineer", initials: "JT",
        statusColor: "bg-green-500", workload: 65, workloadColor: "bg-purple-500 shadow-[0_0_10px_rgba(168,85,247,0.5)]",
        activeTask: { code: "TEST-22", title: "E2E Cypress Automation Scenarios" },
        commits: ["Pushed 12 commits to origin/tests", "Approved PR #890"],
        focus: "6.5h", currentState: "CURRENTLY WORKING", stateColor: "text-purple-400"
     }
  ];

  // Team progress stages data
  const sprintStages = [
    { name: 'Backlog', tasks: 12, color: 'bg-gray-500' },
    { name: 'To Do', tasks: 8, color: 'bg-blue-500' },
    { name: 'In Progress', tasks: 15, color: 'bg-purple-500' },
    { name: 'In Review', tasks: 6, color: 'bg-amber-500' },
    { name: 'Done', tasks: 24, color: 'bg-green-500' },
  ];
  const totalTasks = sprintStages.reduce((s, st) => s + st.tasks, 0);
  const donePct = Math.round((sprintStages[4].tasks / totalTasks) * 100);

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col gap-6 max-w-7xl mx-auto w-full pb-10">
      
      {/* Header */}
      <div className="flex items-center justify-between">
         <div>
            <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Team Directory</h1>
            <p className="text-gray-400 text-sm">Collaborate across {squad.length * 2} active members.</p>
         </div>
         <div className="flex gap-4 items-center">
            <button 
              onClick={() => setShowProgress(!showProgress)}
              className={`px-5 py-2.5 border rounded-lg text-sm font-bold transition-all ${showProgress ? 'bg-purple-600 border-purple-500 text-white shadow-[0_0_15px_rgba(168,85,247,0.4)]' : 'bg-[#161B22] border-[#2D3342] text-gray-300 hover:text-white hover:border-gray-500'}`}
            >
               {showProgress ? '← Back to Team' : '📊 Team Progress'}
            </button>
            <button className="px-5 py-2.5 bg-[#161B22] border border-[#2D3342] text-white hover:bg-[#1C212B] rounded-lg text-sm font-bold transition-colors shadow-lg">
               + Invite Member
            </button>
            <div className="w-10 h-10 rounded-full bg-[#1C212B] border border-[#2D3342] flex items-center justify-center text-white font-bold font-mono">JD</div>
         </div>
      </div>

      {/* Top 2 Metric Cards */}
      <div className="grid grid-cols-2 gap-6">
         <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl flex justify-between items-center group">
            <div>
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-1 drop-shadow-md">Collective Velocity</h3>
               <div className="flex items-baseline gap-2">
                  <span className="text-3xl font-bold text-white tracking-tight group-hover:text-purple-400 transition-colors">84.2</span>
                  <span className="text-sm text-gray-500 font-medium">pts/week</span>
               </div>
            </div>
            <svg width="40" height="30" viewBox="0 0 100 50" fill="none" stroke="currentColor" className="text-purple-500 w-12 h-10 drop-shadow-[0_0_5px_rgba(168,85,247,0.8)]">
               <polyline points="0 25 20 20 40 40 60 10 80 30 100 5" strokeWidth="4" strokeLinecap="round" strokeLinejoin="round" />
            </svg>
         </div>
         <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl flex justify-between items-center group">
            <div>
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-1">Active Sprints</h3>
               <div className="flex items-baseline gap-2">
                  <span className="text-3xl font-bold text-white tracking-tight group-hover:text-green-400 transition-colors">3</span>
                  <span className="text-sm text-gray-500 font-medium">concurrent</span>
               </div>
            </div>
            <svg width="40" height="40" viewBox="0 0 24 24" fill="none" stroke="currentColor" className="text-green-500 w-10 h-10 drop-shadow-[0_0_5px_rgba(34,197,94,0.8)]">
               <polygon points="12 2 2 7 12 12 22 7 12 2" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
               <polyline points="2 12 12 17 22 12" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
               <polyline points="2 17 12 22 22 17" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
         </div>
      </div>

      <AnimatePresence mode="wait">
      {showProgress ? (
        /* ────── TEAM PROGRESS VIEW ────── */
        <motion.div key="progress" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -20 }} className="space-y-6">
          
          {/* Sprint Pipeline */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-8 shadow-2xl">
            <div className="flex justify-between items-center mb-8">
              <div>
                <h2 className="text-lg font-bold text-white mb-1">Sprint Pipeline</h2>
                <p className="text-sm text-gray-400">Current sprint progress across all stages</p>
              </div>
              <div className="text-right">
                <span className="text-3xl font-bold text-purple-400">{donePct}%</span>
                <p className="text-xs text-gray-500 font-medium">Sprint Complete</p>
              </div>
            </div>

            {/* Full-width Progress Bar */}
            <div className="h-4 w-full bg-[#161B22] rounded-full flex overflow-hidden mb-8 shadow-inner">
              {sprintStages.map((st, i) => {
                const w = (st.tasks / totalTasks) * 100;
                return <div key={i} className={`h-full ${st.color} transition-all duration-1000`} style={{ width: `${w}%` }} />;
              })}
            </div>

            {/* Stage Cards */}
            <div className="grid grid-cols-5 gap-4">
              {sprintStages.map((st, i) => (
                <div key={i} className="bg-[#0A0D14]/80 border border-[#1C212B] rounded-xl p-5 text-center hover:border-[#2D3342] transition-colors group">
                  <div className={`w-3 h-3 rounded-full ${st.color} mx-auto mb-3 shadow-[0_0_8px_currentColor]`} />
                  <h4 className="text-xs font-bold text-gray-400 uppercase tracking-widest mb-2">{st.name}</h4>
                  <span className="text-2xl font-bold text-white group-hover:text-purple-400 transition-colors">{st.tasks}</span>
                  <p className="text-[10px] text-gray-500 font-medium mt-1">tasks</p>
                </div>
              ))}
            </div>
          </div>

          {/* Member Contribution */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
            <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-6">Member Contribution This Sprint</h3>
            <div className="space-y-4">
              {squad.map(user => {
                const contribution = Math.round((user.workload / 100) * 24);
                return (
                  <div key={user.id} className="flex items-center gap-4 group">
                    <div className="w-8 h-8 rounded-full bg-[#1C212B] border border-[#2D3342] flex items-center justify-center text-[10px] font-bold text-gray-400 shrink-0">{user.initials}</div>
                    <span className="text-sm font-semibold text-white w-32 truncate">{user.name}</span>
                    <div className="flex-1 h-2 bg-[#161B22] rounded-full overflow-hidden">
                      <div className={`h-full rounded-full ${user.workloadColor} transition-all duration-1000`} style={{ width: `${user.workload}%` }} />
                    </div>
                    <span className="text-xs text-gray-400 font-bold w-20 text-right">{contribution} tasks</span>
                  </div>
                );
              })}
            </div>
          </div>

        </motion.div>
      ) : (
        /* ────── DEFAULT ROSTER VIEW ────── */
        <motion.div key="roster" initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -20 }} className="grid grid-cols-2 gap-6">
          {squad.map(user => (
            <div key={user.id} className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl relative group hover:border-[#2D3342] transition-colors">
               
               <div className="flex items-center gap-4 mb-6">
                  <div className="relative">
                     <div className="w-12 h-12 rounded-full border border-[#2D3342] bg-[#161B22] flex flex-col items-center justify-center font-bold text-lg text-white font-mono shadow-inner group-hover:bg-[#1C212B] transition-colors">
                        {user.initials}
                     </div>
                     <div className={`absolute bottom-0 right-0 w-3.5 h-3.5 rounded-full border-[2.5px] border-[#0D1117] ${user.statusColor}`} />
                  </div>
                  <div>
                     <h3 className="text-white font-bold text-[17px] tracking-tight">{user.name}</h3>
                     <p className="text-xs text-gray-400">{user.role}</p>
                  </div>
               </div>

               <div className="mb-6">
                  <div className="flex justify-between items-center text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2">
                     <span>Workload</span>
                     <span>{user.workload}%</span>
                  </div>
                  <div className="h-1.5 w-full bg-[#161B22] rounded-full overflow-hidden">
                     <div className={`h-full rounded-full transition-all duration-1000 ${user.workloadColor}`} style={{ width: `${user.workload}%` }} />
                  </div>
               </div>

               <div className="mb-6">
                  <div className="text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-2">Active Task</div>
                  <div className="bg-[#0A0D14] border border-[#1C212B] rounded-xl p-3 flex gap-3 cursor-pointer group-hover:border-[#2D3342] transition-colors">
                     <span className="text-[10px] font-bold text-purple-400 bg-purple-500/10 px-2 py-0.5 rounded h-min mt-0.5">{user.activeTask.code}</span>
                     <span className="text-sm font-semibold text-white group-hover:text-purple-400 transition-colors">{user.activeTask.title}</span>
                  </div>
               </div>

               <div>
                  <div className="text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-3">Recent</div>
                  <ul className="space-y-2">
                     {user.commits.map((c, i) => (
                        <li key={i} className="flex gap-2 items-start text-xs text-gray-400">
                           <span className="text-[#2D3342] mt-0.5">▪</span>
                           <span>{c}</span>
                        </li>
                     ))}
                  </ul>
               </div>

            </div>
          ))}
        </motion.div>
      )}
      </AnimatePresence>

    </motion.div>
  );
}
