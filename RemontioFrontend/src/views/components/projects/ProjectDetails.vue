<template>
  <MainLayout>
    <div class="project-details-container">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie projektu...</p>
      </div>
      <div v-else-if="error" class="error-state">
        <i class="fas fa-exclamation-triangle"></i>
        <h2>Nie udało się załadować projektu</h2>
        <p>{{ error }}</p>
        <button class="btn btn-primary" @click="reload">Spróbuj ponownie</button>
      </div>
      <div v-else-if="project" class="details-wrapper">
        <div class="details-header">
          <div class="title-block">
            <h1 class="details-title">{{ project.name }}</h1>
            <span class="project-status" :class="`status-${project.status}`">
              {{ getStatusLabel(project.status) }}
            </span>
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
            <p class="description-text">
              {{ project.description || 'Brak opisu' }}
            </p>
          </section>

          <section class="content-section meta-grid">
            <div class="meta-item">
              <i class="fas fa-clock"></i>
              <div class="meta-text">
                <span class="meta-label">Utworzono</span>
                <span class="meta-value">{{ formatDate(project.createAt) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-lock"></i>
              <div class="meta-text">
                <span class="meta-label">Zamknięto</span>
                <span class="meta-value">{{ formatDate(project.closedAt) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-user"></i>
              <div class="meta-text">
                <span class="meta-label">Użytkownik</span>
                <span class="meta-value">{{ project.user?.userName }}</span>
              </div>
            </div>
            <div class="meta-item" v-if="project.user?.email">
              <i class="fas fa-envelope"></i>
              <div class="meta-text">
                <span class="meta-label">Email</span>
                <span class="meta-value">{{ project.user.email }}</span>
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
import type { ProjectDataDTO } from '@/backend/BackendBase'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { formatDate } from '@/helpers/dateFormatter'

const route = useRoute()
const router = useRouter()
const projectId = route.params.projectId as string
const project = ref<ProjectDataDTO | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)

const fetchData = async () => {
  try {
    loading.value = true
    error.value = null
    project.value = await Backend.getProjectById(projectId)
  } catch (e: any) {
    error.value = e?.message || 'Nieznany błąd'
  } finally {
    loading.value = false
  }
}

const reload = () => fetchData()
const goBack = () => router.push({ name: 'ProjectList' })

// Placeholder action handlers (ready for future integration)
const onEdit = () => {
  if (projectId) {
    router.push({
      name: 'ProjectEdit',
      params: { projectId: projectId },
    })
  }
}
const onChangeStatus = () => {
  if (projectId) {
    router.push({
      name: 'ProjectChangeStatus',
      params: { projectId: projectId },
    })
  }
}
const onDelete = () => {
  if (projectId) {
    router.push({
      name: 'ProjectDelete',
      params: { projectId: projectId },
    })
  }
}

onMounted(async () => {
  await fetchData()
})
</script>

<style scoped>
.project-details-container {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.details-title {
  font-size: 2.4rem;
}

.header-btn.danger {
  color: var(--color-red);
}

@media (max-width: 768px) {
  .project-details-container {
    padding: 1rem;
  }
  .header-actions {
    width: 100%;
    justify-content: flex-start;
  }
  .meta-grid {
    grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
  }
}

@media (max-width: 480px) {
  .header-actions {
    gap: 0.5rem;
  }
}
</style>
