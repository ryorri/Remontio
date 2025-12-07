<template>
  <MainLayout>
    <div class="contacts-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="contacts-title mb-4 d-flex align-items-center gap-2">
          <font-awesome-icon icon="calculator" />
          Nowa kalkulacja
        </h1>

        <!-- Progress steps -->
        <div class="steps-container mb-4">
          <div class="step" :class="{ active: currentStep === 1, completed: currentStep > 1 }">
            <div class="step-number">1</div>
            <div class="step-label">Wybór parametrów</div>
          </div>
          <div class="step-divider"></div>
          <div class="step" :class="{ active: currentStep === 2, completed: currentStep > 2 }">
            <div class="step-number">2</div>
            <div class="step-label">Szczegóły kalkulacji</div>
          </div>
        </div>

        <!-- Step 1: Project, Room, Type Selection -->
        <div v-if="currentStep === 1">
          <form @submit.prevent="goToStep2">
            <div class="mb-3">
              <label class="form-label">Projekt *</label>
              <select
                v-model="selectedProjectId"
                class="form-select"
                required
                :disabled="loadingProjects"
                @change="onProjectChange"
              >
                <option value="">
                  {{ loadingProjects ? 'Ładowanie projektów...' : 'Wybierz projekt' }}
                </option>
                <option v-for="project in projects" :key="project.id" :value="project.id">
                  {{ project.name }}
                </option>
              </select>
            </div>

            <div class="mb-3">
              <label class="form-label">Pokój *</label>
              <select
                v-model="selectedRoomId"
                class="form-select"
                required
                :disabled="!selectedProjectId || loadingRooms"
              >
                <option value="">
                  {{
                    !selectedProjectId
                      ? 'Najpierw wybierz projekt'
                      : loadingRooms
                        ? 'Ładowanie pokoi...'
                        : 'Wybierz pokój'
                  }}
                </option>
                <option v-for="room in rooms" :key="room.id" :value="room.id">
                  {{ room.name }}
                </option>
              </select>
            </div>

            <div class="mb-4">
              <label class="form-label">Typ kalkulacji *</label>
              <select v-model="selectedType" class="form-select" required>
                <option value="">Wybierz typ kalkulacji</option>
                <option v-for="type in calculationTypes" :key="type.value" :value="type.value">
                  {{ type.label }}
                </option>
              </select>
            </div>

            <div class="d-flex flex-wrap gap-3">
              <button
                type="submit"
                class="btn btn-primary header-btn"
                :disabled="!canProceedToStep2"
              >
                <font-awesome-icon icon="arrow-right" /> Dalej
              </button>
              <button type="button" class="btn btn-secondary header-btn" @click="cancel">
                <font-awesome-icon icon="arrow-left" /> Anuluj
              </button>
            </div>

            <div v-if="error" class="alert alert-danger mt-3">{{ error }}</div>
          </form>
        </div>

        <!-- Step 2: Calculation Details -->
        <div v-if="currentStep === 2">
          <div class="meta-grid mb-4">
            <div class="meta-item">
              <font-awesome-icon icon="folder" />
              <div class="meta-text">
                <span class="meta-label">Projekt</span>
                <span class="meta-value">{{ selectedProjectName }}</span>
              </div>
            </div>
            <div class="meta-item">
              <font-awesome-icon icon="door-open" />
              <div class="meta-text">
                <span class="meta-label">Pokój</span>
                <span class="meta-value">{{ selectedRoomName }}</span>
              </div>
            </div>
            <div class="meta-item">
              <font-awesome-icon icon="calculator" />
              <div class="meta-text">
                <span class="meta-label">Typ</span>
                <span class="meta-value">{{ selectedTypeName }}</span>
              </div>
            </div>
          </div>

          <!-- Type 0: Wall Calculation -->
          <WallCalculation
            v-if="selectedType === CalculationsTypeEnum._0"
            :room-id="selectedRoomId"
            :submitting="submitting"
            @back="goBackToStep1"
            @submit="handleCalculationSubmit"
          />

          <!-- Type 1: Floor Calculation -->
          <FloorCalculation
            v-else-if="selectedType === CalculationsTypeEnum._1"
            :room-id="selectedRoomId"
            :submitting="submitting"
            @back="goBackToStep1"
            @submit="handleCalculationSubmit"
          />

          <!-- Type 2: Repair Wall Calculation -->
          <RepairWallCalculation
            v-else-if="selectedType === CalculationsTypeEnum._2"
            :room-id="selectedRoomId"
            :submitting="submitting"
            @back="goBackToStep1"
            @submit="handleCalculationSubmit"
          />

          <!-- Type 3: Paint Calculation -->
          <PaintCalculation
            v-else-if="selectedType === CalculationsTypeEnum._3"
            :room-id="selectedRoomId"
            :submitting="submitting"
            @back="goBackToStep1"
            @submit="handleCalculationSubmit"
          />

          <!-- Type 4: Custom Value Calculation -->
          <CustomValueCalculation
            v-else-if="selectedType === CalculationsTypeEnum._4"
            :room-id="selectedRoomId"
            :submitting="submitting"
            @back="goBackToStep1"
            @submit="handleCalculationSubmit"
          />
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import WallCalculation from './calculatorTypes/WallCalculation.vue'
import PaintCalculation from './calculatorTypes/PaintCalculation.vue'
import RepairWallCalculation from './calculatorTypes/RepairWallCalculation.vue'
import FloorCalculation from './calculatorTypes/FloorCalculation.vue'
import CustomValueCalculation from './calculatorTypes/CustomValueCalculation.vue'
import { Backend } from '@/main'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { getAllTypesOptions } from '@/helpers/calculationTypeEnumFormatter'
import type { ProjectDataDTO, RoomDataDTO } from '@/backend/BackendBase'
import { CalculationsTypeEnum } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const router = useRouter()
const currentStep = ref(1)

