import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { getProjects } from '../../api';
import { MoreHorizontal, Plus } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import CreateProjectModal from '../../components/employer/CreateProjectModal';

const COLUMNS = [
  { id: 'planning', label: 'PLANNING', dbStatus: 'Planning', color: 'bg-gray-500' },
  { id: 'in_progress', label: 'IN PROGRESS', dbStatus: 'Active', color: 'bg-purple-500' },
  { id: 'at_risk', label: 'AT RISK', dbStatus: 'At Risk', color: 'bg-amber-500' },
  { id: 'done', label: 'DONE', dbStatus: 'Completed', color: 'bg-green-500' }
];

function asList(payload) {
  if (Array.isArray(payload)) return payload;
  if (Array.isArray(payload?.items)) return payload.items;
  if (Array.isArray(payload?.Items)) return payload.Items;
  if (Array.isArray(payload?.data)) return payload.data;
  return [];
}

export default function EmployerProjectsBoard() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [projects, setProjects] = useState([]);
  const [showModal, setShowModal] = useState(false);

  const fetchProjects = async () => {
    const token = localStorage.getItem('token');
    if (!token) return navigate('/login');
    try {
      const res = await getProjects(token);
      setProjects(asList(res));
    } catch (err) {
      console.error("Failed fetching board", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchProjects();
  }, [navigate]);

  if (loading) {
    return (
      <div className="flex h-full flex-col items-center justify-center">
        <div className="w-10 h-10 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <>
    <CreateProjectModal 
      open={showModal} 
      onClose={() => setShowModal(false)}
      onCreated={() => {
         setShowModal(false);
         setLoading(true);
         fetchProjects();
      }}
    />
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      className="h-full flex flex-col"
    >
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Projects Board</h1>
          <p className="text-gray-400 text-sm">{projects.length} active projects across 4 stages</p>
        </div>
        <div className="flex items-center gap-4">
          <div className="flex p-1 bg-[#1C212B] rounded-lg border border-[#2D3342]">
             <button className="px-4 py-1.5 text-xs font-medium bg-[#252A36] text-white rounded-md shadow-sm border border-[#2D3342]/50">Board</button>
             <button className="px-4 py-1.5 text-xs font-medium text-gray-400 hover:text-white transition-colors">List View</button>
          </div>
          <button onClick={() => setShowModal(true)} className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)] text-white rounded-lg text-sm font-medium transition-all">
            <Plus size={16} />
            Create Project
          </button>
        </div>
      </div>

      <div className="flex-1 flex gap-6 overflow-x-auto custom-scrollbar pb-4 min-w-0">
        {COLUMNS.map(col => {
          // In backend: Planning, Active, At Risk, Completed (or AtRisk)
          const columnProjects = projects.filter(p => {
             const stat = p.status?.replace(" ", "") || "";
             const colStat = col.dbStatus.replace(" ", "");
             return stat === colStat;
          });
          
          return (
            <div key={col.id} className="flex-none w-80 flex flex-col">
              <div className="flex items-center justify-between mb-4">
                <div className="flex items-center gap-2">
                  <h3 className="text-xs font-bold text-gray-400 tracking-wider whitespace-nowrap">{col.label}</h3>
                  <span className={`w-5 h-5 flex items-center justify-center rounded-full text-[10px] font-bold text-white shadow-lg ${col.color}`}>
                    {columnProjects.length}
                  </span>
                </div>
                <button className="text-gray-500 hover:text-gray-300">
                  <MoreHorizontal size={16} />
                </button>
              </div>

              <div className="flex-1 overflow-y-auto custom-scrollbar space-y-4 pr-1">
                {columnProjects.length === 0 ? (
                   <div className="border border-dashed border-[#2D3342] rounded-xl p-6 text-center text-xs text-gray-500">
                     Empty Column
                   </div>
                ) : (
                  columnProjects.map(project => (
                    <BoardCard key={project.id} project={project} onClick={() => navigate(`/employer/projects/${project.id}`)} />
                  ))
                )}
              </div>
            </div>
          );
        })}
      </div>
    </motion.div>
    </>
  );
}

function BoardCard({ project, onClick }) {
  const isAtRisk = project.status === "AtRisk" || project.status === "At Risk";
  const isDone = project.status === "Completed" || project.status === "Archived";
  
  let glowColor = "rgba(168,85,247,0.5)"; // Purple
  let barColor = "bg-purple-500";
  let statusClass = "border-[#1C212B] hover:border-[#2D3342]";
  
  if (isAtRisk) {
     glowColor = "rgba(245,158,11,0.5)"; // Amber
     barColor = "bg-amber-500";
     statusClass = "border-amber-500/30 shadow-[0_4px_20px_rgba(245,158,11,0.05)]";
  } else if (isDone) {
     glowColor = "rgba(34,197,94,0.5)"; // Green
     barColor = "bg-green-500";
  }

  // Faked if missing
  const progressPercent = project.completionPercent || (isDone ? 100 : (isAtRisk ? 42 : 64));

  return (
    <button 
      onClick={onClick}
      className={`w-full text-left bg-[#0D1117]/80 backdrop-blur-sm border ${statusClass} rounded-xl p-5 hover:bg-[#161B22]/80 transition-all group`}
    >
      <div className="flex items-center justify-between mb-3">
        <span className="px-2 py-0.5 text-[10px] uppercase tracking-wider font-semibold text-gray-400 bg-[#1C212B] border border-[#2D3342] rounded">
          {project.type || "INTERNAL"}
        </span>
        <div className="flex items-center gap-2">
           <span className="text-[10px] text-gray-500 font-mono tracking-wider">ID: {project.id?.slice(0, 3)}</span>
           <div className={`w-1.5 h-1.5 rounded-full ${barColor}`} style={{ boxShadow: `0 0 8px ${glowColor}` }} />
        </div>
      </div>

      <h4 className="text-white font-semibold mb-4 leading-tight group-hover:text-purple-300 transition-colors">{project.title}</h4>

      <div className="space-y-2 mb-6">
        <div className="flex justify-between text-[11px] font-medium text-gray-400">
           {isAtRisk ? (
              <span className="text-amber-500">Behind Schedule</span>
           ) : isDone ? (
              <span className="text-green-500">Completed</span>
           ) : (
              <span>Progress</span>
           )}
           <span>{progressPercent}%</span>
        </div>
        <div className="h-1 w-full bg-[#1C212B] rounded-full overflow-hidden">
          <div 
            className={`h-full ${barColor} rounded-full`}
            style={{ width: `${progressPercent}%`, boxShadow: `0 0 10px ${glowColor}` }}
          />
        </div>
      </div>

      <div className="flex items-center justify-between text-xs text-gray-500">
        <div className={isAtRisk ? "text-amber-500 font-medium" : ""}>
          {isAtRisk ? "2 days overdue" : (isDone ? "Oct 15" : "Nov 12")}
        </div>
        <div className="flex -space-x-2">
           {/* Mock avatars */}
           <div className="w-6 h-6 rounded-full border border-gray-800 bg-[#252A36] flex items-center justify-center text-[9px] font-bold text-gray-300">AK</div>
           <div className="w-6 h-6 rounded-full border border-gray-800 bg-[#252A36] flex items-center justify-center text-[9px] font-bold text-gray-300">JD</div>
           <div className="w-6 h-6 rounded-full border border-gray-800 bg-[#1C212B] flex items-center justify-center text-[9px] font-bold text-gray-400">+2</div>
        </div>
      </div>
    </button>
  );
}
