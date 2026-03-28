import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { getDaySummaries, addDaySummary, updateDaySummary, doLogout } from '../../api/workspaceApi.js';

function getUserId() {
  try { return JSON.parse(localStorage.getItem('user') || '{}').userId || 0; } catch { return 0; }
}

export default function EndOfDayPage() {
  const navigate = useNavigate();
  const [busy, setBusy] = useState(false);
  const [summary, setSummary] = useState(null);
  const [form, setForm] = useState({ p1: '', p2: '', p3: '' });

  useEffect(() => {
    const userId = getUserId();
    getDaySummaries().then(all => {
      const today = (all ?? []).find(s => {
        const d = new Date(s.summaryDate);
        const n = new Date();
        return s.userId === userId && d.getDate() === n.getDate() && d.getMonth() === n.getMonth() && d.getFullYear() === n.getFullYear();
      });
      if (today) {
        setSummary(today);
        setForm({ p1: today.tomorrowPriority1 ?? '', p2: today.tomorrowPriority2 ?? '', p3: today.tomorrowPriority3 ?? '' });
      }
    });
  }, []);

  async function handleConfirm() {
    const userId = getUserId();
    if (!userId) return;
    setBusy(true);
    try {
      if (summary?.id) {
        await updateDaySummary(summary.id, {
          tomorrowPriority1: form.p1,
          tomorrowPriority2: form.p2,
          tomorrowPriority3: form.p3,
        });
      } else {
        await addDaySummary({
          userId,
          summaryDate: new Date().toISOString(),
          tasksCompleted: 0,
          tasksTotal: 0,
          focusHours: 0,
          productivityScore: 0,
          productivityGrade: null,
          streakDays: 0,
          tomorrowPriority1: form.p1,
          tomorrowPriority2: form.p2,
          tomorrowPriority3: form.p3,
        });
      }
      doLogout();
    } finally {
      setBusy(false);
    }
  }

  const tasksCompleted = summary?.tasksCompleted ?? 0;
  const tasksTotal = summary?.tasksTotal ?? 0;

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2 max-w-4xl">
      <div className="grid grid-cols-1 lg:grid-cols-[1fr_280px] gap-6">
        {/* Main panel */}
        <section className="glass-panel rounded-2xl p-6 flex flex-col gap-5">
          <div className="flex items-center justify-between">
            <div>
              <h2 className="text-2xl font-bold text-white">End of day wrap-up</h2>
            </div>
            <span className="text-xs px-3 py-1 bg-green-500/10 border border-green-500/30 text-green-400 rounded-full">
              Shift complete
            </span>
          </div>

          {/* Completed today */}
          <div>
            <h3 className="text-sm font-semibold text-white mb-2">
              Completed today{' '}
              <span className="text-gray-500 font-normal">({tasksCompleted}/{tasksTotal})</span>
            </h3>
            <div className="w-full bg-[#0D1117] border border-[#30363D] rounded-xl h-2">
              <div
                className="h-2 bg-purple-500 rounded-xl transition-all"
                style={{ width: tasksTotal > 0 ? `${(tasksCompleted / tasksTotal) * 100}%` : '0%' }}
              />
            </div>
          </div>

          {/* Tomorrow's priorities */}
          <div>
            <h3 className="text-sm font-semibold text-white mb-3">Tomorrow's priorities</h3>
            <div className="flex flex-col gap-3">
              {[
                ['Top priority', 'p1'],
                ['Secondary', 'p2'],
                ['Tertiary', 'p3'],
              ].map(([label, key]) => (
                <label key={key} className="flex flex-col gap-1">
                  <span className="text-xs text-gray-400">{label}</span>
                  <input
                    className="bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
                    value={form[key]}
                    onChange={e => setForm(f => ({ ...f, [key]: e.target.value }))}
                    placeholder={label}
                  />
                </label>
              ))}
            </div>
          </div>

          <button
            onClick={handleConfirm}
            disabled={busy}
            className="w-full bg-red-600/80 hover:bg-red-600 disabled:opacity-50 text-white font-semibold py-3 rounded-xl transition-all flex items-center justify-center gap-2"
          >
            {busy ? 'Saving…' : 'Confirm & log off'}
          </button>
        </section>

        {/* Side panel */}
        <aside className="glass-panel rounded-2xl p-5 flex flex-col gap-4">
          <h4 className="text-sm font-semibold text-white">Session summary</h4>
          <div className="flex flex-col gap-3">
            <StatRow label="Tasks done" value={`${tasksCompleted} / ${tasksTotal}`} />
            <StatRow label="Focus hours" value={`${parseFloat(summary?.focusHours ?? 0).toFixed(1)} h`} />
            <StatRow label="Streak" value={`${summary?.streakDays ?? 0} days`} />
            <StatRow label="Score" value={`${summary?.productivityScore ?? 0} pts`} />
          </div>
          <button
            onClick={() => navigate('/workspace')}
            className="mt-auto text-sm text-gray-400 hover:text-white transition-colors"
          >
            ← Back to workspace
          </button>
        </aside>
      </div>
    </motion.div>
  );
}

function StatRow({ label, value }) {
  return (
    <div className="flex justify-between items-center border-b border-[#30363D] pb-2">
      <span className="text-xs text-gray-400">{label}</span>
      <span className="text-sm text-white font-medium">{value}</span>
    </div>
  );
}