// Step 1 data
const projects = ref<ProjectDataDTO[]>([])
const rooms = ref<RoomDataDTO[]>([])
const selectedProjectId = ref('')
const selectedRoomId = ref('')
const selectedType = ref<CalculationsTypeEnum | ''>('')
const calculationTypes = getAllTypesOptions()

const loadingProjects = ref(true)
const loadingRooms = ref(false)

// Form state
const error = ref('')
const success = ref(false)
const submitting = ref(false)

const canProceedToStep2 = computed(() => {
  return selectedProjectId.value && selectedRoomId.value && selectedType.value !== ''
})

const selectedProjectName = computed(() => {
  return projects.value.find((p) => p.id === selectedProjectId.value)?.name || ''
})

const selectedRoomName = computed(() => {
  return rooms.value.find((r) => r.id === selectedRoomId.value)?.name || ''
})

const selectedTypeName = computed(() => {
  return calculationTypes.find((t) => t.value === selectedType.value)?.label || ''
})

const fetchProjects = async () => {
  try {
    loadingProjects.value = true
    const userId = getCurrentUserId()
    if (!userId) {
      error.value = 'Nie znaleziono użytkownika'
      return
    }
    projects.value = await Backend.getProjectListByUserId(userId)
  } catch (e) {
    console.error('Error fetching projects:', e)
    error.value = 'Nie udało się załadować projektów'
  } finally {
    loadingProjects.value = false
  }
}

const onProjectChange = async () => {
  selectedRoomId.value = ''
  rooms.value = []

  if (!selectedProjectId.value) return

  try {
    loadingRooms.value = true
    rooms.value = await Backend.getRoomListByProjectId(selectedProjectId.value)
  } catch (e) {
    console.error('Error fetching rooms:', e)
    error.value = 'Nie udało się załadować pokoi'
  } finally {
    loadingRooms.value = false
  }
}

const goToStep2 = () => {
  error.value = ''
  currentStep.value = 2
}

const goBackToStep1 = () => {
  currentStep.value = 1
  error.value = ''
  success.value = false
}

