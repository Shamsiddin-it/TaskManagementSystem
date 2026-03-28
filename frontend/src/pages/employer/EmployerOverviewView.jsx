import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { getWorkspaceOverview, getProjects, getUsers, getCurrentUser } from '../../api';
import { Plus, Users as UsersIcon, ChevronRight } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

export default function EmployerOverviewView() {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [stats, setStats] = useState(null);
  const [projects, setProjects] = useState([]);
  const [team, setTeam] = useState([]);

  useEffect(() => {
    async function loadData() {
      const token = localStorage.getItem('token');
      if (!token) return navigate('/login');
      
      try {
        const [overviewRes, projectsRes, usersRes] = await Promise.all([
          getWorkspaceOverview(token),
          getProjects(token),
          getUsers(token)
        ]);
        
        setStats(overviewRes);
        // Only get active projects
        setProjects((projectsRes || []).filter(p => p.status !== 'Completed' && p.status !== 'Archived').slice(0, 3));
        setTeam((usersRes || []).slice(0, 5));
      } catch (err) {
        console.error("Failed to load overview data", err);
      } finally {
        setLoading(false);
      }
    }
    loadData();
  }, [navigate]);

  if (loading) {
    return (
      <div className="flex h-full items-center justify-center">
        <div className="w-10 h-10 border-4 border-purple-500/20 border-t-purple-500 rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      className="max-w-7xl mx-auto space-y-8"
    >
      {/* Header section */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-white mb-1 tracking-tight">Employer Workspace</h1>
          <p className="text-gray-400 text-sm">Manage your organization and track resources.</p>
        </div>
        <div className="flex items-center gap-3">
          <button onClick={() => navigate('/employer/team')} className="flex items-center gap-2 px-4 py-2 bg-[#1C212B] hover:bg-[#252A36] text-white border border-[#2D3342] rounded-lg text-sm font-medium transition-colors">
            <UsersIcon size={16} />
            Invite Team
          </button>
          <button onClick={() => navigate('/employer/projects')} className="flex items-center gap-2 px-4 py-2 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)] text-white rounded-lg text-sm font-medium transition-all">
            <Plus size={16} />
            New Project
          </button>
        </div>
      </div>

      {/* Stats Grid */}
      <div className="grid grid-cols-4 gap-4">
        <StatCard 
          title="Total Projects" 
          value={stats?.totalProjects ?? 0} 
          subValue={`${stats?.activeProjects ?? 0} New`} 
        />
        <StatCard 
          title="Active Members" 
          value={stats?.activeMembers ?? 0} 
          subValue="100% capacity" 
          subColor="text-green-400" 
        />
        <StatCard 
          title="Monthly Cost" 
          value={`$${(stats?.monthlyCost ?? 0).toLocaleString()}`} 
          subValue="Under Budget" 
        />
        <StatCard 
          title="Completion Rate" 
          value={`${stats?.completionRate ?? 0}%`} 
          subValue="↗ Up 5%" 
          subColor="text-green-400" 
        />
      </div>

      <div className="grid grid-cols-3 gap-6">
        {/* Active Projects List */}
        <div className="col-span-2 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-white">Active Projects</h2>
            <button onClick={() => navigate('/employer/projects')} className="text-sm text-gray-400 hover:text-purple-400 transition-colors">
              View All
            </button>
          </div>
          
          <div className="space-y-4">
            {projects.length === 0 ? (
              <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-xl p-8 text-center">
                <p className="text-gray-500 text-sm">No active projects yet.</p>
              </div>
            ) : (
              projects.map(project => (
                <ProjectCard key={project.id} project={project} />
              ))
            )}
          </div>
        </div>

        {/* Team Directory Snippet */}
        <div className="col-span-1 space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold text-white">Team Directory</h2>
            <button onClick={() => navigate('/employer/team')} className="text-gray-400 hover:text-white transition-colors">
              <Plus size={18} />
            </button>
          </div>
          
          <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-xl p-4 space-y-4">
            {team.length === 0 ? (
              <p className="text-xs text-gray-500 text-center py-4">No team members yet.</p>
            ) : (
              team.map(member => (
                <TeamMemberRow key={member.id} member={member} />
              ))
            )}
            <button onClick={() => navigate('/employer/team')} className="w-full py-2.5 mt-2 bg-[#1C212B] hover:bg-[#252A36] text-white rounded-lg text-sm font-medium transition-colors">
              View Full Directory
            </button>
          </div>
        </div>
      </div>
    </motion.div>
  );
}

function StatCard({ title, value, subValue, subColor = "text-gray-500" }) {
  return (
    <div className="bg-[#0D1117]/80 backdrop-blur-sm border border-[#1C212B] rounded-xl p-6 hover:border-[#2D3342] transition-colors relative overflow-hidden group">
      <div className="absolute inset-0 bg-gradient-to-b from-white/[0.02] to-transparent opacity-0 group-hover:opacity-100 transition-opacity" />
      <h3 className="text-sm font-medium text-gray-400 mb-2">{title}</h3>
      <div className="flex items-baseline justify-between">
        <span className="text-3xl font-semibold text-white tracking-tight">{value}</span>
      </div>
      <div className={`mt-2 text-xs font-medium ${subColor}`}>
        {subValue}
      </div>
    </div>
  );
}

function ProjectCard({ project }) {
  const isAtRisk = project.status === "AtRisk" || project.status === "At Risk";
  const progressLineColor = isAtRisk ? "bg-amber-500" : "bg-purple-500";
  const progressLineGlow = isAtRisk ? "shadow-[0_0_10px_rgba(245,158,11,0.5)]" : "shadow-[0_0_10px_rgba(168,85,247,0.5)]";
  const statusColors = isAtRisk ? "text-amber-400 border-amber-500/20 bg-amber-500/10" : "text-gray-300 border-gray-600/30 bg-gray-800/30";
  
  // Fake calculation just for display if not provided by backend
  const progress = project.completionPercent || 42; 

  return (
    <div className={`bg-[#0D1117]/80 backdrop-blur-sm border ${isAtRisk ? 'border-amber-500/30' : 'border-[#1C212B]'} rounded-xl p-6 group hover:bg-[#161B22]/80 transition-colors`}>
      <div className="flex items-center justify-between mb-2">
        <div className="flex items-center gap-3">
          <h3 className="text-lg font-semibold text-white">{project.title}</h3>
          <span className="px-2 py-0.5 text-[10px] uppercase tracking-wider font-semibold text-gray-400 bg-[#1C212B] border border-[#2D3342] rounded">
            {project.type || "Enterprise"}
          </span>
        </div>
        <div className={`px-2.5 py-1 text-xs font-medium rounded-full border flex items-center gap-1.5 ${statusColors}`}>
          <span className={`w-1.5 h-1.5 rounded-full ${isAtRisk ? 'bg-amber-400' : 'bg-purple-400 shadow-[0_0_8px_rgba(168,85,247,1)]'}`} />
          {isAtRisk ? "At Risk" : "On Track"}
        </div>
      </div>
      
      <p className="text-sm text-gray-500 mb-6">{project.description || "Core infrastructure update and migration"}</p>
      
      <div className="space-y-2 mb-6">
        <div className="flex justify-between text-xs font-medium text-gray-400">
          <span>Completion Progress</span>
          <span>{progress}%</span>
        </div>
        <div className="h-1.5 w-full bg-[#1C212B] rounded-full overflow-hidden">
          <div 
            className={`h-full ${progressLineColor} ${progressLineGlow} rounded-full`}
            style={{ width: `${progress}%` }}
          />
        </div>
      </div>
      
      <div className="flex items-center justify-between text-xs text-gray-500">
        <div className="flex border border-[#2D3342] rounded-full px-2 py-1 items-center gap-1 bg-black/20">
          <span>AK JD MR TS +2</span>
          <span className="ml-1 text-gray-400">Members</span>
        </div>
        <div className={isAtRisk ? "text-amber-500 font-medium" : ""}>
          {isAtRisk ? "Overdue by 2 days" : "Due Oct 30"}
        </div>
      </div>
    </div>
  );
}

function TeamMemberRow({ member }) {
  const initials = member.avatarInitials || member.fullName?.substring(0,2).toUpperCase() || "??";
  
  return (
    <div className="flex items-center gap-3 p-2 rounded-lg hover:bg-[#1C212B]/50 transition-colors">
      <div className="w-10 h-10 rounded-full border border-[#2D3342] bg-[#161B22] flex items-center justify-center text-xs font-semibold text-purple-300">
        {initials}
      </div>
      <div className="flex-1 min-w-0">
        <h4 className="text-sm font-medium text-white truncate">{member.fullName}</h4>
        <p className="text-xs text-gray-500 truncate">{member.role || "Developer"}</p>
      </div>
      <div className="flex gap-1.5">
         {/* Fake skills tag based on visual */}
         {member.skills?.slice(0,1).map(skill => (
           <span key={skill} className="px-2 py-0.5 text-[10px] uppercase tracking-wider font-semibold text-purple-400 bg-purple-500/10 border border-purple-500/20 rounded">
             {skill}
           </span>
         ))}
         {(!member.skills || member.skills.length === 0) && (
            <span className="px-2 py-0.5 text-[10px] uppercase tracking-wider font-semibold text-blue-400 bg-blue-500/10 border border-blue-500/20 rounded">
              FRONTEND
            </span>
         )}
      </div>
    </div>
  );
}
