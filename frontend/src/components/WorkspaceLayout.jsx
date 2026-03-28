import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { LogOut } from 'lucide-react';
import { doLogout } from '../api/workspaceApi.js';

function getRole() {
  return (localStorage.getItem('nexus_role') || '')
    .toLowerCase()
    .replace(/[\s_-]+/g, '');
}

export default function WorkspaceLayout() {
  const navigate = useNavigate();
  const isTeamLead = getRole() === 'teamlead';

  const handleLogout = () => {
    doLogout();
    navigate('/login', { replace: true });
  };

  return (
    <div className="w-full h-full relative overflow-hidden flex">
      <div className="bg-glow bg-glow-1"></div>
      <div className="bg-glow bg-glow-2"></div>

      <div className="w-full h-full grid grid-cols-[240px_1fr] p-4 gap-4 z-10 relative">
        {/* Sidebar */}
        <aside className="sidebar glass-panel flex flex-col py-5 px-3 gap-6 overflow-y-auto">
          <div className="flex items-center gap-3 px-3">
            <div
              className="w-5 h-5 rounded-[4px] shadow-[0_0_10px_rgba(124,58,237,0.5)]"
              style={{ background: 'linear-gradient(135deg,#a78bfa,#7c3aed)' }}
            />
            <h3 className="font-medium text-[15px] tracking-tight text-white">Nexus</h3>
          </div>

          {/* Worker workspace nav */}
          <div className="flex flex-col gap-1 mt-2">
            <span className="label-caps px-3 mb-2">Workspace</span>
            <SideNavItem to="/workspace" end label="Dashboard">
              <GridIcon />
            </SideNavItem>
            <SideNavItem to="/workspace/today" label="Today's focus">
              <FocusIcon />
            </SideNavItem>
            <SideNavItem to="/workspace/schedule" label="Schedule">
              <CalIcon />
            </SideNavItem>
            <SideNavItem to="/workspace/backlog" label="Backlog">
              <ListIcon />
            </SideNavItem>
            <SideNavItem to="/workspace/activity" label="Activity">
              <ActivityIcon />
            </SideNavItem>
          </div>

          {/* Team Lead extra nav — visible to all, TL-specific */}
          {isTeamLead ? (
            <div className="flex flex-col gap-1">
              <span className="label-caps px-3 mb-2">Team Lead</span>
              <SideNavItem to="/workspace/team" label="Team directory">
                <TeamIcon />
              </SideNavItem>
              <SideNavItem to="/team-lead/sprint-board" label="Sprint board">
                <BoardIcon />
              </SideNavItem>
              <SideNavItem to="/team-lead/sprint-retro" label="Sprint retro">
                <RetroIcon />
              </SideNavItem>
              <SideNavItem to="/team-lead/analytics" label="Analytics">
                <ChartIcon />
              </SideNavItem>
              <SideNavItem to="/team-lead/backlog" label="TL Backlog">
                <ListIcon />
              </SideNavItem>
            </div>
          ) : null}

          {/* System */}
          <div className="flex flex-col gap-1 mt-auto">
            <span className="label-caps px-3 mb-2">System</span>
            <SideNavItem to="/workspace/notifications" label="Notifications">
              <BellIcon />
            </SideNavItem>
            <SideNavItem to="/workspace/settings" label="Settings">
              <SettingsIcon />
            </SideNavItem>

            <div className="mt-4 pt-4 border-t border-white/10">
              <button
                className="flex items-center gap-3 w-full px-4 py-3 rounded-xl transition-all duration-200 text-red-400 hover:text-red-300 hover:bg-red-500/10 active:scale-95 group"
                onClick={handleLogout}
                id="workspace-logout-btn"
              >
                <div className="flex items-center justify-center w-5 h-5 transition-transform group-hover:-translate-x-1">
                  <LogOut size={20} strokeWidth={2.5} />
                </div>
                <span className="font-semibold text-sm tracking-wide uppercase">Logout Properly</span>
              </button>
            </div>
          </div>
        </aside>

        {/* Main content */}
        <main className="flex flex-col gap-4 overflow-auto h-full">
          <Outlet />
        </main>
      </div>
    </div>
  );
}

function SideNavItem({ to, label, children, end }) {
  return (
    <NavLink
      to={to}
      end={end}
      className={({ isActive }) =>
        `nav-item flex items-center gap-3 px-4 py-2.5 rounded-xl text-sm transition-all duration-200 ${
          isActive
            ? 'bg-purple-600/20 text-white font-medium'
            : 'text-gray-400 hover:text-white hover:bg-white/5'
        }`
      }
    >
      <span className="w-4 h-4 flex-shrink-0">{children}</span>
      {label}
    </NavLink>
  );
}

// ─── Inline SVG Icons ─────────────────────────────────────────────
function GridIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <rect x="3" y="3" width="7" height="7" /><rect x="14" y="3" width="7" height="7" />
      <rect x="14" y="14" width="7" height="7" /><rect x="3" y="14" width="7" height="7" />
    </svg>
  );
}
function FocusIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <circle cx="12" cy="12" r="10" /><circle cx="12" cy="12" r="3" />
    </svg>
  );
}
function CalIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <rect x="3" y="4" width="18" height="18" rx="2" />
      <line x1="16" y1="2" x2="16" y2="6" /><line x1="8" y1="2" x2="8" y2="6" /><line x1="3" y1="10" x2="21" y2="10" />
    </svg>
  );
}
function ListIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <line x1="8" y1="6" x2="21" y2="6" /><line x1="8" y1="12" x2="21" y2="12" /><line x1="8" y1="18" x2="21" y2="18" />
      <line x1="3" y1="6" x2="3.01" y2="6" /><line x1="3" y1="12" x2="3.01" y2="12" /><line x1="3" y1="18" x2="3.01" y2="18" />
    </svg>
  );
}
function ActivityIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <polyline points="22 12 18 12 15 21 9 3 6 12 2 12" />
    </svg>
  );
}
function TeamIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" /><circle cx="9" cy="7" r="4" />
      <path d="M23 21v-2a4 4 0 0 0-3-3.87" /><path d="M16 3.13a4 4 0 0 1 0 7.75" />
    </svg>
  );
}
function BoardIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <rect x="3" y="4" width="18" height="18" rx="2" />
      <line x1="3" y1="10" x2="21" y2="10" /><line x1="9" y1="10" x2="9" y2="22" />
    </svg>
  );
}
function RetroIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <polyline points="22 12 18 12 15 21 9 3 6 12 2 12" />
    </svg>
  );
}
function ChartIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <line x1="18" y1="20" x2="18" y2="10" /><line x1="12" y1="20" x2="12" y2="4" /><line x1="6" y1="20" x2="6" y2="14" />
    </svg>
  );
}
function BellIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" /><path d="M13.73 21a2 2 0 0 1-3.46 0" />
    </svg>
  );
}
function SettingsIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
      <circle cx="12" cy="12" r="3" />
      <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1-2.83 2.83l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-4 0v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83-2.83l.06-.06A1.65 1.65 0 0 0 4.68 15a1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1 0-4h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 2.83-2.83l.06.06A1.65 1.65 0 0 0 9 4.68a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 4 0v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 2.83l-.06.06A1.65 1.65 0 0 0 19.4 9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 0 4h-.09a1.65 1.65 0 0 0-1.51 1z" />
    </svg>
  );
}
