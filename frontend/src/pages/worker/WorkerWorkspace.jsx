import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
import { Play, Square, AlertCircle, Check, MoreVertical, Layers, Plus } from 'lucide-react';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

export default function WorkerWorkspace() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState(null);
  const [tasks, setTasks] = useState([]);
  const [activities, setActivities] = useState([]);
  const [notes, setNotes] = useState('');
  const [newTaskTitle, setNewTaskTitle] = useState('');
  const [hasRealTasks, setHasRealTasks] = useState(null); // null = unknown, true/false
  const [currentUser, setCurrentUser] = useState(null);
  
  const [scaleMode, setScaleMode] = useState('focus');

  useEffect(() => {
    try {
      const rawUser = localStorage.getItem('user');
      if (rawUser) {
        setCurrentUser(JSON.parse(rawUser));
      }
    } catch {}

    async function loadData() {
      const token = localStorage.getItem('token');
      try {
        const [statsRes, taskRes, actRes] = await Promise.all([
          fetch(`${API_BASE_URL}/api/worker-dashboard/stats`, { headers: { 'Authorization': `Bearer ${token}` } }),
          fetch(`${API_BASE_URL}/api/tasks`, { headers: { 'Authorization': `Bearer ${token}` } }),
          fetch(`${API_BASE_URL}/api/worker-dashboard/activities`, { headers: { 'Authorization': `Bearer ${token}` } }),
        ]);
        
        if (statsRes.ok) setStats(await statsRes.json());
        if (taskRes.ok) {
           const d = await taskRes.json();
           const items =
             d?.Data?.Items ??
             d?.Data?.items ??
             d?.data?.Items ??
             d?.data?.items ??
             d?.Items ??
             d?.items ??
             (Array.isArray(d?.Data) ? d.Data : null) ??
             (Array.isArray(d?.data) ? d.data : null) ??
             (Array.isArray(d) ? d : []);
           setTasks(items);
           setHasRealTasks(items.length > 0);
        } else {
           setHasRealTasks(false);
        }
        if (actRes.ok) setActivities(await actRes.json());
      } catch (err) {
        setHasRealTasks(false);
      }
      setLoading(false);
    }
    loadData();
  }, []);

  const handleTaskAction = async (taskId, action) => {
    const token = localStorage.getItem('token');
    
    let endpoint = "";
    let method = "PATCH";
    if (action === "start") endpoint = `/api/tasks/${taskId}/status?status=InProgress`;
    if (action === "stop") endpoint = `/api/tasks/${taskId}/status?status=Todo`;
    if (action === "complete") endpoint = `/api/tasks/${taskId}/status?status=Done`;
    if (action === "block") endpoint = `/api/tasks/${taskId}/blocked?isBlocked=true&reason=User+Requested`;
    if (action === "reject") endpoint = `/api/tasks/${taskId}/reject?reason=Rejected+by+worker`;

    try {
      await fetch(`${API_BASE_URL}${endpoint}`, { method, headers: { 'Authorization': `Bearer ${token}` }});
      
      if (action === "start") {
         await fetch(`${API_BASE_URL}/api/focussession/start`, { 
           method: 'POST', 
           headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
           body: JSON.stringify({ taskId })
         });
      }
      
      if (['complete', 'block', 'reject'].includes(action)) {
         setTasks(t => t.filter(x => x.id !== taskId));
      } else {
         setTasks(prev => prev.map(t => t.id === taskId ? {...t, status: action === 'start' ? 'InProgress' : 'Todo'} : t)); 
      }
    } catch (err) {}
  };

  const handleAddTask = async () => {
    if (!newTaskTitle.trim()) return;
    const token = localStorage.getItem('token');
    try {
      const res = await fetch(`${API_BASE_URL}/api/tasks`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
        body: JSON.stringify({
          title: newTaskTitle,
          description: '',
          assignedTo: currentUser?.userId || currentUser?.id || null,
          createdBy: currentUser?.userId || currentUser?.id || null,
          priority: 'Medium',
          ticketType: 'Task',
          status: 'Todo'
        })
      });
      if (res.ok) {
        setNewTaskTitle('');
        // Reload tasks
        const taskRes = await fetch(`${API_BASE_URL}/api/tasks`, { headers: { 'Authorization': `Bearer ${token}` } });
        if (taskRes.ok) {
          const d = await taskRes.json();
          const items =
            d?.Data?.Items ??
            d?.Data?.items ??
            d?.data?.Items ??
            d?.data?.items ??
            d?.Items ??
            d?.items ??
            (Array.isArray(d?.Data) ? d.Data : null) ??
            (Array.isArray(d?.data) ? d.data : null) ??
            (Array.isArray(d) ? d : []);
          setTasks(items);
          setHasRealTasks(items.length > 0);
        }
      }
    } catch(err){}
  };

  if (loading) return <div className="p-8 text-white">Loading Workspace...</div>;

  const m = stats || {
     completedToday: 0, currentStreak: 0, focusHours: 0, dailyGoalHours: 8, dailyGoalProgressPercent: 0,
     weeklyLoadDistribution: [
       { day: 'MON', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
       { day: 'TUE', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
       { day: 'WED', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
       { day: 'THU', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
       { day: 'FRI', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
       { day: 'SAT', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
       { day: 'SUN', estimatedHours: 0, taskCount: 0, focusTimeLogged: 0 },
     ]
  };

  const isEmpty = tasks.length === 0;

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col gap-6 max-w-7xl mx-auto w-full pb-10">
      
      {/* Header */}
      <div className="flex items-center justify-between">
         <div>
            <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Worker Workspace</h1>
            <p className="text-gray-400 text-sm">{isEmpty ? 'Your horizon is clear. Ready to begin?' : 'Focus mode active. Minimize distractions.'}</p>
         </div>
         <div className="flex gap-4 items-center">
            <div className="bg-[#1C212B] p-1 rounded-full border border-[#2D3342] flex text-[11px] font-bold tracking-widest uppercase">
               <button className="flex items-center gap-2 px-4 py-1.5 rounded-full bg-[#2D3342] text-white shadow-xl"><div className="w-1.5 h-1.5 rounded-full bg-green-500 shadow-[0_0_8px_rgba(34,197,94,1)]"/> Online</button>
               <button className="flex items-center gap-2 px-4 py-1.5 rounded-full text-gray-500 hover:text-gray-300"><div className="w-1.5 h-1.5 rounded-full bg-red-500 opacity-50"/> Busy</button>
               <button className="flex items-center gap-2 px-4 py-1.5 rounded-full text-gray-500 hover:text-gray-300"><div className="w-1.5 h-1.5 rounded-full bg-amber-500 opacity-50"/> Away</button>
            </div>
            <div className="w-10 h-10 rounded-full bg-[#1C212B] border border-[#2D3342] flex items-center justify-center text-white font-bold font-mono">JD</div>
         </div>
      </div>

      {/* ───── EMPTY STATE ───── */}
      {isEmpty ? (
        <div className="grid grid-cols-[1fr_320px] gap-6">
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl shadow-2xl flex flex-col items-center justify-center py-20 px-10">
            
            {/* Glowing Layers Icon */}
            <div className="w-24 h-24 rounded-2xl bg-purple-500/10 border border-purple-500/20 flex items-center justify-center mb-8 shadow-[0_0_40px_rgba(168,85,247,0.2)]">
              <Layers size={48} className="text-purple-400 drop-shadow-[0_0_10px_rgba(168,85,247,0.6)]" />
            </div>

            <h2 className="text-xl font-bold text-white mb-2 text-center">Start something new</h2>
            <p className="text-sm text-gray-400 text-center max-w-md mb-8 leading-relaxed">
              Your task queue is currently empty. It's a great time to plan your next big milestone or pick up a backlog item.
            </p>

            {/* Add Task Input */}
            <div className="flex gap-3 w-full max-w-lg mb-6">
              <input
                type="text"
                value={newTaskTitle}
                onChange={e => setNewTaskTitle(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && handleAddTask()}
                placeholder="What are you working on next?"
                className="flex-1 bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/60 rounded-xl px-4 py-3 text-sm text-white outline-none transition-colors"
              />
              <button 
                onClick={handleAddTask}
                className="px-6 py-3 bg-purple-600 hover:bg-purple-500 text-white rounded-xl text-sm font-bold transition-all shadow-[0_0_15px_rgba(168,85,247,0.4)] flex items-center gap-2"
              >
                <Plus size={16} /> Add Task
              </button>
            </div>

            <div className="flex gap-4">
              <button className="px-5 py-2.5 bg-[#161B22] border border-[#2D3342] text-gray-300 hover:text-white hover:border-gray-500 rounded-xl text-xs font-bold transition-all">
                Browse Backlog
              </button>
              <button className="px-5 py-2.5 bg-[#161B22] border border-[#2D3342] text-gray-300 hover:text-white hover:border-gray-500 rounded-xl text-xs font-bold transition-all">
                Import from Jira
              </button>
            </div>
          </div>

          {/* Right sidebar even in empty state */}
          <div className="space-y-6">
            {/* Quick Notes */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl">
               <h3 className="text-xs font-bold text-gray-400 tracking-widest uppercase mb-4 flex items-center gap-2">
                  <Square size={14} className="text-purple-500" /> Quick Notes
               </h3>
               <textarea 
                  value={notes}
                  onChange={e => setNotes(e.target.value)}
                  placeholder="Jot down temporary thoughts..."
                  className="w-full h-24 bg-[#0A0D14]/50 border border-purple-500/20 focus:border-purple-500/60 rounded-xl p-3 text-sm text-white resize-none outline-none transition-colors"
               />
            </div>

            {/* Motivational Insight */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl">
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-4">Motivational Insight</h3>
               <p className="text-sm text-gray-300 italic leading-relaxed">
                  "The secret of getting ahead is getting started."
               </p>
               <p className="text-xs text-gray-500 mt-3 font-medium">— Mark Twain</p>
            </div>
          </div>
        </div>

      ) : (
      /* ───── POPULATED STATE (existing layout) ───── */
      <>
      {/* Stats Grid */}
      <div className="grid grid-cols-4 gap-6">
         <StatCard title="Completed today" value={`${m.completedToday} tasks`} sub="✓ On pace" subColor="text-green-500" />
         <StatCard title="Current Streak" value={`${m.currentStreak} days`} sub="☁ Consistent" subColor="text-amber-500" />
         <StatCard title="Focus Hours" value={`${m.focusHours} h`} sub="◎ Deep Work" subColor="text-purple-500" />
         <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 flex justify-between items-center shadow-xl">
            <div>
               <h3 className="text-sm font-semibold text-gray-400 mb-1">Daily Goal</h3>
               <div className="text-2xl font-bold text-white mb-2 tracking-tight">{m.focusHours} / {m.dailyGoalHours} <span className="text-sm text-gray-500 font-medium">hrs</span></div>
               <span className="text-xs font-semibold text-purple-400">{m.dailyGoalProgressPercent}% Complete</span>
            </div>
            <div 
               className="w-16 h-16 rounded-full flex items-center justify-center"
               style={{ backgroundImage: `conic-gradient(#a855f7 ${m.dailyGoalProgressPercent}%, #2D3342 0)` }}
            >
               <div className="w-12 h-12 bg-[#0D1117] rounded-full flex items-center justify-center text-xs font-bold text-white">
                  {m.dailyGoalProgressPercent}%
               </div>
            </div>
         </div>
      </div>

      <div className="grid grid-cols-[1fr_320px] gap-6">
         {/* Main Column */}
         <div className="space-y-6">
            <div className="flex justify-between items-end">
               <div>
                  <h2 className="text-lg font-bold text-white mb-1">Today's Focus</h2>
                  <p className="text-sm text-gray-400">Prioritized queue for your shift</p>
               </div>
               <span className="text-[10px] font-bold tracking-widest text-gray-500 uppercase px-3 py-1 bg-[#1C212B] rounded-full">{tasks.length} Tasks</span>
            </div>

            <div className="space-y-4">
               {tasks.map(t => (
                  <TaskCard key={t.id} task={t} action={handleTaskAction} onOpen={() => navigate(`/worker/task/${t.id}`)} />
               ))}
            </div>

            {/* Weekly Graph */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 mt-6 shadow-xl">
               <div className="flex justify-between items-center mb-8">
                  <h3 className="text-lg font-bold text-white">Weekly Load Distribution</h3>
                  <div className="flex gap-2">
                     <ScaleButton active={scaleMode==='focus'} onClick={() => setScaleMode('focus')} label="Focus Time" />
                     <ScaleButton active={scaleMode==='estimated'} onClick={() => setScaleMode('estimated')} label="Estimated" />
                     <ScaleButton active={scaleMode==='tasks'} onClick={() => setScaleMode('tasks')} label="Task Count" />
                  </div>
               </div>
               
               <div className="flex justify-between items-end h-32 px-4 border-b border-[#2D3342] pb-2 relative">
                  {m.weeklyLoadDistribution.map((d, i) => {
                     const maxScale = scaleMode === 'tasks' ? 10 : 12;
                     const val = scaleMode === 'tasks' ? d.taskCount : scaleMode === 'estimated' ? d.estimatedHours : d.focusTimeLogged;
                     const pct = Math.min((val / maxScale) * 100, 100);
                     const today = new Date().getDay();
                     const dayMap = ['SUN','MON','TUE','WED','THU','FRI','SAT'];
                     const isToday = d.day === dayMap[today];

                     return (
                        <div key={d.day} className="flex flex-col items-center gap-3 w-8 group">
                           <div className="w-2.5 h-32 bg-[#161B22] rounded-full relative flex items-end">
                              <div 
                                 className={`w-full rounded-full transition-all duration-500 ${isToday ? 'bg-purple-500 shadow-[0_0_15px_rgba(168,85,247,0.6)]' : 'bg-gray-500 group-hover:bg-purple-400/50'}`}
                                 style={{ height: `${pct}%` }} 
                              />
                           </div>
                           <span className={`text-[10px] font-bold tracking-widest ${isToday ? 'text-purple-400' : 'text-gray-600'}`}>{d.day}</span>
                        </div>
                     );
                  })}
               </div>
            </div>

         </div>

         {/* Sidebar Column */}
         <div className="space-y-6">
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl">
               <h3 className="text-xs font-bold text-gray-400 tracking-widest uppercase mb-4 flex items-center gap-2">
                  <Square size={14} className="text-purple-500" /> Quick Notes
               </h3>
               <textarea 
                  value={notes}
                  onChange={e => setNotes(e.target.value)}
                  placeholder="Jot down temporary thoughts or task specifics here..."
                  className="w-full h-24 bg-[#0A0D14]/50 border border-purple-500/20 focus:border-purple-500/60 rounded-xl p-3 text-sm text-white resize-none outline-none transition-colors"
               />
            </div>

            <div className="bg-[#0A0D14] border border-[#1C212B] rounded-2xl overflow-hidden shadow-xl relative">
               <div className="bg-[#0D1117] p-3 border-b border-[#1C212B] flex items-center justify-between">
                  <div className="flex gap-1.5">
                     <div className="w-2.5 h-2.5 rounded-full bg-red-500" />
                     <div className="w-2.5 h-2.5 rounded-full bg-amber-500" />
                     <div className="w-2.5 h-2.5 rounded-full bg-green-500" />
                  </div>
                  <span className="text-[10px] font-mono text-gray-500">deadlines ~ bin/watch</span>
               </div>
               <div className="p-4 space-y-4">
                  <DeadlineRow title="Q3 Reporting Data" proj="Analytics" time="IN 2 HOURS" isUrgent />
                  <DeadlineRow title="Design Handoff" proj="Client Portal" time="TOMORROW" />
               </div>
            </div>

            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl">
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-4">Recent Activity</h3>
               <div className="space-y-4 border-l border-[#2D3342] ml-2 pl-4 relative">
                  {(activities.length > 0 ? activities : [
                     { title: "Started working on", target: "API Integration", time: "15 mins ago", dot: "bg-purple-500" },
                     { title: "Completed", target: "Daily Standup", time: "2 hours ago", dot: "bg-gray-500" },
                     { title: "MR commented on your PR", quote: "Looks good, just minor formatting...", time: "Yesterday", dot: "bg-gray-500" }
                  ]).map((act, i) => (
                     <div key={i} className="relative">
                        <div className={`absolute -left-[21px] top-1 w-2 h-2 rounded-full ${act.dot || 'bg-purple-500 shadow-[0_0_8px_rgba(168,85,247,0.8)]'}`} />
                        <p className="text-white text-xs font-medium mb-0.5">{act.title} <span className="text-gray-400 font-normal">{act.target}</span></p>
                        {act.quote && <p className="text-xs text-gray-400 italic bg-[#1C212B]/50 p-2 rounded block mt-1 border-l-2 border-gray-500">"{act.quote}"</p>}
                        <span className="text-[10px] text-gray-600 font-medium block mt-1">{act.time}</span>
                     </div>
                  ))}
               </div>
            </div>

         </div>
      </div>
      </>
      )}
    </motion.div>
  );
}

// Sub-components
function StatCard({ title, value, sub, subColor }) {
   return (
      <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl relative overflow-hidden group">
         <div className="absolute top-0 right-0 w-24 h-24 bg-gradient-to-br from-purple-500/5 to-transparent rounded-bl-full pointer-events-none" />
         <h3 className="text-sm font-semibold text-gray-400 mb-2">{title}</h3>
         <div className="text-3xl font-bold text-white mb-2 tracking-tight group-hover:text-purple-400 transition-colors">{value}</div>
         <div className={`text-xs font-semibold ${subColor} flex items-center bg-[#161B22] w-max px-2 py-0.5 rounded cursor-pointer border border-[#2D3342]`}>
            {sub}
         </div>
      </div>
   );
}

function TaskCard({ task, action, onOpen }) {
   const isInProgress = task.status === 'InProgress' || task.status === 1;
   const priorityMap = { 0: 'Low', 1: 'Medium', 2: 'High', Low: 'Low', Medium: 'Medium', High: 'High' };
   const priority = priorityMap[task.priority] || 'Medium';
   
   return (
      <div className={`bg-[#0D1117]/80 backdrop-blur-xl border rounded-2xl p-5 shadow-xl transition-all ${isInProgress ? 'border-purple-500/40 shadow-[0_0_20px_rgba(168,85,247,0.1)]' : 'border-[#1C212B] hover:border-[#2D3342]'}`}>
         <div className="flex justify-between items-start mb-3">
            <div className="flex gap-2 items-center">
               <span className={`text-[9px] font-bold tracking-widest uppercase px-2 py-0.5 rounded border ${priority === 'High' ? 'text-amber-500 border-amber-500/20 bg-amber-500/10' : priority === 'Medium' ? 'text-purple-400 border-purple-500/20 bg-purple-500/10' : 'text-gray-400 border-gray-500/30'}`}>{priority} Priority</span>
               {task.tags?.map(tg => <span key={tg} className="text-[9px] font-bold tracking-widest uppercase px-2 py-0.5 rounded border border-gray-600/30 bg-[#1C212B] text-gray-400">{tg}</span>)}
               {isInProgress && <span className="text-[10px] font-medium text-purple-400 flex items-center gap-1.5 ml-2"><div className="w-1.5 h-1.5 bg-purple-500 rounded-full animate-pulse" /> In Progress</span>}
            </div>
            <span className="text-xs text-gray-500 flex items-center gap-1"><AlertCircle size={12}/> Due {task.deadline ? new Date(task.deadline).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) : task.due || 'TBD'}</span>
         </div>
         
         <div className="mb-5 cursor-pointer" onClick={onOpen}>
            <h3 className="text-lg font-bold text-white mb-1 hover:text-purple-400 transition-colors">{task.title}</h3>
            <p className="text-sm text-gray-400">{task.description}</p>
            <span className="text-[11px] text-gray-500 font-medium block mt-2">{task.estimatedTimeHours || task.estimatedHours || 0}h estimated</span>
         </div>

         <div className="flex gap-3 pt-4 border-t border-[#1C212B]/60">
            {isInProgress ? (
               <>
                 <button onClick={() => action(task.id, 'stop')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-purple-500/10 border border-purple-500/30 text-purple-400 hover:bg-purple-500/20 text-xs font-bold transition-all"><Square size={14} /> Stop</button>
                 <button onClick={() => action(task.id, 'complete')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-[#161B22] border border-[#2D3342] text-gray-400 hover:text-white hover:border-gray-500 text-xs font-bold transition-all"><Check size={14} /> Complete</button>
                 <button onClick={() => action(task.id, 'block')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-red-500/10 border border-red-500/30 text-red-500 hover:bg-red-500/20 text-xs font-bold transition-all"><AlertCircle size={14} /> Blocked</button>
                 <button onClick={() => action(task.id, 'reject')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-amber-500/10 border border-amber-500/30 text-amber-400 hover:bg-amber-500/20 text-xs font-bold transition-all"><AlertCircle size={14} /> Reject</button>
               </>
            ) : (
               <>
                 <button onClick={() => action(task.id, 'start')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-[#1C212B] border border-[#2D3342] text-gray-300 hover:text-white hover:bg-purple-500/20 hover:border-purple-500/40 text-xs font-bold transition-all"><Play size={14} /> Start</button>
                 <button onClick={() => action(task.id, 'complete')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-[#161B22] border border-[#2D3342] text-gray-500 hover:text-white hover:border-gray-500 text-xs font-bold transition-all"><Check size={14} /> Complete</button>
                 <button onClick={() => action(task.id, 'reject')} className="flex items-center gap-2 py-1.5 px-4 rounded-lg bg-amber-500/10 border border-amber-500/30 text-amber-400 hover:bg-amber-500/20 text-xs font-bold transition-all"><AlertCircle size={14} /> Reject</button>
               </>
            )}
         </div>
      </div>
   );
}

function DeadlineRow({ title, proj, time, isUrgent }) {
   return (
      <div className="flex justify-between items-center group cursor-pointer border border-transparent hover:border-[#2D3342] p-1.5 -mx-1.5 rounded transition-all">
         <div>
            <h4 className="text-xs font-bold text-white mb-0.5 group-hover:text-purple-400">{title}</h4>
            <p className="text-[10px] text-gray-500">Project: {proj}</p>
         </div>
         <span className={`text-[9px] font-bold tracking-widest uppercase px-2 py-0.5 rounded border ${isUrgent ? 'border-red-500/20 bg-red-500/10 text-red-500' : 'border-amber-500/20 bg-amber-500/10 text-amber-500'}`}>{time}</span>
      </div>
   );
}

function ScaleButton({ active, label, onClick }) {
   return (
      <button 
         onClick={onClick}
         className={`px-3 py-1 rounded-full text-[10px] font-bold tracking-widest uppercase transition-colors border ${active ? 'bg-purple-500/20 border-purple-500/50 text-purple-400' : 'bg-[#1C212B] border-transparent text-gray-500 hover:text-gray-300'}`}
      >
         {label}
      </button>
   )
}
