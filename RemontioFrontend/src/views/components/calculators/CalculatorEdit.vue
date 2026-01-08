<template>
  <MainLayout>
    <div class="contacts-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="contacts-title mb-4 d-flex align-items-center gap-2">
          <font-awesome-icon icon="calculator" />
          Edycja kalkulacji
        </h1>

        <div v-if="loading" class="loading-state">
          <div class="spinner"></div>
          <p>Ładowanie kalkulacji...</p>
        </div>

        <div v-else-if="error && !calculation" class="alert alert-danger">
          {{ error }}
        </div>

        <div v-else-if="calculation">
          <div class="meta-grid mb-4">
            <div class="meta-item">
              <font-awesome-icon icon="folder" />
              <div class="meta-text">
                <span class="meta-label">Projekt</span>
                <span class="meta-value">{{ projectName }}</span>
              </div>
            </div>
            <div class="meta-item">
              <font-awesome-icon icon="door-open" />
              <div class="meta-text">
                <span class="meta-label">Pokój</span>
                <span class="meta-value">{{ roomName }}</span>
              </div>
            </div>
            <div class="meta-item">
              <font-awesome-icon icon="calculator" />
              <div class="meta-text">
                <span class="meta-label">Typ</span>
                <span class="meta-value">{{ typeName }}</span>
              </div>
            </div>
          </div>

          <form @submit.prevent="handleSubmit">
            <div class="params-box mb-4">
              <div class="mb-3">
                <label class="form-label">Nazwa kalkulacji *</label>
                <input
                  v-model.trim="calculationName"
                  type="text"
                  class="form-control"
                  placeholder="Nazwa kalkulacji"
                  required
                />
              </div>

              <div class="mb-3">
                <label class="form-label">Wartość *</label>
                <input
                  v-model.number="calculationValue"
                  type="number"
                  step="0.01"
                  min="0"
                  class="form-control"
                  placeholder="Wartość"
                  required
                />
                <small class="text-muted">
                  Wartość kalkulacji (ilość materiału, powierzchnia, itp.)
                </small>
              </div>
            </div>

            <div class="calculation-summary mb-4">
              <h4 class="mb-3">Podsumowanie</h4>
              <div class="summary-box">
                <div class="summary-item">
                  <span class="summary-label">Nazwa:</span>
                  <span class="summary-value">{{ calculationName || 'Brak nazwy' }}</span>
                </div>
                <div class="summary-item total-item">
                  <span class="summary-label">Wartość:</span>
                  <span class="summary-value">{{ calculationValue.toFixed(2) }}</span>
                </div>
              </div>
            </div>

            <div class="d-flex flex-wrap gap-3">
              <button
                type="submit"
                class="btn btn-primary header-btn"
                :disabled="submitting || !calculationName.trim() || calculationValue <= 0"
              >
                <font-awesome-icon icon="save" />
                {{ submitting ? 'Zapisywanie...' : 'Zapisz zmiany' }}
              </button>
              <button
                type="button"
                class="btn btn-secondary header-btn"
                @click="cancel"
                :disabled="submitting"
              >
                <font-awesome-icon icon="arrow-left" /> Anuluj
              </button>
            </div>

            <div v-if="error" class="alert alert-danger mt-3">{{ error }}</div>
            <div v-if="success" class="alert alert-success mt-3">
              Kalkulacja zaktualizowana. Przekierowywanie...
            </div>
          </form>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { Backend } from '@/main'
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { getSpecLabel } from '@/helpers/calculationTypeEnumFormatter'
import type { CalculationDataDTO, ProjectDataDTO, RoomDataDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const router = useRouter()
const route = useRoute()

const calculation = ref<CalculationDataDTO | null>(null)
const project = ref<ProjectDataDTO | null>(null)
const room = ref<RoomDataDTO | null>(null)
const loading = ref(true)
const error = ref('')
const success = ref(false)
const submitting = ref(false)

const calculationName = ref('')
const calculationValue = ref(0)

const projectName = ref('')
const roomName = ref('')
const typeName = ref('')

const fetchCalculation = async () => {
  const calculationId = route.params.calculationId as string
  if (!calculationId) {
    error.value = 'Brak ID kalkulacji'
    loading.value = false
    return
  }

  try {
    loading.value = true
    error.value = ''

    calculation.value = await Backend.getCalculationById(calculationId)

    if (!calculation.value) {
      error.value = 'Nie znaleziono kalkulacji'
      return
    }

    // Populate form fields
    calculationName.value = calculation.value.name || ''
    calculationValue.value = calculation.value.value || 0
    typeName.value = getSpecLabel(calculation.value.type)

    // Fetch room data
    if (calculation.value.roomId) {
      room.value = await Backend.getRoomById(calculation.value.roomId)
      roomName.value = room.value?.name || 'Nieznany pokój'

      // Fetch project data
      if (room.value?.projectId) {
        project.value = await Backend.getProjectById(room.value.projectId)
        projectName.value = project.value?.name || 'Nieznany projekt'
      }
    }
  } catch (e) {
    console.error('Error fetching calculation:', e)
    error.value = 'Nie udało się załadować kalkulacji'
  } finally {
    loading.value = false
  }
}

const handleSubmit = async () => {
  if (!calculation.value?.id) {
    error.value = 'Brak ID kalkulacji'
    return
  }

  if (!calculationName.value.trim()) {
    error.value = 'Podaj nazwę kalkulacji'
    return
  }

  if (calculationValue.value <= 0) {
    error.value = 'Wartość musi być większa od 0'
    return
  }

  try {
    submitting.value = true
    error.value = ''

    await Backend.editCalculation({
      id: calculation.value.id,
      name: calculationName.value,
      value: calculationValue.value,
      type: calculation.value.type,
      roomId: calculation.value.roomId,
      projectId: calculation.value.projectId,
      userId: calculation.value.userId,
    })

    success.value = true
    setTimeout(() => {
      router.push({ name: 'CalculatorList' })
    }, 1500)
  } catch (e) {
    console.error('Error updating calculation:', e)
    error.value = 'Nie udało się zaktualizować kalkulacji'
  } finally {
    submitting.value = false
  }
}

const cancel = () => {
  router.push({ name: 'CalculatorList' })
}

onMounted(() => {
  fetchCalculation()
})
</script>
