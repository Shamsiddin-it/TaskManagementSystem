import { useEffect, useState } from 'react'
import type { FormEvent, ReactNode } from 'react'
import './App.css'
import {
  addProjectMember,
  createProject,
  createUser,
  downloadInvoices,
  getBudget,
  getNotifications,
  getProjectBudgetHistory,
  getProjectMembers,
  getProjectRisks,
  getProjects,
  getProjectStats,
  getProjectTimeline,
  getReports,
  getUsers,
  getWorkspaceOverview,
  getWorkspaceSettings,
  integrationAction,
  loginEmployer,
  markAllNotifications,
  registerEmployer,
  updateProject,
  updateWorkspaceSettings,
  workspaceAction,
} from './api'
import { formatMoney, mapBackendNotifications, mapProjectsToBoard } from './data'
import { AlertIcon, ArrowLeftIcon, BellIcon, ChartIcon, CheckCircleIcon, CheckIcon, ChevronDownIcon, ClockIcon, DeviceIcon, DollarIcon, FolderIcon, GridIcon, LanguageIcon, PlusIcon, SearchIcon, SettingsIcon, ThemeIcon, UsersIcon } from './icons'
import type { CreateProjectForm, NavKey, NotificationDto, OrgBudgetDto, ProjectDto, UserSession } from './types'

type DirectoryUser = NonNullable<Awaited<ReturnType<typeof getUsers>>>[number]
type ReportsData = Awaited<ReturnType<typeof getReports>>
type OverviewDto = Awaited<ReturnType<typeof getWorkspaceOverview>>
type WorkspaceSettingsData = Awaited<ReturnType<typeof getWorkspaceSettings>>
type MemberItem = { userId: string; fullName?: string; avatarInitials?: string; projectRole: string }
type TimelineItem = { phaseName: string; startDate: string; endDate: string; colorHex: string }
type RiskItem = { description: string; severity: string; status: string }
type StatsItem = { completionPercent: number; totalMembers: number; budgetBurnPercent: number }
type HistoryItem = { amount: number; description: string; recordDate: string }
type Language = 'en' | 'ru' | 'tj'
type AuthMode = 'login' | 'register'