const handleCalculationSubmit = async (data: {
  calculationName: string
  totalEfficiency?: number
  value?: number
}) => {
  try {
    submitting.value = true
    error.value = ''

    const calculationValue = data.totalEfficiency ?? data.value ?? 0

    await Backend.createCalculation(selectedRoomId.value, {
      name: data.calculationName,
      value: calculationValue,
      type: selectedType.value as CalculationsTypeEnum,
    })

    success.value = true
    setTimeout(() => {
      router.push({ name: 'CalculatorList' })
    }, 1500)
  } catch (e) {
    console.error('Error creating calculation:', e)
    error.value = 'Nie udało się utworzyć kalkulacji'
  } finally {
    submitting.value = false
  }
}

const cancel = () => {
  router.push({ name: 'CalculatorList' })
}

onMounted(() => {
  fetchProjects()
})
</script>

<style scoped>
.steps-container {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1.5rem;
  margin-bottom: 2rem;
  padding: 2rem;
  background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
  border-radius: 16px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.step {
  display: flex;
  align-items: center;
  gap: 0.875rem;
  padding: 1rem 2rem;
  background: white;
  border-radius: 12px;
  border: 2px solid #dee2e6;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
}

.step.active {
  border-color: var(--color-primary-purple);
  background: linear-gradient(135deg, #ffffff 0%, #f3f4ff 100%);
  box-shadow: 0 4px 12px rgba(143, 159, 230, 0.25);
  transform: translateY(-2px);
}

.step.completed {
  border-color: #28a745;
  background: linear-gradient(135deg, #ffffff 0%, #f0fff4 100%);
  box-shadow: 0 4px 12px rgba(40, 167, 69, 0.2);
}

.step-number {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  background: linear-gradient(135deg, #e9ecef 0%, #dee2e6 100%);
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 700;
  font-size: 1.1rem;
  color: #6c757d;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.step.active .step-number {
  background: linear-gradient(
    135deg,
    var(--color-primary-purple) 0%,
    var(--color-primary-pink) 100%
  );
  color: white;
  box-shadow: 0 3px 8px rgba(143, 159, 230, 0.4);
}

.step.completed .step-number {
  background: linear-gradient(135deg, #28a745 0%, #20c997 100%);
  color: white;
  box-shadow: 0 3px 8px rgba(40, 167, 69, 0.35);
}

.step-label {
  font-weight: 600;
  font-size: 0.95rem;
  color: #495057;
  transition: all 0.3s ease;
}

.step.active .step-label {
  color: var(--color-primary-purple);
  font-weight: 700;
}

.step.completed .step-label {
  color: #28a745;
  font-weight: 700;
}

.step-divider {
  width: 80px;
  height: 3px;
  background: linear-gradient(90deg, #dee2e6 0%, #ced4da 50%, #dee2e6 100%);
  border-radius: 2px;
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1rem;
  padding: 1.5rem;
  background: var(--color-bg-light-gray);
  border-radius: 12px;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: white;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
}

.meta-item svg {
  font-size: 1.5rem;
  color: var(--color-primary);
  flex-shrink: 0;
}

.meta-text {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  min-width: 0;
}

.meta-label {
  font-size: 0.75rem;
  color: #666;
  text-transform: uppercase;
  font-weight: 600;
  letter-spacing: 0.5px;
}

.meta-value {
  font-size: 1rem;
  font-weight: 600;
  color: #333;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
}

.section-title svg {
  color: var(--color-primary);
}

.calculation-summary {
  padding: 1.5rem;
  background: var(--color-bg-light-gray);
  border-radius: 12px;
}

.summary-box {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: white;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
}

.summary-label {
  font-weight: 600;
  color: #666;
}

.summary-value {
  font-weight: 700;
  color: var(--color-primary);
  font-size: 1.125rem;
}

.header-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  transition: all 0.3s ease;
}

.header-btn:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

@media (max-width: 768px) {
  .steps-container {
    flex-direction: column;
  }

  .step-divider {
    width: 2px;
    height: 30px;
  }

  .meta-grid {
    grid-template-columns: 1fr;
  }
}
</style>
