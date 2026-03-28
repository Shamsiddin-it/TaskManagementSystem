import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Check, Layers } from 'lucide-react';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';

export default function WorkerActivity() {
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    // Simulated aggregate data load
    setLoading(false);
  }, []);

  if (loading) return null;

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col max-w-7xl mx-auto w-full pb-20">
      
      {/* ───────────────────────────────────────────────────────── */}
      {/* HERO SECTION 1: END OF DAY WRAP-UP (Screenshot 4) */}
      {/* ───────────────────────────────────────────────────────── */}
      <div className="flex items-center justify-between mt-2 mb-6">
         <div>
            <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">End of Day Wrap-up</h1>
            <p className="text-gray-400 text-sm">Wednesday, Oct 23 • Review your progress and plan ahead.</p>
         </div>
         <div className="flex gap-4 items-center">
            <span className="text-[10px] font-bold text-gray-500 uppercase tracking-widest">Shift Complete</span>
            <div className="w-10 h-10 rounded-full bg-[#1C212B] border border-[#2D3342] flex items-center justify-center text-white font-bold font-mono">JD</div>
         </div>
      </div>

      <div className="grid grid-cols-[1fr_360px] gap-6 mb-16">
         
         {/* Left Column */}
         <div className="space-y-6 flex flex-col">
            {/* Completed Today Box */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl flex flex-col p-6 shadow-xl flex-1">
               <div className="flex justify-between items-center mb-6">
                  <h2 className="text-[14px] font-bold text-white">Completed Today</h2>
                  <span className="text-[10px] font-bold text-gray-500 uppercase tracking-widest">8/8 Tasks</span>
               </div>
               <div className="space-y-3">
                  <CompletedTaskRow title="API Integration for Payment Gateway" meta="2.5h Focus Time • Backend" />
                  <CompletedTaskRow title="Fix Navigation Bug on Mobile Safari" meta="1.2h Focus Time • UI/UX" />
                  <CompletedTaskRow title="Update API Documentation for v2.1" meta="0.8h Focus Time • Docs" />
                  
                  <div className="p-4 rounded-xl border border-[#1C212B] bg-[#0A0D14]/50 flex items-center font-medium text-xs text-gray-500">
                     <Check size={14} className="text-green-500 mr-3" />
                     + 5 other routine maintenance tasks
                  </div>
               </div>
            </div>

            {/* Tomorrow's Priorities */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl flex flex-col p-6 shadow-xl">
               <div className="flex justify-between items-center mb-6">
                  <h2 className="text-[14px] font-bold text-white">Tomorrow's Priorities</h2>
                  <span className="text-[10px] font-bold text-gray-500 uppercase tracking-widest">Plan Ahead</span>
               </div>
               <div className="space-y-5">
                  <div>
                     <span className="text-[9px] font-bold text-gray-500 uppercase tracking-widest mb-1.5 block">Top Priority</span>
                     <div className="bg-[#0A0D14]/50 border border-[#1C212B] rounded-xl p-4 text-sm font-semibold text-white">
                        Database migration for legacy accounts
                     </div>
                  </div>
                  <div>
                     <span className="text-[9px] font-bold text-gray-500 uppercase tracking-widest mb-1.5 block">Secondary</span>
                     <div className="bg-[#0A0D14]/50 border border-[#1C212B] rounded-xl p-4 text-sm font-semibold text-gray-400">
                        Compile release notes for Sprint 42
                     </div>
                  </div>
               </div>
            </div>
         </div>

         {/* Right Column */}
         <div className="space-y-6 flex flex-col">
            
            {/* Productivity Score */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl flex flex-col p-6 shadow-xl relative overflow-hidden">
               <div className="absolute top-0 right-0 w-32 h-32 bg-purple-500/5 rounded-bl-[100px] pointer-events-none" />
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase text-center mb-8">Productivity Score</h3>
               
               {/* Radial Ring */}
               <div className="flex justify-center mb-8 relative">
                  <svg className="w-32 h-32 drop-shadow-[0_0_15px_rgba(168,85,247,0.4)]" viewBox="0 0 100 100">
                     {/* Background Ring */}
                     <circle cx="50" cy="50" r="40" fill="transparent" stroke="#1C212B" strokeWidth="8" />
                     {/* Progress Ring (92%) */}
                     <circle cx="50" cy="50" r="40" fill="transparent" stroke="#a855f7" strokeWidth="8" strokeDasharray="251.2" strokeDashoffset="20.096" strokeLinecap="round" className="origin-center -rotate-90" />
                  </svg>
                  <div className="absolute inset-0 flex flex-col items-center justify-center">
                     <span className="text-3xl font-bold text-white tracking-tight leading-none mb-0.5">92</span>
                     <span className="text-[10px] font-bold text-purple-400">A+ Elite</span>
                  </div>
               </div>

               {/* Sub Scores */}
               <div className="space-y-4">
                  <ScoreBar label="Focus" val="95%" width="w-[95%]" />
                  <ScoreBar label="Velocity" val="88%" width="w-[88%]" />
                  <ScoreBar label="Consistency" val="92%" width="w-[92%]" />
               </div>
            </div>

            {/* Split Metrics */}
            <div className="grid grid-cols-2 gap-6">
               <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl">
                  <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-2">Focus Hours</h3>
                  <div className="text-2xl font-bold text-white mb-2">6.8 <span className="text-sm font-medium text-gray-400">hrs</span></div>
                  <div className="text-xs font-semibold text-green-500">↑ 12% vs avg</div>
               </div>
               <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl">
                  <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-2">Streak Progress</h3>
                  <div className="text-2xl font-bold text-white mb-2">6 <span className="text-sm font-medium text-gray-400">days</span></div>
                  <div className="text-xs font-medium text-gray-300">Consistent Level</div>
               </div>
            </div>

            {/* Badges Box Placeholder */}
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-5 shadow-xl flex items-center gap-4">
               <div className="w-12 h-12 rounded-xl bg-purple-500/10 flex items-center justify-center shrink-0 border border-purple-500/20 shadow-[0_0_15px_rgba(168,85,247,0.2)]">
                  <Layers size={20} className="text-purple-400" />
               </div>
               <div>
                  <h4 className="text-sm font-bold text-white">New Badge Earned</h4>
                  <p className="text-xs text-gray-400">"The Finisher" - 100% completion</p>
               </div>
            </div>
         </div>
      </div>


      {/* ───────────────────────────────────────────────────────── */}
      {/* SECTION 2: TEAM AVAILABILITY (Screenshot 5) */}
      {/* ───────────────────────────────────────────────────────── */}
      <div className="flex items-center justify-between mb-6 pt-6 border-t border-[#1C212B]">
         <div>
            <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Team Availability</h1>
            <p className="text-gray-400 text-sm">Collaborate and synchronize in real-time.</p>
         </div>
         <button className="px-4 py-2 border border-[#2D3342] bg-[#161B22] text-gray-300 hover:text-white rounded-lg text-xs font-bold transition-colors">
            + Add Member
         </button>
      </div>

      <div className="grid grid-cols-[1fr_360px] gap-6">
         
         {/* Team Cards Grid */}
         <div className="grid grid-cols-2 gap-6 h-fit">
            
            <TeamMemberCard 
               initials="SC" name="Sarah Chen" role="Senior Frontend Engineer" color="bg-green-500"
               focus="5.2h Focus" stateLabel="CURRENTLY WORKING" stateVal="Refactoring Component Library"
               stateColor="text-purple-400" stateBar="bg-purple-500 shadow-[0_0_10px_rgba(168,85,247,0.5)]"
               tags={['UI/UX', 'VUE']}
            />

            <TeamMemberCard 
               initials="MR" name="Marcus Reed" role="DevOps Specialist" color="bg-red-500"
               focus="2.8h Focus" stateLabel="MEETING" stateVal="Sprint Planning - Core Infra"
               stateDesc="Ends in 12 mins" stateColor="text-gray-400" stateBar="bg-[#2D3342]"
               tags={['AWS', 'K8S']}
            />

            <TeamMemberCard 
               initials="EV" name="Elena Volkov" role="Product Designer" color="bg-amber-500"
               focus="4.0h Focus" stateLabel="PAUSED TASK" stateVal="Mobile Navigation Audit"
               stateColor="text-gray-400" stateBar="bg-[#2D3342]"
               tags={['FIGMA', 'RESEARCH']}
            />

            <TeamMemberCard 
               initials="JT" name="Jordan Taylor" role="Backend Developer" color="bg-green-500"
               focus="6.5h Focus" stateLabel="CURRENTLY WORKING" stateVal="Schema Migration v4"
               stateColor="text-purple-400" stateBar="bg-purple-500 shadow-[0_0_10px_rgba(168,85,247,0.5)]"
               tags={['REDIS', 'SQL']}
            />

         </div>

         {/* Right Sidebar Timeline */}
         <div className="space-y-6 flex flex-col">
            
            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl flex flex-col p-6 shadow-xl flex-1 relative">
               <div className="flex justify-between items-center mb-6">
                  <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase">Real-Time Feed</h3>
                  <span className="text-[10px] font-bold text-purple-400 tracking-widest uppercase animate-pulse">Live</span>
               </div>
               
               <div className="space-y-6 relative border-l border-[#1C212B] ml-1.5 pl-4">
                  
                  <FeedItem dot="bg-purple-500 shadow-[0_0_8px_rgba(168,85,247,0.8)]" time="Just now">
                     <span className="font-bold text-white">Sarah Chen</span> pushed 4 commits to <span className="font-mono text-xs bg-[#161B22] px-1 rounded text-gray-300 border border-[#2D3342]">main</span>
                  </FeedItem>

                  <FeedItem dot="bg-gray-500" time="14 mins ago">
                     <span className="font-bold text-white">Jordan Taylor</span> resolved issue <span className="text-gray-300">#402</span>
                  </FeedItem>

                  <FeedItem dot="bg-gray-500" time="42 mins ago">
                     <span className="font-bold text-white">Marcus Reed</span> updated build status
                     <div className="mt-2 text-[11px] font-mono text-green-500 bg-green-500/10 border border-green-500/20 px-2 py-1 rounded w-max">
                        Deploy successful to production
                     </div>
                  </FeedItem>

                  <FeedItem dot="bg-gray-500" time="2 hours ago">
                     <span className="font-bold text-white">Elena Volkov</span> commented on <span className="text-gray-300">Design Audit</span>
                  </FeedItem>

                  <FeedItem dot="bg-gray-500" time="3 hours ago">
                     <span className="font-bold text-white">Sarah Chen</span> started a Focus Session
                  </FeedItem>
                  
               </div>
            </div>

            <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
               <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-4">Team Velocity</h3>
               <div className="flex items-end gap-3 mb-3">
                  <span className="text-3xl font-bold text-white tracking-tight leading-none">92%</span>
                  <span className="text-xs font-semibold text-green-500 pb-0.5">+4.5% vs last week</span>
               </div>
               <div className="h-1.5 w-full bg-[#161B22] rounded-full overflow-hidden">
                  <div className="h-full bg-purple-500 shadow-[0_0_8px_rgba(168,85,247,0.5)] w-[92%] rounded-full" />
               </div>
            </div>

         </div>

      </div>

    </motion.div>
  );
}

// Micro-components
function CompletedTaskRow({ title, meta }) {
   return (
      <div className="p-4 rounded-xl border border-[#1C212B] bg-[#161B22]/30 flex items-center justify-between group hover:border-[#2D3342] transition-colors">
         <div className="flex items-center gap-3">
            <Check size={16} className="text-green-500 shrink-0" />
            <div>
               <h4 className="text-sm font-bold text-white mb-0.5 group-hover:text-purple-400 transition-colors">{title}</h4>
               <p className="text-[11px] text-gray-500 font-medium">{meta}</p>
            </div>
         </div>
      </div>
   );
}

function ScoreBar({ label, val, width }) {
   return (
      <div className="grid grid-cols-[80px_1fr_40px] items-center gap-4 text-xs">
         <span className="text-gray-400 font-medium">{label}</span>
         <div className="h-1.5 w-full bg-[#1C212B] rounded-full">
            <div className={`h-full bg-purple-500 shadow-[0_0_8px_rgba(168,85,247,0.8)] rounded-full ${width}`} />
         </div>
         <span className="text-white font-bold text-right">{val}</span>
      </div>
   );
}

function TeamMemberCard({ initials, name, role, color, focus, stateLabel, stateVal, stateDesc, stateColor, stateBar, tags }) {
   return (
      <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl hover:border-[#2D3342] transition-colors flex flex-col group">
         
         <div className="flex items-start justify-between mb-6">
            <div className="flex items-center gap-4">
               <div className="relative">
                  <div className="w-10 h-10 rounded-full border border-[#2D3342] bg-[#161B22] flex flex-col items-center justify-center font-bold text-white font-mono group-hover:bg-[#1C212B] transition-colors">
                     {initials}
                  </div>
                  <div className={`absolute bottom-0 right-0 w-3 h-3 rounded-full border-[2.5px] border-[#0D1117] ${color}`} />
               </div>
               <div>
                  <h3 className="text-white font-bold tracking-tight">{name}</h3>
                  <p className="text-xs text-gray-400">{role}</p>
               </div>
            </div>
            <span className="text-[10px] text-purple-400 font-mono tracking-tighter">{focus}</span>
         </div>

         <div className="bg-[#0A0D14]/80 border border-[#1C212B] rounded-xl p-4 mb-auto">
            <span className={`text-[9px] font-bold ${stateColor} tracking-widest uppercase mb-1 block`}>{stateLabel}</span>
            <h4 className="text-[14px] font-bold text-white leading-tight mb-3">{stateVal}</h4>
            {stateDesc ? (
               <p className="text-xs text-gray-500 mt-[-4px] mb-3">{stateDesc}</p>
            ) : null}
            <div className="h-[3px] w-3/4 bg-[#161B22] rounded-full overflow-hidden">
               <div className={`h-full rounded-full ${stateBar} w-[80%]`} />
            </div>
         </div>

         <div className="flex gap-2 mt-4">
            {tags.map(t => (
               <span key={t} className="text-[9px] font-bold text-gray-500 uppercase tracking-widest bg-[#161B22] px-2 py-0.5 rounded shadow-inner">
                  {t}
               </span>
            ))}
         </div>

      </div>
   );
}

function FeedItem({ children, dot, time }) {
   return (
      <div className="relative pb-2 last:pb-0">
         <div className={`absolute -left-[20px] top-1.5 w-2 h-2 rounded-full ${dot}`} />
         <p className="text-gray-400 text-sm">{children}</p>
         <div className="text-[10px] text-gray-600 font-bold tracking-widest uppercase mt-1">{time}</div>
      </div>
   );
}
