<template>
  <div>
    <h3 class="section-title mb-3">
      <font-awesome-icon icon="ruler-combined" />
      Podłogi w pokoju
    </h3>

    <div v-if="loadingFloors" class="loading-state">
      <div class="spinner"></div>
      <p>Ładowanie podłóg...</p>
    </div>

    <div v-else-if="floors.length === 0" class="empty-state">
      <font-awesome-icon icon="ruler-combined" style="font-size: 3rem" />
      <h3>Brak podłóg w tym pokoju</h3>
      <p>Dodaj podłogę aby wykonać kalkulację</p>

      <div class="floor-creator mt-3">
        <h4 class="mb-3">Dodaj podłogę</h4>

        <div class="mb-3">
          <label class="form-label">Nazwa podłogi *</label>
          <input
            v-model.trim="newFloorName"
            class="form-control"
            placeholder="np. Podłoga główna"
          />
        </div>

        <div class="canvas-container mb-3">
          <p class="canvas-instructions">
            <font-awesome-icon icon="info-circle" />
            Kliknij na canvas aby dodać punkty narożników podłogi. Minimum 3 punkty.
          </p>
          <div style="position: relative">
            <canvas
              ref="floorCanvas"
              @click="addPointToCanvas"
              @mousemove="updateMousePosition"
              width="600"
              height="400"
              class="floor-canvas"
            ></canvas>
            <input
              v-if="canvasInputPosition"
              ref="canvasDistanceInput"
              v-model="editDistanceValue"
              type="number"
              step="0.1"
              class="canvas-distance-input"
              :style="{
                left: canvasInputPosition.x + 'px',
                top: canvasInputPosition.y + 'px',
              }"
              @keydown.enter="applyDistanceEdit(editingDistanceIndex!)"
              @keydown.esc="cancelDistanceEdit"
              @blur="applyDistanceEdit(editingDistanceIndex!)"
            />
          </div>
          <div class="canvas-controls mt-2">
            <button
              type="button"
              class="btn btn-sm btn-outline-secondary"
              @click="undoLastPoint"
              :disabled="floorPoints.length === 0"
            >
              <font-awesome-icon icon="undo" /> Cofnij punkt
            </button>
            <button
              type="button"
              class="btn btn-sm btn-outline-danger"
              @click="clearCanvasPoints"
              :disabled="floorPoints.length === 0"
            >
              <font-awesome-icon icon="trash-can" /> Wyczyść
            </button>
            <button
              type="button"
              class="btn btn-sm btn-primary"
              @click="addFloor"
              :disabled="addingFloor || !newFloorName || floorPoints.length < 3"
            >
              <font-awesome-icon icon="plus" />
              {{ addingFloor ? 'Dodawanie...' : 'Dodaj podłogę' }}
            </button>
          </div>
        </div>

        <div v-if="floorPoints.length > 0" class="points-info">
          <p class="text-muted mb-3">
            Zaznaczono {{ floorPoints.length }} punktów | Szacowana powierzchnia:
            {{ calculatePolygonArea().toFixed(2) }} m²
          </p>

          <div v-if="floorPoints.length > 1" class="distances-list">
            <h6 class="mb-2">Odległości między punktami:</h6>
            <div
              v-for="(point, index) in floorPoints"
              :key="index"
              class="distance-item"
              v-show="index < floorPoints.length - 1 || floorPoints.length > 2"
            >
              <span class="distance-label"
                >{{ index + 1 }} → {{ index < floorPoints.length - 1 ? index + 2 : 1 }}:</span
              >
              <span
                class="distance-value"
                @click="startDistanceEdit(index)"
                v-if="
                  editingDistanceIndex !== index && floorPoints[(index + 1) % floorPoints.length]
                "
              >
                {{
                  calculateDistance(point, floorPoints[(index + 1) % floorPoints.length]!).toFixed(
                    1,
                  )
                }}
                cm
              </span>
              <input
                v-else-if="editingDistanceIndex === index"
                v-model="editDistanceValue"
                type="number"
                step="0.1"
                class="form-control form-control-sm distance-input"
                @keydown.enter="applyDistanceEdit(index)"
                @keydown.esc="cancelDistanceEdit"
                @blur="applyDistanceEdit(index)"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <div v-else>
      <div class="floors-list mb-3">
        <div v-for="floor in floors" :key="floor.id" class="floor-item">
          <div class="d-flex align-items-center gap-2 flex-grow-1">
            <input
              :id="'floor-' + floor.id"
              type="checkbox"
              :checked="selectedFloorIds.has(floor.id)"
              @change="toggleFloorSelection(floor.id)"
              class="form-check-input m-0"
            />
            <label :for="'floor-' + floor.id" class="floor-info mb-0 cursor-pointer flex-grow-1">
              <strong>{{ floor.name }}</strong>
              <br />
              <small class="text-muted">{{ floor.area.toFixed(2) }} m²</small>
            </label>
          </div>
          <button
            type="button"
            class="btn btn-sm btn-danger"
            @click="removeFloorFromList(floor.id)"
            :disabled="removingFloor"
          >
            <font-awesome-icon icon="trash-can" />
          </button>
        </div>
      </div>

      <div class="mb-3">
        <button
          type="button"
          class="btn btn-outline-primary w-100"
          @click="showAddFloorSection = !showAddFloorSection"
        >
          <font-awesome-icon :icon="showAddFloorSection ? 'chevron-up' : 'chevron-down'" />
          {{ showAddFloorSection ? 'Ukryj formularz dodawania podłogi' : 'Dodaj kolejną podłogę' }}
        </button>
      </div>

      <div v-if="showAddFloorSection" class="floor-creator mb-4">
        <h4 class="mb-3">Dodaj kolejną podłogę</h4>

        <div class="mb-3">
          <label class="form-label">Nazwa podłogi *</label>
          <input
            v-model.trim="newFloorName"
            class="form-control"
            placeholder="np. Podłoga przy oknie"
          />
        </div>

        <div class="canvas-container mb-3">
          <p class="canvas-instructions">
            <font-awesome-icon icon="info-circle" />
            Kliknij na canvas aby dodać punkty narożników podłogi. Minimum 3 punkty.
          </p>
          <div style="position: relative">
            <canvas
              ref="floorCanvas"
              @click="addPointToCanvas"
              @mousemove="updateMousePosition"
              width="600"
              height="400"
              class="floor-canvas"
            ></canvas>
            <input
              v-if="canvasInputPosition"
              ref="canvasDistanceInput"
              v-model="editDistanceValue"
              type="number"
              step="0.1"
              class="canvas-distance-input"
              :style="{
                left: canvasInputPosition.x + 'px',
                top: canvasInputPosition.y + 'px',
              }"
              @keydown.enter="applyDistanceEdit(editingDistanceIndex!)"
              @keydown.esc="cancelDistanceEdit"
              @blur="applyDistanceEdit(editingDistanceIndex!)"
            />
          </div>
          <div class="canvas-controls mt-2">
            <button
              type="button"
              class="btn btn-sm btn-outline-secondary"
              @click="undoLastPoint"
              :disabled="floorPoints.length === 0"
            >
              <font-awesome-icon icon="undo" /> Cofnij punkt
            </button>
            <button
              type="button"
              class="btn btn-sm btn-outline-danger"
              @click="clearCanvasPoints"
              :disabled="floorPoints.length === 0"
            >
              <font-awesome-icon icon="trash-can" /> Wyczyść
            </button>
            <button
              type="button"
              class="btn btn-sm btn-primary"
              @click="addFloor"
              :disabled="addingFloor || !newFloorName || floorPoints.length < 3"
            >
              <font-awesome-icon icon="plus" />
              {{ addingFloor ? 'Dodawanie...' : 'Dodaj podłogę' }}
            </button>
          </div>
        </div>

        <div v-if="floorPoints.length > 0" class="points-info">
          <p class="text-muted mb-3">
            Zaznaczono {{ floorPoints.length }} punktów | Szacowana powierzchnia:
            {{ calculatePolygonArea().toFixed(2) }} m²
          </p>

          <div v-if="floorPoints.length > 1" class="distances-list">
            <h6 class="mb-2">Odległości między punktami:</h6>
            <div
              v-for="(point, index) in floorPoints"
              :key="index"
              class="distance-item"
              v-show="index < floorPoints.length - 1 || floorPoints.length > 2"
            >
              <span class="distance-label"
                >{{ index + 1 }} → {{ index < floorPoints.length - 1 ? index + 2 : 1 }}:</span
              >
              <span
                class="distance-value"
                @click="startDistanceEdit(index)"
                v-if="
                  editingDistanceIndex !== index && floorPoints[(index + 1) % floorPoints.length]
                "
              >
                {{
                  calculateDistance(point, floorPoints[(index + 1) % floorPoints.length]!).toFixed(
                    1,
                  )
                }}
                cm
              </span>
              <input
                v-else-if="editingDistanceIndex === index"
                v-model="editDistanceValue"
                type="number"
                step="0.1"
                class="form-control form-control-sm distance-input"
                @keydown.enter="applyDistanceEdit(index)"
                @keydown.esc="cancelDistanceEdit"
                @blur="applyDistanceEdit(index)"
              />
            </div>
          </div>
        </div>
      </div>

      <div v-if="selectedFloorIds.size > 0" class="calculation-params mb-4">
        <h4 class="mb-3">Parametry kalkulacji</h4>
        <div class="params-box">
          <div class="mb-3">
            <label class="form-label">Nazwa materiału *</label>
            <input
              v-model.trim="calculationName"
              type="text"
              class="form-control"
              placeholder="np. Panele podłogowe, Wykładzina"
              required
            />
          </div>

          <div class="mb-3">
            <div class="form-check form-switch">
              <input
                class="form-check-input"
                type="checkbox"
                id="usePackageEfficiency"
                v-model="usePackageEfficiency"
              />
              <label class="form-check-label" for="usePackageEfficiency">
                Oblicz na podstawie wydajności opakowania
              </label>
            </div>
          </div>

          <div v-if="usePackageEfficiency" class="mb-3">
            <label class="form-label">Wydajność opakowania na m² *</label>
            <input
              v-model.number="packageEfficiencyPerM2"
              type="number"
              step="0.01"
              min="0.01"
              class="form-control"
              placeholder="np. 2.5"
              required
            />
            <small class="text-muted"> Ile m² pokrywa jedno opakowanie materiału </small>
          </div>

          <div v-else>
            <div class="mb-3">
              <label class="form-label">Długość elementu (m) *</label>
              <input
                v-model.number="itemLength"
                type="number"
                step="0.01"
                min="0.01"
                class="form-control"
                placeholder="np. 1.2"
                required
              />
            </div>

            <div class="mb-3">
              <label class="form-label">Szerokość elementu (m) *</label>
              <input
                v-model.number="itemWidth"
                type="number"
                step="0.01"
                min="0.01"
                class="form-control"
                placeholder="np. 0.2"
                required
              />
            </div>
          </div>
        </div>
      </div>

      <div class="calculation-summary mb-4">
        <h4 class="mb-3">Podsumowanie</h4>
        <div class="summary-box">
          <div class="summary-item">
            <span class="summary-label">Całkowita powierzchnia:</span>
            <span class="summary-value">{{ totalFloorArea.toFixed(2) }} m²</span>
          </div>
          <div v-if="selectedFloorIds.size > 0" class="summary-item">
            <span class="summary-label">Zaznaczone podłogi:</span>
            <span class="summary-value">{{ selectedFloorIds.size }} / {{ floors.length }}</span>
          </div>
          <div v-if="selectedFloorIds.size > 0" class="summary-item">
            <span class="summary-label">Powierzchnia zaznaczonych:</span>
            <span class="summary-value">{{ selectedFloorsArea.toFixed(2) }} m²</span>
          </div>
          <div v-if="calculationName" class="summary-item">
            <span class="summary-label">Materiał:</span>
            <span class="summary-value">{{ calculationName }}</span>
          </div>
          <div v-if="usePackageEfficiency && packageEfficiencyPerM2" class="summary-item">
            <span class="summary-label">Wydajność opakowania:</span>
            <span class="summary-value">{{ packageEfficiencyPerM2 }} m²</span>
          </div>
          <div v-if="!usePackageEfficiency && itemArea > 0" class="summary-item">
            <span class="summary-label">Powierzchnia elementu:</span>
            <span class="summary-value">{{ itemArea.toFixed(4) }} m²</span>
          </div>
          <div v-if="totalQuantity > 0" class="summary-item total-item">
            <span class="summary-label">{{
              usePackageEfficiency ? 'Ilość opakowań:' : 'Ilość elementów:'
            }}</span>
            <span class="summary-value">{{ totalQuantity.toFixed(2) }}</span>
          </div>
        </div>

        <button
          v-if="totalQuantity > 0 && calculationName"
          type="button"
          class="btn btn-success mt-3 w-100"
          @click="openShoppingListModal"
          :disabled="submitting"
        >
          <font-awesome-icon icon="shopping-cart" />
          Dodaj do listy zakupowej
        </button>
      </div>

      <form @submit.prevent="handleSubmit">
        <div class="d-flex flex-wrap gap-3">
          <button
            type="submit"
            class="btn btn-primary header-btn"
            :disabled="
              submitting ||
              floors.length === 0 ||
              selectedFloorIds.size === 0 ||
              !calculationName.trim() ||
              (usePackageEfficiency
                ? !packageEfficiencyPerM2 || packageEfficiencyPerM2 <= 0
                : itemArea <= 0)
            "
          >
            <font-awesome-icon icon="save" />
            {{ submitting ? 'Zapisywanie...' : 'Zapisz kalkulację' }}
          </button>
          <button
            type="button"
            class="btn btn-secondary header-btn"
            @click="$emit('back')"
            :disabled="submitting"
          >
            <font-awesome-icon icon="arrow-left" /> Wstecz
          </button>
        </div>

        <div v-if="error" class="alert alert-danger mt-3">{{ error }}</div>
      </form>
    </div>

    <AddToShoppingListModal
      :is-open="isShoppingListModalOpen"
      :calculation-data="{
        name: calculationName,
        totalEfficiency: totalQuantity,
      }"
      :room-id="roomId"
      @close="isShoppingListModalOpen = false"
    />
  </div>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import { ref, computed, watch } from 'vue'
