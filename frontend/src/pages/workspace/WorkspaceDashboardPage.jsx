import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { getDaySummaries } from '../../api/workspaceApi.js';

const DAILY_GOAL_HOURS = 8;

export default function WorkspaceDashboardPage() {
  const navigate = useNavigate();
  const [summary, setSummary] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getDaySummaries().then(all => {
      const today = all.find(s => {
        const d = new Date(s.summaryDate);
        const now = new Date();
        return d.getFullYear() === now.getFullYear() &&
          d.getMonth() === now.getMonth() &&
          d.getDate() === now.getDate();
      });
      setSummary(today ?? null);
      setLoading(false);
    });
  }, []);

  const completedToday = summary?.tasksCompleted ?? 0;
  const streakDays = summary?.streakDays ?? 0;
  const focusHours = parseFloat(summary?.focusHours ?? 0);
  const ringPercent = Math.min((focusHours / DAILY_GOAL_HOURS) * 100, 100);

  return (
    <motion.div
      initial={{ opacity: 0, y: 12 }}
      animate={{ opacity: 1, y: 0 }}
      className="flex flex-col gap-6 p-2"
    >
      {/* Hero */}
      <section>
        <h2 className="text-2xl font-bold text-white">Workspace</h2>
        <p className="text-gray-400 text-sm mt-1">Your horizon is clear. Ready to begin?</p>
      </section>

      {/* Metrics */}
      <section className="grid grid-cols-2 lg:grid-cols-4 gap-4">
        <MetricCard label="Completed today" value={completedToday} sub="tasks" />
        <MetricCard label="Current streak" value={streakDays} sub="days" />
        <MetricCard label="Focus hours" value={focusHours.toFixed(1)} sub="h" />
        <div className="glass-panel rounded-2xl p-4 flex flex-col items-center gap-2">
          <p className="text-xs text-gray-400 uppercase tracking-wider">Daily goal</p>
          <RingChart percent={ringPercent} label={`${focusHours.toFixed(1)} / ${DAILY_GOAL_HOURS} hrs`} />
        </div>
      </section>

      {/* Main + Side */}
      <div className="grid grid-cols-1 lg:grid-cols-[1fr_300px] gap-4">
        <section className="glass-panel rounded-2xl p-6 flex flex-col gap-4">
          <div className="flex flex-col items-center justify-center gap-4 py-8">
            <div className="w-16 h-16 rounded-2xl bg-purple-600/10 border border-purple-500/20 flex items-center justify-center">
              <span className="text-2xl">✦</span>
            </div>
            <h3 className="text-lg font-semibold text-white">Start something new</h3>
            <p className="text-gray-400 text-sm text-center max-w-sm">
              Your dashboard is ready. Open{' '}
              <button className="text-purple-400 hover:text-purple-300 underline" onClick={() => navigate('/workspace/today')}>
                Today's focus
              </button>{' '}
              to see your active sessions and control the timer.
            </p>
            <input
              type="text"
              className="w-full max-w-sm bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-3 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
              placeholder="What are you working on next?"
            />
            <div className="flex gap-3">
              <button
                onClick={() => navigate('/workspace/today')}
                className="bg-purple-600 hover:bg-purple-500 text-white px-5 py-2 rounded-xl text-sm font-medium transition-all"
              >
                + Add task
              </button>
              <button
                onClick={() => navigate('/workspace/backlog')}
                className="border border-[#30363D] text-gray-400 hover:text-white hover:border-gray-500 px-5 py-2 rounded-xl text-sm transition-all"
              >
                Browse backlog
              </button>
            </div>
          </div>
        </section>

        <aside className="flex flex-col gap-4">
          <div className="glass-panel rounded-2xl p-5">
            <h4 className="text-sm font-semibold text-white mb-3">Quick notes</h4>
            <textarea
              className="w-full bg-[#0D1117] border border-[#30363D] text-gray-300 rounded-xl px-3 py-2.5 text-sm resize-none focus:outline-none focus:ring-1 focus:ring-purple-500/40"
              rows={5}
              placeholder="Jot down temporary thoughts…"
            />
          </div>
          <div className="glass-panel rounded-2xl p-5 border-l-2 border-purple-500/50">
            <p className="text-gray-300 text-sm italic">"The secret of getting ahead is getting started."</p>
            <span className="text-gray-500 text-xs mt-2 block">— Mark Twain</span>
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
        <path
          d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
          fill="none" stroke="#30363D" strokeWidth="3"
        />
        <path
          d="M18 2.0845 a 15.9155 15.9155 0 0 1 0 31.831 a 15.9155 15.9155 0 0 1 0 -31.831"
          fill="none" stroke="#7c3aed" strokeWidth="3" strokeDasharray={dash}
          strokeLinecap="round"
        />
      </svg>
      <div className="absolute inset-0 flex items-center justify-center">
        <span className="text-[9px] text-gray-300 text-center leading-tight rotate-0">{label}</span>
      </div>
    </div>
  );
}
