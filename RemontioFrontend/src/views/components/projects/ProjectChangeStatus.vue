<template>
  <MainLayout>
    <div class="budgets-container">
      <div class="budgets-header d-flex align-items-center flex-wrap gap-3">
        <h1 class="budgets-title mb-3">Zmień status projektu</h1>
        <div
          v-if="project && project.status !== undefined"
          class="d-flex align-items-center gap-2 mb-3"
        >
          <span :class="['project-status', 'status-' + project.status]">{{
            getStatusLabel(project.status)
          }}</span>
        </div>
      </div>

      <div v-if="isLoading" class="loading-state">
        <div class="spinner" />
        <p>Ładowanie danych projektu...</p>
      </div>

      <div v-else-if="loadError" class="empty-state" style="text-align: center; padding: 2rem">
        <i
          class="bi bi-exclamation-triangle"
          style="font-size: 3rem; color: var(--color-primary-purple)"
        ></i>
        <h2>Nie udało się pobrać projektu</h2>
        <p>{{ loadError }}</p>
        <button class="btn btn-secondary" @click="goBack">Powrót</button>
      </div>

      <form v-else class="budget-form" @submit.prevent="submitChange">
        <div class="meta-grid mb-4">
          <div class="meta-item">
            <i class="bi bi-folder"></i>
            <div class="meta-text">
              <span class="meta-label">Nazwa</span>
              <span class="meta-value">{{ project?.name }}</span>
            </div>
          </div>
          <div class="meta-item" v-if="project?.description">
            <i class="bi bi-card-text"></i>
            <div class="meta-text">
              <span class="meta-label">Opis</span>
              <span class="meta-value">{{ project?.description }}</span>
            </div>
          </div>
          <div class="meta-item">
            <i class="bi bi-clock"></i>
            <div class="meta-text">
              <span class="meta-label">Utworzono</span>
              <span class="meta-value">{{ formatDate(project?.createAt) }}</span>
            </div>
          </div>
        </div>

        <div class="status-selector mb-4">
          <h2 class="section-title" style="font-size: 1.4rem; margin-bottom: 0.75rem">
            Wybierz nowy status
          </h2>
          <div class="row g-3">
            <div v-for="s in statuses" :key="s.value" class="col-12 col-md-6 col-lg-3">
              <label
                :class="[
                  'status-option',
                  'w-100',
                  'd-flex',
                  'flex-column',
                  'align-items-start',
                  'gap-2',
                  selectedStatus === s.value ? 'active' : '',
                ]"
              >
                <div class="d-flex align-items-center justify-content-between w-100">
                  <span :class="['project-status', 'status-' + s.value]">{{ s.label }}</span>
                  <input
                    type="radio"
                    class="form-check-input"
                    :value="s.value"
                    v-model="selectedStatus"
                  />
                </div>
                <small class="text-muted" style="font-size: 0.75rem; line-height: 1.1rem">{{
                  s.description
                }}</small>
              </label>
            </div>
          </div>
        </div>

        <div class="d-flex gap-3 flex-wrap">
          <button type="submit" class="btn btn-primary" :disabled="isSubmitting">
            <i class="fas fa-save" aria-hidden="true"></i>
            {{ isSubmitting ? 'Zapisywanie...' : 'Zapisz zmiany' }}
          </button>
          <button type="button" class="btn btn-secondary" @click="goBack" :disabled="isSubmitting">
            <i class="fas fa-arrow-left" aria-hidden="true"></i> Anuluj
          </button>
        </div>

        <div v-if="submitError" class="mt-3 alert alert-danger" role="alert">
          {{ submitError }}
        </div>
        <div v-if="submitSuccess" class="mt-3 alert alert-success" role="alert">
          Status zaktualizowany pomyślnie.
        </div>
      </form>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { useRoute, useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'
import { Backend } from '@/main'
import { StatusEnum, type ProjectDataDTO } from '@/backend/BackendBase'
import { getStatusLabel, getExtendedStatusLabel } from '@/helpers/statusEnumFormatter'
import { formatDate } from '@/helpers/dateFormatter'

const route = useRoute()
const router = useRouter()
const projectId = route.params.projectId as string

const project = ref<ProjectDataDTO | null>(null)
const isLoading = ref(true)
const isSubmitting = ref(false)
const loadError = ref<string | null>(null)
const submitError = ref<string | null>(null)
const submitSuccess = ref(false)
const selectedStatus = ref<StatusEnum | null>(null)
const statuses = getExtendedStatusLabel()

onMounted(async () => {
  if (!projectId) {
    loadError.value = 'Brak identyfikatora projektu.'
    isLoading.value = false
    return
  }
  try {
    project.value = await Backend.getProjectById(projectId)
    selectedStatus.value = project.value.status ?? StatusEnum._0
  } catch (err: any) {
    console.error(err)
    loadError.value = 'Wystąpił błąd podczas pobierania projektu.'
  } finally {
    isLoading.value = false
  }
  console.log('Loaded project:', project.value)
})

async function submitChange() {
  submitError.value = null
  submitSuccess.value = false
  if (!project.value || selectedStatus.value === null) return
  if (selectedStatus.value === project.value.status) return
  isSubmitting.value = true
  try {
    const ok = await Backend.editProjectStatus(project.value.id!, selectedStatus.value.toString())
    if (ok) {
      project.value.status = selectedStatus.value
      submitSuccess.value = true
      setTimeout(() => router.push({ name: 'ProjectDetails', params: { projectId } }), 1200)
    } else {
      submitError.value = 'Serwer zwrócił niepowodzenie.'
    }
  } catch (err: any) {
    console.error(err)
    submitError.value = 'Nie udało się zaktualizować statusu.'
  } finally {
    isSubmitting.value = false
  }
}

function goBack() {
  router.push({ name: 'ProjectDetails', params: { projectId } })
}
</script>

<style scoped>
.btn {
  display: flex;
  align-items: center;
  gap: 8px;
}
</style>
