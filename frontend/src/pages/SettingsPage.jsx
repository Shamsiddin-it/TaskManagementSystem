import { useEffect, useState } from "react";
import { useLocation } from "react-router-dom";
import { api } from "../lib/api.js";
import { TEAM_ID, ACTOR_ID } from "../lib/config.js";
import { devRoleLabel } from "../lib/utils.js";

function applyTheme(theme) {
  if (theme === "light") {
    document.documentElement.setAttribute("data-theme", "light");
  } else {
    document.documentElement.removeAttribute("data-theme");
  }
}

export default function SettingsPage() {
  const location = useLocation();
  const query = new URLSearchParams(location.search);
  const teamId = Number(query.get("teamId")) || TEAM_ID;

  const [theme, setTheme] = useState(() => {
    return localStorage.getItem("nexus_theme") || "dark";
  });

  const [teamInfo, setTeamInfo] = useState(null);
  const [memberCount, setMemberCount] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    applyTheme(theme);
  }, [theme]);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      const res = await api.get(`/api/team-members?TeamId=${teamId}&Page=1&PageSize=100`);
      const items = res.data?.Items || [];
      setMemberCount(items.length);
      setLoading(false);
    };
    load();
  }, [teamId]);

  const toggleTheme = () => {
    const next = theme === "dark" ? "light" : "dark";
    setTheme(next);
    localStorage.setItem("nexus_theme", next);
    applyTheme(next);
  };

  return (
    <>
      {/* Page header */}
      <header className="glass-panel flex justify-between items-center px-5 py-3 flex-shrink-0">
        <div>
          <h2 className="text-[15px]">Settings</h2>
          <div className="text-[11px] text-[var(--text-secondary)] mt-1 tracking-wide">
            Workspace preferences
          </div>
        </div>
      </header>

      <div className="flex-1 overflow-y-auto pb-4">
        <div className="flex flex-col gap-4 max-w-[640px]">

          {/* Appearance */}
          <div className="glass-panel p-5 flex flex-col gap-4">
            <div>
              <h3 className="text-[13px] font-medium text-white mb-0.5">Appearance</h3>
              <p className="text-[11px] text-[var(--text-secondary)]">
                Choose your preferred color scheme.
              </p>
            </div>

            <div className="flex items-center justify-between p-4 rounded-xl border border-[var(--border-subtle)] hover:border-[var(--border-highlight)] transition-colors" style={{ background: "rgba(255,255,255,0.02)" }}>
              <div className="flex items-center gap-3">
                <div
                  className="w-9 h-9 rounded-lg flex items-center justify-center"
                  style={{ background: "linear-gradient(135deg, var(--accent-purple), #6a1b9a)" }}
                >
                  {theme === "dark" ? (
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2">
                      <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z" />
                    </svg>
                  ) : (
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="white" strokeWidth="2">
                      <circle cx="12" cy="12" r="5" />
                      <line x1="12" y1="1" x2="12" y2="3" /><line x1="12" y1="21" x2="12" y2="23" />
                      <line x1="4.22" y1="4.22" x2="5.64" y2="5.64" />
                      <line x1="18.36" y1="18.36" x2="19.78" y2="19.78" />
                      <line x1="1" y1="12" x2="3" y2="12" /><line x1="21" y1="12" x2="23" y2="12" />
                      <line x1="4.22" y1="19.78" x2="5.64" y2="18.36" />
                      <line x1="18.36" y1="5.64" x2="19.78" y2="4.22" />
                    </svg>
                  )}
                </div>
                <div>
                  <div className="text-[13px] text-white">{theme === "dark" ? "Dark Mode" : "Light Mode"}</div>
                  <div className="text-[11px] text-[var(--text-secondary)]">
                    {theme === "dark" ? "Easy on the eyes in low light" : "Bright workspace theme"}
                  </div>
                </div>
              </div>

              {/* Toggle switch */}
              <button
                onClick={toggleTheme}
                className="relative w-11 h-6 rounded-full transition-colors focus:outline-none"
                style={{
                  background: theme === "light" ? "var(--accent-purple)" : "var(--bg-surface-solid)",
                  border: "1px solid var(--border-highlight)",
                }}
              >
                <div
                  className="absolute top-0.5 w-5 h-5 rounded-full transition-transform duration-200"
                  style={{
                    background: "white",
                    transform: theme === "light" ? "translateX(21px)" : "translateX(1px)",
                    boxShadow: "0 1px 3px rgba(0,0,0,0.4)",
                  }}
                />
              </button>
            </div>
          </div>

          {/* Workspace Info */}
          <div className="glass-panel p-5 flex flex-col gap-4">
            <div>
              <h3 className="text-[13px] font-medium text-white mb-0.5">Workspace</h3>
              <p className="text-[11px] text-[var(--text-secondary)]">
                Current session configuration.
              </p>
            </div>

            <div className="flex flex-col gap-2">
              <InfoRow label="Team ID" value={`#${teamId}`} />
              <InfoRow label="Actor ID (You)" value={`#${ACTOR_ID}`} />
              <InfoRow
                label="Team Members"
                value={loading ? "Loading…" : memberCount != null ? String(memberCount) : "—"}
              />
              <InfoRow label="API Base URL" value={import.meta.env.VITE_API_BASE_URL || "http://localhost:5000"} mono />
            </div>
          </div>

          {/* About */}
          <div className="glass-panel p-5 flex flex-col gap-3">
            <div>
              <h3 className="text-[13px] font-medium text-white mb-0.5">About Nexus</h3>
              <p className="text-[11px] text-[var(--text-secondary)]">
                Team Lead workspace for sprint management, analytics, and team oversight.
              </p>
            </div>
            <div className="flex items-center gap-3 pt-1">
              <div
                className="w-8 h-8 rounded-[6px] shadow-[0_0_15px_var(--accent-purple-glow)]"
                style={{ background: "linear-gradient(135deg, var(--accent-purple-light), var(--accent-purple))" }}
              />
              <div>
                <div className="text-[13px] font-medium text-white">Nexus</div>
                <div className="text-[10px] text-[var(--text-secondary)]">Task Management · Team Lead View</div>
              </div>
            </div>
          </div>

        </div>
      </div>
    </>
  );
}

function InfoRow({ label, value, mono = false }) {
  return (
    <div className="flex items-center justify-between py-2.5 border-b border-[var(--border-subtle)] last:border-0">
      <span className="text-[12px] text-[var(--text-secondary)]">{label}</span>
      <span className={`text-[12px] text-white ${mono ? "font-mono" : ""}`}>{value}</span>
    </div>
  );
}
