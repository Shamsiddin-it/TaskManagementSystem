import { useEffect, useState, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { getFocusSessionById, pauseFocusSession } from '../../api/workspaceApi.js';

const STATUS_LABELS = { 0: 'In progress', 1: 'Paused', 2: 'Completed', Active: 'In progress', Paused: 'Paused', Completed: 'Completed' };
const STATUS_COLORS = { 0: 'text-amber-400 bg-amber-500/10 border-amber-500/30', 1: 'text-gray-400 bg-[#161B22] border-[#30363D]', 2: 'text-green-400 bg-green-500/10 border-green-500/30', Active: 'text-amber-400 bg-amber-500/10 border-amber-500/30', Paused: 'text-gray-400 bg-[#161B22] border-[#30363D]', Completed: 'text-green-400 bg-green-500/10 border-green-500/30' };

function useTimer(startedAt, endedAt) {
  const [tick, setTick] = useState(0);
  useEffect(() => {
    if (!endedAt) {
      const id = setInterval(() => setTick(t => t + 1), 1000);
      return () => clearInterval(id);
    }
  }, [endedAt]);
  if (!startedAt) return '00:00:00';
  const end = endedAt ? new Date(endedAt) : new Date();
  let diff = Math.max(0, Math.floor((end - new Date(startedAt)) / 1000));
  const h = String(Math.floor(diff / 3600)).padStart(2, '0');
  const m = String(Math.floor((diff % 3600) / 60)).padStart(2, '0');
  const s = String(diff % 60).padStart(2, '0');
  return `${h}:${m}:${s}`;
}

export default function TaskDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [session, setSession] = useState(null);
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const timerDigits = useTimer(session?.startedAt, session?.endedAt);

  useEffect(() => {
    if (!id) return;
    getFocusSessionById(id).then(s => setSession(s));
  }, [id]);

  async function handleStop() {
    if (!session) return;
    setBusy(true);
    setError('');
    const res = await pauseFocusSession(session.id);
    if (!res && res !== null) setError('Failed to pause session.');
    const fresh = await getFocusSessionById(session.id);
    setSession(fresh);
    setBusy(false);
  }

  const task = session?.task;
  const title = task?.title ?? `Focus session #${id}`;
  const description = task?.description;
  const priority = task?.priority ?? 'Priority';
  const teamName = task?.team?.name ?? 'Team';
  const status = session?.status;

  return (
    <motion.div initial={{ opacity: 0, y: 12 }} animate={{ opacity: 1, y: 0 }} className="flex flex-col gap-4 p-2">
      {/* Breadcrumb */}
      <nav className="text-xs text-gray-500 flex items-center gap-1.5">
        <button onClick={() => navigate('/workspace')} className="hover:text-gray-300">Projects</button>
        <span>/</span>
        <span className="text-white font-medium">Session #{id}</span>
      </nav>

      {/* Header */}
      <div className="flex items-start justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold text-white mb-2">{title}</h1>
          <div className="flex items-center gap-2 flex-wrap">
            <span className={`text-xs px-2 py-0.5 rounded-full border ${STATUS_COLORS[status] ?? ''}`}>
              {STATUS_LABELS[status] ?? 'Unknown'}
            </span>
            <span className="text-xs px-2 py-0.5 rounded-full bg-[#161B22] border border-[#30363D] text-gray-400">{priority}</span>
            <span className="text-xs px-2 py-0.5 rounded-full bg-[#161B22] border border-[#30363D] text-gray-400">{teamName}</span>
          </div>
        </div>
        <div className="glass-panel rounded-2xl p-4 text-center min-w-[140px] flex-shrink-0">
          <p className="text-xs text-gray-400 mb-1">Active session</p>
          <p className="text-3xl font-mono font-bold text-white tracking-tight">{timerDigits}</p>
          <button
            onClick={handleStop}
            disabled={busy}
            className="mt-3 text-xs px-4 py-1.5 bg-red-500/10 hover:bg-red-500/20 text-red-400 border border-red-500/20 rounded-lg transition-all disabled:opacity-50 w-full"
          >
            Stop timer
          </button>
        </div>
      </div>

      {error && <div className="bg-red-500/10 border border-red-500/20 text-red-400 text-xs px-4 py-2 rounded-lg">{error}</div>}

      {/* Body */}
      <div className="grid grid-cols-1 lg:grid-cols-[1fr_260px] gap-4">
        <div className="flex flex-col gap-4">
          {/* Description */}
          <div className="glass-panel rounded-2xl p-5">
            <h3 className="text-sm font-semibold text-white mb-2">Description</h3>
            <p className="text-sm text-gray-400">{description || 'No description loaded.'}</p>

            <h3 className="text-sm font-semibold text-white mt-5 mb-2">Subtasks</h3>
            <ul className="flex flex-col gap-1.5">
              {[
                { done: true, text: 'Create checkout session endpoint' },
                { done: true, text: 'Configure webhook URL' },
                { done: true, text: 'Handle payment_intent.succeeded' },
                { done: true, text: '3DS2 challenge UI' },
                { done: false, text: 'Write unit tests for signature verification' },
                { done: false, text: 'Load test webhook throughput' },
              ].map(s => (
                <li key={s.text} className={`flex items-center gap-2 text-xs ${s.done ? 'text-gray-500 line-through' : 'text-gray-300'}`}>
                  <span className={`w-3.5 h-3.5 rounded-sm border flex items-center justify-center flex-shrink-0 ${s.done ? 'bg-green-500/20 border-green-500/40 text-green-400' : 'border-[#30363D]'}`}>
                    {s.done && '✓'}
                  </span>
                  {s.text}
                </li>
              ))}
            </ul>

            <h3 className="text-sm font-semibold text-white mt-5 mb-2">Attachments</h3>
            <div className="flex gap-2 flex-wrap">
              {['stripe_specs_v2.pdf · 1.2 MB', 'webhook_handler.ts · 4 KB'].map(f => (
                <span key={f} className="text-xs px-3 py-1.5 bg-[#161B22] border border-[#30363D] rounded-lg text-gray-400">{f}</span>
              ))}
              <button className="text-xs px-3 py-1.5 border border-dashed border-[#30363D] text-gray-500 rounded-lg hover:border-gray-500 transition-colors">Upload file</button>
            </div>

            <h3 className="text-sm font-semibold text-white mt-5 mb-2">Comments</h3>
            <div className="flex flex-col gap-3">
              {[{ author: 'Alec Lawson', time: '2h ago', text: 'Can we add idempotency keys on retries?' }, { author: 'Jane Doe', time: '10m ago', text: 'Yes — added in branch feat/stripe-idem.' }].map(c => (
                <div key={c.author + c.time} className="bg-[#0D1117] border border-[#30363D] rounded-xl p-3">
                  <div className="flex items-center gap-2 mb-1">
                    <strong className="text-xs text-white">{c.author}</strong>
                    <span className="text-xs text-gray-500">· {c.time}</span>
                  </div>
                  <p className="text-xs text-gray-400">{c.text}</p>
                </div>
              ))}
            </div>
            <input
              type="text"
              className="mt-3 w-full bg-[#0D1117] border border-[#30363D] text-gray-300 rounded-xl px-4 py-2.5 text-sm focus:outline-none focus:ring-2 focus:ring-purple-500/40"
              placeholder="Type a message or press '/' for shortcuts…"
            />
          </div>
        </div>

        {/* Attributes sidebar */}
        <aside className="glass-panel rounded-2xl p-5 flex flex-col gap-4 h-fit">
          <h3 className="text-sm font-semibold text-white">Attributes</h3>
          <dl className="flex flex-col gap-2">
            {[['Assignee', task?.assignedToUser?.fullName ?? '—'], ['Due', task?.deadline ? new Date(task.deadline).toLocaleDateString() : '—'], ['Priority', priority], ['Team', teamName]].map(([k, v]) => (
              <div key={k} className="flex justify-between border-b border-[#30363D] pb-2">
                <dt className="text-xs text-gray-500">{k}</dt>
                <dd className="text-xs text-white">{v}</dd>
              </div>
            ))}
          </dl>
          <h3 className="text-sm font-semibold text-white mt-2">Activity</h3>
          <ul className="text-xs text-gray-400 flex flex-col gap-1.5">
            <li>Started timer</li><li>Status → In progress</li><li>Attachment added</li><li>Task created</li>
          </ul>
          <button onClick={() => navigate('/workspace/today')} className="text-xs text-purple-400 hover:text-purple-300 mt-2 transition-colors">
            ← Back to today
          </button>
        </aside>
      </div>
    </motion.div>
  );
}
