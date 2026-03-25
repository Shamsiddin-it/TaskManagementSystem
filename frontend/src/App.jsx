import { Navigate, Route, Routes } from "react-router-dom";
import TeamLeadLayout from "./components/TeamLeadLayout.jsx";
import SprintBoardPage from "./pages/SprintBoardPage.jsx";
import SprintRetroPage from "./pages/SprintRetroPage.jsx";
import AnalyticsPage from "./pages/AnalyticsPage.jsx";
import BacklogPage from "./pages/BacklogPage.jsx";
import TeamOverviewPage from "./pages/TeamOverviewPage.jsx";
import NotificationsPage from "./pages/NotificationsPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";

export default function App() {
  return (
    <Routes>
      <Route
        path="/"
        element={<Navigate to="/team-lead/sprint-board" replace />}
      />
      <Route element={<TeamLeadLayout />}>
        <Route path="/team-lead/sprint-board" element={<SprintBoardPage />} />
        <Route path="/team-lead/sprint-retro" element={<SprintRetroPage />} />
        <Route path="/team-lead/analytics" element={<AnalyticsPage />} />
        <Route path="/team-lead/backlog" element={<BacklogPage />} />
        <Route path="/team-lead/team-overview" element={<TeamOverviewPage />} />
        <Route path="/team-lead/notifications" element={<NotificationsPage />} />
        <Route path="/team-lead/settings" element={<SettingsPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
