import React, { useMemo, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X } from 'lucide-react';
import { createProject } from '../../api';

const projectTypeOptions = [
  { label: 'Enterprise', value: 0 },
  { label: 'Mobile App', value: 1 },
  { label: 'API Service', value: 2 },
  { label: 'Web App', value: 3 },
  { label: 'Internal', value: 4 },
];

const initialForm = {
  title: '',
  description: '',
  type: 0,
  budget: '',
  dueDate: '',
};

export default function CreateProjectModal({ isOpen, onClose, onCreated }) {
  const [formData, setFormData] = useState({
    ...initialForm,
  });
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState('');

  const canSubmit = useMemo(() => formData.title.trim().length > 0, [formData.title]);

  if (!isOpen) return null;

  const handleClose = () => {
    if (submitting) return;
    setFormData({ ...initialForm });
    setError('');
    onClose?.();
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    const token = localStorage.getItem('token');
    if (!token) {
      setError('Please log in again.');
      return;
    }

    setSubmitting(true);
    setError('');

    const result = await createProject(token, {
      title: formData.title.trim(),
      description: formData.description.trim() || null,
      type: formData.type,
      globalDeadline: formData.dueDate ? new Date(formData.dueDate).toISOString() : null,
      budgetAllocated: formData.budget ? Number(formData.budget) : null,
    });

    if (!result) {
      setError('Failed to create project.');
      setSubmitting(false);
      return;
    }

    onCreated?.(result);
    setSubmitting(false);
    handleClose();
  };

  return (
    <AnimatePresence>
      <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm">
        <motion.div
          initial={{ opacity: 0, scale: 0.95, y: 10 }}
          animate={{ opacity: 1, scale: 1, y: 0 }}
          exit={{ opacity: 0, scale: 0.95, y: 10 }}
          className="w-full max-w-2xl bg-[#0D1117] border border-[#1C212B] rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[90vh]"
        >
          {/* Header */}
          <div className="flex items-center justify-between p-6 border-b border-[#1C212B]">
            <div>
              <h2 className="text-xl font-bold text-white mb-1 tracking-tight">Create New Project</h2>
              <p className="text-sm text-gray-400">Initialize a new workspace and assign team members.</p>
            </div>
            <button onClick={handleClose} className="p-2 text-gray-500 hover:text-white bg-[#161B22] hover:bg-[#1C212B] rounded-xl transition-colors">
              <X size={20} />
            </button>
          </div>

          {/* Body */}
          <form className="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar text-sm" onSubmit={handleSubmit}>
             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Project Name</label>
                <input 
                  type="text" 
                  placeholder="e.g. Nexus Phase 2" 
                  value={formData.title}
                  onChange={(event) => setFormData((current) => ({ ...current, title: event.target.value }))}
                  className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-white rounded-lg p-3 transition-colors"
                />
             </div>
             
             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Description</label>
                <textarea 
                  rows={3}
                  placeholder="Brief overview of the project goals..." 
                  value={formData.description}
                  onChange={(event) => setFormData((current) => ({ ...current, description: event.target.value }))}
                  className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-white rounded-lg p-3 transition-colors resize-none"
                />
             </div>

             <div className="grid grid-cols-2 gap-4">
                <div>
                   <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Allocated Budget</label>
                   <div className="relative">
                      <span className="absolute left-3 top-3.5 text-gray-500 font-medium">$</span>
                      <input 
                        type="number" 
                        placeholder="15,000" 
                        value={formData.budget}
                        onChange={(event) => setFormData((current) => ({ ...current, budget: event.target.value }))}
                        className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-white rounded-lg p-3 pl-7 transition-colors"
                      />
                   </div>
                </div>
                <div>
                   <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Target Launch Date</label>
                   <input 
                     type="date" 
                     value={formData.dueDate}
                     onChange={(event) => setFormData((current) => ({ ...current, dueDate: event.target.value }))}
                     className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-gray-300 rounded-lg p-3 transition-colors style-scheme-dark"
                     style={{ colorScheme: 'dark' }}
                   />
                </div>
             </div>

             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-3">Project Type</label>
                <div className="grid grid-cols-4 gap-3">
                   {projectTypeOptions.map((type) => (
                      <button 
                        key={type.value}
                        type="button"
                        onClick={() => setFormData((current) => ({ ...current, type: type.value }))}
                        className={`py-2 rounded-lg text-xs font-bold tracking-wider transition-colors border ${
                           formData.type === type.value 
                           ? 'bg-purple-500/10 border-purple-500/50 text-purple-400 shadow-[0_0_10px_rgba(168,85,247,0.2)]' 
                           : 'bg-[#161B22] border-[#2D3342] text-gray-400 hover:text-gray-200'
                        }`}
                      >
                         {type.label}
                      </button>
                   ))}
                </div>
             </div>
             {error ? <div className="text-sm text-red-400">{error}</div> : null}

          {/* Footer */}
          <div className="p-6 border-t border-[#1C212B] bg-[#0A0D14] flex justify-between items-center">
             <button type="button" onClick={handleClose} className="px-5 py-2.5 text-sm font-medium text-gray-400 hover:text-white transition-colors">
               Cancel
             </button>
             <button
               type="submit"
               disabled={!canSubmit || submitting}
               className="px-6 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.4)] text-white font-medium text-sm rounded-xl transition-all disabled:opacity-50"
             >
               {submitting ? 'Creating...' : 'Launch Project'}
             </button>
          </div>
          </form>
        </motion.div>
      </div>
    </AnimatePresence>
  );
}
