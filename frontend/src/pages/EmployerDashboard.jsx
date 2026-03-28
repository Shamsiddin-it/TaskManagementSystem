import React, { useEffect, useState } from 'react';
import {
  getProjects,
  getUsers,
  getNotifications,
  getBudget,
  getWorkspaceOverview,
  getReports,
  getWorkspaceSettings,
  createProject,
  createUser,
  addProjectMember,
  markAllNotifications,
  updateProject,
  updateWorkspaceSettings,
  downloadInvoices,
  integrationAction,
  workspaceAction,
  logout,
  getCurrentUser
} from '../api';
import { formatMoney, mapBackendNotifications, mapProjectsToBoard } from '../data';
import { LogOut } from 'lucide-react';
import { 
  AlertIcon, ArrowLeftIcon, BellIcon, ChartIcon, CheckCircleIcon, CheckIcon, 
  ChevronDownIcon, ClockIcon, DeviceIcon, DollarIcon, FolderIcon, GridIcon, 
  LanguageIcon, PlusIcon, SearchIcon, SettingsIcon, ThemeIcon, UsersIcon 
} from '../icons';

const translations = {
  en: {
    langShort: 'ENG',
    switchTheme: { dark: 'Switch to white theme', light: 'Switch to black theme' },
    management: 'Management', system: 'System', overview: 'Overview', team: 'Team', projects: 'Projects', reports: 'Reports', notifications: 'Notifications', settings: 'Settings',
    syncing: 'Syncing workspace with backend...',
    employerWorkspace: 'Employer Workspace', manageWorkspace: 'Manage your organization and track resources.', inviteTeam: 'Invite Team', newProject: 'New Project',
    totalProjects: 'Total Projects', activeMembers: 'Active Members', monthlyCost: 'Monthly Cost', completionRate: 'Completion Rate', active: 'Active', unreadAlerts: 'unread alerts', noBudgetSet: 'No budget set', burn: 'Burn',
    activeProjects: 'Active Projects', viewAll: 'View All', noProjectsYet: 'No projects yet. Create your first project from backend.', completionProgress: 'Completion Progress',
    totalBudget: 'Total Budget', spentToDate: 'Spent To Date', noBudgetPeriod: 'No budget period', actualBackendValue: 'Actual backend value', estimatedRunway: 'Estimated Runway', calculatedFromBackend: 'Calculated from backend', months: 'Months',
    teamDirectory: 'Team Directory', noMembersYet: 'No members yet.', recentActivity: 'Recent Activity', noActivityYet: 'No activity yet', backendNotificationsHere: 'Notifications from backend will appear here.',
    membersInWorkspace: 'members in workspace.', searchTeam: 'Search team members, skills, or projects...', addMember: 'Add Member', noTeamMembersYet: 'No team members yet. Use Add Member to create real users in backend.', currentProjects: 'Current Projects', noProjects: 'No projects', workload: 'Workload', noSkills: 'No skills',
    projectsBoard: 'Projects Board', projectsFromBackend: 'projects from backend.', createProject: 'Create Project', noItemsInColumn: 'No items in this column.',
    projectTimeline: 'Project Timeline', phases: 'phases', noPhasesYet: 'No phases yet', projectMembers: 'Project Members', noMembersAssigned: 'No members assigned.', unknownUser: 'Unknown user',
    budgetHistory: 'Budget History', records: 'records', noBudgetRecordsYet: 'No budget records yet.', risks: 'Risks', noRisksYet: 'No risks yet.', budgetBurn: 'Budget Burn', realValueFromBackend: 'Real value from backend project stats',
    manageAccess: 'Manage Access', updateStatus: 'Update Status', analyticsReports: 'Analytics and Reports', backendMetrics: 'Backend-generated metrics and utilization.', exportCsv: 'Export CSV', generatePdf: 'Generate PDF',
    projectCompletionTrends: 'Project Completion Trends', monthlyEfficiency: 'Monthly efficiency from backend data', budgetSpendByProject: 'Budget Spend by Project', expenseTotals: 'Expense totals per project', noExpenseRecordsYet: 'No expense records yet.',
    teamActivityHeatmap: 'Team Activity Heatmap', recentBackendActivity: 'Recent backend activity', recent4Weeks: 'Recent 4 weeks', events: 'events',
    notificationsCenter: 'Notifications Center', realNotifications: 'real notifications from backend.', markAllAsRead: 'Mark All as Read', noNotificationsYet: 'No notifications yet.',
    createNewProject: 'Create New Project', realPostRequest: 'This form sends a real POST request to backend.', projectName: 'Project Name', projectType: 'Project Type', description: 'Description', deadline: 'Deadline', budget: 'Budget', cancel: 'Cancel', launchProject: 'Launch Project',
    addWorkspaceMember: 'Add Workspace Member', realUserCreate: 'This creates a real user in backend.', fullName: 'Full Name', email: 'Email', password: 'Password', role: 'Role', skills: 'Skills', skillsPlaceholder: 'React, TypeScript, Docker', createMember: 'Create Member',
    manageAccessModal: 'Manage Access', assignAccess: 'Select a real backend user and assign project access.', user: 'User', projectRole: 'Project Role', noUsers: 'No users', addToProject: 'Add To Project',
  },
  ru: {
    langShort: 'РУС',
    switchTheme: { dark: 'Переключить на белую тему', light: 'Переключить на чёрную тему' },
    management: 'Управление', system: 'Система', overview: 'Обзор', team: 'Команда', projects: 'Проекты', reports: 'Отчёты', notifications: 'Уведомления', settings: 'Настройки',
    syncing: 'Синхронизация рабочего пространства с backend...',
    employerWorkspace: 'Рабочее пространство', manageWorkspace: 'Управляйте организацией и отслеживайте ресурсы.', inviteTeam: 'Пригласить', newProject: 'Новый проект',
    totalProjects: 'Всего проектов', activeMembers: 'Активные участники', monthlyCost: 'Месячные расходы', completionRate: 'Процент завершения', active: 'Активных', unreadAlerts: 'непрочитанных уведомлений', noBudgetSet: 'Бюджет не задан', burn: 'Сгорание',
    activeProjects: 'Активные проекты', viewAll: 'Смотреть все', noProjectsYet: 'Проектов пока нет. Создайте первый проект через backend.', completionProgress: 'Прогресс выполнения',
    totalBudget: 'Общий бюджет', spentToDate: 'Потрачено', noBudgetPeriod: 'Период бюджета не задан', actualBackendValue: 'Фактическое значение из backend', estimatedRunway: 'Оставшийся запас', calculatedFromBackend: 'Рассчитано из backend', months: 'месяцев',
    teamDirectory: 'Список команды', noMembersYet: 'Участников пока нет.', recentActivity: 'Последняя активность', noActivityYet: 'Активности пока нет', backendNotificationsHere: 'Уведомления из backend появятся здесь.',
    membersInWorkspace: 'участников в рабочем пространстве.', searchTeam: 'Поиск по участникам, навыкам или проектам...', addMember: 'Добавить', noTeamMembersYet: 'Участников пока нет. Используйте Add Member, чтобы создать реальных пользователей в backend.', currentProjects: 'Текущие проекты', noProjects: 'Нет проектов', workload: 'Загрузка', noSkills: 'Нет навыков',
    projectsBoard: 'Доска проектов', projectsFromBackend: 'проектов из backend.', createProject: 'Создать проект', noItemsInColumn: 'В этой колонке пока пусто.',
    projectTimeline: 'Таймлайн проекта', phases: 'этапов', noPhasesYet: 'Этапов пока нет', projectMembers: 'Участники проекта', noMembersAssigned: 'Участники не назначены.', unknownUser: 'Неизвестный пользователь',
    budgetHistory: 'История бюджета', records: 'записей', noBudgetRecordsYet: 'Бюджетных записей пока нет.', risks: 'Риски', noRisksYet: 'Рисков пока нет.', budgetBurn: 'Сгорание бюджета', realValueFromBackend: 'Реальное значение из статистики проекта backend',
    manageAccess: 'Доступ', updateStatus: 'Обновить статус', analyticsReports: 'Аналитика и отчёты', backendMetrics: 'Метрики и загрузка из backend.', exportCsv: 'Экспорт CSV', generatePdf: 'Сгенерировать PDF',
    projectCompletionTrends: 'Тренды завершения проектов', monthlyEfficiency: 'Месячная эффективность по данным backend', budgetSpendByProject: 'Расходы по проектам', expenseTotals: 'Суммарные расходы по проектам', noExpenseRecordsYet: 'Записей о расходах пока нет.',
    teamActivityHeatmap: 'Тепловая карта активности команды', recentBackendActivity: 'Недавняя активность backend', recent4Weeks: 'Последние 4 недели', events: 'событий',
    notificationsCenter: 'Центр уведомлений', realNotifications: 'реальных уведомлений из backend.', markAllAsRead: 'Прочитать все', noNotificationsYet: 'Уведомлений пока нет.',
    createNewProject: 'Создать новый проект', realPostRequest: 'Эта форма отправляет реальный POST-запрос в backend.', projectName: 'Название проекта', projectType: 'Тип проекта', description: 'Описание', deadline: 'Дедлайн', budget: 'Бюджет', cancel: 'Отмена', launchProject: 'Запустить проект',
    addWorkspaceMember: 'Добавить участника', realUserCreate: 'Это создаёт реального пользователя в backend.', fullName: 'Полное имя', email: 'Email', password: 'Пароль', role: 'Роль', skills: 'Навыки', skillsPlaceholder: 'React, TypeScript, Docker', createMember: 'Создать участника',
    manageAccessModal: 'Управление доступом', assignAccess: 'Выберите реального пользователя из backend и назначьте доступ к проекту.', user: 'Пользователь', projectRole: 'Роль в проекте', noUsers: 'Нет пользователей', addToProject: 'Добавить в проект',
  },
};

