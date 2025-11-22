<template>
  <MainLayout>
    <div class="rooms-container">
      <div class="rooms-header d-flex align-items-center flex-wrap gap-3">
        <h1 class="rooms-title mb-3">Zmień status i priorytet pokoju</h1>
        <div v-if="room && room.status !== undefined" class="d-flex align-items-center gap-2 mb-3">
          <span :class="['project-status', 'status-' + room.status]">{{
            getStatusLabel(room.status)
          }}</span>
        </div>
      </div>
      <div v-if="isLoading" class="loading-state">
        <div class="spinner" />
        <p>Ładowanie danych pokoju...</p>
      </div>
      <div v-else-if="loadError" class="empty-state" style="text-align: center; padding: 2rem">
        <i
          class="fas fa-exclamation-triangle"
          style="font-size: 3rem; color: var(--color-primary-purple)"
        ></i>
        <h2>Nie udało się pobrać pokoju</h2>
        <p>{{ loadError }}</p>
        <button class="btn btn-secondary" @click="goBack">Powrót</button>
      </div>
      <form v-else class="status-form" @submit.prevent="submitChange">
        <div class="meta-grid mb-4">
          <div class="meta-item">
            <i class="fas fa-door-open"></i>
            <div class="meta-text">
              <span class="meta-label">Nazwa</span><span class="meta-value">{{ room?.name }}</span>
            </div>
          </div>
          <div class="meta-item" v-if="room?.description">
            <i class="fas fa-card"></i>
            <div class="meta-text">
              <span class="meta-label">Opis</span
              ><span class="meta-value">{{ room?.description }}</span>
            </div>
          </div>
          <div class="meta-item">
            <i class="fas fa-clock"></i>
            <div class="meta-text">
              <span class="meta-label">Utworzono</span
              ><span class="meta-value">{{ formatDate(room?.createAt) }}</span>
            </div>
          </div>
        </div>
        <div class="status-selector mb-4">
          <h2 class="section-title" style="font-size: 1.4rem; margin-bottom: 0.75rem">
            Wybierz nowy status
          </h2>
          <div class="row g-3">
            <div v-for="s in statuses" :key="s.value" class="col-12 col-md-6 col-lg-3">
              <label
                :class="[
                  'status-option',
                  'w-100',
                  'd-flex',
                  'flex-column',
                  'align-items-start',
                  'gap-2',
                  selectedStatus === s.value ? 'active' : '',
                ]"
              >
                <div class="d-flex align-items-center justify-content-between w-100">
                  <span :class="['project-status', 'status-' + s.value]">{{ s.label }}</span>
                  <input
                    type="radio"
                    class="form-check-input"
                    :value="s.value"
                    v-model="selectedStatus"
                  />
                </div>
                <small class="text-muted" style="font-size: 0.75rem; line-height: 1.1rem">{{
                  s.description
                }}</small>
              </label>
            </div>
          </div>
        </div>
        <div class="d-flex gap-3 flex-wrap">
          <button type="submit" class="btn btn-primary header-btn">
            <span v-if="!isSubmitting">Zapisz zmiany</span><span v-else>Zapisywanie...</span>
          </button>
          <button
            type="button"
            class="btn btn-outline-secondary header-btn"
            @click="goBack"
            :disabled="isSubmitting"
          >
            Anuluj
          </button>
        </div>
        <div v-if="submitError" class="mt-3 alert alert-danger" role="alert">{{ submitError }}</div>
        <div v-if="submitSuccess" class="mt-3 alert alert-success" role="alert">
          Dane zaktualizowane pomyślnie.
        </div>
      </form>
    </div>
  </MainLayout>
</template>
<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { Backend } from '@/main'
import { StatusEnum, PriorityEnum, type RoomDataDTO } from '@/backend/BackendBase'
import { getStatusLabel, getExtendedStatusLabel } from '@/helpers/statusEnumFormatter'
import { formatDate } from '@/helpers/dateFormatter'

const route = useRoute()
const router = useRouter()
const roomId = route.params.roomId as string
const room = ref<RoomDataDTO | null>(null)
const isLoading = ref(true)
const isSubmitting = ref(false)
const loadError = ref<string | null>(null)
const submitError = ref<string | null>(null)
const submitSuccess = ref(false)
const selectedStatus = ref<StatusEnum | null>(null)
const selectedPriority = ref<PriorityEnum | null>(null)
const statuses = getExtendedStatusLabel()
const priorities = [
  { value: PriorityEnum._0, label: 'Niski', description: 'Najniższy priorytet' },
  { value: PriorityEnum._1, label: 'Normalny', description: 'Standardowy priorytet' },
  { value: PriorityEnum._2, label: 'Wysoki', description: 'Ważne zadania/pokój' },
  { value: PriorityEnum._3, label: 'Krytyczny', description: 'Najwyższy priorytet' },
]

onMounted(async () => {
  if (!roomId) {
    loadError.value = 'Brak identyfikatora pokoju.'
    isLoading.value = false
    return
  }
  try {
    room.value = await Backend.getRoomById(roomId)
    selectedStatus.value = room.value.status ?? StatusEnum._0
    // Priority might not exist on room; keep selection null by default
  } catch (err: any) {
    console.error(err)
    loadError.value = 'Wystąpił błąd podczas pobierania pokoju.'
  } finally {
    isLoading.value = false
  }
})

async function submitChange() {
  submitError.value = null
  submitSuccess.value = false
  if (!room.value) return
  if (selectedStatus.value === room.value.status && selectedPriority.value === null) return
  isSubmitting.value = true
  try {
    let statusOk = true
    if (selectedStatus.value !== null && selectedStatus.value !== room.value.status) {
      statusOk = await Backend.editRoomStatus(room.value.id!, selectedStatus.value.toString())
      if (statusOk) room.value.status = selectedStatus.value
    }
    // Priority update placeholder: backend endpoint not defined in grep results
    // If priority persistence is added later, call appropriate method here.
    if (statusOk) {
      submitSuccess.value = true
      setTimeout(() => router.push({ name: 'RoomDetails', params: { roomId } }), 1200)
    } else submitError.value = 'Serwer zwrócił niepowodzenie.'
  } catch (err: any) {
    console.error(err)
    submitError.value = 'Nie udało się zaktualizować danych.'
  } finally {
    isSubmitting.value = false
  }
}
function goBack() {
  router.push({ name: 'RoomDetails', params: { roomId } })
}
</script>
<style scoped>
.priority-option.active {
  outline: 2px solid var(--color-primary-blue);
  border-radius: 8px;
}
</style>
