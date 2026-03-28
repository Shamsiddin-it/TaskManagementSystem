import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { getProjectById } from '../../api';
import { ArrowLeft, MoreHorizontal, Paperclip } from 'lucide-react';
import { useNavigate, useParams } from 'react-router-dom';

export default function EmployerProjectDetailView() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [project, setProject] = useState(null);

  useEffect(() => {
    async function fetchProject() {
      const token = localStorage.getItem('token');
      if (!token) return navigate('/login');
      try {
        const res = await getProjectById(token, id);
        setProject(res);
      } catch (err) {
        console.error("Failed fetching project", err);
      } finally {
        setLoading(false);
      }
    }
    fetchProject();
  }, [id, navigate]);

  if (loading) {
    return (
      <div className="flex h-full flex-col items-center justify-center">
        <div className="w-10 h-10 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin" />
      </div>
    );
  }

  if (!project) {
    return (
       <div className="text-white">Project not found.</div>
    );
  }

  const isAtRisk = project.status === "AtRisk" || project.status === "At Risk";
  const progressPercent = project.completionPercent || 42;

  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="max-w-6xl mx-auto space-y-6 pb-10"
    >
      {/* Header Bar */}
      <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 flex items-center justify-between">
         <div className="flex items-center gap-6">
            <button onClick={() => navigate('/employer/projects')} className="p-2 bg-[#1C212B] hover:bg-[#252A36] text-white rounded-lg transition-colors">
               <ArrowLeft size={18} />
            </button>
            <div>
               <div className="flex items-center gap-3 mb-1">
                  <h1 className="text-2xl font-bold text-white tracking-tight">{project.title}</h1>
                  <span className={`px-2.5 py-1 text-xs font-semibold rounded-full border flex items-center gap-1.5 ${isAtRisk ? 'text-amber-400 border-amber-500/20 bg-amber-500/10' : 'text-purple-400 border-purple-500/20 bg-purple-500/10'}`}>
                    <span className={`w-1.5 h-1.5 rounded-full ${isAtRisk ? 'bg-amber-400 shadow-[0_0_8px_rgba(245,158,11,1)]' : 'bg-purple-400'}`} />
                    {isAtRisk ? "At Risk" : "Active"}
                  </span>
               </div>
               <p className="text-gray-400 text-sm">{project.description || "Created Sep 12, 2023"}</p>
            </div>
         </div>
         <div className="flex items-center gap-3">
            <button className="px-5 py-2.5 bg-[#1C212B] hover:bg-[#252A36] border border-[#2D3342] text-white rounded-xl text-sm font-medium transition-colors border-b-2">
              Manage Access
            </button>
            <button className="px-5 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)] text-white rounded-xl text-sm font-medium transition-all">
              Update Status
            </button>
         </div>
      </div>

      {/* Grid Layout Top Layer */}
      <div className="grid grid-cols-3 gap-6">
         
         {/* Timeline / Phases */}
         <div className="col-span-2 bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl overflow-hidden flex flex-col">
            <div className="p-5 border-b border-[#1C212B] flex items-center justify-between">
               <h3 className="text-white font-semibold">Project Timeline (Q4)</h3>
               <div className="text-xs font-semibold text-gray-500 flex gap-4 tracking-wider">
                 <span>OCT</span><span>NOV</span><span>DEC</span>
               </div>
            </div>
            
            <div className="flex flex-1 relative h-48">
               {/* Fixed column for names */}
               <div className="w-48 border-r border-[#1C212B] bg-[#0D1117] flex flex-col relative z-10 text-sm text-gray-400">
                  <div className="h-10 border-b border-[#1C212B]/50 flex items-center px-5 font-semibold text-gray-500 text-xs tracking-wider">Task Item</div>
                  <div className="h-10 border-b border-[#1C212B]/50 flex items-center px-5 text-gray-300">UI/UX Design</div>
                  <div className="h-10 border-b border-[#1C212B]/50 flex items-center px-5 text-gray-300">API Integration</div>
                  <div className="h-10 border-b border-[#1C212B]/50 flex items-center px-5 text-gray-300">Swift Refactor</div>
                  <div className="h-10 border-b border-[#1C212B]/50 flex items-center px-5 text-gray-300">QA Testing</div>
               </div>
               
               {/* Scrollable Gantt chart area mocked completely */}
               <div className="flex-1 bg-gradient-to-b from-[#0A0D14] to-[#0D1117] relative">
                  {/* Grid lines */}
                  <div className="absolute inset-0 flex justify-between px-[10%] opacity-10 pointer-events-none">
                     <div className="w-px h-full bg-white"/>
                     <div className="w-px h-full bg-white"/>
                     <div className="w-px h-full bg-white"/>
                     <div className="w-px h-full bg-white"/>
                     <div className="w-px h-full bg-white"/>
                     <div className="w-px h-full bg-white"/>
                     <div className="w-px h-full bg-white"/>
                  </div>
                  
                  {/* Pseudo Bars */}
                  <div className="absolute top-[48px] left-[5%] w-[8%] h-[24px] bg-green-500/80 rounded-full shadow-[0_0_10px_rgba(34,197,94,0.4)]" />
                  <div className="absolute top-[88px] left-[13%] w-[12%] h-[24px] bg-amber-500/80 rounded-full shadow-[0_0_10px_rgba(245,158,11,0.4)] ring-2 ring-amber-300 ring-offset-2 ring-offset-[#0A0D14]" />
                  <div className="absolute top-[128px] left-[25%] w-[20%] h-[24px] bg-purple-500/80 rounded-full" />
                  <div className="absolute top-[168px] left-[45%] w-[15%] h-[24px] bg-gray-600/50 rounded-full" />
               </div>
            </div>
         </div>

         {/* Right Sidebar logic: Risks & Budget */}
         <div className="col-span-1 space-y-6">
            {/* Risks Card */}
            <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-amber-500/20 rounded-2xl p-5 shadow-[0_4px_30px_rgba(245,158,11,0.05)] relative overflow-hidden">
               <div className="absolute top-0 left-0 w-full h-1 bg-amber-500/50"></div>
               <div className="flex items-center gap-2 text-amber-500 font-bold text-[11px] uppercase tracking-wider mb-4">
                  <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
                  Critical Risks (2)
               </div>
               <ul className="text-sm text-gray-300 space-y-3 list-inside list-disc marker:text-amber-500/50">
                  <li>Delayed API documentation from backend team.</li>
                  <li>Potential resource conflict with Project Alpha.</li>
               </ul>
            </div>

            {/* Budget Subtracking */}
            <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl p-6 relative overflow-hidden">
               <h3 className="text-[10px] font-bold text-gray-500 uppercase tracking-widest mb-4">Budget Sub-Tracking</h3>
               <div className="mb-2">
                  <span className="text-2xl font-bold text-white tracking-tight">$12,450</span>
               </div>
               <div className="flex justify-between text-xs font-medium mb-2">
                  <span className="text-gray-500">of $15,000 allocated</span>
                  <span className="text-amber-500 shadow-amber-500/20 drop-shadow-md">83% Burn</span>
               </div>
               <div className="h-1.5 w-full bg-[#1C212B] rounded-full overflow-hidden">
                  <div className="h-full bg-amber-500 rounded-full shadow-[0_0_10px_rgba(245,158,11,0.5)]" style={{ width: '83%' }} />
               </div>
            </div>
         </div>
         
      </div>

      {/* Grid Layout Bottom Layer */}
      <div className="grid grid-cols-5 gap-6">
         
         {/* Task Breakdown */}
         <div className="col-span-3 bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl flex flex-col p-6">
            <div className="flex items-center justify-between mb-6 pb-4 border-b border-[#1C212B]">
               <h3 className="text-white font-semibold flex items-center gap-2">Task Breakdown</h3>
               <span className="text-xs font-semibold text-gray-400 font-mono tracking-tight">12/28 Complete</span>
            </div>
            
            <div className="space-y-4 flex-1 text-sm text-gray-300">
               <div className="flex items-start gap-4 line-through opacity-70">
                  <button className="mt-0.5 min-w-[18px] w-[18px] h-[18px] bg-purple-500 rounded-md flex items-center justify-center border border-purple-400 shadow-[0_0_8px_rgba(168,85,247,0.4)]"><svg className="w-3 h-3 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" /></svg></button>
                  <span className="flex-1">Finalize high-fidelity wireframes</span>
               </div>
               <div className="flex items-start gap-4 line-through opacity-70">
                  <button className="mt-0.5 min-w-[18px] w-[18px] h-[18px] bg-purple-500 rounded-md flex items-center justify-center border border-purple-400 shadow-[0_0_8px_rgba(168,85,247,0.4)]"><svg className="w-3 h-3 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" /></svg></button>
                  <span className="flex-1">Setup CI/CD pipeline for iOS</span>
               </div>
               <div className="flex items-start gap-4 items-center group">
                  <button className="min-w-[18px] w-[18px] h-[18px] border border-[#2D3342] rounded-md bg-[#161B22] group-hover:border-purple-500/50 transition-colors" />
                  <span className="flex-1 text-white font-medium">Implement user authentication flow</span>
                  <span className="text-[10px] font-bold text-amber-500 uppercase tracking-widest bg-amber-500/10 px-2 py-0.5 rounded border border-amber-500/20">Blocked</span>
               </div>
               <div className="flex items-start gap-4 group">
                  <button className="mt-0.5 min-w-[18px] w-[18px] h-[18px] border border-[#2D3342] rounded-md bg-[#161B22] group-hover:border-purple-500/50 transition-colors" />
                  <span className="flex-1">Data persistence layer with CoreData</span>
               </div>
               <div className="flex items-start gap-4 group">
                  <button className="mt-0.5 min-w-[18px] w-[18px] h-[18px] border border-[#2D3342] rounded-md bg-[#161B22] group-hover:border-purple-500/50 transition-colors" />
                  <span className="flex-1">Push notification service registration</span>
               </div>
            </div>
         </div>

         {/* Team Discussion */}
         <div className="col-span-2 bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-2xl flex flex-col pt-6 overflow-hidden">
            <h3 className="text-white font-semibold px-6 mb-6">Team Discussion</h3>
            
            <div className="flex-1 overflow-y-auto px-6 space-y-6">
               <div className="relative">
                  {/* Message Top */}
                  <div className="flex items-start gap-3">
                     <div className="w-8 h-8 rounded-full border border-purple-500/30 bg-[#1A1525] flex items-center justify-center text-[10px] font-bold text-purple-400 z-10">AK</div>
                     <div className="flex-1">
                        <div className="flex items-center justify-between mb-2">
                           <div>
                              <span className="text-sm font-semibold text-white mr-2">Anna Kim</span>
                              <span className="text-xs text-gray-500 font-medium">2h ago</span>
                           </div>
                           <MoreHorizontal size={14} className="text-gray-500" />
                        </div>
                        <div className="bg-[#1C212B]/70 border border-[#2D3342]/50 p-4 rounded-xl rounded-tl-none text-sm text-gray-300 leading-relaxed">
                           The redesign for the checkout screen is done, but I'm waiting on the API response structure to finalize the dynamic fields.
                        </div>
                     </div>
                  </div>
                  
                  {/* Reply Branch */}
                  <div className="ml-11 mt-4 relative">
                     <div className="absolute top-[-24px] left-[-20px] w-5 h-8 border-l-2 border-b-2 border-[#2D3342] rounded-bl-xl" />
                     <div className="flex items-start gap-3">
                        <div className="w-6 h-6 rounded-full border border-green-500/30 bg-[#16221B] flex items-center justify-center text-[8px] font-bold text-green-400 z-10 shadow-[0_0_10px_rgba(34,197,94,0.1)]">MR</div>
                        <div className="flex-1">
                           <div className="mb-1">
                              <span className="text-xs font-semibold text-white mr-2">Mike Ross</span>
                              <span className="text-[10px] text-gray-500 font-medium">45m ago</span>
                           </div>
                           <p className="text-xs text-gray-400">Working on this now. Should have the spec by EOD.</p>
                        </div>
                     </div>
                  </div>
               </div>
            </div>

            {/* Input form */}
            <div className="p-4 border-t border-[#1C212B] bg-[#0A0D14] mt-4">
               <div className="relative flex items-center">
                  <input type="text" placeholder="Write a comment..." className="w-full bg-[#161B22] border border-[#2D3342] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-sm text-white rounded-xl py-3 pl-4 pr-10 transition-colors" />
                  <button className="absolute right-3 text-gray-500 hover:text-white transition-colors"><Paperclip size={16} /></button>
               </div>
            </div>
         </div>

      </div>

    </motion.div>
  );
}
