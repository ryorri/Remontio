<template>
  <MainLayout>
    <div class="rooms-container">
      <div v-if="!projectId" class="project-selection">
        <h1 class="rooms-title">Wybierz Projekt</h1>
        <p class="selection-hint">Wybierz projekt, aby wyświetlić jego pokoje</p>

        <div v-if="loadingProjects" class="loading-state">
          <div class="spinner"></div>
          <p>Ładowanie projektów...</p>
        </div>

        <div v-else-if="projectsList.length === 0" class="empty-state">
          <i class="fas fa-folder-open"></i>
          <h2>Brak projektów</h2>
          <p>Najpierw utwórz projekt, aby móc dodawać pokoje.</p>
          <button class="btn btn-primary btn-large" @click="goToProjects">
            Przejdź do projektów
          </button>
        </div>

        <div v-else class="projects-grid">
          <div
            v-for="project in projectsList"
            :key="project.id"
            class="project-card"
            @click="selectProject(project.id)"
          >
            <h3>{{ project.name }}</h3>
            <p>{{ project.description || 'Brak opisu' }}</p>
            <span class="project-status" :class="'status-' + project.status">
              {{ getStatusLabel(project.status) }}
            </span>
          </div>
        </div>
      </div>

      <div v-else>
        <div class="rooms-header">
          <h1 class="rooms-title">Moje Pokoje</h1>
          <button class="btn btn-primary" @click="createNewRoom">
            <i class="fas fa-plus"></i> Nowy Pokój
          </button>
        </div>
        <div v-if="loading" class="loading-state">
          <div class="spinner"></div>
          <p>Ładowanie pokoi...</p>
        </div>
        <div v-else-if="roomsList.length === 0" class="empty-state">
          <i class="fas fa-door-open"></i>
          <h2>Brak pokoi</h2>
          <p>Dodaj pierwszy pokój, aby kontynuować pracę.</p>
          <button class="btn btn-primary btn-large" @click="createNewRoom">
            Stwórz swój pierwszy pokój
          </button>
        </div>
        <div v-else class="table-container">
          <table class="rooms-table">
            <thead>
              <tr>
                <th>Nazwa pokoju</th>
                <th>Opis</th>
                <th>Status</th>
                <th>Utworzono</th>
                <th>Zamknięto</th>
                <th>Projekt</th>
                <th>Akcje</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="room in roomsList" :key="room.id" class="room-row">
                <td class="room-name" @click="openRoom(room.id)">{{ room.name }}</td>
                <td class="room-description" @click="openRoom(room.id)">
                  {{ room.description || 'Brak opisu' }}
                </td>
                <td @click="openRoom(room.id)">
                  <span class="project-status" :class="'status-' + room.status">{{
                    getStatusLabel(room.status)
                  }}</span>
                </td>
                <td class="date-cell" @click="openRoom(room.id)">
                  {{ formatDate(room.createAt) }}
                </td>
                <td class="date-cell" @click="openRoom(room.id)">
                  {{ formatDate(room.closedAt) }}
                </td>
                <td class="date-cell" @click="openRoom(room.id)">{{ currentProjectName }}</td>
                <td>
                  <div class="action-buttons">
                    <button
                      class="btn btn-sm btn-primary"
                      @click="openRoom(room.id)"
                      title="Otwórz"
                    >
                      <font-awesome-icon :icon="['fas', 'folder-open']" />
                    </button>
                    <button class="btn btn-sm btn-primary" @click="onEdit(room.id)" title="Edytuj">
                      <font-awesome-icon :icon="['fas', 'edit']" />
                    </button>
                    <button
                      class="btn btn-sm btn-warning"
                      @click="onChangeStatus(room.id)"
                      title="Zmień status"
                    >
                      <font-awesome-icon :icon="['fas', 'exchange-alt']" />
                    </button>
                    <button class="btn btn-sm btn-danger" @click="onDelete(room.id)" title="Usuń">
                      <font-awesome-icon :icon="['fas', 'trash-can']" />
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </MainLayout>
</template>
<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { Backend } from '@/main'
import { onMounted, ref } from 'vue'
import type { RoomDataDTO, ProjectDataDTO } from '@/backend/BackendBase'
import { getStatusLabel } from '@/helpers/statusEnumFormatter'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { formatDate } from '@/helpers/dateFormatter'
import { useRouter, useRoute } from 'vue-router'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const roomsList = ref<RoomDataDTO[]>([])
const projectsList = ref<ProjectDataDTO[]>([])
const loading = ref(true)
const loadingProjects = ref(true)
const route = useRoute()
const router = useRouter()
const projectId = ref<string | undefined>(route.params.projectId as string | undefined)
const currentProjectName = ref<string>('')

