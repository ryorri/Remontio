import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AlertDataDTO, CreateAlertDTO } from '@/backend/BackendBase'
import { StatusEnum } from '@/backend/BackendBase'
import { RemontioBackend } from '@/backend/RemontioBackend'

const backend = new RemontioBackend('https://localhost:7259')

export interface Alert {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  message: string
  timestamp: Date
  autoClose?: boolean
  duration?: number
}

export interface CustomAlert extends AlertDataDTO {
  triggered?: boolean
}

export const useAlertStore = defineStore('alert', () => {
  const alerts = ref<Alert[]>([])
  const customAlerts = ref<CustomAlert[]>([])
  const maxAlerts = 5
  let checkInterval: number | null = null

  const activeAlerts = computed(() => alerts.value)
  const alertCount = computed(() => alerts.value.length)

  function addAlert(
    type: Alert['type'],
    message: string,
    autoClose = true,
    duration = 5000,
  ): string {
    const id = `alert-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`

    const alert: Alert = {
      id,
      type,
      message,
      timestamp: new Date(),
      autoClose,
      duration,
    }

    alerts.value.unshift(alert)

    // Limit max alerts
    if (alerts.value.length > maxAlerts) {
      alerts.value = alerts.value.slice(0, maxAlerts)
    }

    // Auto remove if configured
    if (autoClose) {
      setTimeout(() => {
        removeAlert(id)
      }, duration)
    }

    return id
  }

  function removeAlert(id: string) {
    const index = alerts.value.findIndex((alert) => alert.id === id)
    if (index !== -1) {
      alerts.value.splice(index, 1)
    }
  }

  function clearAll() {
    alerts.value = []
  }

  // Helper methods for specific alert types
  function success(message: string, autoClose = true, duration = 5000) {
    return addAlert('success', message, autoClose, duration)
  }

  function error(message: string, autoClose = true, duration = 7000) {
    return addAlert('error', message, autoClose, duration)
  }

  function warning(message: string, autoClose = true, duration = 6000) {
    return addAlert('warning', message, autoClose, duration)
  }

  function info(message: string, autoClose = true, duration = 5000) {
    return addAlert('info', message, autoClose, duration)
  }

  // Custom Alerts (backend)
  async function fetchCustomAlerts() {
    try {
      const userData = localStorage.getItem('remontio_user_data')
      if (!userData) return

      const user = JSON.parse(userData)
      const userId = user.id

      const alertsFromBackend = await backend.getAlertListByUserId(userId)
      customAlerts.value = alertsFromBackend.map((alert) => ({
        ...alert,
        triggered: false,
      }))
    } catch (error) {
      console.error('Failed to fetch custom alerts:', error)
    }
  }

  async function createCustomAlert(
    message: string,
    endDate: Date,
    priority: string = 'Medium',
    status: StatusEnum = StatusEnum._1,
  ) {
    try {
      const userData = localStorage.getItem('remontio_user_data')
      if (!userData) {
        error('Musisz być zalogowany aby utworzyć alert')
        return null
      }

      const user = JSON.parse(userData)
      const userId = user.id

      const createDto: CreateAlertDTO = {
        message,
        startDate: new Date(),
        endDate,
        userId,
        priority,
        status: status.toString(),
      }

      const created = await backend.createAlert(createDto)
      if (created) {
        await fetchCustomAlerts()
        success('Alert został utworzony pomyślnie')
        return true
      }
      return false
    } catch (err) {
      console.error('Failed to create custom alert:', err)
      error('Nie udało się utworzyć alertu')
      return false
    }
  }

  async function deleteCustomAlert(alertId: string) {
    try {
      const deleted = await backend.deleteAlert(alertId)
      if (deleted) {
        customAlerts.value = customAlerts.value.filter((a) => a.id !== alertId)
        success('Alert został usunięty')
      }
    } catch (err) {
      console.error('Failed to delete alert:', err)
      error('Nie udało się usunąć alertu')
    }
  }

  function checkAlertTriggers() {
    const now = new Date()

    customAlerts.value.forEach((alert) => {
      if (alert.triggered || !alert.endDate) return

      const endDate = new Date(alert.endDate)
      // Check if current time matches endDate (with minute precision)
      const nowMinutes =
        now.getFullYear() * 525600 +
        now.getMonth() * 43800 +
        now.getDate() * 1440 +
        now.getHours() * 60 +
        now.getMinutes()
      const endMinutes =
        endDate.getFullYear() * 525600 +
        endDate.getMonth() * 43800 +
        endDate.getDate() * 1440 +
        endDate.getHours() * 60 +
        endDate.getMinutes()

      if (nowMinutes >= endMinutes) {
        console.log('🔔 ALERT TRIGGERED:', {
          id: alert.id,
          message: alert.message,
          endDate: alert.endDate,
          priority: alert.priority,
          status: alert.status,
        })

        // Mark as triggered
        alert.triggered = true

        // Show UI notification
        const alertType =
          alert.priority === 'High' ? 'error' : alert.priority === 'Medium' ? 'warning' : 'info'
        addAlert(alertType as any, alert.message || 'Alert', true, 8000)
      }
    })
  }

  function startAlertMonitoring() {
    if (checkInterval) return

    // Check every minute
    checkInterval = window.setInterval(() => {
      checkAlertTriggers()
    }, 60000)

    // Also check immediately
    checkAlertTriggers()
  }

  function stopAlertMonitoring() {
    if (checkInterval) {
      clearInterval(checkInterval)
      checkInterval = null
    }
  }

  return {
    alerts,
    customAlerts,
    activeAlerts,
    alertCount,
    addAlert,
    removeAlert,
    clearAll,
    success,
    error,
    warning,
    info,
    fetchCustomAlerts,
    createCustomAlert,
    deleteCustomAlert,
    startAlertMonitoring,
    stopAlertMonitoring,
    checkAlertTriggers,
  }
})
