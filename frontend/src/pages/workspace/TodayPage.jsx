import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import {
  getFocusSessions,
  pauseFocusSession,
  completeFocusSession,
  getDaySummaries,
} from '../../api/workspaceApi.js';

const DAILY_GOAL_HOURS = 8;

export default function TodayPage() {
  const navigate = useNavigate();
  const [allSessions, setAllSessions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');

  async function loadSessions() {
    const data = await getFocusSessions();
    setAllSessions(data ?? []);
    setLoading(false);
  }

  useEffect(() => { loadSessions(); }, []);

  const activeSessions = allSessions.filter(s => s.status === 0 || s.status === 'Active');
  const completedToday = allSessions.filter(s => s.status === 2 || s.status === 'Completed').length;
  const streakDays = 0;
  const focusHours = allSessions
    .filter(s => s.status === 0 || s.status === 1 || s.status === 'Active' || s.status === 'Paused')
    .reduce((sum, s) => sum + (s.durationMinutes ?? 0), 0) / 60;
  const ringPercent = Math.min((focusHours / DAILY_GOAL_HOURS) * 100, 100);

  async function handlePause(id) {
    setBusy(true);
    setError('');
    await pauseFocusSession(id);
    await loadSessions();
    setBusy(false);
  }

  async function handleComplete(id) {
    setBusy(true);
    setError('');
    await completeFocusSession(id);
    await loadSessions();
    setBusy(false);
  }

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      {/* Hero */}
      <section className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-white">Today's focus</h2>
          <p className="text-gray-400 text-sm">Focus mode active · live sessions</p>
        </div>
        <div className="flex gap-2">
          <span className="flex items-center gap-1.5 px-3 py-1 bg-green-500/10 border border-green-500/20 text-green-400 rounded-full text-xs">
            <span className="w-1.5 h-1.5 bg-green-400 rounded-full animate-pulse" />Online
          </span>
          <span className="flex items-center gap-1.5 px-3 py-1 bg-[#161B22] border border-[#30363D] text-gray-400 rounded-full text-xs">
            <span className="w-1.5 h-1.5 bg-amber-400 rounded-full" />Away
          </span>
        </div>
      </section>

      {/* Metrics */}
      <section className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <MetricCard label="Completed today" value={completedToday} sub="tasks" />
        <MetricCard label="Current streak" value={streakDays} sub="days" />
        <MetricCard label="Focus hours" value={focusHours.toFixed(1)} sub="h" />
        <div className="glass-panel rounded-2xl p-4 flex flex-col items-center gap-2">
          <p className="text-xs text-gray-400 uppercase tracking-wider">Daily goal</p>
          <RingChart percent={ringPercent} label={`${focusHours.toFixed(1)} / ${DAILY_GOAL_HOURS}`} />
        </div>
      </section>

      {/* Session list + sidebar */}
      <div className="grid grid-cols-1 lg:grid-cols-[1fr_280px] gap-4">
        <section className="glass-panel rounded-2xl p-6">
          <h3 className="text-base font-semibold text-white mb-4">Active tasks</h3>

          {error && (
            <div className="bg-red-500/10 border border-red-500/20 text-red-400 text-xs px-3 py-2 rounded-lg mb-3">{error}</div>
          )}

          {loading ? (
            <p className="text-gray-500 text-sm">Loading sessions…</p>
          ) : activeSessions.length === 0 ? (
            <div className="text-center py-8">
              <p className="text-gray-400 text-sm">No active focus sessions.</p>
              <p className="text-gray-500 text-xs mt-1">
                Go to <button className="text-purple-400 underline" onClick={() => navigate('/workspace')}>dashboard</button> to start.
              </p>
            </div>
          ) : (
            <div className="flex flex-col gap-3">
              {activeSessions.map(session => {
                const priority = session.task?.priority ?? '';
                const isHigh = priority.toLowerCase().includes('high');
                const title = session.task?.title ?? `Task #${session.taskId}`;
                return (
                  <div
                    key={session.id}
                    className="bg-[#0D1117] border border-[#30363D] rounded-xl p-4 cursor-pointer hover:border-purple-500/40 transition-colors"
                    onClick={() => navigate(`/workspace/tasks/${session.id}`)}
                  >
                    <div className="flex items-center gap-2 mb-2">
                      <span className={`text-xs px-2 py-0.5 rounded-full border ${isHigh ? 'bg-red-500/10 border-red-500/30 text-red-400' : 'bg-purple-500/10 border-purple-500/30 text-purple-400'}`}>
                        {priority || 'Priority'}
                      </span>
                      <span className="text-xs px-2 py-0.5 rounded-full bg-[#161B22] border border-[#30363D] text-gray-400">
                        {session.task?.team?.name ?? 'Team'}
                      </span>
                      <span className="ml-auto text-xs text-green-400">In progress</span>
                    </div>
                    <h4 className="text-sm font-medium text-white mb-1">{title}</h4>
                    <p className="text-xs text-gray-500 mb-3">
                      {session.task?.description || 'No description yet.'}
                    </p>
                    <div className="flex gap-2">
                      <button
                        className="text-xs px-3 py-1.5 bg-purple-600/10 hover:bg-purple-600/20 text-purple-400 border border-purple-500/20 rounded-lg transition-all"
                        disabled={busy}
                        onClick={e => { e.stopPropagation(); handlePause(session.id); }}
                      >
                        Pause
                      </button>
                      <button
                        className="text-xs px-3 py-1.5 bg-green-500/10 hover:bg-green-500/20 text-green-400 border border-green-500/20 rounded-lg transition-all"
                        disabled={busy}
                        onClick={e => { e.stopPropagation(); handleComplete(session.id); }}
                      >
                        Complete
                      </button>
                    </div>
                  </div>
                );
              })}
            </div>
          )}

          {/* Templates */}
          <h3 className="text-base font-semibold text-white mt-6 mb-3">Suggested templates</h3>
          <div className="grid grid-cols-3 gap-3">
            {[
              { cat: 'Routine', title: 'Review PR backlog', time: '45m' },
              { cat: 'DevOps', title: 'Deploy checklist', time: '30m' },
              { cat: 'Admin', title: 'Weekly sync notes', time: '20m' },
            ].map(t => (
              <div key={t.title} className="bg-[#0D1117] border border-[#30363D] rounded-xl p-3 hover:border-purple-500/40 transition-colors cursor-default">
                <span className="text-[10px] text-purple-400 uppercase tracking-wider">{t.cat}</span>
                <p className="text-sm font-medium text-white mt-1">{t.title}</p>
                <span className="text-xs text-gray-500">{t.time}</span>
              </div>
            ))}
          </div>
        </section>

        {/* Sidebar */}
        <aside className="flex flex-col gap-4">
          <div className="glass-panel rounded-2xl p-4">
            <h4 className="text-sm font-semibold text-white mb-2">Quick notes</h4>
            <textarea
              className="w-full bg-[#0D1117] border border-[#30363D] text-gray-300 rounded-xl px-3 py-2 text-xs resize-none focus:outline-none focus:ring-1 focus:ring-purple-500/40"
              rows={4}
              placeholder="Jot down temporary thoughts…"
            />
          </div>
          <div className="glass-panel rounded-2xl p-4">
            <h4 className="text-sm font-semibold text-white mb-3">Upcoming deadlines</h4>
            <ul className="flex flex-col gap-2">
              <li className="flex justify-between text-xs"><strong className="text-white">Q3 reporting</strong><span className="text-gray-500">Analytics · in 2h</span></li>
              <li className="flex justify-between text-xs"><strong className="text-white">Security audit</strong><span className="text-gray-500">Core · tomorrow</span></li>
            </ul>
          </div>
          <div className="glass-panel rounded-2xl p-4">
            <h4 className="text-sm font-semibold text-white mb-3">Recent activity</h4>
            <ul className="text-xs text-gray-400 flex flex-col gap-1.5">
              <li>Started API integration <span className="text-gray-600">15m ago</span></li>
              <li>Timer stopped <span className="text-gray-600">1h ago</span></li>
            </ul>
          </div>
        </aside>
      </div>
    </motion.div>
  );
}

function MetricCard({ label, value, sub }) {
  return (
    <div className="glass-panel rounded-2xl p-4">
      <p className="text-xs text-gray-400 uppercase tracking-wider mb-1">{label}</p>
      <p className="text-2xl font-bold text-white">{value}</p>
      <p className="text-xs text-gray-500">{sub}</p>
    </div>
  );
}

function RingChart({ percent, label }) {
  const dash = `${percent.toFixed(0)}, 100`;
  return (
    <div className="relative w-20 h-20">
      <svg viewBox="0 0 36 36" className="w-full h-full -rotate-90">
        <path d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" fill="none" stroke="#30363D" strokeWidth="3" />
        <path d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831" fill="none" stroke="#7c3aed" strokeWidth="3" strokeDasharray={dash} strokeLinecap="round" />
      </svg>
      <div className="absolute inset-0 flex items-center justify-center">
        <span className="text-[9px] text-gray-300 text-center leading-tight">{label}</span>
      </div>
    </div>
  );
}