const translations = {
  en: {
    langShort: 'ENG',
    switchTheme: { dark: 'Switch to white theme', light: 'Switch to black theme' },
    management: 'Management',
    system: 'System',
    overview: 'Overview',
    team: 'Team',
    projects: 'Projects',
    reports: 'Reports',
    notifications: 'Notifications',
    settings: 'Settings',
    syncing: 'Syncing workspace with backend...',
    employerWorkspace: 'Employer Workspace',
    manageWorkspace: 'Manage your organization and track resources.',
    inviteTeam: 'Invite Team',
    newProject: 'New Project',
    totalProjects: 'Total Projects',
    activeMembers: 'Active Members',
    monthlyCost: 'Monthly Cost',
    completionRate: 'Completion Rate',
    active: 'Active',
    unreadAlerts: 'unread alerts',
    noBudgetSet: 'No budget set',
    burn: 'Burn',
    activeProjects: 'Active Projects',
    viewAll: 'View All',
    noProjectsYet: 'No projects yet. Create your first project from backend.',
    completionProgress: 'Completion Progress',
    totalBudget: 'Total Budget',
    spentToDate: 'Spent To Date',
    noBudgetPeriod: 'No budget period',
    actualBackendValue: 'Actual backend value',
    estimatedRunway: 'Estimated Runway',
    calculatedFromBackend: 'Calculated from backend',
    months: 'Months',
    teamDirectory: 'Team Directory',
    noMembersYet: 'No members yet.',
    recentActivity: 'Recent Activity',
    noActivityYet: 'No activity yet',
    backendNotificationsHere: 'Notifications from backend will appear here.',
    membersInWorkspace: 'members in workspace.',
    searchTeam: 'Search team members, skills, or projects...',
    addMember: 'Add Member',
    noTeamMembersYet: 'No team members yet. Use Add Member to create real users in backend.',
    currentProjects: 'Current Projects',
    noProjects: 'No projects',
    workload: 'Workload',
    noSkills: 'No skills',
    projectsBoard: 'Projects Board',
    projectsFromBackend: 'projects from backend.',
    createProject: 'Create Project',
    noItemsInColumn: 'No items in this column.',
    projectTimeline: 'Project Timeline',
    phases: 'phases',
    noPhasesYet: 'No phases yet',
    projectMembers: 'Project Members',
    noMembersAssigned: 'No members assigned.',
    unknownUser: 'Unknown user',
    budgetHistory: 'Budget History',
    records: 'records',
    noBudgetRecordsYet: 'No budget records yet.',
    risks: 'Risks',
    noRisksYet: 'No risks yet.',
    budgetBurn: 'Budget Burn',
    realValueFromBackend: 'Real value from backend project stats',
    manageAccess: 'Manage Access',
    updateStatus: 'Update Status',
    analyticsReports: 'Analytics and Reports',
    backendMetrics: 'Backend-generated metrics and utilization.',
    exportCsv: 'Export CSV',
    generatePdf: 'Generate PDF',
    projectCompletionTrends: 'Project Completion Trends',
    monthlyEfficiency: 'Monthly efficiency from backend data',
    budgetSpendByProject: 'Budget Spend by Project',
    expenseTotals: 'Expense totals per project',
    noExpenseRecordsYet: 'No expense records yet.',
    teamActivityHeatmap: 'Team Activity Heatmap',
    recentBackendActivity: 'Recent backend activity',
    recent4Weeks: 'Recent 4 weeks',
    events: 'events',
    notificationsCenter: 'Notifications Center',
    realNotifications: 'real notifications from backend.',
    markAllAsRead: 'Mark All as Read',
    noNotificationsYet: 'No notifications yet.',
    createNewProject: 'Create New Project',
    realPostRequest: 'This form sends a real POST request to backend.',
    projectName: 'Project Name',
    projectType: 'Project Type',
    description: 'Description',
    deadline: 'Deadline',
    budget: 'Budget',
    cancel: 'Cancel',
    launchProject: 'Launch Project',
    addWorkspaceMember: 'Add Workspace Member',
    realUserCreate: 'This creates a real user in backend.',
    fullName: 'Full Name',
    email: 'Email',
    password: 'Password',
    role: 'Role',
    skills: 'Skills',
    skillsPlaceholder: 'React, TypeScript, Docker',
    createMember: 'Create Member',
    manageAccessModal: 'Manage Access',
    assignAccess: 'Select a real backend user and assign project access.',
    user: 'User',
    projectRole: 'Project Role',
    noUsers: 'No users',
    addToProject: 'Add To Project',
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
  tj: {
    langShort: 'ТҶ',
    switchTheme: { dark: 'Гузариш ба заминаи сафед', light: 'Гузариш ба заминаи сиёҳ' },
    management: 'Идоракунӣ', system: 'Система', overview: 'Шарҳ', team: 'Даста', projects: 'Лоиҳаҳо', reports: 'Ҳисобот', notifications: 'Огоҳӣ', settings: 'Танзимот',
    syncing: 'Ҳамоҳангсозии workspace бо backend...',
    employerWorkspace: 'Муҳити корӣ', manageWorkspace: 'Ташкилот ва захираҳоро идора кунед.', inviteTeam: 'Даъват', newProject: 'Лоиҳаи нав',
    totalProjects: 'Ҳамаи лоиҳаҳо', activeMembers: 'Аъзои фаъол', monthlyCost: 'Хароҷоти моҳона', completionRate: 'Сатҳи анҷом', active: 'Фаъол', unreadAlerts: 'огоҳии хонданашуда', noBudgetSet: 'Буҷет таъин нашудааст', burn: 'Сӯхт',
    activeProjects: 'Лоиҳаҳои фаъол', viewAll: 'Ҳамааш', noProjectsYet: 'Ҳоло лоиҳа нест. Лоиҳаи аввалро аз backend созед.', completionProgress: 'Пешрафти иҷроиш',
    totalBudget: 'Буҷети умумӣ', spentToDate: 'Сарфшуда', noBudgetPeriod: 'Давраи буҷет нест', actualBackendValue: 'Қимати воқеӣ аз backend', estimatedRunway: 'Захираи тахминӣ', calculatedFromBackend: 'Аз backend ҳисоб шудааст', months: 'моҳ',
    teamDirectory: 'Рӯйхати даста', noMembersYet: 'Ҳоло аъзо нест.', recentActivity: 'Фаъолияти охирин', noActivityYet: 'Ҳоло фаъолият нест', backendNotificationsHere: 'Огоҳиҳо аз backend дар ин ҷо пайдо мешаванд.',
    membersInWorkspace: 'аъзо дар workspace.', searchTeam: 'Ҷустуҷӯи аъзо, маҳорат ё лоиҳа...', addMember: 'Илова', noTeamMembersYet: 'Ҳоло аъзо нест. Бо Add Member корбари воқеӣ дар backend созед.', currentProjects: 'Лоиҳаҳои ҷорӣ', noProjects: 'Лоиҳа нест', workload: 'Бор', noSkills: 'Маҳорат нест',
    projectsBoard: 'Тахтаи лоиҳаҳо', projectsFromBackend: 'лоиҳа аз backend.', createProject: 'Сохтани лоиҳа', noItemsInColumn: 'Дар ин сутун ҳоло чизе нест.',
    projectTimeline: 'Ҷадвали лоиҳа', phases: 'марҳила', noPhasesYet: 'Ҳоло марҳила нест', projectMembers: 'Аъзои лоиҳа', noMembersAssigned: 'Аъзо таъин нашудааст.', unknownUser: 'Корбари номаълум',
    budgetHistory: 'Таърихи буҷет', records: 'сабт', noBudgetRecordsYet: 'Ҳоло сабти буҷет нест.', risks: 'Хатарҳо', noRisksYet: 'Ҳоло хатар нест.', budgetBurn: 'Сӯхти буҷет', realValueFromBackend: 'Қимати воқеӣ аз омори backend',
    manageAccess: 'Дастрасӣ', updateStatus: 'Нав кардани ҳолат', analyticsReports: 'Таҳлил ва ҳисобот', backendMetrics: 'Нишондиҳанда ва истифода аз backend.', exportCsv: 'Содироти CSV', generatePdf: 'Эҷоди PDF',
    projectCompletionTrends: 'Тамоюли анҷоми лоиҳаҳо', monthlyEfficiency: 'Самаранокии моҳона аз backend', budgetSpendByProject: 'Хароҷот аз рӯи лоиҳа', expenseTotals: 'Ҳаҷми умумии хароҷот', noExpenseRecordsYet: 'Ҳоло сабти хароҷот нест.',
    teamActivityHeatmap: 'Харитаи фаъолияти даста', recentBackendActivity: 'Фаъолияти охирини backend', recent4Weeks: '4 ҳафтаи охир', events: 'ҳодиса',
    notificationsCenter: 'Маркази огоҳӣ', realNotifications: 'огоҳии воқеӣ аз backend.', markAllAsRead: 'Ҳамаро хондан', noNotificationsYet: 'Ҳоло огоҳӣ нест.',
    createNewProject: 'Сохтани лоиҳаи нав', realPostRequest: 'Ин форма POST дархости воқеӣ ба backend мефиристад.', projectName: 'Номи лоиҳа', projectType: 'Навъи лоиҳа', description: 'Тавсиф', deadline: 'Мӯҳлат', budget: 'Буҷет', cancel: 'Бекор', launchProject: 'Оғози лоиҳа',
    addWorkspaceMember: 'Иловаи аъзо', realUserCreate: 'Ин корбари воқеӣ дар backend месозад.', fullName: 'Ному насаб', email: 'Email', password: 'Рамз', role: 'Нақш', skills: 'Маҳорат', skillsPlaceholder: 'React, TypeScript, Docker', createMember: 'Сохтани аъзо',
    manageAccessModal: 'Идоракунии дастрасӣ', assignAccess: 'Корбари воқеиро аз backend интихоб карда, дастрасӣ диҳед.', user: 'Корбар', projectRole: 'Нақши лоиҳа', noUsers: 'Корбар нест', addToProject: 'Ба лоиҳа илова кардан',
  },
} as const

type Copy = (typeof translations)[Language]

function App() {
  const [theme, setTheme] = useState<'dark' | 'light'>(() => {
    const savedTheme = localStorage.getItem('nexus-theme')
    return savedTheme === 'light' ? 'light' : 'dark'
  })
  const [language, setLanguage] = useState<Language>(() => {
    const savedLanguage = localStorage.getItem('nexus-language')
    return savedLanguage === 'ru' || savedLanguage === 'tj' ? savedLanguage : 'en'
  })
  const [activeNav, setActiveNav] = useState<NavKey>('overview')
  const [session, setSession] = useState<UserSession | null>(null)
  const [projects, setProjects] = useState<ProjectDto[]>([])
  const [users, setUsers] = useState<DirectoryUser[]>([])
  const [notifications, setNotifications] = useState<NotificationDto[]>([])
  const [budget, setBudget] = useState<OrgBudgetDto | null>(null)
  const [overview, setOverview] = useState<OverviewDto | null>(null)
  const [reports, setReports] = useState<ReportsData | null>(null)
  const [workspaceSettings, setWorkspaceSettings] = useState<WorkspaceSettingsData | null>(null)
  const [projectMembers, setProjectMembers] = useState<MemberItem[]>([])
  const [projectTimeline, setProjectTimeline] = useState<TimelineItem[]>([])
  const [projectRisks, setProjectRisks] = useState<RiskItem[]>([])
  const [projectStats, setProjectStats] = useState<StatsItem | null>(null)
  const [budgetHistory, setBudgetHistory] = useState<HistoryItem[]>([])
  const [selectedProjectId, setSelectedProjectId] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [authMode, setAuthMode] = useState<AuthMode>('login')
  const [authForm, setAuthForm] = useState({ fullName: '', email: '', password: '' })
  const [createForm, setCreateForm] = useState<CreateProjectForm>({ title: '', description: '', type: 'Enterprise', deadline: '', budget: '' })
  const [memberForm, setMemberForm] = useState({ fullName: '', email: '', password: '', role: 'Worker', skills: '' })
  const [accessForm, setAccessForm] = useState({ userId: '', projectRole: 'Contributor' })
  const [modal, setModal] = useState<'project' | 'user' | 'access' | null>(null)

  useEffect(() => { void bootstrap() }, [])
  useEffect(() => { if (session) void refreshWorkspace(session.token) }, [session])
  useEffect(() => { if (session && selectedProjectId) void loadProjectDetails(session.token, selectedProjectId) }, [session, selectedProjectId])
  useEffect(() => { localStorage.setItem('nexus-theme', theme) }, [theme])
  useEffect(() => { localStorage.setItem('nexus-language', language) }, [language])

  async function bootstrap() {
    const saved = localStorage.getItem('nexus-session')
    if (saved) {
      setSession(JSON.parse(saved) as UserSession)
      return
    }
    setIsLoading(false)
  }

  async function refreshWorkspace(token: string) {
    const [projectData, userData, notificationData, budgetData, overviewData, reportData, settingsData] = await Promise.all([
      getProjects(token), getUsers(token), getNotifications(token), getBudget(token), getWorkspaceOverview(token), getReports(token), getWorkspaceSettings(token),
    ])
    setProjects(projectData ?? [])
    setUsers(userData ?? [])
    setNotifications(notificationData ?? [])
    setBudget(budgetData)
    setOverview(overviewData)
    setReports(reportData)
    setWorkspaceSettings(settingsData)
    setSelectedProjectId((current) => current ?? projectData?.[0]?.id ?? null)
    setIsLoading(false)
  }

  async function loadProjectDetails(token: string, projectId: string) {
    const [members, timeline, risks, stats, history] = await Promise.all([
      getProjectMembers(token, projectId), getProjectTimeline(token, projectId), getProjectRisks(token, projectId), getProjectStats(token, projectId), getProjectBudgetHistory(token, projectId),
    ])
    setProjectMembers((members ?? []).map((item) => ({ userId: item.userId, fullName: item.fullName, avatarInitials: item.avatarInitials, projectRole: item.projectRole })))
    setProjectTimeline((timeline ?? []).map((item) => ({ phaseName: item.phaseName, startDate: item.startDate, endDate: item.endDate, colorHex: item.colorHex })))
    setProjectRisks((risks ?? []).map((item) => ({ description: item.description, severity: item.severity, status: item.status })))
    setProjectStats(stats ? { completionPercent: stats.completionPercent, totalMembers: stats.totalMembers, budgetBurnPercent: stats.budgetBurnPercent } : null)
    setBudgetHistory(history ?? [])
  }
  async function handleCreateProject(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!session) return
    const created = await createProject(session.token, {
      title: createForm.title,
      description: createForm.description,
      type: createForm.type,
      globalDeadline: createForm.deadline ? new Date(createForm.deadline).toISOString() : null,
      budgetAllocated: createForm.budget ? Number(createForm.budget) : null,
    })
    if (!created) return
    setModal(null)
    setCreateForm({ title: '', description: '', type: 'Enterprise', deadline: '', budget: '' })
    await refreshWorkspace(session.token)
    setSelectedProjectId(created.id)
    setActiveNav('projects')
  }

  async function handleCreateUser(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!session) return
    await createUser(session.token, {
      fullName: memberForm.fullName,
      email: memberForm.email,
      password: memberForm.password,
      role: memberForm.role,
      skills: memberForm.skills.split(',').map((item) => item.trim()).filter(Boolean),
    })
    setModal(null)
    setMemberForm({ fullName: '', email: '', password: '', role: 'Worker', skills: '' })
    await refreshWorkspace(session.token)
  }

  async function handleAuthSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()

    const payload = authMode === 'register'
      ? await registerEmployer({
          fullName: authForm.fullName,
          email: authForm.email,
          password: authForm.password,
        })
      : await loginEmployer({
          email: authForm.email,
          password: authForm.password,
        })

    if (!payload) return

    const nextSession = { token: payload.token, userId: payload.userId, fullName: payload.fullName }
    localStorage.setItem('nexus-session', JSON.stringify(nextSession))
    setSession(nextSession)
    setAuthForm({ fullName: '', email: '', password: '' })
  }

  function handleLogout() {
    localStorage.removeItem('nexus-session')
    setSession(null)
    setProjects([])
    setUsers([])
    setNotifications([])
    setBudget(null)
    setOverview(null)
    setReports(null)
    setWorkspaceSettings(null)
    setProjectMembers([])
    setProjectTimeline([])
    setProjectRisks([])
    setProjectStats(null)
    setBudgetHistory([])
    setSelectedProjectId(null)
  }

  async function handleAddProjectMember(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!session || !selectedProjectId) return
    await addProjectMember(session.token, selectedProjectId, accessForm)
    setModal(null)
    setAccessForm({ userId: '', projectRole: 'Contributor' })
    await loadProjectDetails(session.token, selectedProjectId)
    await refreshWorkspace(session.token)
  }

  async function handleMarkAllRead() {
    if (!session) return
    await markAllNotifications(session.token)
    await refreshWorkspace(session.token)
  }

  async function handleAdvanceStatus() {
    if (!session || !selectedProjectId) return
    const current = projects.find((item) => item.id === selectedProjectId)
    if (!current) return
    const next = current.status === 'Planning' ? 'Active' : current.status === 'Active' ? 'AtRisk' : 'Archived'
    await updateProject(session.token, selectedProjectId, { status: next })
    await refreshWorkspace(session.token)
    await loadProjectDetails(session.token, selectedProjectId)
  }

  const boardProjects = mapProjectsToBoard(projects)
  const selectedProject = projects.find((project) => project.id === selectedProjectId) ?? null
  const selectedProjectBoard = boardProjects.find((project) => project.id === selectedProjectId) ?? null
  const notificationFeed = mapBackendNotifications(notifications)
  const copy = translations[language]

  if (!isLoading && !session) {
    return (
      <div className={`app-shell theme-${theme}`}>
        <main className="auth-shell">
          <form className="auth-card" onSubmit={handleAuthSubmit}>
            <div className="auth-tabs">
              <button type="button" className={authMode === 'login' ? 'active' : ''} onClick={() => setAuthMode('login')}>Login</button>
              <button type="button" className={authMode === 'register' ? 'active' : ''} onClick={() => setAuthMode('register')}>Register</button>
            </div>
            <div className="auth-copy">
              <h1>Nexus</h1>
              <p>Frontend uses only real backend auth. Automatic fake employer creation is disabled.</p>
            </div>
            {authMode === 'register' ? (
              <label className="field">
                <span>{copy.fullName}</span>
                <input value={authForm.fullName} onChange={(event) => setAuthForm({ ...authForm, fullName: event.target.value })} required />
              </label>
            ) : null}
            <label className="field">
              <span>{copy.email}</span>
              <input type="email" value={authForm.email} onChange={(event) => setAuthForm({ ...authForm, email: event.target.value })} required />
            </label>
            <label className="field">
              <span>{copy.password}</span>
              <input type="password" value={authForm.password} onChange={(event) => setAuthForm({ ...authForm, password: event.target.value })} required />
            </label>
            <button type="submit" className="primary-button auth-submit">
              {authMode === 'register' ? 'Create Employer Account' : 'Login'}
            </button>
          </form>
        </main>
      </div>
    )
  }

  return (
    <div className={`app-shell theme-${theme}`}>
      <Sidebar activeNav={activeNav} onSelect={setActiveNav} copy={copy} />
      <main className="workspace">
        <div className="top-controls"><LanguageToggle language={language} onChange={setLanguage} /><ThemeToggle theme={theme} onToggle={setTheme} copy={copy} />{session ? <button className="secondary-button" type="button" onClick={handleLogout}>Logout</button> : null}</div>
        {isLoading ? <LoadingPanel copy={copy} /> : null}
        {!isLoading && activeNav === 'overview' ? <OverviewPage overview={overview} budget={budget} projects={boardProjects.slice(0, 3)} users={users.slice(0, 4)} notifications={notificationFeed.slice(0, 5)} onCreateProject={() => setModal('project')} onInviteTeam={() => setModal('user')} onNavigate={setActiveNav} copy={copy} /> : null}
        {!isLoading && activeNav === 'team' ? <TeamPage users={users} onAddMember={() => setModal('user')} copy={copy} /> : null}
        {!isLoading && activeNav === 'projects' ? <ProjectsPage projects={boardProjects} onCreateProject={() => setModal('project')} onSelectProject={(id) => { setSelectedProjectId(id); setActiveNav('project-detail') }} copy={copy} /> : null}
        {!isLoading && activeNav === 'project-detail' && selectedProject && selectedProjectBoard ? <ProjectDetailPage project={selectedProject} board={selectedProjectBoard} members={projectMembers} timeline={projectTimeline} risks={projectRisks} stats={projectStats} history={budgetHistory} onBack={() => setActiveNav('projects')} onManageAccess={() => setModal('access')} onAdvanceStatus={handleAdvanceStatus} copy={copy} /> : null}
        {!isLoading && activeNav === 'reports' ? <ReportsPage reports={reports} copy={copy} /> : null}
        {!isLoading && activeNav === 'notifications' ? <NotificationsPage notifications={notificationFeed} onMarkAllRead={handleMarkAllRead} copy={copy} /> : null}
        {!isLoading && activeNav === 'settings' && session ? <SettingsPage settings={workspaceSettings} token={session.token} onSettingsChange={setWorkspaceSettings} onRefresh={() => refreshWorkspace(session.token)} /> : null}
      </main>
      {modal === 'project' ? <ProjectModal form={createForm} onChange={setCreateForm} onClose={() => setModal(null)} onSubmit={handleCreateProject} copy={copy} /> : null}
      {modal === 'user' ? <UserModal form={memberForm} onChange={setMemberForm} onClose={() => setModal(null)} onSubmit={handleCreateUser} copy={copy} /> : null}
      {modal === 'access' ? <AccessModal users={users} form={accessForm} onChange={setAccessForm} onClose={() => setModal(null)} onSubmit={handleAddProjectMember} copy={copy} /> : null}
    </div>
  )
}

