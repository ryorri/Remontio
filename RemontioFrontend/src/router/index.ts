import AboutPage from '@/views/AboutPage.vue'
import HomePage from '@/views/HomePage.vue'
import LoginPage from '@/views/LoginPage.vue'
import DashboardPage from '@/views/DashboardPage.vue'
import { createRouter, createWebHistory } from 'vue-router'
import { Backend } from '@/main'

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
  ],
})

// Navigation guard to check authentication
router.beforeEach((to, from, next) => {
  if (to.meta.requiresAuth) {
    // Import authStore dynamically to avoid circular dependency
    import('@/stores/authStore').then(({ useAuthStore }) => {
      const authStore = useAuthStore()
      if (!authStore.checkAndHandleExpiredSession()) {
        next({ name: 'LoginPage' })
      } else {
        next()
      }
    })
  } else {
    next()
  }
})

export default router
