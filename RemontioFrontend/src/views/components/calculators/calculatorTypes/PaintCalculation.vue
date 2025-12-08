<template>
  <div>
    <h3 class="section-title mb-3">
      <font-awesome-icon icon="paint-roller" />
      Ściany do malowania
    </h3>

    <div v-if="loadingWalls" class="loading-state">
      <div class="spinner"></div>
      <p>Ładowanie ścian...</p>
    </div>

    <div v-else-if="walls.length === 0" class="empty-state">
      <font-awesome-icon icon="cube" style="font-size: 3rem" />
      <h3>Brak ścian w tym pokoju</h3>
      <p>Dodaj ściany aby wykonać kalkulację farby</p>

      <div class="wall-creator mt-3">
        <h4 class="mb-3">Dodaj ścianę</h4>

        <div class="mb-3">
          <label class="form-label">Nazwa ściany *</label>
          <input
            v-model.trim="newWallName"
            class="form-control"
            placeholder="np. Ściana północna"
          />
        </div>

        <div class="canvas-container mb-3">
          <p class="canvas-instructions">
            <font-awesome-icon icon="info-circle" />
            Kliknij na canvas aby dodać punkty narożników ściany. Minimum 3 punkty.
          </p>
          <div style="position: relative">
            <canvas
              ref="wallCanvas"
              @click="addPointToCanvas"
              @mousemove="updateMousePosition"
              width="600"
              height="400"
              class="wall-canvas"
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
              :disabled="wallPoints.length === 0"
            >
              <font-awesome-icon icon="undo" /> Cofnij punkt
            </button>
            <button
              type="button"
              class="btn btn-sm btn-outline-danger"
              @click="clearCanvasPoints"
              :disabled="wallPoints.length === 0"
            >
              <font-awesome-icon icon="trash-can" /> Wyczyść
            </button>
            <button
              type="button"
              class="btn btn-sm btn-primary"
              @click="addWall"
              :disabled="addingWall || !newWallName || wallPoints.length < 3"
            >
              <font-awesome-icon icon="plus" />
              {{ addingWall ? 'Dodawanie...' : 'Dodaj ścianę' }}
            </button>
          </div>
        </div>

        <div v-if="wallPoints.length > 0" class="points-info">
          <p class="text-muted mb-3">
            Zaznaczono {{ wallPoints.length }} punktów | Szacowana powierzchnia:
            {{ calculatePolygonArea().toFixed(2) }} m²
          </p>

          <div v-if="wallPoints.length > 1" class="distances-list">
            <h6 class="mb-2">Odległości między punktami:</h6>
            <div
              v-for="(point, index) in wallPoints"
              :key="index"
              class="distance-item"
              v-show="index < wallPoints.length - 1 || wallPoints.length > 2"
            >
              <span class="distance-label"
                >{{ index + 1 }} → {{ index < wallPoints.length - 1 ? index + 2 : 1 }}:</span
              >
              <span
                class="distance-value"
                @click="startDistanceEdit(index)"
                v-if="editingDistanceIndex !== index && wallPoints[(index + 1) % wallPoints.length]"
              >
                {{
                  calculateDistance(point, wallPoints[(index + 1) % wallPoints.length]!).toFixed(1)
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
      <div class="walls-list mb-3">
        <div v-for="wall in walls" :key="wall.id" class="wall-item">
          <div class="d-flex align-items-center gap-2 flex-grow-1">
            <input
              :id="'wall-' + wall.id"
              type="checkbox"
              :checked="selectedWallIds.has(wall.id)"
              @change="toggleWallSelection(wall.id)"
              class="form-check-input m-0"
            />
            <label :for="'wall-' + wall.id" class="wall-info mb-0 cursor-pointer flex-grow-1">
              <strong>{{ wall.name }}</strong>
              <br />
              <small class="text-muted">{{ wall.area.toFixed(2) }} m²</small>
            </label>
          </div>
          <button
            type="button"
            class="btn btn-sm btn-danger"
            @click="removeWallFromList(wall.id)"
            :disabled="removingWall"
          >
            <font-awesome-icon icon="trash-can" />
          </button>
        </div>
      </div>

      <div class="mb-3">
        <button
          type="button"
          class="btn btn-outline-primary w-100"
          @click="showAddWallSection = !showAddWallSection"
        >
          <font-awesome-icon :icon="showAddWallSection ? 'chevron-up' : 'chevron-down'" />
          {{ showAddWallSection ? 'Ukryj formularz dodawania ściany' : 'Dodaj kolejną ścianę' }}
        </button>
      </div>

      <div v-if="showAddWallSection" class="wall-creator mb-4">
        <h4 class="mb-3">Dodaj kolejną ścianę</h4>

        <div class="mb-3">
          <label class="form-label">Nazwa ściany *</label>
          <input
            v-model.trim="newWallName"
            class="form-control"
            placeholder="np. Ściana południowa"
          />
        </div>

        <div class="canvas-container mb-3">
          <p class="canvas-instructions">
            <font-awesome-icon icon="info-circle" />
            Kliknij na canvas aby dodać punkty narożników ściany. Minimum 3 punkty.
          </p>
          <div style="position: relative">
            <canvas
              ref="wallCanvas"
              @click="addPointToCanvas"
              @mousemove="updateMousePosition"
              width="600"
              height="400"
              class="wall-canvas"
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
              :disabled="wallPoints.length === 0"
            >
              <font-awesome-icon icon="undo" /> Cofnij punkt
            </button>
            <button
              type="button"
              class="btn btn-sm btn-outline-danger"
              @click="clearCanvasPoints"
              :disabled="wallPoints.length === 0"
            >
              <font-awesome-icon icon="trash-can" /> Wyczyść
            </button>
            <button
              type="button"
              class="btn btn-sm btn-primary"
              @click="addWall"
              :disabled="addingWall || !newWallName || wallPoints.length < 3"
            >
              <font-awesome-icon icon="plus" />
              {{ addingWall ? 'Dodawanie...' : 'Dodaj ścianę' }}
            </button>
          </div>
        </div>

        <div v-if="wallPoints.length > 0" class="points-info">
          <p class="text-muted mb-3">
            Zaznaczono {{ wallPoints.length }} punktów | Szacowana powierzchnia:
            {{ calculatePolygonArea().toFixed(2) }} m²
          </p>

          <div v-if="wallPoints.length > 1" class="distances-list">
            <h6 class="mb-2">Odległości między punktami:</h6>
            <div
              v-for="(point, index) in wallPoints"
              :key="index"
              class="distance-item"
              v-show="index < wallPoints.length - 1 || wallPoints.length > 2"
            >
              <span class="distance-label"
                >{{ index + 1 }} → {{ index < wallPoints.length - 1 ? index + 2 : 1 }}:</span
              >
              <span
                class="distance-value"
                @click="startDistanceEdit(index)"
                v-if="editingDistanceIndex !== index && wallPoints[(index + 1) % wallPoints.length]"
              >
                {{
                  calculateDistance(point, wallPoints[(index + 1) % wallPoints.length]!).toFixed(1)
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

      <div v-if="selectedWallIds.size > 0" class="calculation-params mb-4">
        <h4 class="mb-3">Parametry kalkulacji</h4>
        <div class="params-box">
          <div class="mb-3">
            <label class="form-label">Nazwa farby *</label>
            <input
              v-model.trim="calculationName"
              type="text"
              class="form-control"
              placeholder="np. Farba biała matowa"
              required
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Wydajność farby na m² (z 1 litra) *</label>
            <input
              v-model.number="paintEfficiencyPerM2"
              type="number"
              step="0.1"
              min="0.1"
              class="form-control"
              placeholder="np. 10"
              required
            />
            <small class="text-muted"> Ile m² pokrywa 1 litr farby w jednej warstwie </small>
          </div>
          <div class="mb-3">
            <label class="form-label">Ilość warstw farby *</label>
            <input
              v-model.number="numberOfLayers"
              type="number"
              step="1"
              min="1"
              class="form-control"
              placeholder="np. 2"
              required
            />
            <small class="text-muted"> Ile warstw farby planujesz nałożyć </small>
          </div>
        </div>
      </div>

      <div class="calculation-summary mb-4">
        <h4 class="mb-3">Podsumowanie</h4>
        <div class="summary-box">
          <div class="summary-item">
            <span class="summary-label">Całkowita powierzchnia:</span>
            <span class="summary-value">{{ totalWallArea.toFixed(2) }} m²</span>
          </div>
          <div v-if="selectedWallIds.size > 0" class="summary-item">
            <span class="summary-label">Zaznaczone ściany:</span>
            <span class="summary-value">{{ selectedWallIds.size }} / {{ walls.length }}</span>
          </div>
          <div v-if="selectedWallIds.size > 0" class="summary-item">
            <span class="summary-label">Powierzchnia zaznaczonych:</span>
            <span class="summary-value">{{ selectedWallsArea.toFixed(2) }} m²</span>
          </div>
          <div v-if="calculationName" class="summary-item">
            <span class="summary-label">Farba:</span>
            <span class="summary-value">{{ calculationName }}</span>
          </div>
          <div v-if="paintEfficiencyPerM2" class="summary-item">
            <span class="summary-label">Wydajność na m²:</span>
            <span class="summary-value">{{ paintEfficiencyPerM2 }} m²/l</span>
          </div>
          <div v-if="numberOfLayers" class="summary-item">
            <span class="summary-label">Ilość warstw:</span>
            <span class="summary-value">{{ numberOfLayers }}</span>
          </div>
          <div v-if="totalPaintNeeded > 0" class="summary-item total-item">
            <span class="summary-label">Potrzebna ilość farby:</span>
            <span class="summary-value">{{ totalPaintNeeded.toFixed(2) }} l</span>
          </div>
        </div>

        <button
          v-if="totalPaintNeeded > 0 && calculationName"
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
              walls.length === 0 ||
              selectedWallIds.size === 0 ||
              !calculationName.trim() ||
              !paintEfficiencyPerM2 ||
              paintEfficiencyPerM2 <= 0 ||
              !numberOfLayers ||
              numberOfLayers <= 0
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
        totalEfficiency: totalPaintNeeded,
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
import type { WallDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import AddToShoppingListModal from '../AddToShoppingListModal.vue'
import { useCanvasDrawing } from '@/composables/useCanvasDrawing'

interface Wall {
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
  points: wallPoints,
  canvas: wallCanvas,
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

const walls = ref<Wall[]>([])
const loadingWalls = ref(false)
const showAddWallSection = ref(false)
const selectedWallIds = ref<Set<string>>(new Set())
const calculationName = ref('')
const paintEfficiencyPerM2 = ref<number | null>(null)
const numberOfLayers = ref<number>(1)
const newWallName = ref('')
const addingWall = ref(false)
const removingWall = ref(false)
const error = ref('')
const isShoppingListModalOpen = ref(false)

const openShoppingListModal = () => {
  isShoppingListModalOpen.value = true
}

const toggleWallSelection = (wallId: string) => {
  if (selectedWallIds.value.has(wallId)) {
    selectedWallIds.value.delete(wallId)
  } else {
    selectedWallIds.value.add(wallId)
  }
}

const totalWallArea = computed(() => {
  return walls.value.reduce((sum, wall) => sum + wall.area, 0)
})

const selectedWallsArea = computed(() => {
  return walls.value
    .filter((wall) => selectedWallIds.value.has(wall.id))
    .reduce((sum, wall) => sum + wall.area, 0)
})

const totalPaintNeeded = computed(() => {
  if (!paintEfficiencyPerM2.value || selectedWallsArea.value === 0 || !numberOfLayers.value)
    return 0

  // Powierzchnia * ilość warstw / wydajność na m²
  return (selectedWallsArea.value * numberOfLayers.value) / paintEfficiencyPerM2.value
})

const loadWalls = async () => {
  if (!props.roomId) return

  try {
    loadingWalls.value = true

    const wallsData = await Backend.getWallsByRoomId(props.roomId)

    walls.value = wallsData.map((wallDto: WallDTO) => ({
      id: wallDto.id || '',
      name: wallDto.name || 'Bez nazwy',
      area: cm2ToM2(wallDto.calculatedArea),
    }))
  } catch (e) {
    console.error('Error loading walls:', e)
    walls.value = []
  } finally {
    loadingWalls.value = false
  }
}

const addWall = async () => {
  if (!newWallName.value || wallPoints.value.length < 3 || !props.roomId) {
    error.value = 'Podaj nazwę ściany i zaznacz minimum 3 punkty'
    return
  }

  try {
    addingWall.value = true
    error.value = ''

    await Backend.addWall(props.roomId, newWallName.value, wallPoints.value)

    const newWall: Wall = {
      id: `wall-${Date.now()}`,
      name: newWallName.value,
      area: calculatePolygonArea(),
    }
    walls.value.push(newWall)

    newWallName.value = ''
    clearCanvasPoints()
  } catch (e) {
    console.error('Error adding wall:', e)
    error.value = 'Nie udało się dodać ściany'
  } finally {
    addingWall.value = false
  }
}

const removeWallFromList = async (wallId: string | undefined) => {
  if (!wallId || !props.roomId) return

  if (!confirm('Czy na pewno chcesz usunąć tę ścianę?')) return

  try {
    removingWall.value = true
    error.value = ''

    await Backend.removeWall(props.roomId, wallId)
    walls.value = walls.value.filter((w) => w.id !== wallId)
  } catch (e) {
    console.error('Error removing wall:', e)
    error.value = 'Nie udało się usunąć ściany'
  } finally {
    removingWall.value = false
  }
}

const handleSubmit = () => {
  if (walls.value.length === 0) {
    error.value = 'Dodaj przynajmniej jedną ścianę'
    return
  }
  if (selectedWallIds.value.size === 0) {
    error.value = 'Zaznacz przynajmniej jedną ścianę'
    return
  }
  if (!calculationName.value.trim()) {
    error.value = 'Podaj nazwę farby'
    return
  }
  if (!paintEfficiencyPerM2.value || paintEfficiencyPerM2.value <= 0) {
    error.value = 'Podaj poprawną wydajność farby na m²'
    return
  }
  if (!numberOfLayers.value || numberOfLayers.value <= 0) {
    error.value = 'Podaj poprawną ilość warstw'
    return
  }

  emit('submit', {
    calculationName: calculationName.value,
    totalEfficiency: totalPaintNeeded.value,
  })
}

watch(
  () => props.roomId,
  (newRoomId) => {
    if (newRoomId) {
      loadWalls()
    }
  },
  { immediate: true },
)
</script>