export default function EmployerDashboard() {
  const [theme, setTheme] = useState(() => {
    const savedTheme = localStorage.getItem('nexus-theme');
    return savedTheme === 'light' ? 'light' : 'dark';
  });
  const [language, setLanguage] = useState(() => {
    const savedLanguage = localStorage.getItem('nexus-language');
    return savedLanguage === 'ru' ? 'ru' : 'en';
  });
  const [activeNav, setActiveNav] = useState('overview');
  const [session, setSession] = useState(null);
  const [projects, setProjects] = useState([]);
  const [users, setUsers] = useState([]);
  const [notifications, setNotifications] = useState([]);
  const [budget, setBudget] = useState(null);
  const [overview, setOverview] = useState(null);
  const [reports, setReports] = useState(null);
  const [workspaceSettings, setWorkspaceSettings] = useState(null);
  const [projectMembers, setProjectMembers] = useState([]);
  const [projectTimeline, setProjectTimeline] = useState([]);
  const [projectRisks, setProjectRisks] = useState([]);
  const [projectStats, setProjectStats] = useState(null);
  const [budgetHistory, setBudgetHistory] = useState([]);
  const [selectedProjectId, setSelectedProjectId] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [createForm, setCreateForm] = useState({ title: '', description: '', type: 'Enterprise', deadline: '', budget: '' });
  const [memberForm, setMemberForm] = useState({ fullName: '', email: '', password: '', role: 'Worker', skills: '' });
  const [accessForm, setAccessForm] = useState({ userId: '', projectRole: 'Contributor' });
  const [modal, setModal] = useState(null);

  useEffect(() => { bootstrap() }, []);
  useEffect(() => { if (session) refreshWorkspace(session.token) }, [session]);
  useEffect(() => { if (session && selectedProjectId) loadProjectDetails(session.token, selectedProjectId) }, [session, selectedProjectId]);
  useEffect(() => { localStorage.setItem('nexus-theme', theme) }, [theme]);
  useEffect(() => { localStorage.setItem('nexus-language', language) }, [language]);

  const bootstrap = () => {
    const token = localStorage.getItem('token');
    const userJson = localStorage.getItem('user');
    if (token && userJson) {
      const user = JSON.parse(userJson);
      setSession({
        token,
        userId: user.userId,
        fullName: user.fullName || user.email
      });
    } else {
      setIsLoading(false);
    }
  };

  const refreshWorkspace = async (token) => {
    try {
      const [projectData, userData, notificationData, budgetData, overviewData, reportData, settingsData] = await Promise.all([
        getProjects(token), getUsers(token), getNotifications(token), getBudget(token), getWorkspaceOverview(token), getReports(token), getWorkspaceSettings(token),
      ]);
      setProjects(projectData ?? []);
      setUsers(userData ?? []);
      setNotifications(notificationData ?? []);
      setBudget(budgetData);
      setOverview(overviewData);
      setReports(reportData);
      setWorkspaceSettings(settingsData);
      setSelectedProjectId((current) => current ?? projectData?.[0]?.id ?? null);
    } catch (e) {
      console.error(e);
    } finally {
      setIsLoading(false);
    }
  };

  const loadProjectDetails = async (token, projectId) => {
    const [members, timeline, risks, stats, history] = await Promise.all([
      getProjectMembers(token, projectId), getProjectTimeline(token, projectId), getProjectRisks(token, projectId), getProjectStats(token, projectId), getProjectBudgetHistory(token, projectId),
    ]);
    setProjectMembers((members ?? []).map((item) => ({ userId: item.userId, fullName: item.fullName, avatarInitials: item.avatarInitials, projectRole: item.projectRole })));
    setProjectTimeline((timeline ?? []).map((item) => ({ phaseName: item.phaseName, startDate: item.startDate, endDate: item.endDate, colorHex: item.colorHex })));
    setProjectRisks((risks ?? []).map((item) => ({ description: item.description, severity: item.severity, status: item.status })));
    setProjectStats(stats ? { completionPercent: stats.completionPercent, totalMembers: stats.totalMembers, budgetBurnPercent: stats.budgetBurnPercent } : null);
    setBudgetHistory(history ?? []);
  };

  const handleCreateProject = async (event) => {
    event.preventDefault();
    if (!session) return;
    const created = await createProject(session.token, {
      title: createForm.title,
      description: createForm.description,
      type: createForm.type,
      globalDeadline: createForm.deadline ? new Date(createForm.deadline).toISOString() : null,
      budgetAllocated: createForm.budget ? Number(createForm.budget) : null,
    });
    if (!created) return;
    setModal(null);
    setCreateForm({ title: '', description: '', type: 'Enterprise', deadline: '', budget: '' });
    await refreshWorkspace(session.token);
    setSelectedProjectId(created.id);
    setActiveNav('projects');
  };

  const handleCreateUser = async (event) => {
    event.preventDefault();
    if (!session) return;
    await createUser(session.token, {
      fullName: memberForm.fullName,
      email: memberForm.email,
      password: memberForm.password,
      role: memberForm.role,
      skills: memberForm.skills.split(',').map((item) => item.trim()).filter(Boolean),
    });
    setModal(null);
    setMemberForm({ fullName: '', email: '', password: '', role: 'Worker', skills: '' });
    await refreshWorkspace(session.token);
  };

  const handleAddProjectMember = async (event) => {
    event.preventDefault();
    if (!session || !selectedProjectId) return;
    await addProjectMember(session.token, selectedProjectId, accessForm);
    setModal(null);
    setAccessForm({ userId: '', projectRole: 'Contributor' });
    await loadProjectDetails(session.token, selectedProjectId);
    await refreshWorkspace(session.token);
  };

  const handleMarkAllRead = async () => {
    if (!session) return;
    await markAllNotifications(session.token);
    await refreshWorkspace(session.token);
  };

  const handleAdvanceStatus = async () => {
    if (!session || !selectedProjectId) return;
    const current = projects.find((item) => item.id === selectedProjectId);
    if (!current) return;
    const next = current.status === 'Planning' ? 'Active' : current.status === 'Active' ? 'AtRisk' : 'Archived';
    await updateProject(session.token, selectedProjectId, { status: next });
    await refreshWorkspace(session.token);
    await loadProjectDetails(session.token, selectedProjectId);
  };

  const boardProjects = mapProjectsToBoard(projects);
  const selectedProject = projects.find((project) => project.id === selectedProjectId) ?? null;
  const selectedProjectBoard = boardProjects.find((project) => project.id === selectedProjectId) ?? null;
  const notificationFeed = mapBackendNotifications(notifications);
  const copy = translations[language];

  if (isLoading) return <LoadingPanel copy={copy} />;

  return (
    <div className={`app-shell theme-${theme}`}>
      <Sidebar activeNav={activeNav} onSelect={setActiveNav} copy={copy} />
      <main className="workspace">
        <div className="top-controls">
          <LanguageToggle language={language} onChange={setLanguage} />
          <ThemeToggle theme={theme} onToggle={setTheme} copy={copy} />
          <button className="secondary-button" type="button" onClick={logout}>
            Logout
          </button>
        </div>
        {activeNav === 'overview' ? <OverviewPage overview={overview} budget={budget} projects={boardProjects.slice(0, 3)} users={users.slice(0, 4)} notifications={notificationFeed.slice(0, 5)} onCreateProject={() => setModal('project')} onInviteTeam={() => setModal('user')} onNavigate={setActiveNav} copy={copy} /> : null}
        {activeNav === 'team' ? <TeamPage users={users} onAddMember={() => setModal('user')} copy={copy} /> : null}
        {activeNav === 'projects' ? <ProjectsPage projects={boardProjects} onCreateProject={() => setModal('project')} onSelectProject={(id) => { setSelectedProjectId(id); setActiveNav('project-detail') }} copy={copy} /> : null}
        {activeNav === 'project-detail' && selectedProject && selectedProjectBoard ? <ProjectDetailPage project={selectedProject} board={selectedProjectBoard} members={projectMembers} timeline={projectTimeline} risks={projectRisks} stats={projectStats} history={budgetHistory} onBack={() => setActiveNav('projects')} onManageAccess={() => setModal('access')} onAdvanceStatus={handleAdvanceStatus} copy={copy} /> : null}
        {activeNav === 'reports' ? <ReportsPage reports={reports} copy={copy} /> : null}
        {activeNav === 'notifications' ? <NotificationsPage notifications={notificationFeed} onMarkAllRead={handleMarkAllRead} copy={copy} /> : null}
        {activeNav === 'settings' && session ? <SettingsPage settings={workspaceSettings} token={session.token} onSettingsChange={setWorkspaceSettings} onRefresh={() => refreshWorkspace(session.token)} /> : null}
      </main>
      {modal === 'project' ? <ProjectModal form={createForm} onChange={setCreateForm} onClose={() => setModal(null)} onSubmit={handleCreateProject} copy={copy} /> : null}
      {modal === 'user' ? <UserModal form={memberForm} onChange={setMemberForm} onClose={() => setModal(null)} onSubmit={handleCreateUser} copy={copy} /> : null}
      {modal === 'access' ? <AccessModal users={users} form={accessForm} onChange={setAccessForm} onClose={() => setModal(null)} onSubmit={handleAddProjectMember} copy={copy} /> : null}
    </div>
  );
}

