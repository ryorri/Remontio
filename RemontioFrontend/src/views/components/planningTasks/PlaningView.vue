<template>
  <MainLayout>
    <div class="p-4">
      <label for="project-select" class="block mb-2 font-medium">Wybierz projekt:</label>
      <select id="project-select" v-model="selectedProjectId" class="border rounded p-2 w-full">
        <option disabled value="">— Wybierz —</option>
        <option v-for="project in projects" :key="project.id" :value="project.id">
          {{ project.name }}
        </option>
      </select>
      <div class="mt-6">
        <div v-if="!selectedProject" class="text-sm text-gray-600">
          Wybierz projekt, aby wyświetlić planer.
        </div>
        <div v-else>
          <GanttChart
            v-if="rooms.length || tasks.length"
            :rooms="rooms"
            :tasks="tasks"
            @createTask="openCreateModal"
            @editTask="openEditModal"
          />
          <div v-else class="text-sm text-gray-500">
            Brak pokoi lub zadań do wyświetlenia dla wybranego projektu.
          </div>
        </div>
      </div>
    </div>
    <TaskCreateModal
      v-if="selectedProject && createModal.open"
      :isOpen="createModal.open"
      :roomId="createModal.roomId"
      :projectId="selectedProject.id!"
      :userId="userId!"
      @close="createModal.open = false"
      @created="handleTaskCreated"
    />
    <TaskEditModal
      v-if="editModal.open && editModal.taskId"
      :isOpen="editModal.open"
      :taskId="editModal.taskId"
      @close="editModal.open = false"
      @saved="handleTaskSaved"
      @deleted="handleTaskDeleted"
    />
  </MainLayout>
</template>

<script setup lang="ts">
import MainLayout from '@/views/layouts/MainLayout.vue'
import GanttChart from '@/views/layouts/GanttChart.vue'
import TaskCreateModal from '@/views/components/planningTasks/TaskCreateModal.vue'
import TaskEditModal from '@/views/components/planningTasks/TaskEditModal.vue'
import { Backend } from '@/main'
import { onMounted, ref, computed, watch } from 'vue'
import { getCurrentUserId } from '@/helpers/userHelpers'
import type { ProjectDataDTO, RoomDataDTO, TaskDataDTO } from '@/backend/BackendBase'

const userId = getCurrentUserId()
const projects = ref<ProjectDataDTO[]>([])
const rooms = ref<RoomDataDTO[]>([])
const tasks = ref<TaskDataDTO[]>([])
const selectedProjectId = ref<string | null>('')
const createModal = ref({ open: false, roomId: '' })
const editModal = ref({ open: false, taskId: '' })

const selectedProject = computed(() => {
  return projects.value.find((p) => p.id === selectedProjectId.value) ?? null
})

onMounted(async () => {
  await loadProjects()
})

watch(selectedProjectId, async (newProjectId) => {
  if (newProjectId) {
    await loadProjectData(newProjectId)
  } else {
    rooms.value = []
    tasks.value = []
  }
})

async function loadProjects() {
  if (userId) {
    projects.value = await Backend.getProjectListByUserId(userId)
  }
}

async function loadProjectData(projectId: string) {
  try {
    const allRooms = await Backend.getRoomListByUserId(userId!)
    rooms.value = allRooms.filter((room) => room.projectId === projectId)
    const allTasks = await Backend.getTaskListByUserId(userId!)
    // Zostawiamy daty dokładnie w formacie zwróconym przez backend (string ISO / MinValue itp.)
    tasks.value = allTasks.filter((task) => task.projectId === projectId)
  } catch (error) {
    console.error('Error loading project data:', error)
    rooms.value = []
    tasks.value = []
  }
}

function openCreateModal(roomId: string) {
  createModal.value = { open: true, roomId }
}

function openEditModal(task: TaskDataDTO) {
  if (task.id) {
    editModal.value = { open: true, taskId: task.id }
  }
}

async function handleTaskCreated() {
  createModal.value.open = false
  if (selectedProjectId.value) {
    await loadProjectData(selectedProjectId.value)
  }
}

async function handleTaskSaved() {
  editModal.value.open = false
  if (selectedProjectId.value) {
    await loadProjectData(selectedProjectId.value)
  }
}

async function handleTaskDeleted() {
  editModal.value.open = false
  if (selectedProjectId.value) {
    await loadProjectData(selectedProjectId.value)
  }
}
</script>
