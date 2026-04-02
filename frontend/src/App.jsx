import { Navigate, Route, Routes } from "react-router-dom";

// Layouts
import TeamLeadLayout from "./components/TeamLeadLayout.jsx";
import EmployerLayout from "./components/employer/EmployerLayout.jsx";
import WorkerLayout from "./components/worker/WorkerLayout.jsx";

// Auth
import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";

// Employer
import EmployerOverviewView from "./pages/employer/EmployerOverviewView.jsx";
import EmployerProjectsBoard from "./pages/employer/EmployerProjectsBoard.jsx";
import EmployerProjectDetailView from "./pages/employer/EmployerProjectDetailView.jsx";
import EmployerTeamDirectory from "./pages/employer/EmployerTeamDirectory.jsx";
import EmployerReportsView from "./pages/employer/EmployerReportsView.jsx";
import EmployerNotificationsView from "./pages/employer/EmployerNotificationsView.jsx";
import EmployerSettingsView from "./pages/employer/EmployerSettingsView.jsx";

// Team Lead pages
import SprintBoardPage from "./pages/SprintBoardPage.jsx";
import SprintRetroPage from "./pages/SprintRetroPage.jsx";
import AnalyticsPage from "./pages/AnalyticsPage.jsx";
import BacklogPage from "./pages/BacklogPage.jsx";
import TeamOverviewPage from "./pages/TeamOverviewPage.jsx";
import NotificationsPage from "./pages/NotificationsPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";

// Worker Pages
import WorkerWorkspace from "./pages/worker/WorkerWorkspace.jsx";
import WorkerSchedule from "./pages/worker/WorkerSchedule.jsx";
import WorkerTeam from "./pages/worker/WorkerTeam.jsx";
import WorkerNotifications from "./pages/worker/WorkerNotifications.jsx";
import WorkerSettings from "./pages/worker/WorkerSettings.jsx";
import WorkerActivity from "./pages/worker/WorkerActivity.jsx";
import WorkerTaskDetail from "./pages/worker/WorkerTaskDetail.jsx";

export default function App() {
  return (
    <Routes>
      {/* Public */}
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* ─── Employer ─── */}
      <Route path="/employer" element={<Navigate to="/employer/overview" replace />} />
      <Route element={<EmployerLayout />}>
        <Route path="/employer/overview" element={<EmployerOverviewView />} />
        <Route path="/employer/projects" element={<EmployerProjectsBoard />} />
        <Route path="/employer/projects/:id" element={<EmployerProjectDetailView />} />
        <Route path="/employer/team" element={<EmployerTeamDirectory />} />
        <Route path="/employer/reports" element={<EmployerReportsView />} />
        <Route path="/employer/notifications" element={<EmployerNotificationsView />} />
        <Route path="/employer/settings" element={<EmployerSettingsView />} />
      </Route>
      <Route path="/employer/project" element={<Navigate to="/employer/projects" replace />} />

      {/* ─── Team Lead section ─── */}
      <Route element={<TeamLeadLayout />}>
        <Route path="/team-lead/sprint-board" element={<SprintBoardPage />} />
        <Route path="/team-lead/sprint-retro" element={<SprintRetroPage />} />
        <Route path="/team-lead/analytics" element={<AnalyticsPage />} />
        <Route path="/team-lead/backlog" element={<BacklogPage />} />
        <Route path="/team-lead/team-overview" element={<TeamOverviewPage />} />
        <Route path="/team-lead/notifications" element={<NotificationsPage />} />
        <Route path="/team-lead/settings" element={<SettingsPage />} />
      </Route>



      {/* ─── Worker section ─── */}
      <Route path="/worker" element={<WorkerLayout />}>
        <Route index element={<Navigate to="/worker/my-tasks" replace />} />
        <Route path="my-tasks" element={<WorkerWorkspace />} />
        <Route path="task/:id" element={<WorkerTaskDetail />} />
        <Route path="schedule" element={<WorkerSchedule />} />
        <Route path="team" element={<WorkerTeam />} />
        <Route path="activity" element={<WorkerActivity />} />
        <Route path="notifications" element={<WorkerNotifications />} />
        <Route path="settings" element={<WorkerSettings />} />
      </Route>



      {/* Catch-all */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}
