<template>
  <div class="main-layout">
    <header class="header">
      <div class="header-inner">
        <h1 class="title">🏠 Remontio</h1>
        <div class="user-info">
          <button @click="showAlertModal = true" class="btn-icon-alert" title="Powiadomienia">
            <font-awesome-icon icon="bell" />
            <span v-if="alertStore.alertCount > 0" class="alert-badge">{{
              alertStore.alertCount
            }}</span>
          </button>
          <span class="user-name">{{ userData.name }} {{ userData.surname }}</span>
          <button type="submit" @click="authStore.logout" class="btn btn-secondary btn-compact">
            Logout
          </button>
        </div>
      </div>
    </header>

    <!-- Alert Modal -->
    <div v-if="showAlertModal" class="modal-overlay" @click.self="showAlertModal = false">
      <div class="alert-modal">
        <div class="alert-modal-header">
          <h3>
            <font-awesome-icon icon="bell" />
            Powiadomienia
          </h3>
          <button @click="showAlertModal = false" class="modal-close-btn">
            <font-awesome-icon icon="times" />
          </button>
        </div>
        <div class="alert-modal-body">
          <div v-if="alertStore.alertCount === 0" class="no-alerts">
            <font-awesome-icon icon="bell" size="2x" />
            <p>Brak aktywnych powiadomień</p>
          </div>
          <div v-else class="alerts-scroll">
            <div
              v-for="alert in alertStore.activeAlerts"
              :key="alert.id"
              :class="['modal-alert-item', `alert-${alert.type}`]"
            >
              <div class="modal-alert-icon">
                <font-awesome-icon :icon="getAlertIcon(alert.type)" />
              </div>
              <div class="modal-alert-content">
                <div class="modal-alert-message">{{ alert.message }}</div>
                <div class="modal-alert-time">{{ formatRelativeTime(alert.timestamp) }}</div>
              </div>
              <button @click="removeAlert(alert.id)" class="modal-alert-close">
                <font-awesome-icon icon="times" />
              </button>
            </div>
          </div>
        </div>
        <div class="alert-modal-footer">
          <button
            @click="clearAllAlerts"
            class="btn btn-secondary btn-sm"
            :disabled="alertStore.alertCount === 0"
          >
            <font-awesome-icon icon="trash-can" />
            Wyczyść wszystkie
          </button>
          <button @click="goToAlertsAndCloseModal" class="btn btn-primary btn-sm">
            <font-awesome-icon icon="bell" />
            Zarządzaj alertami
          </button>
        </div>
      </div>
    </div>
    <div class="container">
      <div v-if="showMenu" class="nav-column">
        <button class="menu-toggle-btn" @click="showMenu = !showMenu">
          {{ showMenu ? '⟨ Ukryj menu' : '☰ Pokaż menu' }}
        </button>
        <nav class="nav-menu">
          <ul class="nav-list">
            <li>
              <a @click="goToProjects()" class="cst-btn">
                <span class="nav-icon">📋</span>
                <span class="nav-text">Projekty</span>
              </a>
            </li>
            <li>
              <a @click="goToRooms()" class="cst-btn">
                <span class="nav-icon">📋</span>
                <span class="nav-text">Pokoje</span>
              </a>
            </li>
            <li>
              <a @click="goToPlanning()" class="cst-btn">
                <span class="nav-icon">✅</span>
                <span class="nav-text">Zadania</span>
              </a>
            </li>
            <li>
              <a @click="goToBudgets()" class="cst-btn">
                <span class="nav-icon">💰</span>
                <span class="nav-text">Budżet</span>
              </a>
            </li>
            <li>
              <a @click="goToCalculators()" class="cst-btn">
                <span class="nav-icon">🧮</span>
                <span class="nav-text">Kalkulatory</span>
              </a>
            </li>
            <li>
              <a @click="goToGallery()" class="cst-btn">
                <span class="nav-icon">📸</span>
                <span class="nav-text">Galeria</span>
              </a>
            </li>
            <li>
              <a @click="goToContacts()" class="cst-btn">
                <span class="nav-icon">📸</span>
                <span class="nav-text">Kontakty</span>
              </a>
            </li>
            <li>
              <a @click="goToAlerts()" class="cst-btn">
                <span class="nav-icon">📸</span>
                <span class="nav-text">Powiadomienia</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">⚙️</span>
                <span class="nav-text">Ustawienia</span>
              </a>
            </li>
          </ul>
        </nav>
      </div>
      <button
        v-else
        class="menu-toggle-btn-floating"
        @click="showMenu = !showMenu"
        title="Pokaż menu"
      >
        ☰
      </button>
      <main class="content">
        <slot></slot>
      </main>
    </div>
    <footer class="footer">
      <p>&copy; {{ year }} Remontio. All rights reserved.</p>
    </footer>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getUserData } from '@/helpers/userHelpers'
import { useAuthStore } from '@/stores/authStore'
import { useAlertStore } from '@/stores/alertStore'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { formatRelativeTime } from '@/helpers/dateFormatter'

