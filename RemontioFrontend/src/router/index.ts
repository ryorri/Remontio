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
