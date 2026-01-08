<template>
  <MainLayout>
    <div class="projects-container">
      <div class="projects-header">
        <h1 class="projects-title">Moje Projekty</h1>
        <button class="btn btn-primary header-btn" @click="createNewProject">
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
              <th>Akcje</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="project in projectsList" :key="project.id" class="project-row">
              <td class="project-name" @click="openProject(project.id)">{{ project.name }}</td>
              <td class="project-description" @click="openProject(project.id)">
                {{ project.description || 'Brak opisu' }}
              </td>
              <td @click="openProject(project.id)">
                <span class="project-status" :class="`status-${project.status}`">
                  {{ getStatusLabel(project.status) }}
                </span>
              </td>
              <td class="date-cell" @click="openProject(project.id)">
                {{ formatDate(project.createAt) }}
              </td>
              <td class="date-cell" @click="openProject(project.id)">
                {{ formatDate(project.closedAt) }}
              </td>
              <td>
                <div class="action-buttons">
                  <button
                    class="btn btn-sm btn-primary"
                    @click="openProject(project.id)"
                    title="Otwórz"
                  >
                    <font-awesome-icon :icon="['fas', 'folder-open']" />
                  </button>
                  <button
                    class="btn btn-sm btn-primary"
                    @click="onRoomList(project.id)"
                    title="Pokoje"
                  >
                    <font-awesome-icon :icon="['fas', 'door-open']" />
                  </button>
                  <button class="btn btn-sm btn-primary" @click="onEdit(project.id)" title="Edytuj">
                    <font-awesome-icon :icon="['fas', 'edit']" />
                  </button>
                  <button
                    class="btn btn-sm btn-warning"
                    @click="onChangeStatus(project.id)"
                    title="Zmień status"
                  >
                    <font-awesome-icon :icon="['fas', 'exchange-alt']" />
                  </button>
                  <button class="btn btn-sm btn-danger" @click="onDelete(project.id)" title="Usuń">
                    <font-awesome-icon :icon="['fas', 'trash-can']" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { Backend } from '@/main'
import { onMounted, ref } from 'vue'
import type { ProjectDataDTO } from '@/backend/BackendBase'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { formatDate } from '@/helpers/dateFormatter'
import { useRouter } from 'vue-router'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const projectsList = ref<ProjectDataDTO[]>([])
const loading = ref(true)
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

// Action handlers
const onEdit = (projectId: string | undefined) => {
  if (projectId) {
    router.push({
      name: 'ProjectEdit',
      params: { projectId: projectId },
    })
  }
}
const onRoomList = (projectId: string | undefined) => {
  if (projectId) {
    router.push({
      name: 'RoomList',
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

onMounted(async () => {
  await fetchProjects()
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

.table-container {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
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
  text-align: center;
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

.project-row:hover {
  background: var(--color-bg-light-gray);
}

.project-name {
  cursor: pointer;
  font-weight: 600;
  color: var(--color-primary-blue);
}

.project-description {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-medium);
  cursor: pointer;
}

.date-cell {
  cursor: pointer;
  white-space: nowrap;
  color: var(--color-text-medium);
}

.date-cell i {
  color: var(--color-primary-blue);
  margin-right: 0.5rem;
  width: 14px;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
}
</style>
