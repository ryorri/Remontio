import AboutPage from '@/views/AboutPage.vue'
import HomePage from '@/views/HomePage.vue'
import LoginPage from '@/views/LoginPage.vue'
import DashboardPage from '@/views/DashboardPage.vue'
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'HomePage',
      component: HomePage,
    },
    {
      path: '/login',
      name: 'LoginPage',
      component: LoginPage,
    },
    {
      path: '/about',
      name: 'AboutPage',
      component: AboutPage,
    },
    {
      path: '/dashboard',
      name: 'DashboardPage',
      component: DashboardPage,
      meta: { requiresAuth: true },
    },
    ////////////////////PROJECT PAGES ROUTES////////////////////
    {
      path: '/project-list',
      name: 'ProjectList',
      component: () => import('@/views/components/projects/ProjectList.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/project/:projectId',
      name: 'ProjectDetails',
      component: () => import('@/views/components/projects/ProjectDetails.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/project/:projectId/edit',
      name: 'ProjectEdit',
      component: () => import('@/views/components/projects/ProjectEdit_v2.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/project/:projectId/change-status',
      name: 'ProjectChangeStatus',
      component: () => import('@/views/components/projects/ProjectChangeStatus.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/project/:projectId/delete',
      name: 'ProjectDelete',
      component: () => import('@/views/components/projects/ProjectDelete.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/project/create',
      name: 'ProjectCreate',
      component: () => import('@/views/components/projects/ProjectCreate.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////ROOM PAGES ROUTES/////////////////////////
    {
      path: '/room/:roomId/change-status-and-priority',
      name: 'RoomChangeStatusAndPriority',
      component: () => import('@/views/components/rooms/RoomChangeStatusAndPriority_v2.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room/create',
      name: 'RoomCreate',
      component: () => import('@/views/components/rooms/RoomCreate.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room/create/:projectId',
      name: 'RoomCreate',
      component: () => import('@/views/components/rooms/RoomCreate.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room/:roomId',
      name: 'RoomDetails',
      component: () => import('@/views/components/rooms/RoomDetails.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room/:roomId/edit',
      name: 'RoomEdit',
      component: () => import('@/views/components/rooms/RoomEdit.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room/:roomId/delete',
      name: 'RoomDelete',
      component: () => import('@/views/components/rooms/RoomDelete.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room-list/',
      name: 'RoomList',
      component: () => import('@/views/components/rooms/RoomList.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/room-list/:projectId?',
      name: 'RoomList',
      component: () => import('@/views/components/rooms/RoomList.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////PLANNING PAGES ROUTES////////////////////////////
    {
      path: '/planning',
      name: 'PlanningView',
      component: () => import('@/views/components/planningTasks/PlaningView.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////BUDGET AND SHOPPING PAGES ROUTES////////////////////////////
    {
      path: '/budget-list',
      name: 'BudgetList',
      component: () => import('@/views/components/budget/BudgetList.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/budget-create',
      name: 'BudgetCreate',
      component: () => import('@/views/components/budget/BudgetCreate.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/budget/:budgetId',
      name: 'BudgetDetails',
      component: () => import('@/views/components/budget/BudgetDetailsAndEdit.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/budget/:budgetId/edit',
      name: 'BudgetEdit',
      component: () => import('@/views/components/budget/BudgetEdit.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/budget/:budgetId/shopping-lists',
      name: 'ShoppingListsByBudget',
      component: () => import('@/views/components/shoppinglists/ShoppinglistList.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/budget/:budgetId/shopping-list/create',
      name: 'ShoppingListCreate',
      component: () => import('@/views/components/shoppinglists/ShoppingListCreateAndEdit.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/budget/:budgetId/shopping-list/:listId/edit',
      name: 'ShoppingListEdit',
      component: () => import('@/views/components/shoppinglists/ShoppingListCreateAndEdit.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////CONTACTS PAGES ROUTES////////////////////////////
    {
      path: '/contacts',
      name: 'ContactList',
      component: () => import('@/views/components/contacts/ContactList.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/contacts-create',
      name: 'ContactCreate',
      component: () => import('@/views/components/contacts/ContactCreate.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/contacts-edit/:contactId',
      name: 'ContactEdit',
      component: () => import('@/views/components/contacts/ContactEdit.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/contacts-details/:contactId',
      name: 'ContactDetails',
      component: () => import('@/views/components/contacts/ContactDetails.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////CONTACTS PAGES ROUTES////////////////////////////
    {
      path: '/calculations',
      name: 'CalculatorList',
      component: () => import('@/views/components/calculators/CalculatorList.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/calculations-create',
      name: 'CalculatorCreate',
      component: () => import('@/views/components/calculators/CalculatorCreate.vue'),
      meta: { requiresAuth: true },
    },
    {
      path: '/calculations-edit/:calculationId',
      name: 'CalculatorEdit',
      component: () => import('@/views/components/calculators/CalculatorEdit.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////GALLERY PAGES ROUTES////////////////////////////
    {
      path: '/gallery',
      name: 'GalleryView',
      component: () => import('@/views/components/photosGallery/PhotoGalleryView.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////ALERTS PAGES ROUTES////////////////////////////
    {
      path: '/alerts',
      name: 'AlertPanel',
      component: () => import('@/views/components/alerts/AlertPanel.vue'),
      meta: { requiresAuth: true },
    },
    ////////////////////Settings PAGES ROUTES////////////////////////////
    {
      path: '/settings',
      name: 'SettingsPanel',
      component: () => import('@/views/components/settings/SettingsPanel.vue'),
      meta: { requiresAuth: true },
    },
    /////////////////////////////////////////////////////////////////
  ],
})

// Navigation guard to check authentication
router.beforeEach(async (to, from, next) => {
  const { useAuthStore } = await import('@/stores/authStore')
  const auth = useAuthStore()

  if (to.meta.requiresAuth && !auth.checkAndHandleExpiredSession()) {
    return next({ name: 'LoginPage' })
  }

  next()
})
export default router
