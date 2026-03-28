import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { MessageSquare, Clock, UserPlus, Activity, PieChart, MinusCircle } from 'lucide-react';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

export default function WorkerNotifications() {
  const [loading, setLoading] = useState(true);
  const [notifications, setNotifications] = useState([]);
  const [filter, setFilter] = useState('All'); // 'All', 'Mentions', 'Deadlines'

  useEffect(() => {
    async function fetchNotifications() {
       const token = localStorage.getItem('token');
       try {
          const res = await fetch(`${API_BASE_URL}/api/notification`, { headers: { 'Authorization': `Bearer ${token}` }});
          if (res.ok) {
             // setNotifications(await res.json());
          }
       } catch (err) {}
       setLoading(false);
    }
    fetchNotifications();
  }, []);

  const handleMarkAllRead = async () => {
     // Loop or bulk trigger endpoint
     setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
  };

  if (loading) return null;

  // Use the exact pixel-perfect mocks correlating directly to the provided design
  const items = notifications.length > 0 ? notifications : [
     {
        id: 1, type: "mention", title: "PR Comment", time: "2 mins ago", isRead: false,
        icon: <MessageSquare size={16} />, color: "text-purple-400 bg-purple-500/10 border-purple-500/30", 
        border: "border-l-purple-500",
        content: <span className="text-gray-400 text-sm"><span className="w-5 h-5 rounded-full bg-[#1C212B] text-gray-400 font-bold text-[9px] inline-flex items-center justify-center mr-1.5 border border-[#2D3342]">MR</span> <span className="text-white font-medium">Marcus Reed</span> mentioned you in <span className="text-white font-bold">Refactor Payment Flow</span></span>,
        quote: "Hey @John, could you check if this new endpoint handles the idempotency key correctly? I noticed some latency during testing."
     },
     {
        id: 2, type: "deadline", title: "Upcoming Deadline", time: "15 mins ago", isRead: false,
        icon: <Clock size={16} />, color: "text-red-400 bg-red-500/10 border-red-500/30",
        border: "border-l-red-500",
        content: <span className="text-gray-400 text-sm">Task <span className="text-white font-bold">Q3 Security Audit</span> is due in 2 hours.</span>
     },
     {
        id: 3, type: "assigned", title: "Task Assigned", time: "1 hour ago", isRead: true,
        icon: <UserPlus size={16} />, color: "text-gray-400 bg-[#161B22] border-gray-600/30",
        border: "border-l-transparent",
        content: <span className="text-gray-400 text-sm">Sarah Chen assigned you to <span className="text-white font-bold">Optimize Database Queries</span> in Project: Analytics.</span>
     },
     {
        id: 4, type: "system", title: "System Update", time: "3 hours ago", isRead: true,
        icon: <Activity size={16} />, color: "text-blue-400 bg-blue-500/10 border-blue-500/30",
        border: "border-l-transparent",
        content: <span className="text-gray-400 text-sm">Nexus Core has been updated to v2.4.1. New productivity charts are now available in your dashboard.</span>
     },
     {
        id: 5, type: "summary", title: "Daily Summary", time: "Yesterday", isRead: true,
        icon: <PieChart size={16} />, color: "text-purple-400 bg-purple-500/10 border-purple-500/30",
        border: "border-l-transparent",
        content: <span className="text-gray-400 text-sm">You completed 3 out of 6 tasks today. Great progress!</span>
     },
     {
        id: 6, type: "blocked", title: "Task Blocked", time: "Yesterday", isRead: true,
        icon: <MinusCircle size={16} />, color: "text-gray-400 bg-[#161B22] border-gray-600/30",
        border: "border-l-transparent",
        content: <span className="text-gray-400 text-sm"><span className="text-white font-bold">API Integration</span> has been marked as blocked by <span className="text-white font-medium">Alex Rivers</span>.</span>,
        quote: "Waiting on the dev-ops team to provision the production environment secrets."
     }
  ];

  const filteredItems = items.filter(n => {
     if (filter === 'Mentions') return n.type === 'mention';
     if (filter === 'Deadlines') return n.type === 'deadline';
     return true;
  });

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col gap-6 max-w-7xl mx-auto w-full pb-10">
      
      {/* Header */}
      <div className="flex items-center justify-between">
         <div>
            <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Notifications</h1>
            <p className="text-gray-400 text-sm">Stay updated on your team and projects.</p>
         </div>
         <div className="flex gap-4 items-center">
            <div className="bg-[#0A0D14] p-1 rounded-xl border border-[#1C212B] flex text-xs font-medium">
               <button onClick={() => setFilter('All')} className={`px-4 py-2 rounded-lg transition-colors ${filter==='All' ? 'bg-[#1C212B] text-white shadow-md' : 'text-gray-500 hover:text-white'}`}>All</button>
               <button onClick={() => setFilter('Mentions')} className={`px-4 py-2 rounded-lg transition-colors ${filter==='Mentions' ? 'bg-[#1C212B] text-white shadow-md' : 'text-gray-500 hover:text-white'}`}>Mentions</button>
               <button onClick={() => setFilter('Deadlines')} className={`px-4 py-2 rounded-lg transition-colors ${filter==='Deadlines' ? 'bg-[#1C212B] text-white shadow-md' : 'text-gray-500 hover:text-white'}`}>Deadlines</button>
            </div>
            <button onClick={handleMarkAllRead} className="px-5 py-2.5 bg-[#0D1117]/80 hover:bg-[#1C212B] border border-[#1C212B] text-gray-300 rounded-xl text-xs font-bold transition-colors">
               Mark all as read
            </button>
         </div>
      </div>

      {/* Main Container */}
      <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl overflow-hidden shadow-2xl flex flex-col">
         <div className="divide-y divide-[#1C212B]/60">
            {filteredItems.map(notif => (
               <div key={notif.id} className={`p-5 pl-0 flex gap-5 hover:bg-[#161B22]/50 transition-colors cursor-pointer group`}>
                  
                  {/* Active Indicator & Icon Block */}
                  <div className={`w-1 rounded-r-lg ${!notif.isRead ? notif.border : 'border-l-transparent'} transition-all`} style={{ minHeight: '100%', borderWidth: '0 0 0 3px' }} />
                  
                  <div className="flex-1 flex gap-5 py-1">
                     <div className={`w-12 h-12 rounded-2xl border flex items-center justify-center shrink-0 shadow-lg ${notif.color}`}>
                        {notif.icon}
                     </div>

                     <div className="flex-1 pt-1">
                        <div className="flex justify-between items-start mb-1">
                           <h3 className="text-[14px] font-bold text-white flex items-center gap-2">
                              {notif.title}
                              {!notif.isRead && <div className={`w-1.5 h-1.5 rounded-full ${notif.border.replace('border-l-','bg-')} shadow-[0_0_8px_currentColor]`} />}
                           </h3>
                           <span className="text-xs text-gray-500 font-medium">{notif.time}</span>
                        </div>
                        
                        <div className="mb-3">{notif.content}</div>

                        {notif.quote && (
                           <div className="text-xs text-gray-500 italic bg-[#0A0D14]/80 p-3 rounded-lg border-l-2 border-[#2D3342] mt-2 group-hover:border-purple-500/50 transition-colors">
                              "{notif.quote}"
                           </div>
                        )}
                     </div>
                  </div>

               </div>
            ))}
         </div>
      </div>

    </motion.div>
  );
}
