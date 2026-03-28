import React from 'react';
import { motion } from 'framer-motion';
import { Construction, ArrowLeft, LogOut } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { logout } from '../api';

export default function PlaceholderPage({ title }) {
  const navigate = useNavigate();
  return (
    <div className="min-h-screen bg-[#0D1117] flex flex-col items-center justify-center p-4">
      <motion.div 
        initial={{ opacity: 0, scale: 0.9 }}
        animate={{ opacity: 1, scale: 1 }}
        className="text-center"
      >
        <div className="inline-flex items-center justify-center w-20 h-20 bg-yellow-500/10 rounded-full border border-yellow-500/20 mb-6">
          <Construction className="w-10 h-10 text-yellow-500" />
        </div>
        <h1 className="text-3xl font-bold text-white mb-4">{title} Dashboard</h1>
        <p className="text-gray-400 max-w-md mx-auto mb-8">
          This section of the Nexus platform is currently under development by another team. 
          Please check back later for full functionality.
        </p>
        <div className="flex flex-col sm:flex-row items-center justify-center gap-4">
          <button 
            onClick={() => navigate('/login')}
            className="inline-flex items-center gap-2 text-gray-400 hover:text-white transition-colors text-sm"
          >
            <ArrowLeft className="w-4 h-4" />
            Back to Login
          </button>
          <button 
            onClick={logout}
            className="inline-flex items-center gap-2 bg-red-600/10 hover:bg-red-600/20 text-red-500 px-6 py-2.5 rounded-xl border border-red-500/20 transition-all font-medium"
          >
            <LogOut className="w-4 h-4" />
            Logout properly
          </button>
        </div>
      </motion.div>
    </div>
  );
}