const router = useRouter()
const authStore = useAuthStore()
const alertStore = useAlertStore()

const year = new Date().getFullYear()

const showMenu = ref(true)
const showAlertModal = ref(false)
const userData = getUserData()

// Initialize alerts
onMounted(async () => {
  await alertStore.fetchCustomAlerts()
  alertStore.startAlertMonitoring()
})

onUnmounted(() => {
  alertStore.stopAlertMonitoring()
})

// Alert helper functions
function getAlertIcon(type: string): string {
  const icons: Record<string, string> = {
    success: 'check-circle',
    error: 'times-circle',
    warning: 'exclamation-triangle',
    info: 'info-circle',
  }
  return icons[type] || 'info-circle'
}

function removeAlert(id: string) {
  alertStore.removeAlert(id)
}

function clearAllAlerts() {
  if (confirm('Czy na pewno chcesz wyczyścić wszystkie powiadomienia?')) {
    alertStore.clearAll()
  }
}

////////ROUTING////////

const goToProjects = () => {
  router.push({ name: 'ProjectList' })
}
const goToRooms = () => {
  router.push({ name: 'RoomList' })
}
const goToPlanning = () => {
  router.push({ name: 'PlanningView' })
}

const goToBudgets = () => {
  router.push({ name: 'BudgetList' })
}
const goToContacts = () => {
  router.push({ name: 'ContactList' })
}
const goToCalculators = () => {
  router.push({ name: 'CalculatorList' })
}
const goToGallery = () => {
  router.push({ name: 'GalleryView' })
}
const goToAlerts = () => {
  router.push({ name: 'AlertPanel' })
}

const goToAlertsAndCloseModal = () => {
  showAlertModal.value = false
  goToAlerts()
}
//////////////////////
</script>

<style scoped lang="css">
.main-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  width: 100%;
  background: var(--gradient-primary);
  color: var(--color-text-white);
  padding: 20px 0;
  box-shadow: 0 2px 8px var(--shadow-light);
}

.header-inner {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
}

.title {
  max-width: 1200px;
  margin: 0;
  padding: 0 20px;
  font-size: 2rem;
  font-weight: 700;
}

/* User info (right side) */
.user-info {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-left: auto;
  padding: 8px 12px;
  border: 1px solid rgba(255, 255, 255, 0.28);
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.06);
  box-shadow: 0 2px 6px var(--shadow-light);
}

.user-name {
  color: var(--color-text-white);
  font-weight: 600;
  font-size: 0.95rem;
}

/* Compact button size that reuses global .btn styles */
.btn-compact {
  padding: 8px 14px;
  font-size: 0.9rem;
}

.container {
  max-width: 100%;
  width: 100%;
  margin: 0;
  padding: 0 20px;
  flex: 1;
  display: flex;
  flex-direction: row;
  gap: 30px;
  position: relative;
}