import { cm2ToM2 } from '@/helpers/distanceFormatter'
import type { FloorDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import AddToShoppingListModal from '../AddToShoppingListModal.vue'
import { useCanvasDrawing } from '@/composables/useCanvasDrawing'

interface Floor {
  id: string
  name: string
  area: number
}

interface Props {
  roomId: string
  submitting: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  back: []
  submit: [data: { calculationName: string; totalEfficiency: number }]
}>()

const {
  points: floorPoints,
  canvas: floorCanvas,
  editingDistanceIndex,
  editDistanceValue,
  canvasInputPosition,
  canvasDistanceInput,
  addPointToCanvas,
  clearCanvasPoints,
  undoLastPoint,
  updateMousePosition,
  startDistanceEdit,
  cancelDistanceEdit,
  applyDistanceEdit,
  calculatePolygonArea,
  calculateDistance,
} = useCanvasDrawing()

const floors = ref<Floor[]>([])
const loadingFloors = ref(false)
const showAddFloorSection = ref(false)
const selectedFloorIds = ref<Set<string>>(new Set())
const calculationName = ref('')
const usePackageEfficiency = ref(true)
const packageEfficiencyPerM2 = ref<number | null>(null)
const itemLength = ref<number | null>(null)
const itemWidth = ref<number | null>(null)
const newFloorName = ref('')
const addingFloor = ref(false)
const removingFloor = ref(false)
const error = ref('')
const isShoppingListModalOpen = ref(false)

const openShoppingListModal = () => {
  isShoppingListModalOpen.value = true
}

const toggleFloorSelection = (floorId: string) => {
  if (selectedFloorIds.value.has(floorId)) {
    selectedFloorIds.value.delete(floorId)
  } else {
    selectedFloorIds.value.add(floorId)
  }
}

const totalFloorArea = computed(() => {
  return floors.value.reduce((sum, floor) => sum + floor.area, 0)
})

const selectedFloorsArea = computed(() => {
  return floors.value
    .filter((floor) => selectedFloorIds.value.has(floor.id))
    .reduce((sum, floor) => sum + floor.area, 0)
})

const itemArea = computed(() => {
  if (!itemLength.value || !itemWidth.value) return 0
  return itemLength.value * itemWidth.value
})

const totalQuantity = computed(() => {
  if (selectedFloorsArea.value === 0) return 0

  if (usePackageEfficiency.value) {
    if (!packageEfficiencyPerM2.value || packageEfficiencyPerM2.value <= 0) return 0
    return selectedFloorsArea.value / packageEfficiencyPerM2.value
  } else {
    if (itemArea.value <= 0) return 0
    return selectedFloorsArea.value / itemArea.value
  }
})

const loadFloors = async () => {
  if (!props.roomId) return

  try {
    loadingFloors.value = true

    const floorsData = await Backend.getFloorsByRoomId(props.roomId)

    floors.value = floorsData.map((floorDto: FloorDTO) => ({
      id: floorDto.id || '',
      name: floorDto.name || 'Bez nazwy',
      area: cm2ToM2(floorDto.calculatedArea),
    }))
  } catch (e) {
    console.error('Error loading floors:', e)
    floors.value = []
  } finally {
    loadingFloors.value = false
  }
}

const addFloor = async () => {
  if (!newFloorName.value || floorPoints.value.length < 3 || !props.roomId) {
    error.value = 'Podaj nazwę podłogi i zaznacz minimum 3 punkty'
    return
  }

  try {
    addingFloor.value = true
    error.value = ''

    await Backend.addFloor(props.roomId, newFloorName.value, floorPoints.value)

    const newFloor: Floor = {
      id: `floor-${Date.now()}`,
      name: newFloorName.value,
      area: calculatePolygonArea(),
    }
    floors.value.push(newFloor)

    newFloorName.value = ''
    clearCanvasPoints()
  } catch (e) {
    console.error('Error adding floor:', e)
    error.value = 'Nie udało się dodać podłogi'
  } finally {
    addingFloor.value = false
  }
}

const removeFloorFromList = async (floorId: string | undefined) => {
  if (!floorId || !props.roomId) return

  if (!confirm('Czy na pewno chcesz usunąć tę podłogę?')) return

  try {
    removingFloor.value = true
    error.value = ''

    await Backend.removeFloor(props.roomId, floorId)
    floors.value = floors.value.filter((f) => f.id !== floorId)
  } catch (e) {
    console.error('Error removing floor:', e)
    error.value = 'Nie udało się usunąć podłogi'
  } finally {
    removingFloor.value = false
  }
}

const handleSubmit = () => {
  if (floors.value.length === 0) {
    error.value = 'Dodaj przynajmniej jedną podłogę'
    return
  }
  if (selectedFloorIds.value.size === 0) {
    error.value = 'Zaznacz przynajmniej jedną podłogę'
    return
  }
  if (!calculationName.value.trim()) {
    error.value = 'Podaj nazwę materiału'
    return
  }
  if (usePackageEfficiency.value) {
    if (!packageEfficiencyPerM2.value || packageEfficiencyPerM2.value <= 0) {
      error.value = 'Podaj poprawną wydajność opakowania'
      return
    }
  } else {
    if (itemArea.value <= 0) {
      error.value = 'Podaj poprawne wymiary elementu'
      return
    }
  }

  emit('submit', {
    calculationName: calculationName.value,
    totalEfficiency: totalQuantity.value,
  })
}

watch(
  () => props.roomId,
  (newRoomId) => {
    if (newRoomId) {
      loadFloors()
    }
  },
  { immediate: true },
)
</script>
