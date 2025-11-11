import { defineStore } from 'pinia'
import { ref } from 'vue'
import { RemontioBackend } from '@/backend/RemontioBackend'
import type { UserDataDTO } from '@/backend/BackendBase'
import router from '@/router'

const backend = new RemontioBackend('https://localhost:7259')

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserDataDTO | null>(null)
  const error = ref<string | null>(null)
  const isLoading = ref(false)
  const sessionExpiredMessage = ref<string | null>(null)

  // Restore user from localStorage on store initialization
  function restoreUser() {
    const storedUser = localStorage.getItem('remontio_user_data')
    if (storedUser && backend.isAuthenticated()) {
      try {
        user.value = JSON.parse(storedUser)
      } catch (err) {
        console.error('Failed to restore user:', err)
      }
    } else if (storedUser && !backend.isAuthenticated()) {
      handleSessionExpired()
    }
  }

  function handleSessionExpired() {
    sessionExpiredMessage.value = 'Sesja wygasła'
    logout()
    router.push({ name: 'LoginPage' })
  }

  function checkAndHandleExpiredSession() {
    if (!backend.isAuthenticated()) {
      handleSessionExpired()
      return false
    }
    return true
  }

  async function login(username: string, password: string): Promise<boolean> {
    isLoading.value = true
    error.value = null

    try {
      const response = await backend.logIn(username, password)

      if (response) {
        const data: any = response
        const userData = data.result || response
        user.value = userData

        if (data.token && data.refreshToken) {
          backend.setAuthTokens(data.token, data.refreshToken, userData)
        } else {
          console.error('Tokens not found in response! Response structure:', data)
        }
        return true
      }

      error.value = 'Login failed'
      return false
    } catch (err: any) {
      console.error('Login error:', err)
      error.value = 'Invalid credentials'
      return false
    } finally {
      isLoading.value = false
    }
  }

  function logout() {
    user.value = null
    backend.clearAuth()
  }

  function clearError() {
    error.value = null
  }

  function clearSessionExpiredMessage() {
    sessionExpiredMessage.value = null
  }

  restoreUser()

  return {
    user,
    error,
    isLoading,
    sessionExpiredMessage,
    login,
    logout,
    clearError,
    clearSessionExpiredMessage,
    handleSessionExpired,
    checkAndHandleExpiredSession,
  }
})
