<template>
  <MainLayout>
    <div class="budgets-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="budgets-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-plus-circle" aria-hidden="true"></i>
          Nowy pokój
        </h1>
        <form @submit.prevent="onSubmit" class="budget-form">
          <div class="mb-3">
            <label class="form-label">Projekt *</label>
            <select
              v-model="selectedProjectId"
              class="form-control"
              required
              :disabled="submitting || loadingProjects"
            >
              <option value="" disabled>Wybierz projekt...</option>
              <option v-for="p in projects" :key="p.id" :value="p.id">{{ p.name }}</option>
            </select>
            <small v-if="prefilledFromRoute" class="text-muted d-block mt-1"
              >Wstępnie wybrano</small
            >
          </div>
          <div class="mb-3">
            <label class="form-label">Nazwa pokoju *</label>
            <input
              v-model.trim="name"
              class="form-control"
              required
              :disabled="submitting"
              placeholder="np. Salon"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Opis</label>
            <textarea
              v-model.trim="description"
              class="form-control"
              rows="4"
              :disabled="submitting"
              placeholder="Krótki opis pokoju..."
            ></textarea>
          </div>

          <div class="mb-4">
            <label class="form-label">Status początkowy</label>
            <div class="row g-3">
              <div v-for="s in statuses" :key="s.value" class="col-12 col-md-6 col-lg-3">
                <label
                  :class="[
                    'status-option',
                    'w-100',
                    'd-flex',
                    'flex-column',
                    'gap-2',
                    initialStatus === s.value ? 'active' : '',
                  ]"
                >
                  <div class="d-flex align-items-center justify-content-between w-100">
                    <span :class="['project-status', 'status-' + s.value]">{{ s.label }}</span>
                    <input
                      type="radio"
                      class="form-check-input"
                      :value="s.value"
                      v-model="initialStatus"
                      :disabled="submitting"
                    />
                  </div>
                  <small class="text-muted" style="font-size: 0.7rem; line-height: 1rem">{{
                    s.description
                  }}</small>
                </label>
              </div>
            </div>
          </div>

          <div class="d-flex flex-wrap gap-3">
            <button type="submit" class="btn btn-primary" :disabled="submitting || !canSubmit">
              <i class="fas fa-save" aria-hidden="true"></i>
              {{ submitting ? 'Tworzenie...' : 'Utwórz pokój' }}
            </button>
            <button type="button" class="btn btn-secondary" @click="cancel" :disabled="submitting">
              <i class="fas fa-arrow-left" aria-hidden="true"></i> Anuluj
            </button>
          </div>

          <div v-if="error" class="alert alert-danger mt-3" role="alert">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3" role="alert">
            Pokój utworzony. Przekierowywanie...
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { Backend } from '@/main'
import { StatusEnum, type ProjectDataDTO } from '@/backend/BackendBase'
import { getExtendedStatusLabel } from '@/helpers/statusEnumFormatter'
import { getCurrentUserId } from '@/helpers/userHelpers'

const router = useRouter()
const name = ref('')
const description = ref('')
const initialStatus = ref<StatusEnum>(StatusEnum._0)
const submitting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)
const projects = ref<ProjectDataDTO[]>([])
const loadingProjects = ref(true)
const selectedProjectId = ref<string>('')
const prefilledFromRoute = ref(false)
const route = useRoute()
const routeProjectId = route.params.projectId as string | undefined

const statuses = getExtendedStatusLabel()
const canSubmit = computed(() => name.value.trim().length > 0 && selectedProjectId.value)

function cancel() {
  router.push({ name: 'RoomList' })
}

async function loadProjects() {
  try {
    loadingProjects.value = true
    const userId = getCurrentUserId()
    if (userId) {
      projects.value = await Backend.getProjectListByUserId(userId)
    }
  } catch (e: any) {
    console.error(e)
  } finally {
    loadingProjects.value = false
  }
}

onMounted(async () => {
  await loadProjects()
  if (routeProjectId) {
    const found = projects.value.find((p) => p.id === routeProjectId)
    if (found) {
      selectedProjectId.value = routeProjectId
      prefilledFromRoute.value = true
    }
  }
})

async function onSubmit() {
  if (!canSubmit.value) return
  submitting.value = true
  error.value = null
  success.value = false
  try {
    const ok = await Backend.createRoom(selectedProjectId.value, {
      name: name.value,
      description: description.value || undefined,
      createdAt: new Date(),
      status: initialStatus.value,
      userId: getCurrentUserId()!,
    })
    if (ok) {
      success.value = true
      setTimeout(
        () => router.push({ name: 'RoomList', params: { projectId: selectedProjectId.value } }),
        1000,
      )
    } else {
      error.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się utworzyć pokoju.'
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.btn {
  display: flex;
  align-items: center;
  gap: 8px;
}
</style>
