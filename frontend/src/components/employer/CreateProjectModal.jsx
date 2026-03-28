import React, { useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, Search } from 'lucide-react';

export default function CreateProjectModal({ isOpen, onClose }) {
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    type: 'Enterprise',
    budget: '',
    dueDate: '',
  });

  if (!isOpen) return null;

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
            <button onClick={onClose} className="p-2 text-gray-500 hover:text-white bg-[#161B22] hover:bg-[#1C212B] rounded-xl transition-colors">
              <X size={20} />
            </button>
          </div>

          {/* Body */}
          <div className="flex-1 overflow-y-auto p-6 space-y-6 custom-scrollbar text-sm">
             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Project Name</label>
                <input 
                  type="text" 
                  placeholder="e.g. Nexus Phase 2" 
                  className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-white rounded-lg p-3 transition-colors"
                />
             </div>
             
             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Description</label>
                <textarea 
                  rows={3}
                  placeholder="Brief overview of the project goals..." 
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
                        className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-white rounded-lg p-3 pl-7 transition-colors"
                      />
                   </div>
                </div>
                <div>
                   <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Target Launch Date</label>
                   <input 
                     type="date" 
                     className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 focus:ring-1 focus:ring-purple-500/30 text-gray-300 rounded-lg p-3 transition-colors style-scheme-dark"
                     style={{ colorScheme: 'dark' }}
                   />
                </div>
             </div>

             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-3">Project Type</label>
                <div className="grid grid-cols-4 gap-3">
                   {['Enterprise', 'Mobile App', 'API Service', 'Web PWA'].map(type => (
                      <button 
                        key={type}
                        onClick={() => setFormData({...formData, type})}
                        className={`py-2 rounded-lg text-xs font-bold tracking-wider transition-colors border ${
                           formData.type === type 
                           ? 'bg-purple-500/10 border-purple-500/50 text-purple-400 shadow-[0_0_10px_rgba(168,85,247,0.2)]' 
                           : 'bg-[#161B22] border-[#2D3342] text-gray-400 hover:text-gray-200'
                        }`}
                      >
                         {type}
                      </button>
                   ))}
                </div>
             </div>

             <div>
                <label className="block text-xs font-bold text-gray-500 tracking-widest uppercase mb-2">Assign Team Lead</label>
                <div className="relative">
                   <Search className="absolute left-3 top-3 w-4 h-4 text-gray-500" />
                   <input 
                     type="text" 
                     placeholder="Search directory..." 
                     className="w-full bg-[#0A0D14] border border-[#1C212B] focus:border-purple-500/50 text-white rounded-lg p-2.5 pl-9 transition-colors text-sm"
                   />
                </div>
                <div className="mt-3 flex gap-2">
                   {/* Mock selected lead */}
                   <div className="inline-flex items-center gap-2 bg-[#161B22] border border-[#2D3342] pr-3 pl-1 py-1 rounded-full">
                      <div className="w-6 h-6 rounded-full bg-purple-500/20 border border-purple-400 flex items-center justify-center text-[9px] font-bold text-purple-400">AK</div>
                      <span className="text-xs text-gray-300 font-medium">Anna Kim</span>
                      <button className="ml-1 text-gray-500 hover:text-red-400"><X size={12} /></button>
                   </div>
                </div>
             </div>
          </div>

          {/* Footer */}
          <div className="p-6 border-t border-[#1C212B] bg-[#0A0D14] flex justify-between items-center">
             <button onClick={onClose} className="px-5 py-2.5 text-sm font-medium text-gray-400 hover:text-white transition-colors">
               Save as Draft
             </button>
             <button className="px-6 py-2.5 bg-gradient-to-r from-purple-600 to-purple-500 hover:from-purple-500 hover:to-purple-400 shadow-[0_0_15px_rgba(168,85,247,0.4)] text-white font-medium text-sm rounded-xl transition-all">
               Launch Project
             </button>
          </div>
        </motion.div>
      </div>
    </AnimatePresence>
  );
}
