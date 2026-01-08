<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <div class="modal-header">
        <h3>
          <i class="fas fa-plus-circle"></i>
          Nowe Zadanie
        </h3>
        <button class="close-btn" @click="closeModal">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="task-name">Nazwa zadania *</label>
          <input
            id="task-name"
            v-model="formData.name"
            type="text"
            placeholder="Np. Malowanie ścian"
            required
          />
        </div>

        <div class="form-group">
          <label for="task-description">Opis</label>
          <textarea
            id="task-description"
            v-model="formData.description"
            rows="3"
            placeholder="Dodatkowe informacje o zadaniu..."
          ></textarea>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="task-priority">Priorytet</label>
            <select id="task-priority" v-model="formData.priority">
              <option :value="undefined">— Brak —</option>
              <option
                v-for="opt in priorityOptions"
                :key="opt.value"
                :value="opt.value"
                :title="opt.description"
              >
                {{ opt.label }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="task-status">Status</label>
            <select id="task-status" v-model="formData.status">
              <option :value="undefined">— Brak —</option>
              <option
                v-for="opt in statusOptions"
                :key="opt.value"
                :value="opt.value"
                :title="opt.description"
              >
                {{ opt.label }}
              </option>
            </select>
          </div>
        </div>
        <div v-if="dateValidationError" class="date-error">
          <i class="fas fa-exclamation-triangle"></i>
          {{ dateValidationError }}
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="task-start">Data rozpoczęcia</label>
            <input
              id="task-start"
              v-model="formData.startAt"
              type="date"
              :disabled="isDateLocked"
            />
          </div>

          <div class="form-group">
            <label for="task-end">Szacowany czas zakończenia</label>
            <input
              id="task-end"
              v-model="formData.estimatedTime"
              type="date"
              :disabled="isDateLocked"
            />
          </div>
        </div>

        <div class="modal-actions">
          <button type="button" class="btn btn-secondary" @click="closeModal">Anuluj</button>
          <button type="submit" class="btn btn-primary" :disabled="isSaving">
            <i class="fas fa-save"></i>
            {{ isSaving ? 'Tworzenie...' : 'Utwórz zadanie' }}
          </button>
        </div>
        <div v-if="saveError" class="mt-3 text-sm" style="color: #dc2626">{{ saveError }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, toRefs, computed } from 'vue'
import { StatusEnum, PriorityEnum, type CreateTaskDTO } from '@/backend/BackendBase'
import { getTodayString, makeLocalMidday, getDateRangeError } from '@/helpers/dateFormatter'
import { getExtendedPriorityLabel } from '@/helpers/priorityEnumFormatter'
import { getExtendedStatusLabel } from '@/helpers/statusEnumFormatter'
import { Backend } from '@/main'

const props = defineProps<{
  isOpen: boolean
  roomId: string
  projectId: string
  userId: string
}>()

const { isOpen } = toRefs(props)
const emit = defineEmits(['close', 'created'])

const formData = ref({
  name: '',
  description: '',
  startAt: getTodayString(),
  estimatedTime: getTodayString(),
  priority: undefined as number | undefined,
  status: undefined as number | undefined,
})

const isDateLocked = computed(
  () => formData.value.status === undefined || formData.value.status === StatusEnum._0,
)

const dateValidationError = computed(() =>
  getDateRangeError(formData.value.startAt, formData.value.estimatedTime),
)

const isSaving = ref(false)
const saveError = ref('')

const priorityOptions = getExtendedPriorityLabel()
const statusOptions = getExtendedStatusLabel()

watch(isOpen, (open) => {
  if (open) {
    const todayStr = getTodayString()
    formData.value = {
      name: '',
      description: '',
      startAt: todayStr,
      estimatedTime: todayStr,
      priority: undefined,
      status: undefined,
    }
    saveError.value = ''
  }
})

watch(
  () => formData.value.status,
  (newStatus) => {
    if (newStatus === undefined || newStatus === StatusEnum._0) {
      const todayStr = getTodayString()
      formData.value.startAt = todayStr
      formData.value.estimatedTime = todayStr
    }
  },
)

function closeModal() {
  emit('close')
}

async function handleSubmit() {
  if (!formData.value.name.trim()) {
    saveError.value = 'Nazwa zadania jest wymagana.'
    return
  }

  if (dateValidationError.value) {
    saveError.value = dateValidationError.value
    return
  }

  isSaving.value = true
  saveError.value = ''

  const startAtDate = makeLocalMidday(formData.value.startAt)
  const estimatedTimeDate = makeLocalMidday(formData.value.estimatedTime)

  if (!startAtDate) {
    saveError.value = 'Nieprawidłowa data rozpoczęcia.'
    isSaving.value = false
    return
  }

  const createAtDate = new Date(startAtDate.getTime() - 1000) // 1 sekunda przed startAt

  const dto: CreateTaskDTO = {
    name: formData.value.name,
    description: formData.value.description || undefined,
    status: formData.value.status !== undefined ? (formData.value.status as StatusEnum) : undefined,
    priority:
      formData.value.priority !== undefined ? (formData.value.priority as PriorityEnum) : undefined,
    createAt: createAtDate,
    startAt: startAtDate,
    closedAt: undefined,
    roomId: props.roomId,
    projectId: props.projectId,
    userId: props.userId,
    estimatedTime: estimatedTimeDate,
  }

  try {
    const success = await Backend.createTask(dto)
    if (success) {
      emit('created')
      closeModal()
    } else {
      saveError.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    saveError.value = e?.message || e?.toString() || 'Nie udało się utworzyć zadania.'
  } finally {
    isSaving.value = false
  }
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  padding: 0;
  width: 90%;
  max-width: 600px;
  max-height: 90vh;
  overflow-y: auto;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px 24px;
  border-bottom: 2px solid #f0f0f0;
}

.modal-header h3 {
  margin: 0;
  font-size: 1.5rem;
  color: var(--color-primary-purple);
  display: flex;
  align-items: center;
  gap: 10px;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #999;
  cursor: pointer;
  padding: 0;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 4px;
  transition: all 0.2s;
}

.close-btn:hover {
  background: #f0f0f0;
  color: #333;
}

form {
  padding: 24px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
  color: #333;
}

.form-group input,
.form-group textarea,
.form-group select {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid #ddd;
  border-radius: 6px;
  font-size: 14px;
  transition: border-color 0.2s;
}

.form-group input:focus,
.form-group textarea:focus,
.form-group select:focus {
  outline: none;
  border-color: var(--color-primary-purple);
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 16px;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  margin-top: 24px;
  padding-top: 20px;
  border-top: 2px solid #f0f0f0;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  gap: 8px;
}

.btn-secondary {
  background: #f0f0f0;
  color: #333;
}

.btn-secondary:hover {
  background: #e0e0e0;
}

.btn-primary {
  background: var(--color-primary-purple);
  color: white;
}

.btn-primary:hover {
  background: var(--color-primary-purple-dark, #6b46c1);
}
</style>
