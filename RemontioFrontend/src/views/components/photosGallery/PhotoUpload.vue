<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="upload-modal">
      <div class="modal-header">
        <h3>
          <font-awesome-icon :icon="['fas', 'plus']" />
          Dodaj Zdjęcie
        </h3>
        <button @click="$emit('close')" class="icon-btn">
          <font-awesome-icon :icon="['fas', 'times']" />
        </button>
      </div>

      <div class="modal-body">
        <div v-if="errorMessage" class="alert alert-danger">
          <font-awesome-icon :icon="['fas', 'info-circle']" />
          {{ errorMessage }}
        </div>

        <div v-if="successMessage" class="alert alert-success">
          <font-awesome-icon :icon="['fas', 'info-circle']" />
          {{ successMessage }}
        </div>

        <form @submit.prevent="uploadPhoto">
          <!-- Project Selection -->
          <div class="mb-3">
            <label class="form-label">
              <font-awesome-icon :icon="['fas', 'folder']" />
              Projekt *
            </label>
            <select v-model="formData.projectId" class="form-select" required>
              <option value="">Wybierz projekt</option>
              <option v-for="project in projects" :key="project.id" :value="project.id">
                {{ project.name }}
              </option>
            </select>
          </div>

          <!-- Room Selection -->
          <div class="mb-3">
            <label class="form-label">
              <font-awesome-icon :icon="['fas', 'door-open']" />
              Pokój *
            </label>
            <select
              v-model="formData.roomId"
              class="form-select"
              :disabled="!formData.projectId"
              required
            >
              <option value="">Wybierz projekt</option>
              <option v-for="room in rooms" :key="room.id" :value="room.id">
                {{ room.name }}
              </option>
            </select>
          </div>

          <!-- Description -->
          <div class="mb-3">
            <label class="form-label">
              <font-awesome-icon :icon="['fas', 'info-circle']" />
              Opis
            </label>
            <textarea
              v-model="formData.description"
              class="form-control"
              rows="3"
              placeholder="Dodaj opis zdjęcia..."
            ></textarea>
          </div>

          <!-- File Upload -->
          <div class="mb-3">
            <label class="form-label">
              <font-awesome-icon :icon="['fas', 'cube']" />
              Plik zdjęcia *
            </label>
            <input
              type="file"
              @change="handleFileChange"
              accept="image/*"
              class="form-control"
              required
            />
            <small class="text-muted">Obsługiwane formaty: JPG, PNG, GIF (max 10MB)</small>
          </div>

          <!-- Preview -->
          <div v-if="previewUrl" class="preview-container mb-3">
            <label class="form-label">Podgląd:</label>
            <img :src="previewUrl" alt="Preview" class="preview-image" />
          </div>

          <div class="modal-footer">
            <button type="button" @click="$emit('close')" class="btn btn-primary">Anuluj</button>
            <button type="submit" class="btn btn-primary" :disabled="isUploading">
              <span v-if="isUploading">
                <div class="spinner-small"></div>
                Przesyłanie...
              </span>
              <span v-else>
                <font-awesome-icon :icon="['fas', 'save']" />
                Dodaj Zdjęcie
              </span>
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import { Backend } from '@/main'
import { getCurrentUserId } from '@/helpers/userHelpers'
import type { ProjectDataDTO, RoomDataDTO, FileParameter } from '@/backend/BackendBase'

interface Props {
  projects: ProjectDataDTO[]
  selectedProjectId?: string
}

const props = defineProps<Props>()
const emit = defineEmits(['close', 'uploaded'])

const formData = ref({
  projectId: props.selectedProjectId || '',
  roomId: '',
  description: '',
})

const selectedFile = ref<File | null>(null)
const previewUrl = ref<string>('')
const rooms = ref<RoomDataDTO[]>([])
const isUploading = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

// Watch for project changes to load rooms
watch(
  () => formData.value.projectId,
  async (newProjectId) => {
    formData.value.roomId = ''
    rooms.value = []

    if (newProjectId) {
      await loadRooms(newProjectId)
    }
  },
)

onMounted(async () => {
  if (formData.value.projectId) {
    await loadRooms(formData.value.projectId)
  }
})

async function loadRooms(projectId: string) {
  try {
    rooms.value = await Backend.getRoomListByProjectId(projectId)
  } catch (error) {
    console.error('Error loading rooms:', error)
  }
}

function handleFileChange(event: Event) {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]

  if (file) {
    // Validate file size (10MB)
    if (file.size > 10 * 1024 * 1024) {
      errorMessage.value = 'Plik jest za duży. Maksymalny rozmiar to 10MB.'
      return
    }

    // Validate file type
    if (!file.type.startsWith('image/')) {
      errorMessage.value = 'Wybierz plik obrazu (JPG, PNG, GIF).'
      return
    }

    selectedFile.value = file
    errorMessage.value = ''

    // Create preview
    const reader = new FileReader()
    reader.onload = (e) => {
      previewUrl.value = e.target?.result as string
    }
    reader.readAsDataURL(file)
  }
}

async function uploadPhoto() {
  if (!selectedFile.value) {
    errorMessage.value = 'Wybierz plik do przesłania.'
    return
  }

  if (!formData.value.projectId) {
    errorMessage.value = 'Wybierz projekt.'
    return
  }

  try {
    isUploading.value = true
    errorMessage.value = ''
    successMessage.value = ''

    const userId = getCurrentUserId()
    if (!userId) {
      errorMessage.value = 'Nie znaleziono użytkownika.'
      return
    }

    const fileParam: FileParameter = {
      data: selectedFile.value,
      fileName: selectedFile.value.name,
    }

    await Backend.uploadPhoto(
      formData.value.description || 'Brak opisu',
      'local', // storage provider
      formData.value.roomId || undefined,
      formData.value.projectId,
      userId,
      fileParam,
    )

    successMessage.value = 'Zdjęcie zostało przesłane!'

    setTimeout(() => {
      emit('uploaded')
    }, 1000)
  } catch (error) {
    console.error('Error uploading photo:', error)
    errorMessage.value = 'Błąd podczas przesyłania zdjęcia.'
  } finally {
    isUploading.value = false
  }
}
</script>

<style scoped>
.upload-modal {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 8px 32px var(--shadow-dark);
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
}

@media (max-width: 768px) {
  .upload-modal {
    width: 95%;
  }
}
</style>
