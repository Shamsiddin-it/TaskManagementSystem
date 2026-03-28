import React, { useState, useEffect } from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { LogOut, Grid, Users, Folder, BarChart3, Bell, Settings } from 'lucide-react';
import { motion } from 'framer-motion';
import { logout } from '../../api';

export default function EmployerLayout() {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-[#06080A] text-white flex overflow-hidden font-sans selection:bg-purple-500/30">
      {/* Background radial gradients for that premium deep glow */}
      <div className="fixed inset-0 pointer-events-none z-0">
        <div className="absolute top-[-20%] left-[-10%] w-[50%] h-[50%] bg-purple-900/10 blur-[150px] rounded-full mix-blend-screen" />
        <div className="absolute bottom-[-20%] right-[-10%] w-[50%] h-[50%] bg-blue-900/10 blur-[150px] rounded-full mix-blend-screen" />
      </div>

      {/* Sidebar Navigation */}
      <aside className="w-64 border-r border-[#1C212B] bg-[#0A0D14]/80 backdrop-blur-xl flex flex-col z-10 relative">
        <div className="p-6 flex items-center gap-3">
          <div className="w-8 h-8 bg-gradient-to-br from-purple-500 to-indigo-600 rounded-lg flex items-center justify-center shadow-lg shadow-purple-500/20" />
          <span className="font-semibold text-lg tracking-tight">Nexus</span>
        </div>

        <div className="px-4 py-2 flex-grow overflow-y-auto custom-scrollbar">
          <div className="mb-8">
            <h3 className="px-3 mb-3 text-xs font-semibold text-gray-500 tracking-wider">MANAGEMENT</h3>
            <nav className="space-y-1">
              <NavItem to="/employer/overview" icon={<Grid size={18} />} label="Overview" />
              <NavItem to="/employer/team" icon={<Users size={18} />} label="Team" />
              <NavItem to="/employer/projects" icon={<Folder size={18} />} label="Projects" />
              <NavItem to="/employer/reports" icon={<BarChart3 size={18} />} label="Reports" />
            </nav>
          </div>

          <div>
            <h3 className="px-3 mb-3 text-xs font-semibold text-gray-500 tracking-wider">SYSTEM</h3>
            <nav className="space-y-1">
              <NavItem to="/employer/notifications" icon={<Bell size={18} />} label="Notifications" dot={true} />
              <NavItem to="/employer/settings" icon={<Settings size={18} />} label="Settings" />
            </nav>
          </div>
        </div>

        <div className="p-4 border-t border-[#1C212B]">
          <button
            onClick={() => {
              logout();
              navigate('/login');
            }}
            className="w-full flex items-center gap-3 px-3 py-2 text-sm font-medium text-gray-400 hover:text-red-400 hover:bg-red-400/10 rounded-lg transition-colors"
          >
            <LogOut size={18} />
            Logout
          </button>
        </div>
      </aside>

      {/* Main Content Pane */}
      <main className="flex-1 flex flex-col h-screen overflow-hidden z-10 relative">
        <div className="flex-1 overflow-y-auto px-8 py-8 custom-scrollbar relative">
           <Outlet />
        </div>
      </main>
    </div>
  );
}

function NavItem({ to, icon, label, dot }) {
  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        `flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-medium transition-all ${
          isActive 
            ? 'bg-[#1C212B] text-white shadow-sm' 
            : 'text-gray-400 hover:text-gray-200 hover:bg-[#161B22]'
        }`
      }
    >
      {({ isActive }) => (
        <>
          <span className={isActive ? 'text-purple-400' : 'text-gray-500'}>{icon}</span>
          <span className="flex-1">{label}</span>
          {dot && (
            <span className="w-1.5 h-1.5 rounded-full bg-amber-500 shadow-[0_0_8px_rgba(245,158,11,0.6)]" />
          )}
        </>
      )}
    </NavLink>
  );
}
