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
    ////////////////////PROJECT PAGES ROUTES HERE////////////////////
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
