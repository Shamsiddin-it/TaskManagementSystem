import { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { getScheduleEvents, addScheduleEvent, deleteScheduleEvent } from '../../api/workspaceApi.js';

const EVENT_TYPES = ['Task', 'FocusBlock', 'Meeting', 'Personal'];

export default function SchedulePage() {
  const [events, setEvents] = useState([]);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [form, setForm] = useState({
    title: '',
    type: 'Task',
    startTime: toLocalInput(new Date()),
    endTime: toLocalInput(new Date(Date.now() + 3600000)),
    isUrgent: false,
    userId: getUserId(),
  });

  function getUserId() {
    try { return JSON.parse(localStorage.getItem('user') || '{}').userId || 0; } catch { return 0; }
  }

  async function reload() {
    const userId = getUserId();
    const all = await getScheduleEvents();
    const mine = (all ?? []).filter(e => e.userId === userId || !e.userId);
    mine.sort((a, b) => new Date(a.startTime) - new Date(b.startTime));
    setEvents(mine);
  }

  useEffect(() => { reload(); }, []);

  async function handleAdd(e) {
    e.preventDefault();
    setBusy(true);
    setError('');
    try {
      await addScheduleEvent({
        title: form.title,
        type: form.type,
        startTime: new Date(form.startTime).toISOString(),
        endTime: new Date(form.endTime).toISOString(),
        isUrgent: form.isUrgent,
        userId: form.userId,
      });
      setForm(f => ({ ...f, title: '' }));
      await reload();
    } catch { setError('Failed to add event.'); }
    finally { setBusy(false); }
  }

  async function handleDelete(id) {
    setBusy(true);
    await deleteScheduleEvent(id);
    await reload();
    setBusy(false);
  }

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      <div className="flex items-center justify-between">
        <h2 className="text-2xl font-bold text-white">Schedule</h2>
      </div>

      {error && (
        <div className="bg-red-500/10 border border-red-500/20 text-red-400 text-xs px-4 py-2 rounded-lg">{error}</div>
      )}

      {/* Add event form */}
      <form onSubmit={handleAdd} className="glass-panel rounded-2xl p-6">
        <h3 className="text-base font-semibold text-white mb-4">Add schedule event</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <label className="flex flex-col gap-1">
            <span className="text-xs text-gray-400">Title</span>
            <input
              className="bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
              value={form.title}
              onChange={e => setForm(f => ({ ...f, title: e.target.value }))}
              required
            />
          </label>
          <label className="flex flex-col gap-1">
            <span className="text-xs text-gray-400">Type</span>
            <select
              className="bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
              value={form.type}
              onChange={e => setForm(f => ({ ...f, type: e.target.value }))}
            >
              {EVENT_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
            </select>
          </label>
          <label className="flex flex-col gap-1">
            <span className="text-xs text-gray-400">Start</span>
            <input
              type="datetime-local"
              className="bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
              value={form.startTime}
              onChange={e => setForm(f => ({ ...f, startTime: e.target.value }))}
            />
          </label>
          <label className="flex flex-col gap-1">
            <span className="text-xs text-gray-400">End</span>
            <input
              type="datetime-local"
              className="bg-[#0D1117] border border-[#30363D] text-white rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
              value={form.endTime}
              onChange={e => setForm(f => ({ ...f, endTime: e.target.value }))}
            />
          </label>
        </div>
        <label className="flex items-center gap-2 mt-4 cursor-pointer">
          <input
            type="checkbox"
            className="accent-purple-500"
            checked={form.isUrgent}
            onChange={e => setForm(f => ({ ...f, isUrgent: e.target.checked }))}
          />
          <span className="text-sm text-gray-300">Urgent</span>
        </label>
        <button
          type="submit"
          disabled={busy}
          className="mt-4 bg-purple-600 hover:bg-purple-500 disabled:opacity-50 text-white px-5 py-2 rounded-xl text-sm font-medium transition-all"
        >
          {busy ? 'Saving…' : 'Add'}
        </button>
      </form>

      {/* Events list */}
      <div className="glass-panel rounded-2xl p-6">
        <h3 className="text-base font-semibold text-white mb-4">Your events</h3>
        {events.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-gray-500 text-sm">No events yet. Add one above.</p>
          </div>
        ) : (
          <div className="flex flex-col gap-3">
            {events.map(ev => (
              <div
                key={ev.id}
                className={`flex items-center gap-4 p-4 rounded-xl border transition-colors ${
                  ev.isUrgent ? 'border-amber-500/30 bg-amber-500/5' : 'border-[#30363D] bg-[#0D1117]'
                }`}
              >
                <div className="w-2 h-2 rounded-full bg-purple-500 flex-shrink-0" />
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <strong className="text-sm text-white truncate">{ev.title}</strong>
                    <span className="text-[10px] text-gray-500">{fmtTime(ev.startTime)}</span>
                  </div>
                  <p className="text-xs text-gray-500 mt-0.5">{ev.type} · ends {fmtTime(ev.endTime)}</p>
                </div>
                <button
                  disabled={busy}
                  onClick={() => handleDelete(ev.id)}
                  className="text-xs text-red-400 hover:text-red-300 border border-red-500/20 px-3 py-1 rounded-lg transition-colors"
                >
                  Delete
                </button>
              </div>
            ))}
          </div>
        )}
      </div>
    </motion.div>
  );
}

function toLocalInput(date) {
  const d = new Date(date);
  d.setMinutes(d.getMinutes() - d.getTimezoneOffset());
  return d.toISOString().slice(0, 16);
}

function fmtTime(iso) {
  if (!iso) return '';
  return new Date(iso).toLocaleString(undefined, { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' });
}
