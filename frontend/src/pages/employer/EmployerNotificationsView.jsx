import React from 'react';
import { motion } from 'framer-motion';
import { Check, Settings, Clock, DollarSign, Users, Target } from 'lucide-react';

export default function EmployerNotificationsView() {
  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="max-w-4xl mx-auto space-y-8">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Notifications Center</h1>
          <p className="text-gray-400 text-sm">Monitor system alerts and team activity.</p>
        </div>
        <div className="flex gap-3">
          <button className="flex items-center gap-2 px-4 py-2 bg-[#1C212B] hover:bg-[#252A36] text-white border border-[#2D3342] rounded-lg text-sm font-medium transition-colors">
            <Check size={16} /> Mark All as Read
          </button>
          <button className="flex items-center gap-2 px-4 py-2 bg-[#1C212B] hover:bg-[#252A36] text-white border border-[#2D3342] rounded-lg text-sm font-medium transition-colors">
            <Settings size={16} /> Preferences
          </button>
        </div>
      </div>

      <div className="flex gap-2">
         <button className="px-5 py-2 rounded-full border border-purple-500/50 bg-[#1A1525] text-purple-400 text-xs font-bold tracking-wide">All Alerts</button>
         <button className="px-5 py-2 rounded-full border border-transparent text-gray-500 hover:text-white text-xs font-bold tracking-wide transition-colors">Deadlines</button>
         <button className="px-5 py-2 rounded-full border border-transparent text-gray-500 hover:text-white text-xs font-bold tracking-wide transition-colors">Budget</button>
         <button className="px-5 py-2 rounded-full border border-transparent text-gray-500 hover:text-white text-xs font-bold tracking-wide transition-colors">Team</button>
         <button className="px-5 py-2 rounded-full border border-transparent text-gray-500 hover:text-white text-xs font-bold tracking-wide transition-colors">Milestones</button>
      </div>

      <div className="space-y-6">
         <div>
            <h3 className="text-sm font-bold text-white mb-4">High Priority</h3>
            <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl overflow-hidden divide-y divide-[#1C212B]">
               <div className="p-5 flex gap-5 relative group">
                  <div className="absolute left-0 top-0 bottom-0 w-1 bg-amber-500 shadow-[0_0_10px_rgba(245,158,11,0.5)] opacity-100" />
                  <div className="w-10 h-10 rounded-full border border-amber-500/20 bg-amber-500/10 flex items-center justify-center text-amber-500 shrink-0">
                     <Clock size={18} />
                  </div>
                  <div className="flex-1">
                     <span className="text-[10px] font-bold text-amber-500 uppercase tracking-widest bg-amber-500/10 px-2 py-0.5 rounded border border-amber-500/20 mb-2 inline-block">Deadline Warning</span>
                     <h4 className="text-white font-semibold mb-1">Project Beta is 48 hours past scheduled milestone</h4>
                     <p className="text-sm text-gray-400">Phase 2 redesign remains in 'At Risk' status. Action required by Lead Designer.</p>
                  </div>
                  <div className="flex flex-col items-end justify-between">
                     <span className="text-xs text-gray-500">10 min ago</span>
                     <button className="px-3 py-1.5 bg-[#1C212B] border border-[#2D3342] hover:bg-amber-500/10 hover:border-amber-500/30 hover:text-amber-400 text-gray-300 text-xs font-semibold rounded-md transition-colors">Action</button>
                  </div>
               </div>

               <div className="p-5 flex gap-5 relative group">
                  <div className="absolute left-0 top-0 bottom-0 w-1 bg-green-500 shadow-[0_0_10px_rgba(34,197,94,0.5)] opacity-100" />
                  <div className="w-10 h-10 rounded-full border border-green-500/20 bg-green-500/10 flex items-center justify-center text-green-500 shrink-0">
                     <DollarSign size={18} />
                  </div>
                  <div className="flex-1">
                     <span className="text-[10px] font-bold text-green-500 uppercase tracking-widest bg-green-500/10 px-2 py-0.5 rounded border border-green-500/20 mb-2 inline-block">Budget Alert</span>
                     <h4 className="text-white font-semibold mb-1">Infrastructure burn rate exceeded threshold</h4>
                     <p className="text-sm text-gray-400">AWS costs for Project Alpha spiked by 15% in the last 24 hours. Verify cloud resources.</p>
                  </div>
                  <div className="flex flex-col items-end justify-between">
                     <span className="text-xs text-gray-500">2 hours ago</span>
                     <button className="px-3 py-1.5 bg-[#1C212B] border border-[#2D3342] text-gray-300 text-xs font-semibold rounded-md transition-colors">View Detail</button>
                  </div>
               </div>
            </div>
         </div>

         <div>
            <h3 className="text-sm font-bold text-white mb-4">Team & Updates</h3>
            <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl overflow-hidden divide-y divide-[#1C212B]">
               <div className="p-5 flex gap-5">
                  <div className="w-10 h-10 rounded-full border border-[#2D3342] bg-[#161B22] flex items-center justify-center text-blue-400 shrink-0">
                     <Users size={18} />
                  </div>
                  <div className="flex-1">
                     <span className="text-[10px] font-bold text-blue-400 uppercase tracking-widest border border-blue-500/20 mb-2 inline-block">Team Update</span>
                     <h4 className="text-white font-semibold mb-1">New member onboarded to Backend Team</h4>
                     <p className="text-sm text-gray-400">Mike Ross has completed security training and gained access to Project Gamma repos.</p>
                  </div>
                  <div className="flex flex-col items-end justify-start">
                     <span className="text-xs text-gray-500">Yesterday</span>
                  </div>
               </div>

               <div className="p-5 flex gap-5">
                  <div className="w-10 h-10 rounded-full border border-[#2D3342] bg-[#161B22] flex items-center justify-center text-purple-400 shrink-0">
                     <Target size={18} />
                  </div>
                  <div className="flex-1">
                     <span className="text-[10px] font-bold text-gray-400 uppercase tracking-widest border border-gray-600/30 mb-2 inline-block">Milestone Reached</span>
                     <h4 className="text-white font-semibold mb-1">Project Gamma: Authentication v2.0 Live</h4>
                     <p className="text-sm text-gray-400">Final testing complete. Service successfully deployed to production environment.</p>
                  </div>
                  <div className="flex flex-col items-end justify-start">
                     <span className="text-xs text-gray-500">Oct 15</span>
                  </div>
               </div>
            </div>
         </div>
      </div>
    </motion.div>
  );
}
