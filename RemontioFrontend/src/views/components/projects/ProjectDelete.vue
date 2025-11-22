<template>
  <MainLayout>
    <div class="projects-container">
      <div class="details-wrapper p-4" style="max-width: 860px; margin: 0 auto">
        <h1 class="projects-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-trash" aria-hidden="true"></i>
          Usuń projekt
        </h1>
        <div v-if="isDeleting" class="loading-state">
          <div class="spinner" />
          <p>Usuwanie projektu...</p>
        </div>
        <div v-else>
          <div class="alert alert-warning d-flex align-items-start gap-3" role="alert">
            <i class="fas fa-exclamation-triangle mt-1" aria-hidden="true"></i>
            <div>
              <strong>Czy na pewno chcesz usunąć ten projekt?</strong><br />
              Tej operacji nie można cofnąć. Wszystkie powiązane dane (pokoje, zadania, budżety
              itd.) zostaną utracone.
            </div>
          </div>
          <div class="d-flex flex-wrap gap-3 mt-3">
            <button
              class="btn header-btn btn-primary"
              type="button"
              @click="goBack"
              :disabled="isDeleting"
            >
              <i class="fas fa-arrow-left" aria-hidden="true"></i> Anuluj
            </button>
            <button
              class="btn header-btn danger-btn"
              type="button"
              @click="onDelete"
              :disabled="isDeleting"
            >
              <i class="fas fa-trash" aria-hidden="true"></i> Usuń projekt
            </button>
          </div>
          <div v-if="error" class="alert alert-danger mt-3" role="alert">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3" role="alert">
            Projekt został usunięty. Przekierowywanie...
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { useRoute, useRouter } from 'vue-router'
import { ref } from 'vue'
import { Backend } from '@/main'

const route = useRoute()
const router = useRouter()
const projectId = route.params.projectId as string

const isDeleting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

function goBack() {
  router.push({ name: 'ProjectDetails', params: { projectId } })
}

async function onDelete() {
  if (!projectId) {
    error.value = 'Brak identyfikatora projektu.'
    return
  }
  error.value = null
  isDeleting.value = true
  try {
    const ok = await Backend.deleteProject(projectId)
    if (ok) {
      success.value = true
      setTimeout(() => router.push({ name: 'ProjectList' }), 1000)
    } else {
      error.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się usunąć projektu.'
  } finally {
    isDeleting.value = false
  }
}
</script>

<style scoped>
.danger-btn {
  background: #dc3545;
  color: #fff;
  border: none;
  transition: background 0.2s ease;
}
.danger-btn:hover {
  background: #c82333;
}
.danger-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style>
