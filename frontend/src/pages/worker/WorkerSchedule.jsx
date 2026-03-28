import React, { useState, useEffect, useRef } from 'react';
import { motion } from 'framer-motion';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

export default function WorkerSchedule() {
  const [loading, setLoading] = useState(true);
  const [events, setEvents] = useState([]);
  const containerRef = useRef(null);

  // Constants
  const START_HOUR = 9;
  const END_HOUR = 17;
  const TOTAL_HOURS = END_HOUR - START_HOUR; // 8 hours duration shown
  // We'll map each hour to a specific height pixel value for precision drawing
  const HOUR_HEIGHT = 80; // 80px per hour
  const GRID_HEIGHT = TOTAL_HOURS * HOUR_HEIGHT; 

  const days = [
    { name: 'MON', date: 21 },
    { name: 'TUE', date: 22 },
    { name: 'WED', date: 23, isToday: true },
    { name: 'THU', date: 24 },
    { name: 'FRI', date: 25 },
    { name: 'SAT', date: 26 },
    { name: 'SUN', date: 27 },
  ];

  // Colors map
  const colors = {
    yellow: "border-amber-500 bg-[#161B22] text-amber-500",
    purple: "border-purple-500 bg-[#161B22] text-purple-400",
    green: "border-green-500 bg-[#161B22] text-green-400",
    red: "border-red-500 bg-[#161B22] text-red-500",
    orange: "border-orange-500 bg-[#161B22] text-orange-400",
    blue: "border-blue-500 bg-[#161B22] text-blue-400"
  };

  useEffect(() => {
    async function fetchSchedule() {
       const token = localStorage.getItem('token');
       try {
          const res = await fetch(`${API_BASE_URL}/api/scheduleEvents`, { headers: { 'Authorization': `Bearer ${token}` }});
          if (res.ok) {
             setEvents(await res.json());
          }
       } catch (err) {}
       setLoading(false);
    }
    fetchSchedule();
  }, []);

  // Use robust high-fidelity mocks matching the image identical design
  const scheduleEvents = events.length > 0 ? events : [
    { id: 1, dayIdx: 0, startMins: 0, durationMins: 120, title: "Deep Work: API Architecture", time: "09:00 - 11:00", color: "green", desc: "" },
    { id: 2, dayIdx: 0, startMins: 150, durationMins: 60, title: "Standup Sync", time: "11:30 - 12:30", color: "purple", desc: "Team Core" },
    { id: 3, dayIdx: 0, startMins: 300, durationMins: 90, title: "Bug Triage", time: "14:00 - 15:30", color: "orange", desc: "" },
    
    { id: 4, dayIdx: 1, startMins: 60, durationMins: 60, title: "Client Kickoff", time: "10:00 - 11:00", color: "purple", desc: "" },
    { id: 5, dayIdx: 1, startMins: 180, durationMins: 180, title: "Frontend Implementation", time: "12:00 - 15:00", color: "green", desc: "Document" },
    
    { id: 6, dayIdx: 2, startMins: 0, durationMins: 150, title: "Payment Gateway Core", time: "09:00 - 11:30", color: "green", desc: "High Priority" },
    { id: 7, dayIdx: 2, startMins: 240, durationMins: 60, title: "Architecture Review", time: "13:00 - 14:00", color: "purple", desc: "Current Session Focus" },
    { id: 8, dayIdx: 2, startMins: 330, durationMins: 60, title: "Documentation Update", time: "14:30 - 15:30", color: "orange", desc: "" },
    
    { id: 9, dayIdx: 3, startMins: 60, durationMins: 120, title: "Code Refactoring", time: "10:00 - 12:00", color: "green", desc: "Design Sync" },
    { id: 10, dayIdx: 3, startMins: 300, durationMins: 120, title: "Design Workshop", time: "14:00 - 16:00", color: "purple", desc: "Release Prep" },
    
    { id: 11, dayIdx: 4, startMins: 0, durationMins: 60, title: "Weekly Retrospective", time: "09:00 - 10:00", color: "yellow", desc: "" },
    { id: 12, dayIdx: 4, startMins: 120, durationMins: 240, title: "Innovation Lab Time", time: "11:00 - 15:00", color: "green", desc: "" },
  ];

  const getPos = (mins) => (mins / 60) * HOUR_HEIGHT;

  if (loading) return null;

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col gap-6 max-w-7xl mx-auto w-full pb-10">
      
      {/* Header */}
      <div className="flex items-center justify-between">
         <div>
            <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Weekly Schedule</h1>
            <p className="text-gray-400 text-sm">October 21 - 27, 2024</p>
         </div>
         <div className="flex gap-4 items-center">
            <div className="bg-[#1C212B] p-1 rounded-lg border border-[#2D3342] flex text-[11px] font-bold tracking-widest uppercase">
               <button className="px-5 py-1.5 rounded-md text-gray-400 hover:text-white transition-colors">Day</button>
               <button className="px-5 py-1.5 rounded-md bg-[#2D3342] text-white shadow-xl">Week</button>
               <button className="px-5 py-1.5 rounded-md text-gray-400 hover:text-white transition-colors">Month</button>
            </div>
            <div className="w-10 h-10 rounded-full bg-[#1C212B] border border-[#2D3342] flex items-center justify-center text-white font-bold font-mono shadow-[0_0_10px_rgba(255,255,255,0.05)]">JD</div>
         </div>
      </div>

      <div className="grid grid-cols-[1fr_320px] gap-6">
         
         {/* Main Calendar Grid */}
         <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl overflow-hidden shadow-2xl flex flex-col">
            
            {/* Days Header */}
            <div className="grid grid-cols-[60px_1fr_1fr_1fr_1fr_1fr_1fr_1fr] border-b border-[#1C212B] bg-[#0A0D14]/50">
               <div className="p-4 border-r border-[#1C212B]" />
               {days.map((d, i) => (
                  <div key={i} className="flex flex-col items-center justify-center p-3 border-r border-[#1C212B] last:border-r-0">
                     <span className={`text-[10px] font-bold tracking-widest uppercase mb-1 ${d.isToday ? 'text-purple-400' : 'text-gray-500'}`}>{d.name}</span>
                     <span className={`text-xl font-bold ${d.isToday ? 'text-purple-400' : 'text-white'}`}>{d.date}</span>
                  </div>
               ))}
            </div>

            {/* Time Grid Scrollable Area */}
            <div className="flex-1 overflow-y-auto custom-scrollbar relative" ref={containerRef} style={{ height: '600px' }}>
               <div className="relative" style={{ height: `${GRID_HEIGHT}px` }}>
                  
                  {/* Background Grid Lines (Horizontal for hours) */}
                  {Array.from({ length: TOTAL_HOURS + 1 }).map((_, i) => (
                     <div key={i} className="absolute w-full border-t border-[#1C212B]/50 flex" style={{ top: `${i * HOUR_HEIGHT}px` }}>
                        <div className="w-[60px] pr-2 -mt-2 text-right text-[10px] font-bold text-gray-600 bg-[#0D1117] relative z-20">
                           {String(START_HOUR + i).padStart(2, '0')}:00
                        </div>
                        <div className="flex-1" />
                     </div>
                  ))}

                  {/* Vertical Day Lines */}
                  {Array.from({ length: 8 }).map((_, i) => (
                     <div key={i} className="absolute top-0 bottom-0 border-l border-[#1C212B]/50" style={{ left: i===0 ? '60px' : `calc(60px + ${(100/7) * i}%)` }} />
                  ))}

                  {/* The Current Time Red Line (e.g. Wed 12:20 -> 3.33 hours from 9:00 = 200px) */}
                  {/* Fake it on Wed col (index 2) */}
                  <div className="absolute z-30 pointer-events-none flex items-center" style={{ top: '266px', left: 'calc(60px + (100%/7) * 2)', width: 'calc(100%/7)' }}>
                     <div className="w-2 h-2 rounded-full bg-red-500 shadow-[0_0_8px_rgba(239,68,68,1)] ml-[-3px]" />
                     <div className="h-[2px] w-full bg-red-500 shadow-[0_0_8px_rgba(239,68,68,0.8)]" />
                  </div>

                  {/* Events Overlay */}
                  {scheduleEvents.map((evt) => (
                     <div 
                        key={evt.id} 
                        className={`absolute z-10 p-2 border-l-4 rounded-xl shadow-lg border-t border-r border-[#2D3342] opacity-90 hover:opacity-100 hover:scale-[1.02] hover:z-20 transition-all cursor-pointer ${colors[evt.color]}`}
                        style={{
                           top: `${getPos(evt.startMins)}px`,
                           height: `${getPos(evt.durationMins)}px`,
                           left: `calc(60px + ${(100/7) * evt.dayIdx}% + 6px)`,
                           width: `calc((100%/7) - 12px)`
                        }}
                     >
                        <h4 className="text-xs font-bold text-white mb-0.5 leading-tight">{evt.title}</h4>
                        {evt.desc && <div className="text-[9px] text-gray-300 font-medium mb-1">{evt.desc}</div>}
                        <div className="text-[9px] font-mono text-gray-500">{evt.time}</div>
                     </div>
                  ))}

               </div>
            </div>
         </div>

         {/* Today's Agenda Sidebar */}
         <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl flex flex-col overflow-hidden shadow-2xl relative">
            <div className="p-6 pb-2">
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-1">Today's Agenda</h3>
               <p className="text-sm font-semibold text-white">Wed, Oct 23</p>
            </div>
            
            <div className="flex-1 overflow-y-auto p-4 space-y-3 custom-scrollbar">
               {/* Fixed List representing Today's events visually translated */}
               <AgendaCard time="09:00" title="Email Triage" desc="Inbox zero protocol" color="bg-purple-500" />
               <AgendaCard time="10:00" title="API Integration" desc="Stripe Webhooks" color="bg-red-500" />
               <AgendaCard time="12:00" title="Quick Lunch" color="bg-gray-500" />
               <AgendaCard time="13:00" title="Deep Work Session" desc="Focus mode active" color="bg-purple-500" isFocus />
               <AgendaCard time="16:00" title="Code Review" desc="PR #842, #845" color="bg-green-500" />
            </div>

            <div className="p-4 mt-auto">
               <div className="border border-dashed border-[#2D3342] hover:border-purple-500/50 hover:bg-purple-500/5 transition-colors rounded-xl p-4 flex items-center justify-center text-xs font-bold tracking-widest uppercase text-gray-500 hover:text-purple-400 cursor-pointer">
                  + Drop task to schedule
               </div>
            </div>
         </div>
      </div>
    </motion.div>
  );
}

function AgendaCard({ time, title, desc, color, isFocus }) {
   return (
      <div className={`p-4 rounded-xl border flex gap-4 ${isFocus ? 'border-purple-500/30 bg-purple-500/5 shadow-[0_0_15px_rgba(168,85,247,0.1)]' : 'border-[#1C212B] bg-[#161B22]/50'}`}>
         <div className="text-[10px] font-bold font-mono text-gray-400 mt-0.5">{time}</div>
         <div style={{ borderColor: isFocus ? 'rgba(168,85,247,0.5)' : 'transparent' }} className={`flex-1 ${isFocus ? 'border-l-2 pl-3 border-purple-500' : 'border-l-2 pl-3 ' + color.replace('bg-','border-')}`}>
            <h4 className={`text-sm font-bold mb-0.5 ${isFocus ? 'text-purple-400' : 'text-white'}`}>{title}</h4>
            {desc && <p className="text-xs text-gray-500">{desc}</p>}
         </div>
      </div>
   );
}
