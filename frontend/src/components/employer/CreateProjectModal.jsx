import React, { useState } from 'react';
import { createProject } from "../../api";

export default function CreateProjectModal({ open, onClose, onCreated }) {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [type, setType] = useState('Enterprise');
  const [budget, setBudget] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  if (!open) return null;

  const submit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');
    const token = localStorage.getItem('token');

    try {
      const res = await createProject(token, {
         title,
         description,
         type,
         budgetAllocated: budget ? Number(budget) : 0
      });
      if (res) {
          onCreated?.(res);
          onClose?.();
      } else {
          setError("Failed to create project");
      }
    } catch(err) {
      setError(err.message || 'Error creating project');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/70 backdrop-blur-sm">
      <div className="bg-[#0D1117] border border-[#1C212B] w-[450px] rounded-xl p-6 shadow-2xl">
        <h3 className="text-lg font-bold text-white mb-4">Create New Project</h3>
        <form onSubmit={submit} className="flex flex-col gap-4">
          <div>
            <label className="text-xs text-gray-400 font-bold tracking-wide uppercase mb-1 block">Project Title</label>
            <input 
              required
              className="w-full bg-[#161B22] border border-[#2D3342] focus:border-purple-500 rounded-md px-4 py-2 text-sm text-white outline-none transition-colors"
              value={title}
              onChange={e => setTitle(e.target.value)} 
            />
          </div>
          <div>
            <label className="text-xs text-gray-400 font-bold tracking-wide uppercase mb-1 block">Description</label>
            <textarea 
              className="w-full bg-[#161B22] border border-[#2D3342] focus:border-purple-500 rounded-md px-4 py-2 text-sm text-white resize-none outline-none transition-colors"
              rows="3"
              value={description}
              onChange={e => setDescription(e.target.value)} 
            />
          </div>
          <div className="flex gap-4">
            <div className="flex-1">
               <label className="text-xs text-gray-400 font-bold tracking-wide uppercase mb-1 block">Project Type</label>
               <select 
                 className="w-full bg-[#161B22] border border-[#2D3342] text-white rounded-md px-3 py-2 text-sm outline-none"
                 value={type}
                 onChange={e => setType(e.target.value)}
               >
                 <option value="Enterprise">Enterprise</option>
                 <option value="Internal">Internal</option>
                 <option value="Web">Web</option>
               </select>
            </div>
            <div className="flex-1">
               <label className="text-xs text-gray-400 font-bold tracking-wide uppercase mb-1 block">Budget ($)</label>
               <input 
                 type="number"
                 placeholder="0.00"
                 className="w-full bg-[#161B22] border border-[#2D3342] focus:border-purple-500 rounded-md px-4 py-2 text-sm text-white outline-none transition-colors"
                 value={budget}
                 onChange={e => setBudget(e.target.value)} 
               />
            </div>
          </div>
          {error && <div className="text-xs text-red-500 font-medium">{error}</div>}
          <div className="flex items-center justify-end gap-3 mt-4">
            <button 
              type="button" 
              onClick={onClose}
              className="px-4 py-2 text-xs font-bold text-gray-400 hover:text-white transition-colors"
            >
              Cancel
            </button>
            <button 
              type="submit" 
              disabled={loading}
              className="px-5 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 text-white rounded-lg text-xs font-bold hover:shadow-[0_0_15px_rgba(168,85,247,0.4)] transition-all disabled:opacity-50"
            >
              {loading ? 'Creating...' : 'Create Project'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