// Sub-components
function ThemeToggle({ theme, onToggle, copy }) {
  return (
    <div className="theme-toggle">
      <button className="theme-icon-button" onClick={() => onToggle(theme === 'dark' ? 'light' : 'dark')} title={theme === 'dark' ? copy.switchTheme.dark : copy.switchTheme.light}>
        <ThemeIcon />
      </button>
    </div>
  );
}

function LanguageToggle({ language, onChange }) {
  const items = ['en', 'ru'];
  return (
    <div className="language-toggle">
      <button className="language-button" type="button" onClick={() => onChange(items[(items.indexOf(language) + 1) % items.length])}>
        <LanguageIcon />
        <span>{translations[language].langShort}</span>
        <ChevronDownIcon />
      </button>
    </div>
  );
}

function Sidebar({ activeNav, onSelect, copy }) {
  const items = [
    ['overview', copy.overview, <GridIcon />],
    ['team', copy.team, <UsersIcon />],
    ['projects', copy.projects, <FolderIcon />],
    ['reports', copy.reports, <ChartIcon />],
    ['notifications', copy.notifications, <BellIcon />],
  ];
  return (
    <aside className="sidebar">
      <div className="brand"><div className="brand-mark" /><span>Nexus</span></div>
      <div className="sidebar-group">
        <p className="sidebar-label">{copy.management}</p>
        {items.map(([key, label, icon]) => (
          <button key={key} className={`nav-button ${activeNav === key || (key === 'projects' && activeNav === 'project-detail') ? 'active' : ''}`} onClick={() => onSelect(key)}>
            <span className="nav-icon">{icon}</span><span>{label}</span>
          </button>
        ))}
      </div>
      <div className="sidebar-group sidebar-bottom">
        <p className="sidebar-label">{copy.system}</p>
        <button className={`nav-button ${activeNav === 'settings' ? 'active' : ''}`} onClick={() => onSelect('settings')}>
          <span className="nav-icon"><SettingsIcon /></span><span>{copy.settings}</span>
        </button>
        <button 
          className="nav-button mt-4 text-red-400 hover:text-red-300 hover:bg-red-500/10" 
          onClick={logout}
          id="sidebar-logout-btn"
        >
          <span className="nav-icon"><LogOut size={18} /></span><span>Logout</span>
        </button>
      </div>
    </aside>
  );
}

