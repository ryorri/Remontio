<template>
  <div class="photo-grid-container">
    <div class="photo-grid">
      <div v-for="photo in photos" :key="photo.id" class="photo-card" @click="$emit('view', photo)">
        <div class="photo-image-wrapper">
          <img
            v-if="photo.url"
            :src="`https://localhost:7259${photo.url}`"
            :alt="photo.fileName || 'Photo'"
            class="photo-image"
            @error="handleImageError"
          />
          <div v-else class="photo-placeholder">
            <font-awesome-icon :icon="['fas', 'cube']" size="3x" />
          </div>

          <div class="photo-overlay">
            <button @click.stop="$emit('view', photo)" class="overlay-btn" title="Zobacz">
              <font-awesome-icon :icon="['fas', 'eye']" />
            </button>
            <button
              @click.stop="$emit('delete', photo)"
              class="overlay-btn delete-btn"
              title="Usuń"
            >
              <font-awesome-icon :icon="['fas', 'trash-can']" />
            </button>
          </div>
        </div>

        <div class="photo-info">
          <div class="photo-filename">
            {{ photo.fileName || 'Bez nazwy' }}
          </div>
          <div class="photo-meta">
            <span v-if="photo.size" class="meta-item">
              <font-awesome-icon :icon="['fas', 'cube']" />
              {{ formatFileSize(photo.size) }}
            </span>
            <span v-if="photo.createdAt" class="meta-item">
              <font-awesome-icon :icon="['fas', 'globe']" />
              {{ formatDate(photo.createdAt) }}
            </span>
          </div>
          <div v-if="getProjectName(photo.projectId)" class="photo-project">
            <font-awesome-icon :icon="['fas', 'folder']" />
            {{ getProjectName(photo.projectId) }}
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { PhotoDataDTO } from '@/backend/BackendBase'
import { formatDate } from '@/helpers/dateFormatter'

interface Props {
  photos: PhotoDataDTO[]
}

defineProps<Props>()
defineEmits(['view', 'delete'])

function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B'
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

function getProjectName(projectId?: string): string {
  // This would ideally come from a store or prop with project data
  // For now, return empty string
  return ''
}

function handleImageError(event: Event) {
  const img = event.target as HTMLImageElement
  img.style.display = 'none'
  const parent = img.parentElement
  if (parent) {
    parent.innerHTML = `
      <div class="photo-placeholder">
        <i class="fas fa-cube fa-3x"></i>
        <p>Nie można załadować</p>
      </div>
    `
  }
}
</script>

<style scoped>
.photo-info {
  padding: 1rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  flex: 1;
}

.photo-filename {
  font-weight: 600;
  font-size: 0.95rem;
  color: var(--color-text-dark);
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.photo-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.75rem;
  font-size: 0.85rem;
  color: var(--color-text-medium);
}

.photo-meta .meta-item {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  padding: 0;
  background: none;
  box-shadow: none;
}

.photo-project {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0.75rem;
  background: var(--color-bg-blue-pale);
  color: var(--color-primary-blue);
  border-radius: 8px;
  font-size: 0.85rem;
  font-weight: 600;
}
</style>
