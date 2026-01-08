<template>
  <MainLayout>
    <div class="room-edit-container">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie pokoju...</p>
      </div>
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <h2>Nie udało się załadować pokoju</h2>
        <p>{{ error }}</p>
        <button class="btn btn-primary" @click="reload">Spróbuj ponownie</button>
      </div>
      <div v-else-if="room" class="details-wrapper">
        <div class="details-header">
          <div class="title-block"><h1 class="details-title">Edytuj pokój</h1></div>
          <div class="header-actions">
            <button class="btn btn-secondary" @click="goBack">
              <i class="fas fa-arrow-left"></i> Anuluj
            </button>
            <button class="btn btn-primary" @click="onSave" :disabled="saving">
              <i class="fas fa-save"></i> Zapisz
            </button>
            <button class="btn btn-danger" @click="onDelete" :disabled="deleting">
              <i class="fas fa-trash"></i> Usuń
            </button>
          </div>
        </div>
        <form @submit.prevent="onSave" class="details-content">
          <section class="content-section">
            <label class="form-label">Nazwa pokoju</label>
            <input v-model="room.name" class="form-control" required />
          </section>
          <section class="content-section">
            <label class="form-label">Opis</label>
            <textarea v-model="room.description" class="form-control" rows="5" />
          </section>
          <section class="content-section meta-grid">
            <div class="meta-item meta-item_padding">
              <i class="fas fa-clock"></i>
              <div class="meta-text">
                <span class="meta-label">Utworzono</span
                ><span class="meta-value">{{ formatDate(room.createAt) }}</span>
              </div>
            </div>
            <div class="meta-item meta-item_padding">
              <i class="fas fa-lock"></i>
              <div class="meta-text">
                <span class="meta-label">Zamknięto</span
                ><span class="meta-value">{{ formatDate(room.closedAt) || '—' }}</span>
              </div>
            </div>
            <div class="meta-item meta-item_padding">
              <i class="fas fa-list"></i>
              <div class="meta-text">
                <span class="meta-label">Status</span
                ><span class="project-status" :class="'status-' + room.status">{{
                  getStatusLabel(room.status)
                }}</span>
              </div>
            </div>
          </section>
          <div class="content-section" style="display: flex; gap: 12px">
            <button type="submit" class="btn btn-primary">Zapisz zmiany</button>
            <button type="button" class="btn btn-secondary" @click="goBack">Anuluj</button>
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>
<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Backend } from '@/main'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { formatDate } from '@/helpers/dateFormatter'
import type { RoomDataDTO } from '@/backend/BackendBase'

const route = useRoute()
const router = useRouter()
const roomId = route.params.roomId as string
const room = ref<RoomDataDTO | any>(null)
const loading = ref(true)
const saving = ref(false)
const deleting = ref(false)
const error = ref<string | null>(null)

async function fetchData() {
  try {
    loading.value = true
    error.value = null
    room.value = await Backend.getRoomById(roomId)
  } catch (e: any) {
    error.value = e?.message || 'Nieznany błąd'
  } finally {
    loading.value = false
  }
}
const reload = () => fetchData()
const goBack = () => router.push({ name: 'RoomDetails', params: { roomId } })
const onSave = async () => {
  if (!room.value) return
  try {
    saving.value = true
    error.value = null
    const ok = await Backend.editRoom(room.value)
    if (ok) {
      router.push({ name: 'RoomDetails', params: { roomId } })
    } else {
      error.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się zapisać pokoju'
  } finally {
    saving.value = false
  }
}
const onDelete = async () => {
  if (!confirm('Czy na pewno chcesz usunąć ten pokój?')) return
  try {
    deleting.value = true
    await Backend.deleteRoom(roomId)
    router.push({ name: 'RoomList' })
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się usunąć pokoju'
  } finally {
    deleting.value = false
  }
}
onMounted(fetchData)
</script>
<style scoped>
.room-edit-container {
  width: 100%;
  max-width: 980px;
  margin: 0 auto;
  padding: 2rem;
}

.details-title {
  font-size: 1.6rem;
}

.details-header {
  margin-bottom: 1rem;
}

@media (max-width: 768px) {
  .room-edit-container {
    padding: 1rem;
  }
  .details-header {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
