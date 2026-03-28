import { Navigate, Outlet, Route, Routes } from "react-router-dom";

import TeamLeadLayout from "./components/TeamLeadLayout.jsx";
import WorkspaceLayout from "./components/WorkspaceLayout.jsx";
import EmployerLayout from "./components/employer/EmployerLayout.jsx";
import WorkerLayout from "./components/worker/WorkerLayout.jsx";

import LoginPage from "./pages/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";

import EmployerOverviewView from "./pages/employer/EmployerOverviewView.jsx";
import EmployerProjectsBoard from "./pages/employer/EmployerProjectsBoard.jsx";
import EmployerProjectDetailView from "./pages/employer/EmployerProjectDetailView.jsx";
import EmployerTeamDirectory from "./pages/employer/EmployerTeamDirectory.jsx";
import EmployerReportsView from "./pages/employer/EmployerReportsView.jsx";
import EmployerNotificationsView from "./pages/employer/EmployerNotificationsView.jsx";
import EmployerSettingsView from "./pages/employer/EmployerSettingsView.jsx";

import SprintBoardPage from "./pages/SprintBoardPage.jsx";
import SprintRetroPage from "./pages/SprintRetroPage.jsx";
import AnalyticsPage from "./pages/AnalyticsPage.jsx";
import BacklogPage from "./pages/BacklogPage.jsx";
import TeamOverviewPage from "./pages/TeamOverviewPage.jsx";
import NotificationsPage from "./pages/NotificationsPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";

import WorkspaceDashboardPage from "./pages/workspace/WorkspaceDashboardPage.jsx";
import TodayPage from "./pages/workspace/TodayPage.jsx";
import SchedulePage from "./pages/workspace/SchedulePage.jsx";
import EndOfDayPage from "./pages/workspace/EndOfDayPage.jsx";
import WorkerNotificationsPage from "./pages/workspace/WorkerNotificationsPage.jsx";
import WorkerSettingsPage from "./pages/workspace/WorkerSettingsPage.jsx";
import TaskDetailPage from "./pages/workspace/TaskDetailPage.jsx";
import WorkspaceBacklogPage from "./pages/workspace/WorkspaceBacklogPage.jsx";
import WorkspaceActivityPage from "./pages/workspace/WorkspaceActivityPage.jsx";
import WorkspaceTeamPage from "./pages/workspace/WorkspaceTeamPage.jsx";

import WorkerWorkspace from "./pages/worker/WorkerWorkspace.jsx";
import WorkerSchedule from "./pages/worker/WorkerSchedule.jsx";
import WorkerTeam from "./pages/worker/WorkerTeam.jsx";
import WorkerNotifications from "./pages/worker/WorkerNotifications.jsx";
import WorkerSettings from "./pages/worker/WorkerSettings.jsx";
import WorkerActivity from "./pages/worker/WorkerActivity.jsx";
import WorkerTaskDetail from "./pages/worker/WorkerTaskDetail.jsx";

function normalizeRole(role) {
  const normalized = (role || "")
    .toString()
    .trim()
    .toLowerCase()
    .replace(/[\s_-]+/g, "");

  if (normalized === "employer") return "employer";
  if (normalized === "teamlead") return "team-lead";
  if (normalized === "worker") return "worker";
  return "";
}

function getStoredRole() {
  return normalizeRole(localStorage.getItem("nexus_role"));
}

function isAuthenticated() {
  return Boolean(localStorage.getItem("token"));
}

function getDefaultRouteForRole(role) {
  if (role === "employer") return "/employer/overview";
  if (role === "team-lead") return "/team-lead/sprint-board";
  if (role === "worker") return "/worker/my-tasks";
  return "/login";
}

function HomeRedirect() {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }

  return <Navigate to={getDefaultRouteForRole(getStoredRole())} replace />;
}

function PublicOnlyRoute() {
  if (isAuthenticated()) {
    return <Navigate to={getDefaultRouteForRole(getStoredRole())} replace />;
  }

  return <Outlet />;
}

function RequireAuth({ roles }) {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }

  const currentRole = getStoredRole();
  if (roles?.length && !roles.includes(currentRole)) {
    return <Navigate to={getDefaultRouteForRole(currentRole)} replace />;
  }

  return <Outlet />;
}

export default function App() {
  return (
    <Routes>
      <Route element={<PublicOnlyRoute />}>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Route>

      <Route path="/" element={<HomeRedirect />} />

      <Route element={<RequireAuth roles={["employer"]} />}>
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
      </Route>

      <Route path="/employer/project" element={<Navigate to="/employer/projects" replace />} />

      <Route element={<RequireAuth roles={["team-lead"]} />}>
        <Route element={<TeamLeadLayout />}>
          <Route path="/team-lead/sprint-board" element={<SprintBoardPage />} />
          <Route path="/team-lead/sprint-retro" element={<SprintRetroPage />} />
          <Route path="/team-lead/analytics" element={<AnalyticsPage />} />
          <Route path="/team-lead/backlog" element={<BacklogPage />} />
          <Route path="/team-lead/team-overview" element={<TeamOverviewPage />} />
          <Route path="/team-lead/notifications" element={<NotificationsPage />} />
          <Route path="/team-lead/settings" element={<SettingsPage />} />
        </Route>
      </Route>

      <Route element={<RequireAuth roles={["worker", "team-lead"]} />}>
        <Route element={<WorkspaceLayout />}>
          <Route path="/workspace" element={<WorkspaceDashboardPage />} />
          <Route path="/workspace/today" element={<TodayPage />} />
          <Route path="/workspace/schedule" element={<SchedulePage />} />
          <Route path="/workspace/end-of-day" element={<EndOfDayPage />} />
          <Route path="/workspace/notifications" element={<WorkerNotificationsPage />} />
          <Route path="/workspace/settings" element={<WorkerSettingsPage />} />
          <Route path="/workspace/tasks/:id" element={<TaskDetailPage />} />
          <Route path="/workspace/backlog" element={<WorkspaceBacklogPage />} />
          <Route path="/workspace/activity" element={<WorkspaceActivityPage />} />
          <Route path="/workspace/team" element={<WorkspaceTeamPage />} />
        </Route>
      </Route>

      <Route element={<RequireAuth roles={["worker"]} />}>
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
      </Route>

      <Route path="/worker-legacy" element={<Navigate to="/workspace/today" replace />} />
      <Route path="*" element={<HomeRedirect />} />
    </Routes>
  );
}
