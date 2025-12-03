<template>
  <MainLayout>
    <div class="budgets-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="budgets-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-plus-circle" aria-hidden="true"></i>
          Nowy budżet
        </h1>
        <form @submit.prevent="onSubmit" class="budget-form">
          <div class="mb-3">
            <label class="form-label">Nazwa budżetu *</label>
            <input
              v-model.trim="name"
              class="form-control"
              required
              :disabled="submitting"
              placeholder="np. Budżet kuchni"
            />
          </div>

          <div class="mb-3">
            <label class="form-label">Opis</label>
            <textarea
              v-model.trim="description"
              class="form-control"
              rows="4"
              :disabled="submitting"
              placeholder="Krótki opis budżetu..."
            ></textarea>
          </div>

          <div class="row g-3 mb-3">
            <div class="col-12 col-md-4">
              <label class="form-label">Szacowana cena (PLN)</label>
              <input
                v-model.number="estimatedPrice"
                type="number"
                step="0.01"
                min="0"
                class="form-control"
                :disabled="submitting"
                placeholder="0.00"
              />
            </div>
          </div>

          <div class="mb-3">
            <label class="form-label">Projekt *</label>
            <select v-model="projectId" class="form-select" required :disabled="submitting">
              <option value="">Wybierz projekt</option>
              <option v-for="project in projects" :key="project.id" :value="project.id">
                {{ project.name }}
              </option>
            </select>
          </div>

          <div class="mb-4">
            <label class="form-label">Pokój *</label>
            <select
              v-model="roomId"
              class="form-select"
              required
              :disabled="submitting || !projectId"
            >
              <option value="">Wybierz pokój</option>
              <option v-for="room in filteredRooms" :key="room.id" :value="room.id">
                {{ room.name }}
              </option>
            </select>
            <small v-if="!projectId" class="text-muted">Najpierw wybierz projekt</small>
          </div>

          <div class="d-flex flex-wrap gap-3">
            <button
              type="submit"
              class="btn btn-primary header-btn"
              :disabled="submitting || !canSubmit"
            >
              <span v-if="!submitting"
                ><i class="fas fa-save" aria-hidden="true"></i> Utwórz budżet</span
              >
              <span v-else>Tworzenie...</span>
            </button>
            <button
              type="button"
              class="btn btn-secondary header-btn"
              @click="cancel"
              :disabled="submitting"
            >
              <i class="fas fa-arrow-left" aria-hidden="true"></i> Anuluj
            </button>
          </div>

          <div v-if="error" class="alert alert-danger mt-3" role="alert">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3" role="alert">
            Budżet utworzony. Przekierowywanie...
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '../../layouts/MainLayout.vue'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Backend } from '@/main'
import type { ProjectDataDTO, RoomDataDTO } from '@/backend/BackendBase'
import { getCurrentUserId } from '@/helpers/userHelpers'

const router = useRouter()

const name = ref('')
const description = ref('')
const total = ref<number | undefined>(undefined)
const spent = ref<number | undefined>(undefined)
const estimatedPrice = ref<number | undefined>(undefined)
const projectId = ref('')
const roomId = ref('')

const projects = ref<ProjectDataDTO[]>([])
const rooms = ref<RoomDataDTO[]>([])

const submitting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

const filteredRooms = computed(() => {
  if (!projectId.value) return []
  return rooms.value.filter((room) => room.projectId === projectId.value)
})

const canSubmit = computed(() => name.value.trim().length > 0 && projectId.value && roomId.value)

async function loadData() {
  try {
    const userId = getCurrentUserId()
    if (!userId) {
      error.value = 'Nie znaleziono ID użytkownika'
      return
    }

    projects.value = await Backend.getProjectListByUserId(userId)
    rooms.value = await Backend.getRoomListByUserId(userId)
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się załadować danych'
  }
}

function cancel() {
  router.push({ name: 'BudgetList' })
}

async function onSubmit() {
  if (!canSubmit.value) return
  submitting.value = true
  error.value = null
  success.value = false

  try {
    const ok = await Backend.createBudget({
      name: name.value,
      description: description.value || undefined,
      createAt: new Date(),
      total: 0,
      spent: 0,
      estimatedPrice: estimatedPrice.value,
      projectId: projectId.value,
      roomId: roomId.value,
      userId: getCurrentUserId(),
    })

    if (ok) {
      success.value = true
      setTimeout(() => router.push({ name: 'BudgetList' }), 1000)
    } else {
      error.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się utworzyć budżetu.'
  } finally {
    submitting.value = false
  }
}

onMounted(async () => {
  await loadData()
})
</script>

<style scoped>
.budgets-container {
  width: 100%;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.budgets-title {
  font-size: 2.5rem;
  font-weight: 700;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
}

.budget-form .form-label {
  font-weight: 600;
  color: var(--color-text-dark);
  margin-bottom: 0.5rem;
}

.budget-form .form-control,
.budget-form .form-select {
  border: 1px solid var(--color-bg-light-gray);
  border-radius: 8px;
  padding: 0.75rem;
  transition: all 0.2s ease;
}

.budget-form .form-control:focus,
.budget-form .form-select:focus {
  border-color: var(--color-primary-blue);
  box-shadow: 0 0 0 0.2rem rgba(59, 130, 246, 0.1);
}

.header-btn {
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  border-radius: 8px;
  transition: all 0.2s ease;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