function ThemeToggle({ theme, onToggle, copy }: { theme: 'dark' | 'light'; onToggle: (theme: 'dark' | 'light') => void; copy: Copy }) {
  return (
    <div className="theme-toggle">
      <button
        className="theme-icon-button"
        onClick={() => onToggle(theme === 'dark' ? 'light' : 'dark')}
        aria-label={theme === 'dark' ? copy.switchTheme.dark : copy.switchTheme.light}
        title={theme === 'dark' ? copy.switchTheme.dark : copy.switchTheme.light}
      >
        <ThemeIcon />
      </button>
    </div>
  )
}

function LanguageToggle({ language, onChange }: { language: Language; onChange: (language: Language) => void }) {
  const items: Language[] = ['en', 'ru', 'tj']
  return (
    <div className="language-toggle">
      <button className="language-button" type="button" onClick={() => onChange(items[(items.indexOf(language) + 1) % items.length])}>
        <LanguageIcon />
        <span>{translations[language].langShort}</span>
        <ChevronDownIcon />
      </button>
    </div>
  )
}

function Sidebar({ activeNav, onSelect, copy }: { activeNav: NavKey; onSelect: (nav: NavKey) => void; copy: Copy }) {
  const items: Array<[NavKey, string, ReactNode]> = [
    ['overview', copy.overview, <GridIcon key="overview" />],
    ['team', copy.team, <UsersIcon key="team" />],
    ['projects', copy.projects, <FolderIcon key="projects" />],
    ['reports', copy.reports, <ChartIcon key="reports" />],
    ['notifications', copy.notifications, <BellIcon key="notifications" />],
  ]
  return <aside className="sidebar"><div className="brand"><div className="brand-mark" /><span>Nexus</span></div><div className="sidebar-group"><p className="sidebar-label">{copy.management}</p>{items.map(([key, label, icon]) => <button key={key} className={`nav-button ${activeNav === key || (key === 'projects' && activeNav === 'project-detail') ? 'active' : ''}`} onClick={() => onSelect(key)}><span className="nav-icon">{icon}</span><span>{label}</span>{key === 'notifications' ? <span className="nav-badge-dot" /> : null}</button>)}</div><div className="sidebar-group sidebar-bottom"><p className="sidebar-label">{copy.system}</p><button className="nav-button" onClick={() => onSelect('settings')}><span className="nav-icon"><SettingsIcon /></span><span>{copy.settings}</span></button></div></aside>
}

