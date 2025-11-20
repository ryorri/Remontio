<template>
  <MainLayout>
    <div class="projects-container">
      <div class="projects-header">
        <h1 class="projects-title">Moje Projekty</h1>
        <button class="btn btn-primary" @click="createNewProject">
          <i class="fas fa-plus"></i> Nowy Projekt
        </button>
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Loading projects...</p>
      </div>

      <div v-else-if="projectsList.length === 0" class="empty-state">
        <i class="fas fa-folder-open"></i>
        <h2>Brak projektów</h2>
        <p>Rozpocznij swoją przygodę z remontem, tworząc pierwszy projekt</p>
        <button class="btn btn-primary btn-large" @click="createNewProject">
          Stwórz swój pierwszy projekt
        </button>
      </div>

      <div v-else class="table-container">
        <table class="projects-table">
          <thead>
            <tr>
              <th>Nazwa projektu</th>
              <th>Opis</th>
              <th>Status</th>
              <th>Utworzono</th>
              <th>Zamknięto</th>
            </tr>
          </thead>
          <tbody>
            <template v-for="project in projectsList" :key="project.id">
              <tr class="project-row" @click="openProject(project.id)">
                <td class="project-name">{{ project.name }}</td>
                <td class="project-description">
                  {{ project.description || 'Brak opisu' }}
                </td>
                <td>
                  <span class="project-status" :class="`status-${project.status}`">
                    {{ getStatusLabel(project.status) }}
                  </span>
                </td>
                <td class="date-cell">
                  {{ formatDate(project.createAt) }}
                </td>
                <td class="date-cell">
                  {{ formatDate(project.closedAt) }}
                </td>
              </tr>
              <tr class="toggle-arrow" @click.stop="toggleMenu(project.id)">
                <td colspan="5">{{ openMenuId === project.id ? '▲' : '▼' }} Akcje</td>
              </tr>
              <tr v-if="openMenuId === project.id" class="actions-dropdown-row" @click.stop>
                <td colspan="5">
                  <div class="actions-panel">
                    <div class="panel-actions">
                      <button
                        class="panel-item"
                        @click="handleAction(() => openProject(project.id))"
                      >
                        Otwórz
                      </button>
                      <button class="panel-item" @click="handleAction(() => onEdit(project.id))">
                        Edytuj
                      </button>
                      <button
                        class="panel-item"
                        @click="handleAction(() => onChangeStatus(project.id))"
                      >
                        Zmień status
                      </button>
                      <button
                        class="panel-item danger"
                        @click="handleAction(() => onDelete(project.id))"
                      >
                        Usuń
                      </button>
                    </div>
                  </div>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { Backend } from '@/main'
import { onMounted, onUnmounted, ref } from 'vue'
import type { ProjectDataDTO } from '@/backend/BackendBase'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { formatDate } from '@/helpers/dateFormatter'
import { useRouter } from 'vue-router'

const projectsList = ref<ProjectDataDTO[]>([])
const loading = ref(true)
const openMenuId = ref<string | null>(null)
const router = useRouter()

const fetchProjects = async () => {
  try {
    loading.value = true
    projectsList.value = await Backend.getProjectListByUserId(getCurrentUserId()!)
  } catch (error) {
    console.error('Error fetching projects:', error)
  } finally {
    loading.value = false
  }
}

const createNewProject = () => {
  router.push({
    name: 'ProjectCreate',
  })
}

const openProject = (projectId: string | undefined) => {
  if (projectId) {
    router.push({
      name: 'ProjectDetails',
      params: { projectId: projectId },
    })
  }
}

const toggleMenu = (projectId: string | undefined) => {
  if (!projectId) return
  openMenuId.value = openMenuId.value === projectId ? null : projectId
}

const handleAction = (action: () => void) => {
  action()
  openMenuId.value = null
}

// Placeholder action handlers
const onEdit = (projectId: string | undefined) => {
  if (projectId) {
    router.push({
      name: 'ProjectEdit',
      params: { projectId: projectId },
    })
  }
}

const onChangeStatus = (projectId: string | undefined) => {
  if (projectId) {
    router.push({
      name: 'ProjectChangeStatus',
      params: { projectId: projectId },
    })
  }
}

const onDelete = (projectId: string | undefined) => {
  if (projectId) {
    router.push({
      name: 'ProjectDelete',
      params: { projectId: projectId },
    })
  }
}

// Close dropdown on outside click
const handleOutsideClick = () => {
  openMenuId.value = null
}

onMounted(async () => {
  window.addEventListener('click', handleOutsideClick)
  await fetchProjects()
})

onUnmounted(() => {
  window.removeEventListener('click', handleOutsideClick)
})
</script>

<style scoped>
.projects-container {
  width: 100%;
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.projects-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.projects-title {
  font-size: 2.5rem;
  font-weight: 700;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
}

.projects-header .btn {
  display: flex;
  align-items: center;
  gap: 8px;
}

/* Empty State */
.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  gap: 1rem;
}

.empty-state i {
  font-size: 5rem;
  color: var(--color-primary-purple);
  opacity: var(--opacity-medium);
}

.empty-state h2 {
  font-size: 2rem;
  color: var(--color-text-dark);
  margin: 0;
}

.empty-state p {
  font-size: 1.1rem;
  color: var(--color-text-medium);
  margin: 0;
}

/* Table */
.table-container {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  max-height: 300px;
  overflow: hidden auto;
}

.projects-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}

.projects-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--gradient-primary);
}

.projects-table th {
  padding: 1rem;
  text-align: left;
  color: var(--color-text-white);
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.projects-table td {
  padding: 1rem;
  color: var(--color-text-dark);
}

.projects-table tbody tr {
  border-bottom: 1px solid var(--color-bg-light-gray);
  transition: all 0.2s ease;
}

.project-row {
  cursor: pointer;
}

/* Table cells */
.project-name {
  font-weight: 600;
  color: var(--color-primary-blue);
}

.project-description {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-medium);
}

.date-cell,
.owner-cell {
  white-space: nowrap;
  color: var(--color-text-medium);
}

.date-cell i {
  color: var(--color-primary-blue);
  margin-right: 0.5rem;
  width: 14px;
}

.owner-cell i {
  color: var(--color-primary-purple);
  margin-right: 0.5rem;
}

/* Status badges (moved to global styles) */

/* Toggle arrow row */
.toggle-arrow {
  background-color: var(--color-bg-light-gray);
  text-align: center;
  cursor: pointer;
}

.toggle-arrow td {
  padding: 8px 12px;
  color: var(--color-text-medium);
  font-weight: 600;
}

/* Actions dropdown panel */
.actions-dropdown-row td {
  padding: 0;
  background: var(--color-bg-light-gray);
}

.actions-panel {
  background: var(--color-bg-white);
  border-top: 1px solid var(--color-bg-light-gray);
  box-shadow: inset 0 1px 0 var(--shadow-light);
  padding: 12px 16px;
}

.panel-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  justify-content: center;
}

.panel-item {
  padding: 10px 14px;
  background: var(--color-bg-light-gray);
  border: 1px solid var(--color-bg-light-gray);
  border-radius: 8px;
  color: var(--color-text-dark);
  cursor: pointer;
  transition: all 0.2s ease;
}

.panel-item:hover {
  background: var(--color-bg-white);
  transform: translateY(-1px);
  box-shadow: 0 4px 10px var(--shadow-light);
}

.panel-item.danger {
  color: var(--color-red);
}
</style>
