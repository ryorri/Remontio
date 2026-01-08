<template>
  <MainLayout>
    <template #header>
      <h1 class="contacts-title">Panel powiadomień</h1>
    </template>

    <div class="contacts-container">
      <!-- Alert List -->
      <div class="card-section">
        <div class="section-header">
          <font-awesome-icon icon="bell" />
          <h3>Lista Alertów</h3>
          <button
            v-if="alertStore.alertCount > 0"
            @click="clearAllAlerts"
            class="btn btn-secondary btn-sm"
          >
            <font-awesome-icon icon="trash-can" />
            Wyczyść wszystkie
          </button>
        </div>

        <!-- Empty State -->
        <div v-if="alertStore.alertCount === 0" class="empty-state">
          <font-awesome-icon icon="bell" size="4x" />
          <h2>Brak aktywnych alertów</h2>
        </div>

        <!-- Alerts List -->
        <div v-else class="alerts-list">
          <transition-group name="alert-fade">
            <div
              v-for="alert in alertStore.activeAlerts"
              :key="alert.id"
              :class="['alert-item', `alert-${alert.type}`]"
            >
              <div class="alert-icon">
                <font-awesome-icon :icon="getAlertIcon(alert.type)" />
              </div>
              <div class="alert-content">
                <div class="alert-message">{{ alert.message }}</div>
                <div class="alert-timestamp">{{ formatRelativeTime(alert.timestamp) }}</div>
              </div>
              <button @click="removeAlert(alert.id)" class="alert-close">
                <font-awesome-icon icon="times" />
              </button>
            </div>
          </transition-group>
        </div>
      </div>

      <!-- Create Custom Alert -->
      <div class="card-section mt-3">
        <div class="section-header">
          <font-awesome-icon icon="plus" />
          <h3>Utwórz Nowy Alert</h3>
        </div>

        <div class="alert-form">
          <div class="form-group">
            <label class="form-label">Wiadomość alertu</label>
            <textarea
              v-model="newAlert.message"
              class="form-control"
              rows="3"
              placeholder="Wpisz treść alertu..."
            ></textarea>
          </div>

          <div class="form-group">
            <label class="form-label">Data i godzina końca (dokładność co do minuty)</label>
            <input v-model="newAlert.endDate" type="datetime-local" class="form-control" />
          </div>

          <div class="form-group">
            <label class="form-label">Priorytet</label>
            <select v-model="newAlert.priority" class="form-select">
              <option value="Low">Niski</option>
              <option value="Medium">Średni</option>
              <option value="High">Wysoki</option>
            </select>
          </div>

          <div class="alert alert-danger" v-if="formError">
            <font-awesome-icon icon="exclamation-triangle" />
            {{ formError }}
          </div>

          <div class="d-flex gap-2">
            <button @click="createAlert" class="btn btn-primary" :disabled="isCreating">
              <span v-if="isCreating" class="spinner-small"></span>
              <font-awesome-icon v-else icon="save" />
              Utwórz Alert
            </button>
            <button @click="resetForm" class="btn btn-secondary">
              <font-awesome-icon icon="undo" />
              Wyczyść
            </button>
          </div>
        </div>
      </div>

      <!-- Custom Alerts List -->
      <div class="card-section mt-3">
        <div class="section-header">
          <font-awesome-icon icon="bell" />
          <h3>Zaplanowane Alerty</h3>
          <span class="badge">{{ alertStore.customAlerts.length }} zaplanowanych</span>
        </div>

        <div v-if="alertStore.customAlerts.length === 0" class="empty-state">
          <font-awesome-icon icon="bell" size="3x" />
          <p>Brak zaplanowanych alertów</p>
        </div>

        <div v-else class="custom-alerts-list">
          <div
            v-for="alert in alertStore.customAlerts"
            :key="alert.id"
            :class="['custom-alert-card', alert.triggered ? 'triggered' : '']"
            @click="markAlertAsDone(alert.id!)"
            style="cursor: pointer"
          >
            <div class="custom-alert-header">
              <font-awesome-icon
                :icon="alert.priority === 'High' ? 'exclamation-triangle' : 'info-circle'"
                :class="['priority-icon', `priority-${alert.priority?.toLowerCase()}`]"
              />
              <span class="alert-priority-badge">{{ alert.priority }}</span>
            </div>
            <div class="custom-alert-body">
              <p class="custom-alert-message">{{ alert.message }}</p>
              <div class="custom-alert-meta">
                <div class="meta-row">
                  <span class="meta-label">Data końcowa:</span>
                  <span class="meta-value">{{ formatDateTime(alert.endDate) }}</span>
                </div>
                <div class="meta-row">
                  <span class="meta-label">Status:</span>
                  <span :class="['status-badge', alert.triggered ? 'triggered' : 'active']">
                    {{ alert.triggered ? 'Wywołany' : getStatusLabel(parseStatus(alert.status)) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { computed, ref, onMounted, onUnmounted } from 'vue'
import MainLayout from '@/views/layouts/MainLayout.vue'
import { useAlertStore } from '@/stores/alertStore'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { StatusEnum } from '@/backend/BackendBase'
import { getStatusLabel, getStatusToAlerts } from '@/helpers/statusEnumFormatter'
import { formatDateTime, formatRelativeTime } from '@/helpers/dateFormatter'

const alertStore = useAlertStore()

// Form state
const newAlert = ref({
  message: '',
  endDate: '',
  priority: 'Medium',
  status: StatusEnum._1,
})

const statusOptions = getStatusToAlerts()

const isCreating = ref(false)
const formError = ref('')

// Lifecycle
onMounted(async () => {
  await alertStore.fetchCustomAlerts()
  alertStore.startAlertMonitoring()
})

onUnmounted(() => {
  alertStore.stopAlertMonitoring()
})

// Helper functions
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
  if (confirm('Czy na pewno chcesz wyczyścić wszystkie alerty?')) {
    alertStore.clearAll()
  }
}

async function createAlert() {
  formError.value = ''

  if (!newAlert.value.message.trim()) {
    formError.value = 'Wiadomość nie może być pusta'
    return
  }

  if (!newAlert.value.endDate) {
    formError.value = 'Musisz wybrać datę końca'
    return
  }

  const endDate = new Date(newAlert.value.endDate)
  if (endDate <= new Date()) {
    formError.value = 'Data końca musi być w przyszłości'
    return
  }

  isCreating.value = true

  try {
    await alertStore.createCustomAlert(newAlert.value.message, endDate, newAlert.value.priority)
    resetForm()
  } catch (err) {
    formError.value = 'Wystąpił błąd podczas tworzenia alertu'
  } finally {
    isCreating.value = false
  }
}

function resetForm() {
  newAlert.value = {
    message: '',
    endDate: '',
    priority: 'Medium',
    status: StatusEnum._1,
  }
  formError.value = ''
}

function parseStatus(status: string | undefined): StatusEnum {
  if (!status) return StatusEnum._1
  const numStatus = parseInt(status)
  return isNaN(numStatus) ? StatusEnum._1 : (numStatus as StatusEnum)
}

function markAlertAsDone(alertId: string) {
  alertStore.deleteCustomAlert(alertId)
}
</script>

<style scoped>
/* Custom Alerts */
.custom-alerts-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.custom-alert-card {
  background: var(--color-bg-white);
  border-radius: 12px;
  border: 2px solid var(--color-bg-light-gray);
  box-shadow: 0 2px 8px var(--shadow-light);
  overflow: hidden;
  transition: all 0.3s ease;
  position: relative;
}

.custom-alert-card::after {
  content: '✓ Kliknij aby oznaczyć jako wykonane';
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  background: rgba(76, 175, 80, 0.95);
  color: white;
  padding: 1rem 1.5rem;
  border-radius: 8px;
  font-weight: 600;
  font-size: 0.95rem;
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.3s ease;
  white-space: nowrap;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.custom-alert-card:hover::after {
  opacity: 1;
}

.custom-alert-card:hover {
  box-shadow: 0 4px 16px var(--shadow-dark);
  border-color: var(--color-green);
  transform: scale(1.02);
}

.custom-alert-card.triggered {
  opacity: 0.7;
  border-color: var(--color-text-medium);
}

.custom-alert-header {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 1rem;
  background: var(--color-bg-light-gray);
  border-bottom: 1px solid #ddd;
}

.priority-icon {
  font-size: 1.25rem;
}

.priority-icon.priority-high {
  color: var(--color-red);
}

.priority-icon.priority-medium {
  color: #ff9800;
}

.priority-icon.priority-low {
  color: var(--color-primary-blue);
}

.alert-priority-badge {
  flex: 1;
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--color-text-dark);
}

.custom-alert-body {
  padding: 1.25rem;
}

.custom-alert-message {
  font-size: 1rem;
  font-weight: 500;
  color: var(--color-text-dark);
  margin: 0 0 1rem 0;
  line-height: 1.5;
}

.custom-alert-meta {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.meta-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem;
  background: var(--color-bg-light-gray);
  border-radius: 6px;
}

.status-badge {
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 0.8rem;
  font-weight: 600;
  text-transform: uppercase;
}

.status-badge.active {
  background: var(--color-bg-green-light);
  color: var(--color-green);
}

.status-badge.triggered {
  background: var(--color-bg-light-gray);
  color: var(--color-text-medium);
}

/* Alert Form */
.alert-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  padding: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group textarea {
  resize: vertical;
  min-height: 80px;
}

@media (max-width: 768px) {
  .alert-item {
    padding: 1rem;
  }

  .alert-icon {
    font-size: 1.25rem;
  }

  .alert-message {
    font-size: 0.9rem;
  }

  .alert-timestamp {
    font-size: 0.75rem;
  }

  .test-panel .d-flex {
    flex-direction: column;
  }

  .test-panel button {
    width: 100%;
    justify-content: center;
  }

  .custom-alert-header {
    padding: 0.75rem;
  }

  .custom-alert-body {
    padding: 1rem;
  }

  .meta-row {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.25rem;
  }
}
</style>