function Header({ title, subtitle, actions, leading }: { title: string; subtitle: string; actions?: ReactNode; leading?: ReactNode }) { return <div className="page-hero">{leading ? <div className="detail-title-wrap">{leading}<div><h1>{title}</h1><p>{subtitle}</p></div></div> : <div><h1>{title}</h1><p>{subtitle}</p></div>}{actions ? <div className="hero-actions">{actions}</div> : null}</div> }
function LoadingPanel({ copy }: { copy: Copy }) { return <div className="loading-panel"><div className="loading-orb" /><p>{copy.syncing}</p></div> }
function StatCard({ title, value, hint, tone }: { title: string; value: string; hint: string; tone?: 'green' }) { return <article className="stat-card"><p className="eyebrow">{title}</p><strong>{value}</strong><span className={tone === 'green' ? 'hint-green' : ''}>{hint}</span></article> }
function EmptyCard({ text }: { text: string }) { return <div className="glass-card"><p>{text}</p></div> }
function OverviewPage({ overview, budget, projects, users, notifications, onCreateProject, onInviteTeam, onNavigate, copy }: { overview: OverviewDto | null; budget: OrgBudgetDto | null; projects: ReturnType<typeof mapProjectsToBoard>; users: DirectoryUser[]; notifications: ReturnType<typeof mapBackendNotifications>; onCreateProject: () => void; onInviteTeam: () => void; onNavigate: (nav: NavKey) => void; copy: Copy }) {
  return <section className="page-frame"><Header title={copy.employerWorkspace} subtitle={copy.manageWorkspace} actions={<><button className="secondary-button" onClick={onInviteTeam}><UsersIcon />{copy.inviteTeam}</button><button className="primary-button" onClick={onCreateProject}>{copy.newProject}</button></>} /><div className="stat-grid"><StatCard title={copy.totalProjects} value={String(overview?.totalProjects ?? 0)} hint={`${overview?.activeProjects ?? 0} ${copy.active}`} /><StatCard title={copy.activeMembers} value={String(overview?.activeMembers ?? 0)} hint={`${overview?.unreadNotifications ?? 0} ${copy.unreadAlerts}`} tone="green" /><StatCard title={copy.monthlyCost} value={`$${formatMoney(overview?.monthlyCost ?? 0)}`} hint={budget?.period ?? copy.noBudgetSet} /><StatCard title={copy.completionRate} value={`${overview?.completionRate ?? 0}%`} hint={`${copy.burn} ${Math.round(budget?.burnPercent ?? 0)}%`} tone="green" /></div><div className="overview-grid"><div className="overview-main"><div className="section-head"><h2>{copy.activeProjects}</h2><button className="text-button" onClick={() => onNavigate('projects')}>{copy.viewAll}</button></div><div className="project-stack">{projects.length === 0 ? <EmptyCard text={copy.noProjectsYet} /> : projects.map((project) => <article key={project.id} className={`overview-project-card ${project.status === 'At Risk' ? 'risk' : ''}`}><div className="overview-project-top"><div><h3>{project.title}<span className="mini-chip">{project.type}</span></h3><p>{project.note}</p></div><span className={`project-state ${project.status.toLowerCase().replaceAll(' ', '-')}`}>{project.status}</span></div><div className="progress-label-row"><span>{copy.completionProgress}</span><strong>{project.progress}%</strong></div><div className="line-progress"><span className="line-progress-bar" style={{ width: `${project.progress}%`, background: project.accent }} /></div><div className="avatar-row"><span>{project.dueLabel}</span></div></article>)}</div><div className="budget-panel"><div className="budget-panel-top"><div className="window-dots"><span className="dot amber" /><span className="dot red" /><span className="dot green" /></div><span>budget_tracker</span></div><div className="budget-metrics"><div className="budget-ring"><div className="budget-ring-core"><strong>{Math.round(budget?.burnPercent ?? 0)}%</strong><span>{copy.burn}</span></div></div><div className="budget-copy"><span>{copy.totalBudget}</span><strong>${formatMoney(budget?.totalBudget ?? 0)}</strong><p>{budget?.period ?? copy.noBudgetPeriod}</p></div><div className="budget-copy"><span>{copy.spentToDate}</span><strong>${formatMoney(budget?.spentAmount ?? 0)}</strong><p>{copy.actualBackendValue}</p></div><div className="budget-copy align-end"><span>{copy.estimatedRunway}</span><strong>{budget?.estimatedRunwayMonths ?? 0} {copy.months}</strong><p>{copy.calculatedFromBackend}</p></div></div></div></div><div className="overview-side"><div className="side-card"><div className="section-head compact"><h2>{copy.teamDirectory}</h2><button className="icon-button" onClick={onInviteTeam}>+</button></div><div className="directory-list">{users.length === 0 ? <p>{copy.noMembersYet}</p> : users.map((member) => <div key={member.id} className="directory-item"><div className="member-chip"><span className="member-chip-mark" style={{ backgroundColor: `${member.avatarColor ?? '#2a2238'}22`, color: member.avatarColor ?? '#fff' }}>{member.avatarInitials ?? member.fullName.slice(0, 2).toUpperCase()}</span><div><strong>{member.fullName}</strong><p>{member.role}</p></div></div><span className="tag-chip">{member.onlineStatus}</span></div>)}</div></div><div className="side-card"><div className="section-head compact"><h2>{copy.recentActivity}</h2></div><ul className="activity-list">{notifications.length === 0 ? <li><div><strong>{copy.noActivityYet}</strong><p>{copy.backendNotificationsHere}</p></div></li> : notifications.map((item, index) => <li key={item.id}><span className={`activity-dot ${index === 0 ? 'purple' : ''}`} /><div><strong>{item.title}</strong><p>{item.createdAt}</p></div></li>)}</ul></div></div></div></section>
}

