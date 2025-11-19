<template>
  <MainLayout>
    <div class="project-edit-container">
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
            <h1 class="details-title">Edytuj projekt</h1>
          </div>
          <div class="header-actions">
            <button class="btn btn-outline-secondary header-btn" @click="goBack">
              <i class="fas fa-arrow-left"></i> Anuluj
            </button>
            <button class="btn btn-primary header-btn" @click="onSave" :disabled="saving">
              <i class="fas fa-save"></i> Zapisz
            </button>
            <button class="btn header-btn danger" @click="onDelete" :disabled="deleting">
              <i class="fas fa-trash"></i> Usuń
            </button>
          </div>
        </div>

        <form @submit.prevent="onSave" class="details-content">
          <section class="content-section">
            <label class="form-label">Nazwa projektu</label>
            <input v-model="project.name" class="form-control" required />
          </section>

          <section class="content-section">
            <label class="form-label">Opis</label>
            <textarea v-model="project.description" class="form-control" rows="5" />
          </section>

          <section class="content-section meta-grid">
            <div class="meta-item meta-item_padding">
              <i class="fas fa-clock"></i>
              <div class="meta-text">
                <span class="meta-label">Utworzono</span>
                <span class="meta-value">{{ formatDate(project.createAt) }}</span>
              </div>
            </div>

            <div class="meta-item meta-item_padding">
              <i class="fas fa-lock"></i>
              <div class="meta-text">
                <span class="meta-label">Zamknięto</span>
                <span class="meta-value">{{ formatDate(project.closedAt) || '—' }}</span>
              </div>
            </div>

            <div class="meta-item meta-item_padding">
              <i class="fas fa-list"></i>
              <div class="meta-text">
                <span class="meta-label">Status</span>
                <span class="project-status" :class="`status-${project.status}`">{{
                  getStatusLabel(project.status)
                }}</span>
              </div>
            </div>
          </section>

          <div class="content-section" style="display: flex; gap: 12px">
            <button type="submit" class="btn btn-primary">Zapisz zmiany</button>
            <button type="button" class="btn btn-outline-secondary" @click="goBack">Anuluj</button>
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import type { ProjectDataDTO } from '@/backend/BackendBase'
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Backend } from '@/main'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { formatDate } from '@/helpers/dateFormatter'

const route = useRoute()
const router = useRouter()
const projectId = route.params.projectId as string

const project = ref<ProjectDataDTO | any>(null)
const loading = ref(true)
const saving = ref(false)
const deleting = ref(false)
const error = ref<string | null>(null)

const statusOptions = [
  { value: 0, label: getStatusLabel(0) },
  { value: 1, label: getStatusLabel(1) },
  { value: 2, label: getStatusLabel(2) },
]

// Fetch project
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

const goBack = () => router.push({ name: 'ProjectDetails', params: { projectId } })

// Save handler (assumption: Backend.updateProject exists and accepts the project object)
const onSave = async () => {
  if (!project.value) return
  try {
    saving.value = true
    error.value = null
    // Assumption: Backend.updateProject returns updated project
    const updated = await Backend.editProject(project.value)
    project.value = updated || project.value
    // Navigate back to details after save
    router.push({ name: 'ProjectDetails', params: { projectId } })
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się zapisać projektu'
  } finally {
    saving.value = false
  }
}

// Delete handler (assumption: Backend.deleteProject exists)
const onDelete = async () => {
  if (!confirm('Czy na pewno chcesz usunąć ten projekt?')) return
  try {
    deleting.value = true
    await Backend.deleteProject(projectId)
    router.push({ name: 'ProjectList' })
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się usunąć projektu'
  } finally {
    deleting.value = false
  }
}

onMounted(async () => {
  await fetchData()
})
</script>

<style scoped>
.meta-item_padding {
  margin-top: 12px;
  margin-bottom: 12px;
}

/* Reuse styles from ProjectDetails and main.css conventions */
.project-edit-container {
  width: 100%;
  max-width: 980px;
  margin: 0 auto;
  padding: 2rem;
}
.details-wrapper {
  background: var(--color-bg-white);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 6px 20px var(--shadow-light);
}
.details-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
  margin-bottom: 1rem;
}
.title-block {
  display: flex;
  align-items: center;
  gap: 12px;
}
.details-title {
  margin: 0;
  font-size: 1.6rem;
}
.header-actions {
  display: flex;
  gap: 8px;
}
.form-label {
  font-weight: 600;
  margin-bottom: 6px;
  display: block;
}
.form-control {
  width: 100%;
  padding: 10px 12px;
  border-radius: 8px;
  border: 1px solid #ddd;
}
.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
  gap: 8px;
}
.project-status {
  padding: 6px 12px;
  border-radius: 20px;
  font-weight: 700;
}
.error-state {
  text-align: center;
  padding: 2rem;
}

@media (max-width: 768px) {
  .project-edit-container {
    padding: 1rem;
  }
  .details-header {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
