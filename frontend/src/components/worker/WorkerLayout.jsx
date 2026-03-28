import React from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { LogOut, CheckSquare, Calendar, Users, Activity, Bell, Settings } from 'lucide-react';
import { logout } from '../../api';

export default function WorkerLayout() {
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login', { replace: true });
  };

  return (
    <div className="w-full h-full relative overflow-hidden flex bg-[#0A0D14]">
      {/* Deep Purple Radial Glows */}
      <div className="absolute top-[-20%] left-[-10%] w-[50%] h-[50%] rounded-full bg-purple-900/10 blur-[150px] pointer-events-none" />
      <div className="absolute bottom-[-20%] right-[-10%] w-[50%] h-[50%] rounded-full bg-indigo-900/10 blur-[150px] pointer-events-none" />

      <div className="w-full h-full grid grid-cols-[260px_1fr] p-4 gap-6 z-10 relative">
        
        {/* Sidebar */}
        <aside className="bg-[#0D1117]/80 backdrop-blur-md border border-[#1C212B] rounded-2xl flex flex-col py-6 px-4 gap-8 shadow-2xl">
          <div className="flex items-center gap-3 px-3">
            <div className="w-6 h-6 rounded-md shadow-[0_0_15px_rgba(168,85,247,0.5)] bg-gradient-to-br from-purple-400 to-purple-600" />
            <h3 className="font-bold text-lg tracking-tight text-white">Nexus</h3>
          </div>

          <div className="flex flex-col gap-1 mt-2">
            <span className="text-[10px] font-bold text-gray-500 uppercase tracking-widest px-3 mb-3">Workspace</span>
            <SidebarLink to="/worker/my-tasks" icon={<CheckSquare size={18} />} label="My Tasks" />
            <SidebarLink to="/worker/schedule" icon={<Calendar size={18} />} label="Schedule" />
            <SidebarLink to="/worker/team" icon={<Users size={18} />} label="Team" />
            <SidebarLink to="/worker/activity" icon={<Activity size={18} />} label="Activity" />
          </div>

          <div className="flex flex-col gap-1 mt-auto">
            <span className="text-[10px] font-bold text-gray-500 uppercase tracking-widest px-3 mb-3">System</span>
            <SidebarLink to="/worker/notifications" icon={<Bell size={18} />} label="Notifications" />
            <SidebarLink to="/worker/settings" icon={<Settings size={18} />} label="Settings" />

            <div className="mt-6 pt-6 border-t border-[#1C212B]">
              <button 
                className="flex items-center gap-3 w-full px-4 py-3 rounded-xl transition-all duration-300 text-red-500 hover:text-red-400 hover:bg-red-500/10 active:scale-95 group"
                onClick={handleLogout}
              >
                <div className="transition-transform duration-300 group-hover:-translate-x-1">
                  <LogOut size={18} strokeWidth={2.5} />
                </div>
                <span className="font-semibold text-sm tracking-wide">Logout</span>
              </button>
            </div>
          </div>
        </aside>

        {/* Main Content Pane */}
        <main className="flex flex-col overflow-y-auto custom-scrollbar h-full rounded-2xl">
          <Outlet />
        </main>

      </div>
    </div>
  );
}

function SidebarLink({ to, icon, label }) {
  return (
    <NavLink
      to={to}
      className={({ isActive }) =>
        `flex items-center gap-3 px-4 py-3 rounded-xl text-sm transition-all duration-300 ${
          isActive
            ? "bg-purple-500/10 text-purple-400 font-bold border border-purple-500/20 shadow-[0_0_15px_rgba(168,85,247,0.1)]"
            : "text-gray-400 hover:text-gray-200 hover:bg-[#161B22] border border-transparent"
        }`
      }
    >
      <span className="shrink-0">{icon}</span>
      {label}
    </NavLink>
  );
}