async function fetchProjects() {
  try {
    loadingProjects.value = true
    projectsList.value = await Backend.getProjectListByUserId(getCurrentUserId()!)
  } catch (e) {
    console.error('Error fetching projects:', e)
  } finally {
    loadingProjects.value = false
  }
}

async function fetchRooms() {
  if (!projectId.value) return

  try {
    loading.value = true
    roomsList.value = await Backend.getRoomListByProjectId(projectId.value)

    // Pobierz nazwę projektu
    if (projectId.value) {
      try {
        const project = await Backend.getProjectById(projectId.value)
        currentProjectName.value = project?.name || projectId.value
      } catch (e) {
        currentProjectName.value = projectId.value
      }
    }
  } catch (e) {
    console.error('Error fetching rooms:', e)
  } finally {
    loading.value = false
  }
}

function selectProject(selectedProjectId: string | undefined) {
  if (selectedProjectId) {
    projectId.value = selectedProjectId
    router.push({
      name: 'RoomList',
      params: { projectId: selectedProjectId },
    })
    fetchRooms()
  }
}

function goToProjects() {
  router.push({ name: 'ProjectList' })
}

function createNewRoom() {
  router.push({ name: 'RoomCreate' })
}
function openRoom(roomId: string | undefined) {
  roomId && router.push({ name: 'RoomDetails', params: { roomId } })
}

const onEdit = (roomId: string | undefined) =>
  roomId && router.push({ name: 'RoomEdit', params: { roomId } })
const onChangeStatus = (roomId: string | undefined) =>
  roomId && router.push({ name: 'RoomChangeStatusAndPriority', params: { roomId } })
const onDelete = (roomId: string | undefined) =>
  roomId && router.push({ name: 'RoomDelete', params: { roomId } })

onMounted(async () => {
  if (projectId.value) {
    await fetchRooms()
  } else {
    await fetchProjects()
    loading.value = false
  }
})
</script>
<style scoped>
.rooms-container {
  width: 100%;
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.project-selection {
  text-align: center;
}

.selection-hint {
  font-size: 1.1rem;
  color: var(--color-text-medium);
  margin-bottom: 2rem;
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.5rem;
  margin-top: 2rem;
}

.project-card {
  background: var(--color-bg-white);
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 4px 12px var(--shadow-light);
  cursor: pointer;
  transition: all 0.3s ease;
  text-align: left;
}

.project-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 20px var(--shadow-medium);
}

.project-card h3 {
  font-size: 1.4rem;
  font-weight: 600;
  color: var(--color-primary-blue);
  margin: 0 0 0.5rem 0;
}

.project-card p {
  font-size: 0.95rem;
  color: var(--color-text-medium);
  margin: 0 0 1rem 0;
  min-height: 2.5rem;
}

.project-card .project-status {
  display: inline-block;
  margin-top: 0.5rem;
}

.rooms-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}
.rooms-title {
  font-size: 2.5rem;
  font-weight: 700;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
  margin: 0;
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
.rooms-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}
.table-container {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  overflow: hidden auto;
}
.rooms-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--gradient-primary);
}
.rooms-table th {
  padding: 1rem;
  text-align: center;
  color: var(--color-text-white);
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  white-space: nowrap;
}
.rooms-table td {
  padding: 1rem;
  color: var(--color-text-dark);
}
.rooms-table tbody tr {
  border-bottom: 1px solid var(--color-bg-light-gray);
  transition: all 0.2s ease;
}
.room-row:hover {
  background: var(--color-bg-light-gray);
}
.room-name {
  font-weight: 600;
  color: var(--color-primary-blue);
  cursor: pointer;
}
.room-description {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-medium);
  cursor: pointer;
}
.date-cell {
  white-space: nowrap;
  color: var(--color-text-medium);
  cursor: pointer;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
}
</style>
