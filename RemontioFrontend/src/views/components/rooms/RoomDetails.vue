<template>
  <MainLayout>
    <div class="room-details-container">
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
          <div class="title-block">
            <h1 class="details-title">{{ room.name }}</h1>
            <span class="project-status" :class="'status-' + room.status">{{
              getStatusLabel(room.status)
            }}</span>
          </div>
          <div class="header-actions">
            <button class="btn btn-primary header-btn" @click="goBack">Powrót</button>
            <button class="btn btn-primary header-btn" @click="onEdit">Edytuj</button>
            <button class="btn btn-primary header-btn" @click="onChangeStatus">Zmień status</button>
            <button class="btn btn-primary header-btn danger" @click="onDelete">Usuń</button>
          </div>
        </div>
        <div class="details-content">
          <section class="content-section description-section">
            <h2 class="section-title"><i class="fas fa-info-circle"></i> Opis</h2>
            <p class="description-text">{{ room.description || 'Brak opisu' }}</p>
          </section>
          <section class="meta-grid">
            <div class="meta-item">
              <i class="fas fa-clock"></i>
              <div class="meta-text">
                <span class="meta-label">Utworzono</span
                ><span class="meta-value">{{ formatDate(room.createAt) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-lock"></i>
              <div class="meta-text">
                <span class="meta-label">Zamknięto</span
                ><span class="meta-value">{{ formatDate(room.closedAt) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-door-open"></i>
              <div class="meta-text">
                <span class="meta-label">Projekt</span>
                <span class="meta-value">{{ projectName || 'Ładowanie...' }}</span>
              </div>
            </div>
          </section>
        </div>
      </div>
    </div>
  </MainLayout>
</template>
<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { Backend } from '@/main'
import type { RoomDataDTO, ProjectDataDTO } from '@/backend/BackendBase'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { formatDate } from '@/helpers/dateFormatter'

const route = useRoute()
const router = useRouter()
const roomId = route.params.roomId as string
const room = ref<RoomDataDTO | null>(null)
const projectName = ref<string | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)

async function fetchData() {
  try {
    loading.value = true
    error.value = null
    room.value = await Backend.getRoomById(roomId)
    if (room.value?.projectId) {
      try {
        const project: ProjectDataDTO = await Backend.getProjectById(room.value.projectId)
        projectName.value = project?.name ?? room.value.projectId ?? null
      } catch (e) {
        projectName.value = room.value.projectId ?? null
      }
    }
  } catch (e: any) {
    error.value = e?.message || 'Nieznany błąd'
  } finally {
    loading.value = false
  }
}

const reload = () => fetchData()
const goBack = () => router.push({ name: 'RoomList', params: { projectId: room.value?.projectId } })
const onEdit = () => roomId && router.push({ name: 'RoomEdit', params: { roomId } })
const onChangeStatus = () =>
  roomId && router.push({ name: 'RoomChangeStatusAndPriority', params: { roomId } })
const onDelete = () => roomId && router.push({ name: 'RoomDelete', params: { roomId } })

onMounted(fetchData)
</script>
<style scoped>
.room-details-container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.header-btn.danger {
  color: var(--color-red);
}

@media (max-width: 768px) {
  .room-details-container {
    padding: 1rem;
  }
}
</style>
