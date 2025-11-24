<template>
  <div v-if="isOpen" class="modal-overlay" @click.self="closeModal">
    <div class="modal-content">
      <div class="modal-header">
        <h3>
          <i class="fas fa-edit"></i>
          Edytuj Zadanie
        </h3>
        <button class="close-btn" @click="closeModal">
          <i class="fas fa-times"></i>
        </button>
      </div>

      <form @submit.prevent="handleSubmit">
        <div class="form-group">
          <label for="edit-task-name">Nazwa zadania *</label>
          <input
            id="edit-task-name"
            v-model="formData.name"
            type="text"
            placeholder="Np. Malowanie ścian"
            requiredz
          />
        </div>

        <div class="form-group">
          <label for="edit-task-description">Opis</label>
          <textarea
            id="edit-task-description"
            v-model="formData.description"
            rows="3"
            placeholder="Dodatkowe informacje o zadaniu..."
          ></textarea>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="edit-task-start">Data rozpoczęcia</label>
            <input
              id="edit-task-start"
              v-model="formData.startAt"
              type="date"
              :disabled="isDateLocked"
            />
          </div>

          <div class="form-group">
            <label for="edit-task-end">Szacowany czas zakończenia</label>
            <input
              id="edit-task-end"
              v-model="formData.estimatedTime"
              type="date"
              :disabled="isDateLocked"
            />
          </div>
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="edit-task-priority">Priorytet</label>
            <select id="edit-task-priority" v-model="formData.priority">
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
            <label for="edit-task-status">Status</label>
            <select id="edit-task-status" v-model="formData.status">
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

        <div class="modal-actions">
          <button type="button" class="btn btn-danger" @click="handleDelete">
            <i class="fas fa-trash"></i>
            Usuń
          </button>
          <div class="right-actions">
            <button type="button" class="btn btn-secondary" @click="closeModal">Anuluj</button>
            <button type="submit" class="btn btn-primary" :disabled="isSaving || loadingTask">
              <i class="fas fa-save"></i>
              <span v-if="!isSaving">Zapisz</span>
              <span v-else>Zapisywanie...</span>
            </button>
          </div>
        </div>
        <div v-if="loadingTask" class="mt-3 text-sm text-gray-500">Ładowanie danych zadania...</div>
        <div v-if="saveError" class="mt-3 text-sm" style="color: #dc2626">{{ saveError }}</div>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, toRefs, onMounted, computed } from 'vue'
import { type TaskDataDTO, StatusEnum, PriorityEnum } from '@/backend/BackendBase'
import { getTodayString, makeLocalMidday } from '@/helpers/dateFormatter'
import { getExtendedPriorityLabel } from '@/helpers/priorityEnumFormatter'
import { getExtendedStatusLabel } from '@/helpers/statusEnumFormatter'
import { Backend } from '@/main'

const props = defineProps<{
  isOpen: boolean
  taskId: string
}>()

const { isOpen, taskId } = toRefs(props)
const emit = defineEmits(['close', 'saved', 'deleted'])

const formData = ref({
  name: '',
  description: '',
  startAt: '',
  estimatedTime: '',
  priority: undefined as number | undefined,
  status: undefined as number | undefined,
})

// Blokada dat gdy status: undefined, W planach (_0), Zakończony (_2) lub Wstrzymany (_3).
// Reset dat do dzisiaj tylko dla undefined lub W planach, nie dla zakończonych/wstrzymanych.
const isDateLocked = computed(() => {
  const s = formData.value.status
  return s === undefined || s === StatusEnum._0 || s === StatusEnum._2 || s === StatusEnum._3
})

const originalTask = ref<TaskDataDTO | undefined>(undefined)
const isSaving = ref(false)
const loadingTask = ref(false)
const saveError = ref('')

const priorityOptions = getExtendedPriorityLabel()
const statusOptions = getExtendedStatusLabel()

function toDateInputValue(d: unknown, startFallback?: Date | string | undefined): string {
  if (d === undefined || d === null) return ''
  let dateObj: Date
  // Jeśli backend zwraca liczbę (np. ile dni), zinterpretuj jako start + dni
  if (typeof d === 'number') {
    if (startFallback) {
      const base = startFallback instanceof Date ? new Date(startFallback) : new Date(startFallback)
      base.setHours(0, 0, 0, 0)
      base.setDate(base.getDate() + d)
      dateObj = base
    } else {
      // Bez punktu odniesienia traktuj jako epoch
      dateObj = new Date(d)
    }
  } else if (d instanceof Date) {
    dateObj = d
  } else if (typeof d === 'string') {
    dateObj = new Date(d)
  } else {
    return ''
  }
  if (isNaN(dateObj.getTime()) || dateObj.getFullYear() === 1) return '' // .NET MinValue => ukryj
  const pad = (n: number) => n.toString().padStart(2, '0')
  return `${dateObj.getFullYear()}-${pad(dateObj.getMonth() + 1)}-${pad(dateObj.getDate())}`
}

