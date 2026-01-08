<template>
  <MainLayout>
    <div class="rooms-container">
      <div class="details-wrapper p-4" style="max-width: 860px; margin: 0 auto">
        <h1 class="rooms-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-trash" aria-hidden="true"></i>
          Usuń pokój
        </h1>
        <div v-if="isDeleting" class="loading-state">
          <div class="spinner" />
          <p>Usuwanie pokoju...</p>
        </div>
        <div v-else>
          <div class="alert alert-warning d-flex align-items-start gap-3" role="alert">
            <i class="fas fa-exclamation-triangle mt-1" aria-hidden="true"></i>
            <div>
              <strong>Czy na pewno chcesz usunąć ten pokój?</strong><br />
              Tej operacji nie można cofnąć. Powiązane dane (zadania, listy, budżety itd.) zostaną
              utracone.
            </div>
          </div>
          <div class="d-flex flex-wrap gap-3 mt-3">
            <button class="btn btn-secondary" type="button" @click="goBack" :disabled="isDeleting">
              <i class="fas fa-arrow-left" aria-hidden="true"></i> Anuluj
            </button>
            <button class="btn btn-danger" type="button" @click="onDelete" :disabled="isDeleting">
              <i class="fas fa-trash" aria-hidden="true"></i> Usuń pokój
            </button>
          </div>
          <div v-if="error" class="alert alert-danger mt-3" role="alert">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3" role="alert">
            Pokój został usunięty. Przekierowywanie...
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
const roomId = route.params.roomId as string

const isDeleting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

function goBack() {
  router.push({ name: 'RoomDetails', params: { roomId } })
}

async function onDelete() {
  if (!roomId) {
    error.value = 'Brak identyfikatora pokoju.'
    return
  }
  error.value = null
  isDeleting.value = true
  try {
    const ok = await Backend.deleteRoom(roomId)
    if (ok) {
      success.value = true
      setTimeout(() => router.push({ name: 'RoomList' }), 1000)
    } else {
      error.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się usunąć pokoju.'
  } finally {
    isDeleting.value = false
  }
}
</script>
<style scoped>
.btn {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: not-allowed;
}
</style>
