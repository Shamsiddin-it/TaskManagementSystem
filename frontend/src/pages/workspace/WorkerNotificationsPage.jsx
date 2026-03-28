import { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { getWorkerNotifications, markNotificationRead } from '../../api/workspaceApi.js';

const FILTERS = ['All', 'Tasks', 'Comments', 'Invitations', 'System'];
const TYPE_MAP = {
  Tasks: ['TaskAssigned', 'TaskDeadline', 0, 1],
  Comments: ['NewComment', 2],
  Invitations: ['InvitationReceived', 3],
  System: ['AbsenceApproved', 'MemberRemoved', 4, 5],
};

export default function WorkerNotificationsPage() {
  const [items, setItems] = useState([]);
  const [filter, setFilter] = useState('All');
  const [busy, setBusy] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  async function reload() {
    setLoading(true);
    const data = await getWorkerNotifications();
    setItems((data ?? []).sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)));
    setLoading(false);
  }

  useEffect(() => { reload(); }, []);

  function filtered() {
    if (filter === 'All') return items;
    const allowed = TYPE_MAP[filter] ?? [];
    return items.filter(n => allowed.includes(n.type));
  }

  async function handleMarkRead(id) {
    setBusy(true);
    setError('');
    const ok = await markNotificationRead(id);
    if (!ok) setError('Failed to mark read.');
    await reload();
    setBusy(false);
  }

  async function handleMarkAll() {
    setBusy(true);
    for (const n of items.filter(n => !n.isRead)) {
      await markNotificationRead(n.id);
    }
    await reload();
    setBusy(false);
  }

  const visible = filtered();
  const unread = items.filter(n => !n.isRead).length;

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-6 p-2">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-white">Notifications</h2>
          {unread > 0 && <p className="text-xs text-gray-400 mt-0.5">{unread} unread</p>}
        </div>
        <button
          disabled={busy}
          onClick={handleMarkAll}
          className="text-xs px-4 py-2 border border-[#30363D] text-gray-400 hover:text-white hover:border-gray-500 rounded-xl transition-all"
        >
          Mark all read
        </button>
      </div>

      {/* Filter tabs */}
      <div className="flex gap-2 flex-wrap">
        {FILTERS.map(f => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`text-xs px-4 py-1.5 rounded-full border transition-all ${
              filter === f
                ? 'bg-purple-600/20 border-purple-500/40 text-purple-400'
                : 'border-[#30363D] text-gray-500 hover:border-gray-500 hover:text-gray-300'
            }`}
          >
            {f}
          </button>
        ))}
      </div>

      {error && (
        <div className="bg-red-500/10 border border-red-500/20 text-red-400 text-xs px-4 py-2 rounded-lg">{error}</div>
      )}

      {/* List */}
      <div className="flex flex-col gap-3">
        {loading ? (
          <p className="text-gray-500 text-sm py-4">Loading…</p>
        ) : visible.length === 0 ? (
          <div className="glass-panel rounded-2xl p-8 text-center">
            <p className="text-gray-400 text-sm">No notifications yet.</p>
          </div>
        ) : (
          visible.map(n => (
            <div
              key={n.id}
              className={`flex gap-4 p-4 rounded-2xl border transition-colors ${
                n.isRead ? 'border-[#30363D] bg-[#0D1117]/50' : 'border-purple-500/30 bg-purple-600/5'
              }`}
            >
              <div className="w-8 h-8 rounded-xl bg-purple-600/10 border border-purple-500/20 flex items-center justify-center flex-shrink-0">
                <span className="text-purple-400 text-xs">✉</span>
              </div>
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-2">
                  <strong className="text-sm text-white">{n.title}</strong>
                  <span className="text-xs text-gray-500 ml-auto flex-shrink-0">
                    {n.createdAt ? new Date(n.createdAt).toLocaleString() : ''}
                  </span>
                </div>
                <p className="text-xs text-gray-400 mt-0.5">{n.body}</p>
                {!n.isRead && (
                  <button
                    disabled={busy}
                    onClick={() => handleMarkRead(n.id)}
                    className="text-xs text-purple-400 hover:text-purple-300 mt-1.5 transition-colors"
                  >
                    Mark read
                  </button>
                )}
              </div>
            </div>
          ))
        )}
      </div>
    </motion.div>
  );
}
