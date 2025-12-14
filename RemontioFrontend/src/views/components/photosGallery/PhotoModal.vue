<template>
  <div class="modal-overlay" @click.self="$emit('close')">
    <div class="photo-modal">
      <div class="modal-header">
        <h3>
          <font-awesome-icon :icon="['fas', 'eye']" />
          {{ photo.fileName || 'Zdjęcie' }}
        </h3>
        <div class="header-actions">
          <button @click="$emit('delete', photo)" class="icon-btn delete-btn" title="Usuń">
            <font-awesome-icon :icon="['fas', 'trash-can']" />
          </button>
          <button @click="$emit('close')" class="icon-btn">
            <font-awesome-icon :icon="['fas', 'times']" />
          </button>
        </div>
      </div>

      <div class="modal-body">
        <div class="image-container">
          <img
            v-if="photo.url"
            :src="`https://localhost:7259${photo.url}`"
            :alt="photo.fileName || 'Photo'"
            class="modal-image"
            @error="imageLoadError = true"
          />
          <div v-else-if="imageLoadError" class="image-error">
            <font-awesome-icon :icon="['fas', 'info-circle']" size="4x" />
            <p>Nie można załadować zdjęcia</p>
          </div>
          <div v-else class="image-placeholder">
            <font-awesome-icon :icon="['fas', 'cube']" size="4x" />
          </div>
        </div>

        <div class="photo-details">
          <div class="detail-section">
            <h4 class="section-title">
              <font-awesome-icon :icon="['fas', 'info-circle']" />
              Szczegóły
            </h4>

            <div class="meta-grid">
              <div v-if="photo.fileName" class="meta-item">
                <font-awesome-icon :icon="['fas', 'cube']" />
                <div class="meta-text">
                  <span class="meta-label">Nazwa pliku</span>
                  <span class="meta-value">{{ photo.fileName }}</span>
                </div>
              </div>

              <div v-if="photo.size" class="meta-item">
                <font-awesome-icon :icon="['fas', 'ruler-combined']" />
                <div class="meta-text">
                  <span class="meta-label">Rozmiar</span>
                  <span class="meta-value">{{ formatFileSize(photo.size) }}</span>
                </div>
              </div>

              <div v-if="photo.contentType" class="meta-item">
                <font-awesome-icon :icon="['fas', 'cube']" />
                <div class="meta-text">
                  <span class="meta-label">Typ pliku</span>
                  <span class="meta-value">{{ photo.contentType }}</span>
                </div>
              </div>

              <div v-if="photo.createdAt" class="meta-item">
                <font-awesome-icon :icon="['fas', 'globe']" />
                <div class="meta-text">
                  <span class="meta-label">Data dodania</span>
                  <span class="meta-value">{{ formatDate(photo.createdAt) }}</span>
                </div>
              </div>
              <div v-if="projectName" class="meta-item">
                <font-awesome-icon :icon="['fas', 'folder']" />
                <div class="meta-text">
                  <span class="meta-label">Projekt</span>
                  <span class="meta-value">{{ projectName }}</span>
                </div>
              </div>

              <div v-if="roomName" class="meta-item">
                <font-awesome-icon :icon="['fas', 'door-open']" />
                <div class="meta-text">
                  <span class="meta-label">Pokój</span>
                  <span class="meta-value">{{ roomName }}</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Backend } from '@/main'
import type { PhotoDataDTO, ProjectDataDTO, RoomDataDTO } from '@/backend/BackendBase'
import { formatDate } from '@/helpers/dateFormatter'

interface Props {
  photo: PhotoDataDTO
}

const props = defineProps<Props>()
defineEmits(['close', 'delete'])

const imageLoadError = ref(false)
const projectName = ref<string>('')
const roomName = ref<string>('')

onMounted(async () => {
  await loadProjectAndRoomNames()
})

async function loadProjectAndRoomNames() {
  try {
    if (props.photo.projectId) {
      const project: ProjectDataDTO = await Backend.getProjectById(props.photo.projectId)
      projectName.value = project.name || props.photo.projectId
    }

    if (props.photo.roomId) {
      const room: RoomDataDTO = await Backend.getRoomById(props.photo.roomId)
      roomName.value = room.name || props.photo.roomId
    }
  } catch (error) {
    console.error('Error loading project/room names:', error)
    // Fallback to IDs if loading fails
    projectName.value = props.photo.projectId || ''
    roomName.value = props.photo.roomId || ''
  }
}

function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}
</script>

<style scoped>
.modal-overlay {
  background: rgba(0, 0, 0, 0.8);
  padding: 1rem;
}

.photo-modal {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 8px 32px var(--shadow-dark);
  max-width: 1200px;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.modal-header {
  flex-shrink: 0;
}

.modal-header h3 {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  flex: 1;
}

.header-actions {
  display: flex;
  gap: 0.5rem;
  align-items: center;
  flex-shrink: 0;
}

.delete-btn {
  color: var(--color-red);
}

.delete-btn:hover {
  background: var(--color-red);
  color: var(--color-bg-white);
}

.modal-body {
  overflow-y: auto;
  flex: 1;
}

.image-container {
  width: 100%;
  max-height: 600px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: var(--color-bg-light-gray);
  border-radius: 12px;
  overflow: hidden;
  margin-bottom: 2rem;
}

.modal-image {
  max-width: 100%;
  max-height: 600px;
  width: auto;
  height: auto;
  object-fit: contain;
  display: block;
}

.image-placeholder,
.image-error {
  padding: 4rem 2rem;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  color: var(--color-text-medium);
}

.image-error {
  color: var(--color-red);
}

.photo-details {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.detail-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

@media (max-width: 768px) {
  .photo-modal {
    width: 95%;
    max-height: 95vh;
  }

  .image-container {
    max-height: 400px;
    margin-bottom: 1.5rem;
  }

  .modal-image {
    max-height: 400px;
  }
}
</style>
