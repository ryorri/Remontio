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
const onChangeStatus = () => {}
const onDelete = () => {}

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

.details-wrapper {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 4px 16px var(--shadow-light);
  padding: 2rem 2.5rem;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.details-header {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  gap: 1.25rem;
  align-items: flex-start;
}

.title-block {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 1rem;
}

.details-title {
  font-size: 2.4rem;
  font-weight: 700;
  margin: 0;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.header-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  align-items: center;
}

.header-btn {
  padding: auto;
  font-size: 0.85rem;
  display: flex;
  align-items: center;
  gap: 8px;
}

.header-btn.danger {
  color: var(--color-red);
}

.details-content {
  display: flex;
  flex-direction: column;
  gap: 2.5rem;
}

.content-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 10px;
  color: var(--color-primary-blue);
}

.description-text {
  font-size: 1rem;
  line-height: 1.5;
  color: var(--color-text-medium);
  background: var(--color-bg-light-gray);
  padding: 1rem 1.25rem;
  border-radius: 12px;
  box-shadow: inset 0 1px 0 var(--shadow-light);
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(240px, 1fr));
  gap: 1rem;
}

/* Removed unused owner section styles */

/* Status badges reused from ProjectList via same class names */

/* Error State */
.error-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  padding: 4rem 2rem;
  text-align: center;
}
.error-state i {
  font-size: 3.5rem;
  color: var(--color-red);
  opacity: 0.7;
}
.error-state h2 {
  margin: 0;
  font-size: 1.8rem;
  color: var(--color-text-dark);
}
.error-state p {
  margin: 0;
  color: var(--color-text-medium);
  max-width: 480px;
}

/* Responsive */
@media (max-width: 768px) {
  .project-details-container {
    padding: 1rem;
  }
  .details-wrapper {
    padding: 1.5rem 1.25rem;
    gap: 1.75rem;
  }
  .details-title {
    font-size: 2rem;
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
  .details-title {
    font-size: 1.7rem;
  }
  .header-btn {
    flex: 1 1 100%;
    justify-content: center;
  }
  .header-actions {
    gap: 0.5rem;
  }
}
</style>
