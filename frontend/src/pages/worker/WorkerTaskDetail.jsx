import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { ArrowLeft, Play, Square, Check, AlertCircle, Clock, Upload, Send } from 'lucide-react';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5125';
const TASK_STATUS = {
  TODO: 'Todo',
  IN_PROGRESS: 'InProgress',
  DONE: 'Done',
  BLOCKED: 'Blocked',
};

function getStoredUser() {
  try {
    return JSON.parse(localStorage.getItem('user') || 'null');
  } catch {
    return null;
  }
}

function unwrapBody(body) {
  return body?.data ?? body?.Data ?? body ?? null;
}

function unwrapItems(body) {
  const payload = unwrapBody(body);
  return payload?.items ?? payload?.Items ?? payload ?? [];
}

function normalizeTaskStatus(status) {
  if (status === 1 || status === 'Todo') return TASK_STATUS.TODO;
  if (status === 2 || status === 'InProgress') return TASK_STATUS.IN_PROGRESS;
  if (status === 3 || status === 'Review') return 'Review';
  if (status === 4 || status === 'Done') return TASK_STATUS.DONE;
  if (status === 5 || status === 'Blocked') return TASK_STATUS.BLOCKED;
  return TASK_STATUS.TODO;
}

export default function WorkerTaskDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [task, setTask] = useState(null);
  const [subtasks, setSubtasks] = useState([]);
  const [comments, setComments] = useState([]);
  const [attachments, setAttachments] = useState([]);
  const [newComment, setNewComment] = useState('');
  const [timerRunning, setTimerRunning] = useState(false);
  const [elapsedSeconds, setElapsedSeconds] = useState(0);

  useEffect(() => {
    loadAll();
  }, [id]);

  // Live timer
  useEffect(() => {
    let interval;
    if (timerRunning) {
      interval = setInterval(() => setElapsedSeconds(s => s + 1), 1000);
    }
    return () => clearInterval(interval);
  }, [timerRunning]);

  async function loadAll() {
    const token = localStorage.getItem('token');
    const headers = { 'Authorization': `Bearer ${token}` };
    try {
      const [taskRes, subtaskRes, commentRes, attachRes] = await Promise.all([
        fetch(`${API_BASE_URL}/api/tasks/${id}`, { headers }),
        fetch(`${API_BASE_URL}/api/subtasks?TaskId=${id}`, { headers }),
        fetch(`${API_BASE_URL}/api/taskComment`, { headers }),
        fetch(`${API_BASE_URL}/api/attachment`, { headers }),
      ]);

      if (taskRes.ok) {
        const d = await taskRes.json();
        setTask(unwrapBody(d));
      }
      if (subtaskRes.ok) {
        const d = await subtaskRes.json();
        setSubtasks(unwrapItems(d));
      }
      if (commentRes.ok) {
        const d = await commentRes.json();
        const all = unwrapItems(d);
        setComments(all.filter ? all.filter(c => c.taskId === id) : all);
      }
      if (attachRes.ok) {
        const d = await attachRes.json();
        const all = unwrapItems(d);
        setAttachments(all.filter ? all.filter(a => a.taskId === id) : all);
      }
    } catch (err) { console.error(err); }
    setLoading(false);
  }

  const handleToggleSubtask = async (subtaskId, isCompleted) => {
    const token = localStorage.getItem('token');
    try {
      await fetch(`${API_BASE_URL}/api/subtasks/${subtaskId}/completed?isCompleted=${!isCompleted}`, {
        method: 'PATCH', headers: { 'Authorization': `Bearer ${token}` }
      });
      setSubtasks(prev => prev.map(s => s.id === subtaskId ? {...s, isCompleted: !isCompleted} : s));
    } catch(err){}
  };

  const handleSendComment = async () => {
    if (!newComment.trim()) return;
    const token = localStorage.getItem('token');
    const currentUser = getStoredUser();
    const currentUserId = currentUser?.userId ?? currentUser?.id ?? null;

    if (!currentUserId) {
      return;
    }

    try {
      await fetch(`${API_BASE_URL}/api/taskComment/add-comment`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
        body: JSON.stringify({
          taskId: id,
          authorId: currentUserId,
          message: newComment
        })
      });
      setNewComment('');
      loadAll();
    } catch(err){}
  };

  const handleTaskStatus = async (status) => {
    const token = localStorage.getItem('token');
    const currentUser = getStoredUser();
    const currentUserId = currentUser?.userId ?? currentUser?.id ?? null;

    try {
      await fetch(`${API_BASE_URL}/api/tasks/${id}/status?status=${status}`, {
        method: 'PATCH', headers: { 'Authorization': `Bearer ${token}` }
      });
      if (status === TASK_STATUS.IN_PROGRESS && currentUserId) {
        setTimerRunning(true);
        await fetch(`${API_BASE_URL}/api/focussession/start`, {
          method: 'POST',
          headers: { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
          body: JSON.stringify({ taskId: id, userId: currentUserId })
        });
      } else {
        setTimerRunning(false);
      }
      loadAll();
    } catch(err){}
  };

  const formatTimer = (seconds) => {
    const h = String(Math.floor(seconds / 3600)).padStart(2, '0');
    const m = String(Math.floor((seconds % 3600) / 60)).padStart(2, '0');
    const s = String(seconds % 60).padStart(2, '0');
    return `${h}:${m}:${s}`;
  };

  if (loading) return <div className="p-8 text-white">Loading task...</div>;
  if (!task) return <div className="p-8 text-white">Task not found</div>;

  const priorityMap = { 0: 'Low', 1: 'Medium', 2: 'High', Low: 'Low', Medium: 'Medium', High: 'High' };
  const statusMap = { 1: TASK_STATUS.TODO, 2: TASK_STATUS.IN_PROGRESS, 3: 'Review', 4: TASK_STATUS.DONE, 5: TASK_STATUS.BLOCKED, Todo: TASK_STATUS.TODO, InProgress: TASK_STATUS.IN_PROGRESS, Done: TASK_STATUS.DONE, Blocked: TASK_STATUS.BLOCKED };
  const priority = priorityMap[task.priority] || 'Medium';
  const status = statusMap[task.status] || normalizeTaskStatus(task.status);
  const isInProgress = status === TASK_STATUS.IN_PROGRESS;

  // Mock subtasks for display if none exist
  const displaySubtasks = subtasks.length > 0 ? subtasks : [
    { id: 'sub1', title: 'Handle checkout session completed webhook', isCompleted: true },
    { id: 'sub2', title: 'Validate payment intent status transitions', isCompleted: true },
    { id: 'sub3', title: 'Integrate tax calculation service hook', isCompleted: false },
    { id: 'sub4', title: 'Write unit tests for signature verification', isCompleted: false },
  ];

  // Mock attachments if none
  const displayAttachments = attachments.length > 0 ? attachments : [
    { id: 'att1', fileName: 'stripe_specs_v3.pdf', fileType: 'PDF', fileSize: '1.2 MB' },
    { id: 'att2', fileName: 'webhook_handler.ts', fileType: 'TS', fileSize: '4 KB' },
  ];

  // Mock comments if none
  const displayComments = comments.length > 0 ? comments : [
    { id: 'c1', authorName: 'Alex Lawson', authorInitials: 'AL', content: "I've verified the webhook secret in the staging environment. You should be able to test the signature verification now.", createdAt: '2h ago' },
    { id: 'c2', authorName: 'Jane Doe (You)', authorInitials: 'JD', content: 'Thanks Alex! Moving on to tax calculation hooks now.', createdAt: '45m ago', isOwn: true },
  ];

  return (
    <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} className="flex flex-col max-w-7xl mx-auto w-full pb-10">
      
      {/* Back Button */}
      <button onClick={() => navigate('/worker/my-tasks')} className="flex items-center gap-2 text-gray-400 hover:text-white text-sm font-medium mb-6 transition-colors w-max">
        <ArrowLeft size={16} /> Back to My Tasks
      </button>

      <div className="grid grid-cols-[1fr_340px] gap-6">
        
        {/* ── LEFT COLUMN: Task Details ── */}
        <div className="space-y-8">
          
          {/* Task Title Block */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-8 shadow-2xl">
            <div className="flex gap-2 items-center mb-4">
              <span className={`text-[9px] font-bold tracking-widest uppercase px-2 py-0.5 rounded border ${priority === 'High' ? 'text-amber-500 border-amber-500/20 bg-amber-500/10' : priority === 'Medium' ? 'text-purple-400 border-purple-500/20 bg-purple-500/10' : 'text-gray-400 border-gray-500/30'}`}>{priority} Priority</span>
              {task.tags?.map(tg => <span key={tg} className="text-[9px] font-bold tracking-widest uppercase px-2 py-0.5 rounded border border-gray-600/30 bg-[#1C212B] text-gray-400">{tg}</span>)}
            </div>

            <div className="flex items-start justify-between mb-4">
              <h1 className="text-2xl font-bold text-white tracking-tight leading-tight">{task.title}</h1>
              {/* Timer Badge */}
              <div className={`px-4 py-2 rounded-lg border font-mono text-lg tracking-widest font-bold ${isInProgress ? 'bg-purple-500/10 border-purple-500/30 text-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.3)]' : 'bg-[#161B22] border-[#2D3342] text-gray-500'}`}>
                {formatTimer(elapsedSeconds)}
              </div>
            </div>

            <p className="text-sm text-gray-400 leading-relaxed mb-6">{task.description || 'No description provided.'}</p>
            
            {/* Action Buttons */}
            <div className="flex gap-3 pt-4 border-t border-[#1C212B]">
              {isInProgress ? (
                <>
                  <button onClick={() => handleTaskStatus(TASK_STATUS.TODO)} className="flex items-center gap-2 py-2 px-5 rounded-lg bg-purple-500/10 border border-purple-500/30 text-purple-400 hover:bg-purple-500/20 text-xs font-bold transition-all"><Square size={14} /> Stop</button>
                  <button onClick={() => handleTaskStatus(TASK_STATUS.DONE)} className="flex items-center gap-2 py-2 px-5 rounded-lg bg-[#161B22] border border-[#2D3342] text-gray-400 hover:text-white text-xs font-bold transition-all"><Check size={14} /> Complete</button>
                  <button className="flex items-center gap-2 py-2 px-5 rounded-lg bg-red-500/10 border border-red-500/30 text-red-500 hover:bg-red-500/20 text-xs font-bold transition-all"><AlertCircle size={14} /> Blocked</button>
                </>
              ) : status === TASK_STATUS.DONE ? (
                <span className="text-green-500 text-sm font-bold flex items-center gap-2"><Check size={16} /> Task Completed</span>
              ) : (
                <>
                  <button onClick={() => handleTaskStatus(TASK_STATUS.IN_PROGRESS)} className="flex items-center gap-2 py-2 px-5 rounded-lg bg-purple-600 hover:bg-purple-500 text-white text-xs font-bold transition-all shadow-[0_0_15px_rgba(168,85,247,0.4)]"><Play size={14} /> Start Working</button>
                  <button onClick={() => handleTaskStatus(TASK_STATUS.DONE)} className="flex items-center gap-2 py-2 px-5 rounded-lg bg-[#161B22] border border-[#2D3342] text-gray-400 hover:text-white text-xs font-bold transition-all"><Check size={14} /> Mark Complete</button>
                </>
              )}
            </div>
          </div>

          {/* Checklist / Subtasks */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
            <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-5">Checklist</h3>
            <div className="space-y-3">
              {displaySubtasks.map(st => (
                <label key={st.id} className="flex items-center gap-3 cursor-pointer group">
                  <div 
                    onClick={() => handleToggleSubtask(st.id, st.isCompleted)} 
                    className={`w-5 h-5 rounded border-2 flex items-center justify-center transition-all ${st.isCompleted ? 'bg-purple-600 border-purple-600' : 'border-[#2D3342] group-hover:border-purple-500/60'}`}
                  >
                    {st.isCompleted && <Check size={12} className="text-white" />}
                  </div>
                  <span className={`text-sm transition-colors ${st.isCompleted ? 'text-gray-500 line-through' : 'text-white'}`}>{st.title}</span>
                </label>
              ))}
            </div>
          </div>

          {/* Attachments */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
            <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-5">Attachments</h3>
            <div className="flex gap-4 flex-wrap">
              {displayAttachments.map(att => (
                <div key={att.id} className="bg-[#0A0D14] border border-[#1C212B] rounded-xl p-4 w-44 hover:border-purple-500/30 transition-colors cursor-pointer group">
                  <div className="text-[10px] font-bold text-purple-400 bg-purple-500/10 px-2 py-0.5 rounded w-max mb-2">{att.fileType || 'FILE'}</div>
                  <h4 className="text-xs font-bold text-white mb-1 group-hover:text-purple-400 transition-colors truncate">{att.fileName}</h4>
                  <span className="text-[10px] text-gray-500">{att.fileSize || ''}</span>
                </div>
              ))}
              <button className="bg-[#0A0D14] border border-dashed border-[#2D3342] rounded-xl p-4 w-36 hover:border-purple-500/40 hover:bg-purple-500/5 transition-all cursor-pointer flex flex-col items-center justify-center gap-2">
                <Upload size={20} className="text-gray-500" />
                <span className="text-xs text-gray-400 font-medium">Upload File</span>
              </button>
            </div>
          </div>

          {/* Comments Section */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
            <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-5">Comments</h3>
            
            <div className="space-y-6 mb-6">
              {displayComments.map(c => (
                <div key={c.id} className="flex gap-4">
                  <div className={`w-10 h-10 rounded-full flex items-center justify-center text-sm font-bold shrink-0 ${c.isOwn ? 'bg-purple-500 text-white' : 'bg-[#1C212B] text-gray-400 border border-[#2D3342]'}`}>
                    {c.authorInitials || (c.authorName || '??').substring(0, 2).toUpperCase()}
                  </div>
                  <div className="flex-1">
                    <div className="flex justify-between items-center mb-1">
                      <h4 className="text-sm font-bold text-white">{c.authorName || 'User'}</h4>
                      <span className="text-[10px] text-gray-500">{c.createdAt ? (typeof c.createdAt === 'string' && c.createdAt.includes('ago') ? c.createdAt : new Date(c.createdAt).toLocaleDateString()) : ''}</span>
                    </div>
                    <p className="text-sm text-gray-400 bg-[#0A0D14]/50 rounded-xl p-3 border border-[#1C212B]">{c.content}</p>
                  </div>
                </div>
              ))}
            </div>

            {/* Comment Input */}
            <div className="flex gap-3 items-center bg-[#0A0D14] border border-[#1C212B] focus-within:border-purple-500/40 rounded-xl p-1 transition-colors">
              <input 
                type="text"
                value={newComment}
                onChange={e => setNewComment(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && handleSendComment()}
                placeholder="Type a message or press '/' for shortcuts..."
                className="flex-1 bg-transparent px-4 py-3 text-sm text-white outline-none"
              />
              <button onClick={handleSendComment} className="px-4 py-2.5 bg-purple-600 hover:bg-purple-500 rounded-lg transition-all mr-1">
                <Send size={16} className="text-white" />
              </button>
            </div>
          </div>

        </div>

        {/* ── RIGHT COLUMN: Attributes & Activity ── */}
        <div className="space-y-6">
          
          {/* Attributes */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
            <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-5">Attributes</h3>
            <div className="space-y-5">
              <AttrRow label="Assignee" value={task.assigneeName || 'Jane Doe'} initials="JD" />
              <AttrRow label="Reporter" value={task.reporterName || 'Mark K.'} initials="MK" />
              <AttrRow label="Due Date" value={task.deadline ? new Date(task.deadline).toLocaleDateString('en-US', { month: 'short', day: 'numeric', hour: '2-digit', minute: '2-digit' }) : 'Oct 24, 14:00'} isDate />
              <AttrRow label="Time Spent" value={`${Math.floor(elapsedSeconds / 3600)}h ${Math.floor((elapsedSeconds % 3600) / 60)}m`} />
            </div>
          </div>

          {/* Activity Log */}
          <div className="bg-[#0D1117]/80 backdrop-blur-xl border border-[#1C212B] rounded-2xl p-6 shadow-xl">
            <h3 className="text-[10px] font-bold text-gray-500 tracking-widest uppercase mb-5">Activity Log</h3>
            <div className="space-y-5 border-l border-[#1C212B] ml-1.5 pl-4">
              <LogItem dot="border-purple-500" title="Started Timer" time="Today, 09:15 AM" />
              <LogItem dot="border-gray-600" title={<>Changed status to <span className="text-purple-400">In Progress</span></>} time="Today, 09:10 AM" />
              <LogItem dot="border-gray-600" title={<>Added attachment <span className="text-white">webhook_handler.ts</span></>} time="Yesterday, 4:45 PM" />
              <LogItem dot="border-gray-600" title="Task created" time="Oct 21, 10:20 AM" />
            </div>
          </div>
        </div>
      </div>

    </motion.div>
  );
}

function AttrRow({ label, value, initials, isDate }) {
  return (
    <div className="flex justify-between items-center">
      <span className="text-xs text-gray-400 font-medium">{label}</span>
      <div className="flex items-center gap-2">
        {initials && (
          <div className="w-6 h-6 rounded-full bg-[#1C212B] border border-[#2D3342] flex items-center justify-center text-[9px] font-bold text-gray-400">
            {initials}
          </div>
        )}
        <span className={`text-sm font-semibold ${isDate ? 'text-red-400' : 'text-white'}`}>{value}</span>
      </div>
    </div>
  );
}

function LogItem({ dot, title, time }) {
  return (
    <div className="relative">
      <div className={`absolute -left-[20px] top-1.5 w-2.5 h-2.5 rounded-full border-2 bg-[#0D1117] ${dot}`} />
      <p className="text-sm text-gray-300 font-medium">{title}</p>
      <span className="text-[10px] text-gray-500 font-medium mt-0.5 block">{time}</span>
    </div>
  );
}