function Header({ title, subtitle, actions, leading }) {
  return (
    <div className="page-hero">
      {leading ? <div className="detail-title-wrap">{leading}<div><h1>{title}</h1><p>{subtitle}</p></div></div> : <div><h1>{title}</h1><p>{subtitle}</p></div>}
      {actions ? <div className="hero-actions">{actions}</div> : null}
    </div>
  );
}

function LoadingPanel({ copy }) { return <div className="loading-panel"><div className="loading-orb" /><p>{copy.syncing}</p></div>; }
function StatCard({ title, value, hint, tone }) { return <article className="stat-card"><p className="eyebrow">{title}</p><strong>{value}</strong><span className={tone === 'green' ? 'hint-green' : ''}>{hint}</span></article>; }
function EmptyCard({ text }) { return <div className="glass-card"><p>{text}</p></div>; }

function OverviewPage({ overview, budget, projects, users, notifications, onCreateProject, onInviteTeam, onNavigate, copy }) {
  return (
    <section className="page-frame">
      <Header title={copy.employerWorkspace} subtitle={copy.manageWorkspace} actions={<><button className="secondary-button" onClick={onInviteTeam}><UsersIcon />{copy.inviteTeam}</button><button className="primary-button" onClick={onCreateProject}>{copy.newProject}</button></>} />
      <div className="stat-grid">
        <StatCard title={copy.totalProjects} value={String(overview?.totalProjects ?? 0)} hint={`${overview?.activeProjects ?? 0} ${copy.active}`} />
        <StatCard title={copy.activeMembers} value={String(overview?.activeMembers ?? 0)} hint={`${overview?.unreadNotifications ?? 0} ${copy.unreadAlerts}`} tone="green" />
        <StatCard title={copy.monthlyCost} value={`$${formatMoney(overview?.monthlyCost ?? 0)}`} hint={budget?.period ?? copy.noBudgetSet} />
        <StatCard title={copy.completionRate} value={`${overview?.completionRate ?? 0}%`} hint={`${copy.burn} ${Math.round(budget?.burnPercent ?? 0)}%`} tone="green" />
      </div>
      <div className="overview-grid">
        <div className="overview-main">
          <div className="section-head"><h2>{copy.activeProjects}</h2><button className="text-button" onClick={() => onNavigate('projects')}>{copy.viewAll}</button></div>
          <div className="project-stack">
            {projects.length === 0 ? <EmptyCard text={copy.noProjectsYet} /> : projects.map((project) => (
              <article key={project.id} className={`overview-project-card ${project.status === 'At Risk' ? 'risk' : ''}`}>
                <div className="overview-project-top">
                  <div><h3>{project.title}<span className="mini-chip">{project.type}</span></h3><p>{project.note}</p></div>
                  <span className={`project-state ${project.status.toLowerCase().replaceAll(' ', '-')}`}>{project.status}</span>
                </div>
                <div className="progress-label-row"><span>{copy.completionProgress}</span><strong>{project.progress}%</strong></div>
                <div className="line-progress"><span className="line-progress-bar" style={{ width: `${project.progress}%`, background: project.accent }} /></div>
                <div className="avatar-row"><span>{project.dueLabel}</span></div>
              </article>
            ))}
          </div>
        </div>
        <div className="overview-side">
          <div className="side-card">
            <div className="section-head compact"><h2>{copy.teamDirectory}</h2><button className="icon-button" onClick={onInviteTeam}>+</button></div>
            <div className="directory-list">
              {users.length === 0 ? <p>{copy.noMembersYet}</p> : users.map((member) => (
                <div key={member.id} className="directory-item">
                  <div className="member-chip">
                    <span className="member-chip-mark" style={{ backgroundColor: `${member.avatarColor ?? '#2a2238'}22`, color: member.avatarColor ?? '#fff' }}>{member.avatarInitials ?? member.fullName.slice(0, 2).toUpperCase()}</span>
                    <div><strong>{member.fullName}</strong><p>{member.role}</p></div>
                  </div>
                  <span className="tag-chip">{member.onlineStatus}</span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </section>
  );
}

function TeamPage({ users, onAddMember, copy }) {
  return (
    <section className="page-frame">
      <Header title={copy.teamDirectory} subtitle={`${users.length} ${copy.membersInWorkspace}`} actions={<div className="search-actions"><button className="search-box"><SearchIcon /><span>{copy.searchTeam}</span></button><button className="primary-button" onClick={onAddMember}><PlusIcon />{copy.addMember}</button></div>} />
      <div className="team-grid">
        {users.length === 0 ? <EmptyCard text={copy.noTeamMembersYet} /> : users.map((member) => (
          <article key={member.id} className="team-card">
            <div className="team-card-top"><span className="team-avatar" style={{ backgroundColor: `${member.avatarColor ?? '#2a2238'}18`, color: member.avatarColor ?? '#fff' }}>{member.avatarInitials ?? member.fullName.slice(0, 2).toUpperCase()}</span><span className="status-chip"><span className="status-dot" />{member.onlineStatus}</span></div>
            <h3>{member.fullName}</h3><span className="tag-chip">{member.role.toUpperCase()}</span>
            <p className="eyebrow">{copy.currentProjects}</p>
            <div className="chip-row">{member.currentProjects.length === 0 ? <span className="soft-chip">{copy.noProjects}</span> : member.currentProjects.map((project) => <span key={project} className="soft-chip">{project}</span>)}</div>
            <div className="progress-label-row"><span>{copy.workload}</span><strong>{member.workloadPercent}%</strong></div>
            <div className="line-progress compact"><span className="line-progress-bar" style={{ width: `${member.workloadPercent}%`, background: '#a64bff' }} /></div>
          </article>
        ))}
      </div>
    </section>
  );
}

function ProjectsPage({ projects, onCreateProject, onSelectProject, copy }) {
  const columns = ['Planning', 'In Progress', 'At Risk', 'Done'];
  return (
    <section className="page-frame">
      <Header title={copy.projectsBoard} subtitle={`${projects.length} ${copy.projectsFromBackend}`} actions={<button className="primary-button" onClick={onCreateProject}>{copy.createProject}</button>} />
      <div className="board-grid">
        {columns.map((column) => (
          <div key={column} className="board-column">
            <div className="board-column-head"><span>{column}</span><span className={`count-pill ${column.toLowerCase().replaceAll(' ', '-')}`}>{projects.filter((item) => item.status === column).length}</span></div>
            <div className="board-cards">
              {projects.filter((item) => item.status === column).length === 0 ? <EmptyCard text={copy.noItemsInColumn} /> : projects.filter((item) => item.status === column).map((project) => (
                <button key={project.id} className={`board-card ${project.status === 'At Risk' ? 'risk' : ''}`} onClick={() => onSelectProject(project.id)}>
                  <div className="board-card-top"><span className={`small-tag ${project.chipTone}`}>{project.type}</span><span className="board-id">{project.id.slice(0, 8)}</span></div>
                  <h3>{project.title}</h3>
                  <div className="progress-label-row"><span>{project.note}</span><strong>{project.progress}%</strong></div>
                  <div className="line-progress compact"><span className="line-progress-bar" style={{ width: `${project.progress}%`, background: project.accent }} /></div>
                  <div className="board-footer"><span>{project.dueLabel}</span></div>
                </button>
              ))}
            </div>
          </div>
        ))}
      </div>
    </section>
  );
}

function ProjectDetailPage({ project, board, members, timeline, risks, stats, history, onBack, onManageAccess, onAdvanceStatus, copy }) {
  return (
    <section className="page-frame">
      <Header title={project.title} subtitle={`${project.type} project`} leading={<button className="back-button" onClick={onBack}><ArrowLeftIcon /></button>} actions={<><span className="status-badge amber">{project.status}</span><button className="secondary-button" onClick={onManageAccess}>{copy.manageAccess}</button><button className="primary-button" onClick={onAdvanceStatus}>{copy.updateStatus}</button></>} />
      <div className="detail-grid">
        <div className="detail-left">
          <div className="glass-card">
            <div className="section-head compact"><h2>{copy.projectTimeline}</h2><span>{timeline.length} {copy.phases}</span></div>
            <div className="timeline-board">
              <div className="timeline-labels">{timeline.length === 0 ? <div>{copy.noPhasesYet}</div> : timeline.map((item) => <div key={item.phaseName}>{item.phaseName}</div>)}</div>
              <div className="timeline-grid">{timeline.map((item, index) => <span key={item.phaseName} className="timeline-bar" style={{ top: 18 + index * 42, height: 24, background: item.colorHex }} />)}</div>
            </div>
          </div>
        </div>
        <div className="detail-right">
          <div className="glass-card risk-card">
            <div className="risk-title"><AlertIcon /><strong>{copy.risks} ({risks.length})</strong></div>
            {risks.length === 0 ? <p>{copy.noRisksYet}</p> : <ul>{risks.map((item) => <li key={item.description}>{item.description} ({item.severity}/{item.status})</li>)}</ul>}
          </div>
        </div>
      </div>
    </section>
  );
}

function ReportsPage({ reports, copy }) {
  const trend = reports?.completionTrend ?? [];
  const maxActual = Math.max(...trend.map((item) => item.actual), 100);
  return (
    <section className="page-frame reports-page">
      <Header title={copy.analyticsReports} subtitle={copy.backendMetrics} />
      <div className="report-panels">
        <div className="glass-card report-card wide">
          <div className="report-head"><div><h2>{copy.projectCompletionTrends}</h2><p>{copy.monthlyEfficiency}</p></div></div>
          <div className="line-chart">
            <div className="line-chart-months">{trend.map((item) => <span key={item.label}>{item.label}</span>)}</div>
            <svg viewBox="0 0 1000 220" className="chart-svg" preserveAspectRatio="none"><polyline fill="none" stroke="#b36cff" strokeWidth="4" points={trend.map((item, index) => `${index * 180 + 30},${200 - (item.actual / maxActual) * 160}`).join(' ')} /></svg>
          </div>
        </div>
      </div>
    </section>
  );
}

function NotificationsPage({ notifications, onMarkAllRead, copy }) {
  return (
    <section className="page-frame">
      <Header title={copy.notificationsCenter} subtitle={`${notifications.length} ${copy.realNotifications}`} actions={<button className="secondary-button" onClick={onMarkAllRead}><CheckIcon />{copy.markAllAsRead}</button>} />
      <div className="notification-section">
        <div className="notification-list">
          {notifications.length === 0 ? <EmptyCard text={copy.noNotificationsYet} /> : notifications.map((item) => (
            <article key={item.id} className="notification-card">
              <div className={`notification-icon ${item.tone}`}>{item.tone === 'amber' ? <ClockIcon /> : item.tone === 'green' ? <DollarIcon /> : item.tone === 'blue' ? <UsersIcon /> : item.tone === 'violet' ? <CheckCircleIcon /> : <DeviceIcon />}</div>
              <div className="notification-content"><h3>{item.title}</h3><p>{item.body}</p></div>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
}

function SettingsPage({ settings, token, onSettingsChange, onRefresh }) {
  const [form, setForm] = useState(settings);
  useEffect(() => { setForm(settings) }, [settings]);
  if (!form) return <EmptyCard text="Loading settings..." />;
  return (
    <section className="page-frame">
      <Header title="Settings" subtitle="Manage your organization" />
      <div className="settings-panel-body">
         <p>Organization: {form.profile.organizationName}</p>
         <p>Plan: {form.profile.planName}</p>
         {/* More settings can be ported here if needed */}
      </div>
    </section>
  );
}

// Modals
function ProjectModal({ form, onChange, onClose, onSubmit, copy }) {
  return (
    <div className="modal-backdrop"><div className="blur-shell" onClick={onClose} /><div className="create-modal">
      <div className="create-modal-head"><h2>{copy.createNewProject}</h2><button className="close-button" onClick={onClose}>&times;</button></div>
      <form onSubmit={onSubmit} className="create-form">
        <div className="form-grid">
          <label className="field"><span>{copy.projectName}</span><input value={form.title} onChange={e => onChange({...form, title: e.target.value})} required /></label>
          <label className="field"><span>{copy.budget}</span><input type="number" value={form.budget} onChange={e => onChange({...form, budget: e.target.value})} /></label>
        </div>
        <div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>{copy.cancel}</button><button type="submit" className="primary-button">{copy.launchProject}</button></div>
      </form>
    </div></div>
  );
}

function UserModal({ form, onChange, onClose, onSubmit, copy }) {
  return (
    <div className="modal-backdrop"><div className="blur-shell" onClick={onClose} /><div className="create-modal">
      <div className="create-modal-head"><h2>{copy.addWorkspaceMember}</h2><button className="close-button" onClick={onClose}>&times;</button></div>
      <form onSubmit={onSubmit} className="create-form">
        <label className="field"><span>{copy.fullName}</span><input value={form.fullName} onChange={e => onChange({...form, fullName: e.target.value})} required /></label>
        <label className="field"><span>{copy.email}</span><input type="email" value={form.email} onChange={e => onChange({...form, email: e.target.value})} required /></label>
        <label className="field"><span>{copy.password}</span><input type="password" value={form.password} onChange={e => onChange({...form, password: e.target.value})} required /></label>
        <div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>{copy.cancel}</button><button type="submit" className="primary-button">{copy.createMember}</button></div>
      </form>
    </div></div>
  );
}

function AccessModal({ users, form, onChange, onClose, onSubmit, copy }) {
  return (
    <div className="modal-backdrop"><div className="blur-shell" onClick={onClose} /><div className="create-modal">
      <div className="create-modal-head"><h2>{copy.manageAccessModal}</h2><button className="close-button" onClick={onClose}>&times;</button></div>
      <form onSubmit={onSubmit} className="create-form">
        <label className="field"><span>{copy.user}</span>
          <select className="settings-input" value={form.userId} onChange={e => onChange({...form, userId: e.target.value})}>
            <option value="">Select User</option>
            {users.map(u => <option key={u.id} value={u.id}>{u.fullName}</option>)}
          </select>
        </label>
        <div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>{copy.cancel}</button><button type="submit" className="primary-button">{copy.addToProject}</button></div>
      </form>
    </div></div>
  );
}