function TeamPage({ users, onAddMember, copy }: { users: DirectoryUser[]; onAddMember: () => void; copy: Copy }) {
  return <section className="page-frame"><Header title={copy.teamDirectory} subtitle={`${users.length} ${copy.membersInWorkspace}`} actions={<div className="search-actions"><button className="search-box"><SearchIcon /><span>{copy.searchTeam}</span></button><button className="primary-button" onClick={onAddMember}><PlusIcon />{copy.addMember}</button></div>} /><div className="team-grid">{users.length === 0 ? <EmptyCard text={copy.noTeamMembersYet} /> : users.map((member) => <article key={member.id} className="team-card"><div className="team-card-top"><span className="team-avatar" style={{ backgroundColor: `${member.avatarColor ?? '#2a2238'}18`, color: member.avatarColor ?? '#fff' }}>{member.avatarInitials ?? member.fullName.slice(0, 2).toUpperCase()}</span><span className="status-chip"><span className="status-dot" />{member.onlineStatus}</span></div><h3>{member.fullName}</h3><span className="tag-chip">{member.role.toUpperCase()}</span><p className="eyebrow">{copy.currentProjects}</p><div className="chip-row">{member.currentProjects.length === 0 ? <span className="soft-chip">{copy.noProjects}</span> : member.currentProjects.map((project) => <span key={project} className="soft-chip">{project}</span>)}</div><div className="progress-label-row"><span>{copy.workload}</span><strong>{member.workloadPercent}%</strong></div><div className="line-progress compact"><span className="line-progress-bar" style={{ width: `${member.workloadPercent}%`, background: '#a64bff' }} /></div><div className="chip-row">{member.skills.length === 0 ? <span className="ghost-chip">{copy.noSkills}</span> : member.skills.map((skill) => <span key={skill} className="ghost-chip">{skill}</span>)}</div></article>)}</div></section>
}

function ProjectsPage({ projects, onCreateProject, onSelectProject, copy }: { projects: ReturnType<typeof mapProjectsToBoard>; onCreateProject: () => void; onSelectProject: (id: string) => void; copy: Copy }) {
  const columns = ['Planning', 'In Progress', 'At Risk', 'Done'] as const
  return <section className="page-frame"><Header title={copy.projectsBoard} subtitle={`${projects.length} ${copy.projectsFromBackend}`} actions={<button className="primary-button" onClick={onCreateProject}>{copy.createProject}</button>} /><div className="board-grid">{columns.map((column) => <div key={column} className="board-column"><div className="board-column-head"><span>{column}</span><span className={`count-pill ${column.toLowerCase().replaceAll(' ', '-')}`}>{projects.filter((item) => item.status === column).length}</span></div><div className="board-cards">{projects.filter((item) => item.status === column).length === 0 ? <EmptyCard text={copy.noItemsInColumn} /> : projects.filter((item) => item.status === column).map((project) => <button key={project.id} className={`board-card ${project.status === 'At Risk' ? 'risk' : ''}`} onClick={() => onSelectProject(project.id)}><div className="board-card-top"><span className={`small-tag ${project.chipTone}`}>{project.type}</span><span className="board-id">{project.id.slice(0, 8)}</span></div><h3>{project.title}</h3><div className="progress-label-row"><span>{project.note}</span><strong>{project.progress}%</strong></div><div className="line-progress compact"><span className="line-progress-bar" style={{ width: `${project.progress}%`, background: project.accent }} /></div><div className="board-footer"><span>{project.dueLabel}</span></div></button>)}</div></div>)}</div></section>
}

function ProjectDetailPage({ project, board, members, timeline, risks, stats, history, onBack, onManageAccess, onAdvanceStatus, copy }: { project: ProjectDto; board: ReturnType<typeof mapProjectsToBoard>[number]; members: MemberItem[]; timeline: TimelineItem[]; risks: RiskItem[]; stats: StatsItem | null; history: HistoryItem[]; onBack: () => void; onManageAccess: () => void; onAdvanceStatus: () => void; copy: Copy }) {
  return <section className="page-frame"><Header title={project.title} subtitle={`${project.type} project`} leading={<button className="back-button" onClick={onBack}><ArrowLeftIcon /></button>} actions={<><span className="status-badge amber">{project.status}</span><button className="secondary-button" onClick={onManageAccess}>{copy.manageAccess}</button><button className="primary-button" onClick={onAdvanceStatus}>{copy.updateStatus}</button></>} /><div className="detail-grid"><div className="detail-left"><div className="glass-card"><div className="section-head compact"><h2>{copy.projectTimeline}</h2><span>{timeline.length} {copy.phases}</span></div><div className="timeline-board"><div className="timeline-labels">{timeline.length === 0 ? <div>{copy.noPhasesYet}</div> : timeline.map((item) => <div key={item.phaseName}>{item.phaseName}</div>)}</div><div className="timeline-grid">{timeline.map((item, index) => <span key={item.phaseName} className="timeline-bar" style={{ top: 18 + index * 42, height: 24, background: item.colorHex }} />)}</div></div></div><div className="detail-bottom-grid"><div className="glass-card"><div className="section-head compact"><h2>{copy.projectMembers}</h2><span>{stats?.totalMembers ?? 0}</span></div><div className="task-list">{members.length === 0 ? <p>{copy.noMembersAssigned}</p> : members.map((item) => <div key={item.userId} className="task-row"><span className="mini-avatar solid">{item.avatarInitials ?? 'U'}</span><span>{item.fullName ?? copy.unknownUser}</span><span className="meta-text">{item.projectRole}</span></div>)}</div></div><div className="glass-card"><div className="section-head compact"><h2>{copy.budgetHistory}</h2><span>{history.length} {copy.records}</span></div><div className="task-list">{history.length === 0 ? <p>{copy.noBudgetRecordsYet}</p> : history.map((item) => <div key={`${item.recordDate}-${item.description}`} className="task-row"><span>{item.description}</span><span className="meta-text">${formatMoney(item.amount)}</span></div>)}</div></div></div></div><div className="detail-right"><div className="glass-card risk-card"><div className="risk-title"><AlertIcon /><strong>{copy.risks} ({risks.length})</strong></div>{risks.length === 0 ? <p>{copy.noRisksYet}</p> : <ul>{risks.map((item) => <li key={item.description}>{item.description} ({item.severity}/{item.status})</li>)}</ul>}</div><div className="glass-card budget-mini-card"><p className="eyebrow">{copy.budgetBurn}</p><div className="budget-mini-top"><strong>{stats?.budgetBurnPercent ?? 0}%</strong><span>{board.status}</span></div><p>{copy.realValueFromBackend}</p><div className="line-progress"><span className="line-progress-bar" style={{ width: `${Math.min(stats?.budgetBurnPercent ?? 0, 100)}%`, background: '#ffb31f' }} /></div></div></div></div></section>
}
function ReportsPage({ reports, copy }: { reports: ReportsData | null; copy: Copy }) {
  const trend = reports?.completionTrend ?? []
  const maxActual = Math.max(...trend.map((item) => item.actual), 100)
  return <section className="page-frame reports-page"><Header title={copy.analyticsReports} subtitle={copy.backendMetrics} actions={<><button className="secondary-button">{copy.exportCsv}</button><button className="secondary-button">{copy.generatePdf}</button></>} /><div className="report-panels"><div className="glass-card report-card wide"><div className="report-head"><div><h2>{copy.projectCompletionTrends}</h2><p>{copy.monthlyEfficiency}</p></div></div><div className="line-chart"><div className="line-chart-months">{trend.map((item) => <span key={item.label}>{item.label}</span>)}</div><svg viewBox="0 0 1000 220" className="chart-svg" preserveAspectRatio="none"><polyline fill="none" stroke="#b36cff" strokeWidth="4" points={trend.map((item, index) => `${index * 180 + 30},${200 - (item.actual / maxActual) * 160}`).join(' ')} /></svg></div></div><div className="report-bottom"><div className="glass-card report-card"><div className="report-head"><div><h2>{copy.budgetSpendByProject}</h2><p>{copy.expenseTotals}</p></div></div><div className="bar-chart">{(reports?.budgetSpendByProject ?? []).length === 0 ? <p>{copy.noExpenseRecordsYet}</p> : reports?.budgetSpendByProject.map((item) => <div key={item.projectId} className="bar-group"><div className="bar" style={{ height: `${Math.max(item.amount / 1000, 12)}%`, background: '#8c35ff' }} /><span>{item.projectTitle}</span></div>)}</div></div><div className="glass-card report-card"><div className="report-head"><div><h2>{copy.teamActivityHeatmap}</h2><p>{copy.recentBackendActivity}</p></div></div><div className="heatmap-grid">{(reports?.teamHeatmap ?? Array.from({ length: 28 }, () => 0)).map((value, index) => <span key={index} className={`heat-cell v${value}`} />)}</div><div className="heatmap-footer"><span>{copy.recent4Weeks}</span><span>{(reports?.teamHeatmap ?? []).reduce((sum, value) => sum + value, 0)} {copy.events}</span></div></div></div></div></section>
}

