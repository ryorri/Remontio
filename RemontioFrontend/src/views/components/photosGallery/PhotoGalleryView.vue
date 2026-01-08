<template>
  <MainLayout>
    <div class="contacts-container">
      <div class="details-header">
        <div class="title-block">
          <h1 class="contacts-title">Galeria Zdjęć</h1>
        </div>
        <div class="header-actions">
          <button @click="showUploadModal = true" class="btn btn-primary">
            <font-awesome-icon :icon="['fas', 'plus']" />
            Dodaj Zdjęcie
          </button>
        </div>
      </div>

      <!-- Project Selector -->
      <div class="card-section mb-4">
        <div class="section-header">
          <font-awesome-icon :icon="['fas', 'folder']" />
          <h3>Filtruj według projektu</h3>
        </div>
        <div class="d-flex gap-3 flex-wrap">
          <label class="project-select-label">
            <strong>Projekt:</strong>
          </label>
          <select v-model="selectedProjectId" @change="loadPhotos" class="form-select">
            <option value="">Wszystkie projekty</option>
            <option v-for="project in projects" :key="project.id" :value="project.id">
              {{ project.name }}
            </option>
          </select>
        </div>
      </div>

      <!-- Loading State -->
      <div v-if="isLoading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie zdjęć...</p>
      </div>

      <!-- Error State -->
      <div v-else-if="errorMessage" class="error-state">
        <font-awesome-icon :icon="['fas', 'info-circle']" />
        <h2>Błąd</h2>
        <p>{{ errorMessage }}</p>
      </div>

      <!-- Empty State -->
      <div v-else-if="!photos || photos.length === 0" class="empty-state">
        <font-awesome-icon :icon="['fas', 'cube']" size="4x" />
        <h2>Brak zdjęć</h2>
        <p>Nie znaleziono żadnych zdjęć dla wybranego projektu.</p>
        <button @click="showUploadModal = true" class="btn btn-primary">
          <font-awesome-icon :icon="['fas', 'plus']" />
          Dodaj pierwsze zdjęcie
        </button>
      </div>
      <!-- Photo Grid -->
      <PhotoGrid v-else :photos="photos" @view="viewPhoto" @delete="confirmDelete" />

      <!-- Upload Modal -->
      <PhotoUpload
        v-if="showUploadModal"
        :projects="projects"
        :selected-project-id="selectedProjectId"
        @close="showUploadModal = false"
        @uploaded="onPhotoUploaded"
      />

      <!-- View Photo Modal -->
      <PhotoModal
        v-if="selectedPhoto"
        :photo="selectedPhoto"
        @close="selectedPhoto = null"
        @delete="confirmDelete"
      />

      <!-- Delete Confirmation Modal -->
      <div v-if="photoToDelete" class="modal-overlay" @click.self="photoToDelete = null">
        <div class="modal-content">
          <div class="modal-header">
            <h3>Potwierdź usunięcie</h3>
            <button @click="photoToDelete = null" class="icon-btn">
              <font-awesome-icon :icon="['fas', 'times']" />
            </button>
          </div>
          <div class="modal-body">
            <p>Czy na pewno chcesz usunąć to zdjęcie? Ta operacja jest nieodwracalna.</p>
          </div>
          <div class="modal-footer">
            <button @click="photoToDelete = null" class="btn btn-secondary">Anuluj</button>
            <button @click="deletePhoto" class="btn btn-danger">
              <font-awesome-icon :icon="['fas', 'trash-can']" />
              Usuń
            </button>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script setup lang="ts">
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, onMounted } from 'vue'
import { Backend } from '@/main'
import { getCurrentUserId, getCurrentUserProjects } from '@/helpers/userHelpers'
import type { PhotoDataDTO, ProjectDataDTO } from '@/backend/BackendBase'
import PhotoGrid from './PhotoGrid.vue'
import PhotoUpload from './PhotoUpload.vue'
import PhotoModal from './PhotoModal.vue'

const projects = ref<ProjectDataDTO[]>([])
const selectedProjectId = ref<string>('')
const photos = ref<PhotoDataDTO[]>([])
const isLoading = ref(false)
const errorMessage = ref('')
const showUploadModal = ref(false)
const selectedPhoto = ref<PhotoDataDTO | null>(null)
const photoToDelete = ref<PhotoDataDTO | null>(null)

onMounted(async () => {
  await loadProjects()
  await loadPhotos()
})

async function loadProjects() {
  try {
    const userId = getCurrentUserId()
    if (!userId) {
      errorMessage.value = 'Nie znaleziono użytkownika'
      return
    }

    const projectList = await getCurrentUserProjects()
    if (projectList) {
      projects.value = projectList
    }
  } catch (error) {
    console.error('Error loading projects:', error)
    errorMessage.value = 'Błąd podczas ładowania projektów'
  }
}

async function loadPhotos() {
  try {
    isLoading.value = true
    errorMessage.value = ''

    let loadedPhotos: PhotoDataDTO[] = []

    if (selectedProjectId.value) {
      loadedPhotos = await Backend.listByProject(selectedProjectId.value)
    } else {
      // Load all photos for all user projects
      const allPhotos: PhotoDataDTO[] = []
      for (const project of projects.value) {
        if (project.id) {
          const projectPhotos = await Backend.listByProject(project.id)
          allPhotos.push(...projectPhotos)
        }
      }
      loadedPhotos = allPhotos
    }

    photos.value = loadedPhotos
  } catch (error) {
    console.error('Error loading photos:', error)
    errorMessage.value = 'Błąd podczas ładowania zdjęć'
  } finally {
    isLoading.value = false
  }
}

function viewPhoto(photo: PhotoDataDTO) {
  selectedPhoto.value = photo
}

function confirmDelete(photo: PhotoDataDTO) {
  photoToDelete.value = photo
  selectedPhoto.value = null
}

async function deletePhoto() {
  if (!photoToDelete.value?.id) return

  try {
    await Backend.deletePhoto(photoToDelete.value.id)
    await loadPhotos()
    photoToDelete.value = null
  } catch (error) {
    console.error('Error deleting photo:', error)
    errorMessage.value = 'Błąd podczas usuwania zdjęcia'
  }
}

async function onPhotoUploaded() {
  showUploadModal.value = false
  await loadPhotos()
}
</script>
