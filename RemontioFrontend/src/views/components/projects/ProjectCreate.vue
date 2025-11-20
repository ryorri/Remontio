<template>
  <MainLayout>
    <div class="projects-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="projects-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-plus-circle" aria-hidden="true"></i>
          Nowy projekt
        </h1>
        <form @submit.prevent="onSubmit" class="status-form">
          <div class="mb-3">
            <label class="form-label">Nazwa projektu *</label>
            <input
              v-model.trim="name"
              class="form-control"
              required
              :disabled="submitting"
              placeholder="np. Remont kuchni"
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Opis</label>
            <textarea
              v-model.trim="description"
              class="form-control"
              rows="4"
              :disabled="submitting"
              placeholder="Krótki opis projektu..."
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
            <button
              type="submit"
              class="btn btn-primary header-btn"
              :disabled="submitting || !canSubmit"
            >
              <span v-if="!submitting"
                ><i class="fas fa-save" aria-hidden="true"></i> Utwórz projekt</span
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
            Projekt utworzony. Przekierowywanie...
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { Backend } from '@/main'
import { StatusEnum } from '@/backend/BackendBase'
import { getExtendedStatusLabel } from '@/helpers/statusEnumFormatter'

const router = useRouter()

const name = ref('')
const description = ref('')
const initialStatus = ref<StatusEnum>(StatusEnum._0)
const submitting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

const statuses = getExtendedStatusLabel()

const canSubmit = computed(() => name.value.trim().length > 0)

function cancel() {
  router.push({ name: 'ProjectList' })
}

function getCurrentUserId(): string | undefined {
  // Directly read from localStorage (as requested) using the same key RemontioBackend uses.
  try {
    const raw = localStorage.getItem('remontio_user_data')
    if (!raw) return undefined
    const parsed = JSON.parse(raw)
    return parsed.id
  } catch {
    return undefined
  }
}

async function onSubmit() {
  if (!canSubmit.value) return
  submitting.value = true
  error.value = null
  success.value = false
  try {
    const ok = await Backend.createProject({
      name: name.value,
      description: description.value || undefined,
      createdAt: new Date(),
      userId: getCurrentUserId(),
      status: initialStatus.value,
    })
    if (ok) {
      success.value = true
      setTimeout(() => router.push({ name: 'ProjectList' }), 1000)
    } else {
      error.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się utworzyć projektu.'
  } finally {
    submitting.value = false
  }
}
</script>

<style scoped>
.details-wrapper {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 6px 20px var(--shadow-light);
}
.form-label {
  font-weight: 600;
  margin-bottom: 6px;
}
.form-control {
  border-radius: 8px;
  padding: 10px 12px;
}
</style>