function NotificationsPage({ notifications, onMarkAllRead, copy }: { notifications: ReturnType<typeof mapBackendNotifications>; onMarkAllRead: () => void; copy: Copy }) {
  return <section className="page-frame"><Header title={copy.notificationsCenter} subtitle={`${notifications.length} ${copy.realNotifications}`} actions={<button className="secondary-button" onClick={onMarkAllRead}><CheckIcon />{copy.markAllAsRead}</button>} /><div className="notification-section"><div className="notification-list">{notifications.length === 0 ? <EmptyCard text={copy.noNotificationsYet} /> : notifications.map((item) => <article key={item.id} className="notification-card"><div className={`notification-icon ${item.tone}`}>{item.tone === 'amber' ? <ClockIcon /> : item.tone === 'green' ? <DollarIcon /> : item.tone === 'blue' ? <UsersIcon /> : item.tone === 'violet' ? <CheckCircleIcon /> : <DeviceIcon />}</div><div className="notification-content"><div className="notification-title-row"><div><span className={`small-tag ${item.tone}`}>{item.type}</span><h3>{item.title}</h3><p>{item.body}</p></div><div className="notification-meta"><span>{item.createdAt}</span></div></div></div></article>)}</div></div></section>
}

function SettingsPage({ settings, token, onSettingsChange, onRefresh }: { settings: WorkspaceSettingsData | null; token: string; onSettingsChange: (settings: WorkspaceSettingsData | null) => void; onRefresh: () => Promise<void> }) {
  const [form, setForm] = useState<WorkspaceSettingsData | null>(settings)

  useEffect(() => {
    setForm(settings)
  }, [settings])

  async function handleSave() {
    if (!form) return
    const updated = await updateWorkspaceSettings(token, {
      primaryContactName: form.profile.primaryContactName,
      contactEmailAddress: form.profile.contactEmailAddress,
      companyWebsite: form.profile.companyWebsite,
      industry: form.profile.industry,
      companySize: form.profile.companySize,
      defaultTeamSizeLimit: form.teamDefaults.defaultTeamSizeLimit,
      defaultPtoPolicy: form.teamDefaults.defaultPtoPolicy,
      defaultWorkSchedule: form.teamDefaults.defaultWorkSchedule,
      primaryTimezone: form.teamDefaults.primaryTimezone,
      autoProvisionNewHires: form.teamDefaults.autoProvisionNewHires,
      requireManagerApprovalForTimeOff: form.teamDefaults.requireManagerApprovalForTimeOff,
      billingEmail: form.billing.billingEmail,
      taxIdOrVatNumber: form.billing.taxIdOrVatNumber,
      requireTwoFactorAuthentication: form.security.requireTwoFactorAuthentication,
      enforceIpAllowlist: form.security.enforceIpAllowlist,
      idleSessionTimeout: form.security.idleSessionTimeout,
      auditLogRetention: form.security.auditLogRetention,
    })

    if (updated) {
      onSettingsChange(updated)
      await onRefresh()
    }
  }

  async function handleInvoicesDownload() {
    const file = await downloadInvoices(token)
    if (!file) return

    const blob = new Blob([file.content], { type: file.contentType })
    const url = URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = file.fileName
    link.click()
    URL.revokeObjectURL(url)
  }

  async function handleWorkspaceAction(path: string, method?: 'POST' | 'DELETE') {
    await workspaceAction(token, path, method)
    await onRefresh()
  }

  async function handleIntegration(key: string, action: string) {
    const updated = await integrationAction(token, key, action)
    if (updated) onSettingsChange(updated)
    await onRefresh()
  }

  if (!form) return <EmptyCard text="Loading workspace settings from backend..." />

  return (
    <section className="page-frame settings-page">
      <Header
        title="Settings"
        subtitle="Manage your organization preferences"
        actions={
          <>
            <button className="secondary-button" onClick={() => setForm(settings)}>Discard Changes</button>
            <button className="primary-button" onClick={handleSave}>Save Configuration</button>
          </>
        }
      />
      <div className="settings-scroll">
        <div className="settings-grid">
          <div className="settings-column">
            <SettingsPanel path="~/nexus/org/profile">
              <div className="settings-profile-head">
                <button className="avatar-lg" type="button">{form.profile.organizationName.slice(0, 2).toUpperCase()}</button>
                <div className="settings-profile-copy">
                  <span className="settings-id">{form.profile.organizationCode}</span>
                  <strong>{form.profile.organizationName}</strong>
                  <span className="settings-plan-badge"><span className="settings-status-dot" />{form.profile.planName} Plan</span>
                </div>
              </div>
              <div className="settings-two-col">
                <SettingField label="Primary Contact Name">
                  <input className="settings-input" value={form.profile.primaryContactName} onChange={(event) => setForm({ ...form, profile: { ...form.profile, primaryContactName: event.target.value } })} />
                </SettingField>
                <SettingField label="Contact Email Address">
                  <input className="settings-input" type="email" value={form.profile.contactEmailAddress} onChange={(event) => setForm({ ...form, profile: { ...form.profile, contactEmailAddress: event.target.value } })} />
                </SettingField>
              </div>
              <SettingField label="Company Website">
                <input className="settings-input" type="url" value={form.profile.companyWebsite} onChange={(event) => setForm({ ...form, profile: { ...form.profile, companyWebsite: event.target.value } })} />
              </SettingField>
              <div className="settings-two-col">
                <SettingField label="Industry">
                  <select className="settings-input" value={form.profile.industry} onChange={(event) => setForm({ ...form, profile: { ...form.profile, industry: event.target.value } })}>
                    <option>Finance</option>
                    <option>Healthcare</option>
                    <option>Technology</option>
                    <option>Retail</option>
                    <option>Manufacturing</option>
                  </select>
                </SettingField>
                <SettingField label="Company Size">
                  <select className="settings-input" value={form.profile.companySize} onChange={(event) => setForm({ ...form, profile: { ...form.profile, companySize: event.target.value } })}>
                    <option>1-50 employees</option>
                    <option>51-250 employees</option>
                    <option>250-500 employees</option>
                    <option>500-1000 employees</option>
                    <option>1000+ employees</option>
                  </select>
                </SettingField>
              </div>
            </SettingsPanel>

            <SettingsPanel path="~/nexus/org/team_defaults">
              <div className="settings-two-col">
                <SettingField label="Default Team Size Limit">
                  <div className="settings-input-wrap">
                    <input className="settings-input settings-input-mono" type="number" value={String(form.teamDefaults.defaultTeamSizeLimit)} onChange={(event) => setForm({ ...form, teamDefaults: { ...form.teamDefaults, defaultTeamSizeLimit: Number(event.target.value || 0) } })} />
                    <span>members</span>
                  </div>
                </SettingField>
                <SettingField label="Default PTO Policy">
                  <select className="settings-input" value={form.teamDefaults.defaultPtoPolicy} onChange={(event) => setForm({ ...form, teamDefaults: { ...form.teamDefaults, defaultPtoPolicy: event.target.value } })}>
                    <option>Unlimited</option>
                    <option>15 days/year</option>
                    <option>20 days/year</option>
                    <option>25 days/year</option>
                  </select>
                </SettingField>
              </div>
              <div className="settings-two-col">
                <SettingField label="Default Work Schedule">
                  <select className="settings-input" value={form.teamDefaults.defaultWorkSchedule} onChange={(event) => setForm({ ...form, teamDefaults: { ...form.teamDefaults, defaultWorkSchedule: event.target.value } })}>
                    <option>Mon-Fri 9AM-5PM</option>
                    <option>Mon-Fri 8AM-4PM</option>
                    <option>Flexible (Core Hours)</option>
                    <option>4-Day Work Week</option>
                  </select>
                </SettingField>
                <SettingField label="Primary Timezone">
                  <select className="settings-input" value={form.teamDefaults.primaryTimezone} onChange={(event) => setForm({ ...form, teamDefaults: { ...form.teamDefaults, primaryTimezone: event.target.value } })}>
                    <option>America/Los_Angeles</option>
                    <option>America/Chicago</option>
                    <option>America/New_York</option>
                    <option>Europe/London</option>
                    <option>Asia/Tokyo</option>
                  </select>
                </SettingField>
              </div>
              <div className="settings-divider" />
              <ToggleRow title="Auto-provision new hires" description="Automatically create Nexus accounts via HR integration" checked={form.teamDefaults.autoProvisionNewHires} onToggle={() => setForm({ ...form, teamDefaults: { ...form.teamDefaults, autoProvisionNewHires: !form.teamDefaults.autoProvisionNewHires } })} />
              <ToggleRow title="Require manager approval for time off" description="Route all PTO requests to direct managers" checked={form.teamDefaults.requireManagerApprovalForTimeOff} onToggle={() => setForm({ ...form, teamDefaults: { ...form.teamDefaults, requireManagerApprovalForTimeOff: !form.teamDefaults.requireManagerApprovalForTimeOff } })} />
            </SettingsPanel>

            <SettingsPanel path="~/nexus/system/danger_zone" danger>
              <DangerRow title="Cancel Subscription" description="Downgrade to free tier at end of billing cycle" actionLabel="Cancel Plan" onAction={() => handleWorkspaceAction('/workspace/settings/actions/cancel-plan')} />
              <div className="settings-divider danger" />
              <DangerRow title="Export Organization Data" description="Download all member details, projects, and history" actionLabel="Request Export" onAction={() => handleWorkspaceAction('/workspace/settings/actions/request-export')} />
              <div className="settings-divider danger" />
              <DangerRow title="Close Organization Account" description="Permanently delete all data and revoke team access" actionLabel="Delete Organization" destructive onAction={() => handleWorkspaceAction('/workspace/settings/actions/close-organization', 'DELETE')} />
            </SettingsPanel>
          </div>

          <div className="settings-column">
            <SettingsPanel path="~/nexus/org/billing">
              <div className="settings-plan-card">
                <div className="settings-plan-glow" />
                <div className="settings-plan-top">
                  <div>
                    <span className="settings-plan-label">Current Plan</span>
                    <strong>{form.billing.planName}</strong>
                  </div>
                  <div className="settings-price">${form.billing.planPriceMonthly}<span>/mo</span></div>
                </div>
                <UsageBar label="Team Members" value={`${form.billing.teamMembersUsed} / ${form.billing.teamMembersLimit}`} percent={form.billing.teamMembersLimit > 0 ? (form.billing.teamMembersUsed / form.billing.teamMembersLimit) * 100 : 0} />
                <UsageBar label="Active Projects" value={`${form.billing.activeProjectsUsed} / ${form.billing.activeProjectsLimit}`} percent={form.billing.activeProjectsLimit > 0 ? (form.billing.activeProjectsUsed / form.billing.activeProjectsLimit) * 100 : 0} />
              </div>
              <div className="settings-two-col">
                <SettingField label="Next Billing Date">
                  <input className="settings-input settings-input-mono" value={new Date(form.billing.nextBillingDate).toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' })} disabled />
                </SettingField>
                <SettingField label="Payment Method">
                  <div className="settings-card-input">
                    <span className="settings-card-icon">▣</span>
                    <input className="settings-input settings-input-mono" value={`•••• •••• •••• ${form.billing.paymentMethodLast4}`} disabled />
                  </div>
                </SettingField>
              </div>
              <div className="settings-two-col">
                <SettingField label="Billing Email">
                  <input className="settings-input" type="email" value={form.billing.billingEmail} onChange={(event) => setForm({ ...form, billing: { ...form.billing, billingEmail: event.target.value } })} />
                </SettingField>
                <SettingField label="Tax ID / VAT Number">
                  <input className="settings-input" value={form.billing.taxIdOrVatNumber} onChange={(event) => setForm({ ...form, billing: { ...form.billing, taxIdOrVatNumber: event.target.value } })} />
                </SettingField>
              </div>
              <div className="settings-divider" />
              <button className="settings-full-button" type="button" onClick={handleInvoicesDownload}>Download Past Invoices</button>
            </SettingsPanel>

            <SettingsPanel path="~/nexus/org/security">
              <ToggleRow title="Require Two-Factor Authentication" description="Mandatory 2FA for all organization members" checked={form.security.requireTwoFactorAuthentication} onToggle={() => setForm({ ...form, security: { ...form.security, requireTwoFactorAuthentication: !form.security.requireTwoFactorAuthentication } })} />
              <ToggleRow title="Enforce IP Allowlist" description="Restrict access to specific corporate IP addresses" checked={form.security.enforceIpAllowlist} onToggle={() => setForm({ ...form, security: { ...form.security, enforceIpAllowlist: !form.security.enforceIpAllowlist } })} />
              <ToggleRow title="Data Encryption at Rest" description="AES-256 bit encryption (Enterprise feature)" checked={form.security.dataEncryptionAtRest} disabled onToggle={() => undefined} />
              <div className="settings-divider" />
              <div className="settings-two-col">
                <SettingField label="Idle Session Timeout">
                  <select className="settings-input" value={form.security.idleSessionTimeout} onChange={(event) => setForm({ ...form, security: { ...form.security, idleSessionTimeout: event.target.value } })}>
                    <option>15 Minutes</option>
                    <option>30 Minutes</option>
                    <option>1 Hour</option>
                    <option>4 Hours</option>
                    <option>Never</option>
                  </select>
                </SettingField>
                <SettingField label="Audit Log Retention">
                  <select className="settings-input" value={form.security.auditLogRetention} onChange={(event) => setForm({ ...form, security: { ...form.security, auditLogRetention: event.target.value } })}>
                    <option>30 Days</option>
                    <option>60 Days</option>
                    <option>90 Days</option>
                    <option>1 Year</option>
                    <option>Indefinite</option>
                  </select>
                </SettingField>
              </div>
              <div className="settings-sso-card">
                <div className="settings-sso-mark"><div className="settings-sso-mark-inner" /></div>
                <div className="settings-sso-copy">
                  <strong>{form.security.ssoProviderName} SSO Connected</strong>
                  <span>{form.security.ssoConnected ? 'Identity provider active' : 'Identity provider disconnected'}</span>
                </div>
                <button className="secondary-button" type="button" onClick={() => handleWorkspaceAction('/workspace/settings/actions/manage-sso')}>Manage SSO</button>
              </div>
            </SettingsPanel>

            <SettingsPanel path="~/nexus/system/integrations">
              {form.integrations.map((integration, index) => (
                <IntegrationRow
                  key={integration.key}
                  name={integration.name}
                  status={integration.status}
                  action={integration.isConnected ? 'Configure' : 'Connect'}
                  connected={integration.isConnected}
                  accent={integration.accent}
                  last={index === form.integrations.length - 1}
                  onAction={() => handleIntegration(integration.key, integration.isConnected ? 'Configure' : 'Connect')}
                />
              ))}
            </SettingsPanel>
          </div>
        </div>
      </div>
    </section>
  )
}

function SettingsPanel({ path, children, danger }: { path: string; children: ReactNode; danger?: boolean }) {
  return (
    <section className={`setting-panel ${danger ? 'danger' : ''}`}>
      <div className={`terminal-header ${danger ? 'danger' : ''}`}>
        <div className="window-dots">
          <span className={`dot ${danger ? 'red' : 'red'}`} />
          <span className={`dot ${danger ? 'amber dimmed' : 'amber'}`} />
          <span className={`dot ${danger ? 'green dimmed' : 'green'}`} />
        </div>
        <h3 className={`terminal-path ${danger ? 'danger' : ''}`}>{path}</h3>
      </div>
      <div className="settings-panel-body">{children}</div>
    </section>
  )
}

function SettingField({ label, children }: { label: string; children: ReactNode }) {
  return (
    <label className="settings-field">
      <span>{label}</span>
      {children}
    </label>
  )
}

function ToggleRow({ title, description, checked, disabled, onToggle }: { title: string; description: string; checked: boolean; disabled?: boolean; onToggle: () => void }) {
  return (
    <div className="settings-toggle-row">
      <div>
        <strong>{title}</strong>
        <p>{description}</p>
      </div>
      <button className={`settings-toggle ${checked ? 'on' : ''} ${disabled ? 'disabled' : ''}`} type="button" onClick={() => !disabled && onToggle()} />
    </div>
  )
}

function DangerRow({ title, description, actionLabel, destructive, onAction }: { title: string; description: string; actionLabel: string; destructive?: boolean; onAction: () => void }) {
  return (
    <div className="settings-danger-row">
      <div>
        <strong className={destructive ? 'destructive' : ''}>{title}</strong>
        <p>{description}</p>
      </div>
      <button className={destructive ? 'danger-button' : 'secondary-button'} type="button" onClick={onAction}>{actionLabel}</button>
    </div>
  )
}

function UsageBar({ label, value, percent }: { label: string; value: string; percent: number }) {
  return (
    <div className="settings-usage">
      <div className="settings-usage-top">
        <span>{label}</span>
        <strong>{value}</strong>
      </div>
      <div className="progress-bar-bg">
        <div className="progress-bar-fill" style={{ width: `${percent}%` }} />
      </div>
    </div>
  )
}

function IntegrationRow({ name, status, action, connected, accent, last, onAction }: { name: string; status: string; action: string; connected?: boolean; accent: 'okta' | 'google' | 'slack' | 'workday' | 'greenhouse' | 'salesforce'; last?: boolean; onAction: () => void }) {
  return (
    <div className={`integration-row ${last ? 'last' : ''}`}>
      <div className="integration-main">
        <div className={`integration-logo ${accent}`}>{name === 'Google Workspace' ? 'G' : name === 'Workday' ? 'W' : name === 'Salesforce' ? '☁' : name === 'Greenhouse' ? '◈' : name === 'Slack Enterprise' ? 'S' : 'O'}</div>
        <div className="integration-copy">
          <strong>{name}</strong>
          <span className={connected ? 'connected' : 'muted'}>{status}</span>
        </div>
      </div>
      <button className={connected ? 'secondary-button integration-action' : 'integration-connect'} type="button" onClick={onAction}>{action}</button>
    </div>
  )
}

function ProjectModal({ form, onChange, onClose, onSubmit, copy }: { form: CreateProjectForm; onChange: (value: CreateProjectForm) => void; onClose: () => void; onSubmit: (event: FormEvent<HTMLFormElement>) => void; copy: Copy }) {
  return <div className="modal-backdrop"><div className="blur-shell" /><div className="create-modal"><div className="create-modal-head"><div><h2>{copy.createNewProject}</h2><p>{copy.realPostRequest}</p></div><button className="close-button" onClick={onClose}>x</button></div><form className="create-form" onSubmit={onSubmit}><div className="form-grid"><label className="field"><span>{copy.projectName}</span><input value={form.title} onChange={(event) => onChange({ ...form, title: event.target.value })} required /></label><label className="field"><span>{copy.projectType}</span><input value={form.type} onChange={(event) => onChange({ ...form, type: event.target.value as CreateProjectForm['type'] })} /></label><label className="field"><span>{copy.description}</span><textarea value={form.description} onChange={(event) => onChange({ ...form, description: event.target.value })} rows={5} /></label><div className="field-row"><label className="field"><span>{copy.deadline}</span><input type="date" value={form.deadline} onChange={(event) => onChange({ ...form, deadline: event.target.value })} /></label><label className="field"><span>{copy.budget}</span><input type="number" value={form.budget} onChange={(event) => onChange({ ...form, budget: event.target.value })} /></label></div></div><div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>{copy.cancel}</button><button type="submit" className="primary-button">{copy.launchProject}</button></div></form></div></div>
}

function UserModal({ form, onChange, onClose, onSubmit, copy }: { form: { fullName: string; email: string; password: string; role: string; skills: string }; onChange: (value: { fullName: string; email: string; password: string; role: string; skills: string }) => void; onClose: () => void; onSubmit: (event: FormEvent<HTMLFormElement>) => void; copy: Copy }) {
  return <div className="modal-backdrop"><div className="blur-shell" /><div className="create-modal"><div className="create-modal-head"><div><h2>{copy.addWorkspaceMember}</h2><p>{copy.realUserCreate}</p></div><button className="close-button" onClick={onClose}>x</button></div><form className="create-form" onSubmit={onSubmit}><div className="form-grid"><label className="field"><span>{copy.fullName}</span><input value={form.fullName} onChange={(event) => onChange({ ...form, fullName: event.target.value })} required /></label><label className="field"><span>{copy.email}</span><input value={form.email} onChange={(event) => onChange({ ...form, email: event.target.value })} required /></label><label className="field"><span>{copy.password}</span><input value={form.password} onChange={(event) => onChange({ ...form, password: event.target.value })} required /></label><label className="field"><span>{copy.role}</span><input value={form.role} onChange={(event) => onChange({ ...form, role: event.target.value })} /></label><label className="field"><span>{copy.skills}</span><textarea value={form.skills} onChange={(event) => onChange({ ...form, skills: event.target.value })} rows={4} placeholder={copy.skillsPlaceholder} /></label></div><div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>{copy.cancel}</button><button type="submit" className="primary-button">{copy.createMember}</button></div></form></div></div>
}

function AccessModal({ users, form, onChange, onClose, onSubmit, copy }: { users: DirectoryUser[]; form: { userId: string; projectRole: string }; onChange: (value: { userId: string; projectRole: string }) => void; onClose: () => void; onSubmit: (event: FormEvent<HTMLFormElement>) => void; copy: Copy }) {
  return <div className="modal-backdrop"><div className="blur-shell" /><div className="create-modal"><div className="create-modal-head"><div><h2>{copy.manageAccessModal}</h2><p>{copy.assignAccess}</p></div><button className="close-button" onClick={onClose}>x</button></div><form className="create-form" onSubmit={onSubmit}><div className="form-grid"><label className="field"><span>{copy.user}</span><select value={form.userId} onChange={(event) => onChange({ ...form, userId: event.target.value })}>{users.length === 0 ? <option value="">{copy.noUsers}</option> : users.map((user) => <option key={user.id} value={user.id}>{user.fullName}</option>)}</select></label><label className="field"><span>{copy.projectRole}</span><input value={form.projectRole} onChange={(event) => onChange({ ...form, projectRole: event.target.value })} /></label></div><div className="modal-actions"><button type="button" className="secondary-button" onClick={onClose}>{copy.cancel}</button><button type="submit" className="primary-button">{copy.addToProject}</button></div></form></div></div>
}

export default App