async function loadTask() {
  if (!taskId.value) return
  loadingTask.value = true
  saveError.value = ''
  try {
    const task = await Backend.getTaskById(taskId.value)
    if (task) {
      originalTask.value = task
      formData.value.name = task.name || ''
      formData.value.description = task.description || ''
      formData.value.startAt = toDateInputValue(task.startAt)
      formData.value.estimatedTime = toDateInputValue(task.estimatedTime)
      console.log('[TaskEditModal] Raw task dates', {
        startAtRaw: task.startAt,
        estimatedTimeRaw: task.estimatedTime,
        computedEstimatedTimeInput: formData.value.estimatedTime,
      })
      formData.value.priority = task.priority !== undefined ? Number(task.priority) : undefined
      formData.value.status = task.status !== undefined ? Number(task.status) : undefined
      // Jeżeli status niewybrany lub W planach – ustaw dzisiejsze daty
      if (formData.value.status === undefined || formData.value.status === StatusEnum._0) {
        const todayStr = getTodayString()
        formData.value.startAt = todayStr
        formData.value.estimatedTime = todayStr
      }
    } else {
      saveError.value = 'Nie znaleziono zadania.'
    }
  } catch (e: any) {
    console.error(e)
    saveError.value = 'Błąd podczas pobierania zadania.'
  } finally {
    loadingTask.value = false
  }
}

watch([isOpen, taskId], ([open]) => {
  if (open) {
    loadTask()
  }
})

onMounted(() => {
  if (isOpen.value) {
    loadTask()
  }
})

// Reaguj na zmianę statusu podczas edycji
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

  if (!originalTask.value?.id) {
    saveError.value = 'Brak ID zadania do edycji.'
    return
  }

  isSaving.value = true
  saveError.value = ''

  try {
    const startDate = makeLocalMidday(formData.value.startAt)
    const estDate = makeLocalMidday(formData.value.estimatedTime)

    const dto: TaskDataDTO = {
      id: originalTask.value.id,
      name: formData.value.name,
      description: formData.value.description || undefined,
      status:
        formData.value.status !== undefined ? (formData.value.status as StatusEnum) : undefined,
      priority:
        formData.value.priority !== undefined
          ? (formData.value.priority as PriorityEnum)
          : undefined,
      createAt: originalTask.value.createAt,
      startAt: startDate,
      closedAt: originalTask.value.closedAt,
      roomId: originalTask.value.roomId,
      projectId: originalTask.value.projectId,
      userId: originalTask.value.userId,
      estimatedTime: estDate,
    }

    console.log('[TaskEditModal] Wysyłam dane do edycji (surowe DTO):', dto)
    const success = await Backend.editTask(dto)
    console.log('[TaskEditModal] Odpowiedź z backendu (boolean):', success)

    if (success) {
      emit('saved', dto)
      closeModal()
    } else {
      saveError.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    saveError.value = 'Nie udało się zapisać zadania.'
    console.error('[TaskEditModal] Błąd edycji:', e)
  } finally {
    isSaving.value = false
  }
}

async function handleDelete() {
  if (!originalTask.value?.id) return

  if (!confirm('Czy na pewno chcesz usunąć to zadanie?')) return

  try {
    const success = await Backend.deleteTask(originalTask.value.id)
    if (success) {
      emit('deleted', originalTask.value.id)
      closeModal()
    } else {
      saveError.value = 'Nie udało się usunąć zadania.'
    }
  } catch (e: any) {
    saveError.value = 'Błąd podczas usuwania.'
    console.error(e)
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
  justify-content: space-between;
  align-items: center;
  margin-top: 24px;
  padding-top: 20px;
  border-top: 2px solid #f0f0f0;
}

.right-actions {
  display: flex;
  gap: 12px;
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

.btn-danger {
  background: #ef4444;
  color: white;
}

.btn-danger:hover {
  background: #dc2626;
}
</style>