.nav-column {
  width: 250px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.menu-toggle-btn {
  width: 100%;
  padding: 10px 16px;
  margin-top: 10px;
  font-size: 0.9rem;
  border-radius: 8px;
  background: var(--color-bg-white);
  color: var(--color-text-dark);
  border: 1px solid rgba(0, 0, 0, 0.1);
  box-shadow: 0 2px 6px var(--shadow-light);
  opacity: 0.85;
  cursor: pointer;
  transition:
    opacity 0.2s ease,
    box-shadow 0.2s ease;
  font-weight: 500;
}

.menu-toggle-btn:hover {
  opacity: 1;
  box-shadow: 0 4px 8px var(--shadow-medium);
}

.menu-toggle-btn-floating {
  position: absolute;
  top: 10px;
  left: 10px;
  width: 36px;
  height: 36px;
  padding: 0;
  font-size: 1.2rem;
  border-radius: 50%;
  background: var(--color-bg-white);
  color: var(--color-text-dark);
  border: 1px solid rgba(0, 0, 0, 0.1);
  box-shadow: 0 2px 6px var(--shadow-light);
  opacity: 0.6;
  cursor: pointer;
  transition:
    opacity 0.2s ease,
    box-shadow 0.2s ease,
    transform 0.2s ease;
  font-weight: 500;
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
}

.menu-toggle-btn-floating:hover {
  opacity: 1;
  box-shadow: 0 4px 10px var(--shadow-medium);
  transform: scale(1.1);
}

.nav-menu {
  width: 100%;
  padding: 30px 20px;
  flex-shrink: 0;
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  max-height: calc(100vh - 180px);
  overflow-y: auto;
}

.nav-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.content {
  flex: 1;
  padding: 30px 0 30px 40px;
  min-width: 0;
  display: flex;
  flex-direction: column;
  position: relative;
}

.footer {
  width: 100%;
  padding: 20px;
  text-align: center;
  border-top: 1px solid #e0e0e0;
  color: var(--color-text-medium);
  font-size: 0.9rem;
  background: var(--color-bg-white);
  margin-top: auto;
}

/* Responsive */
@media (max-width: 768px) {
  .container {
    flex-direction: column;
    gap: 0;
  }

  .nav-column {
    width: 100%;
  }

  .menu-toggle-btn {
    margin-bottom: 10px;
  }

  .nav-menu {
    width: 100%;
    padding: 20px;
    height: 125px;
    position: static;
    max-height: none;
    border-radius: 0;
    box-shadow: 0 2px 8px var(--shadow-light);
  }

  .nav-list {
    flex-direction: row;
    overflow-x: auto;
    gap: 4px;
  }

  .content {
    padding: 20px 0;
  }

  .user-info {
    gap: 8px;
    padding: 6px 10px;
  }

  .user-name {
    font-size: 0.85rem;
  }
}

/* Alert Button & Modal */
.btn-icon-alert {
  position: relative;
  background: rgba(255, 255, 255, 0.15);
  border: 1px solid rgba(255, 255, 255, 0.3);
  color: var(--color-text-white);
  padding: 8px 12px;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1.1rem;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.btn-icon-alert:hover {
  background: rgba(255, 255, 255, 0.25);
  transform: scale(1.05);
}

.alert-badge {
  position: absolute;
  top: -6px;
  right: -6px;
  background: var(--color-red);
  color: white;
  font-size: 0.7rem;
  font-weight: 700;
  padding: 2px 6px;
  border-radius: 10px;
  min-width: 18px;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: flex-start;
  justify-content: flex-end;
  z-index: 1000;
  animation: fadeIn 0.2s ease;
  padding: 20px;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

.alert-modal {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 8px 32px var(--shadow-dark);
  width: 100%;
  max-width: 480px;
  max-height: calc(100vh - 40px);
  display: flex;
  flex-direction: column;
  animation: slideInRight 0.3s ease;
}

@keyframes slideInRight {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

.alert-modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 2px solid var(--color-bg-light-gray);
}

.alert-modal-header h3 {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text-dark);
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.modal-close-btn {
  background: transparent;
  border: none;
  font-size: 1.5rem;
  color: var(--color-text-medium);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 50%;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close-btn:hover {
  background: var(--color-bg-light-gray);
  color: var(--color-text-dark);
}

.alert-modal-body {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
  min-height: 200px;
  max-height: calc(100vh - 220px);
}

.no-alerts {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 3rem 1rem;
  text-align: center;
  color: var(--color-text-medium);
}

.no-alerts p {
  margin: 0;
  font-size: 1rem;
}

.alerts-scroll {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.modal-alert-item {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  padding: 1rem;
  border-radius: 12px;
  background: var(--color-bg-white);
  border-left: 4px solid;
  box-shadow: 0 2px 8px var(--shadow-light);
  transition: all 0.2s ease;
}

.modal-alert-item:hover {
  box-shadow: 0 4px 12px var(--shadow-dark);
}

.modal-alert-item.alert-success {
  border-left-color: var(--color-green);
  background: linear-gradient(to right, rgba(76, 175, 80, 0.05), var(--color-bg-white));
}

.modal-alert-item.alert-error {
  border-left-color: var(--color-red);
  background: linear-gradient(to right, rgba(244, 67, 54, 0.05), var(--color-bg-white));
}

.modal-alert-item.alert-warning {
  border-left-color: #ff9800;
  background: linear-gradient(to right, rgba(255, 152, 0, 0.05), var(--color-bg-white));
}

.modal-alert-item.alert-info {
  border-left-color: var(--color-primary-blue);
  background: linear-gradient(to right, rgba(52, 152, 219, 0.05), var(--color-bg-white));
}

.modal-alert-icon {
  font-size: 1.25rem;
  min-width: 24px;
  text-align: center;
}

.modal-alert-item.alert-success .modal-alert-icon {
  color: var(--color-green);
}

.modal-alert-item.alert-error .modal-alert-icon {
  color: var(--color-red);
}

.modal-alert-item.alert-warning .modal-alert-icon {
  color: #ff9800;
}

.modal-alert-item.alert-info .modal-alert-icon {
  color: var(--color-primary-blue);
}

.modal-alert-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.modal-alert-message {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-dark);
  line-height: 1.4;
}

.modal-alert-time {
  font-size: 0.8rem;
  color: var(--color-text-medium);
}

.modal-alert-close {
  background: transparent;
  border: none;
  font-size: 1rem;
  color: var(--color-text-medium);
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 50%;
  transition: all 0.2s ease;
  min-width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-alert-close:hover {
  background: rgba(0, 0, 0, 0.05);
  color: var(--color-text-dark);
}

.alert-modal-footer {
  display: flex;
  gap: 0.75rem;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-top: 2px solid var(--color-bg-light-gray);
}

@media (max-width: 768px) {
  .modal-overlay {
    padding: 0;
    align-items: stretch;
  }

  .alert-modal {
    max-width: 100%;
    border-radius: 0;
    max-height: 100vh;
  }

  .alert-modal-footer {
    flex-direction: column;
  }

  .alert-modal-footer button {
    width: 100%;
  }
}
</style>
