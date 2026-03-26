import { Navigate, Route, Routes } from "react-router-dom";
import TeamLeadLayout from "./components/TeamLeadLayout.jsx";
import SprintBoardPage from "./pages/SprintBoardPage.jsx";
import SprintRetroPage from "./pages/SprintRetroPage.jsx";
import AnalyticsPage from "./pages/AnalyticsPage.jsx";
import BacklogPage from "./pages/BacklogPage.jsx";
import TeamOverviewPage from "./pages/TeamOverviewPage.jsx";
import NotificationsPage from "./pages/NotificationsPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import PlaceholderPage from "./pages/PlaceholderPage.jsx";

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={<Navigate to="/login" replace />}
      />
      
      {/* Team Lead Section */}
      <Route element={<TeamLeadLayout />}>
        <Route path="/team-lead/sprint-board" element={<SprintBoardPage />} />
        <Route path="/team-lead/sprint-retro" element={<SprintRetroPage />} />
        <Route path="/team-lead/analytics" element={<AnalyticsPage />} />
        <Route path="/team-lead/backlog" element={<BacklogPage />} />
        <Route path="/team-lead/team-overview" element={<TeamOverviewPage />} />
        <Route path="/team-lead/notifications" element={<NotificationsPage />} />
        <Route path="/team-lead/settings" element={<SettingsPage />} />
      </Route>

      {/* Role-based Placeholders */}
      <Route path="/employer" element={<PlaceholderPage title="Employer" />} />
      <Route path="/worker" element={<PlaceholderPage title="Worker" />} />

      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}
