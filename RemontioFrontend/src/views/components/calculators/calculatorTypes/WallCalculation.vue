<template>
  <div>
    <h3 class="section-title mb-3">
      <font-awesome-icon icon="cube" />
      Ściany w pokoju
    </h3>

    <div v-if="loadingWalls" class="loading-state">
      <div class="spinner"></div>
      <p>Ładowanie ścian...</p>
    </div>

    <div v-else-if="walls.length === 0" class="empty-state">
      <font-awesome-icon icon="cube" style="font-size: 3rem" />
      <h3>Brak scian w tym pokoju</h3>
      <p>Dodaj sciany aby wykonac kalkulacje</p>

      <div class="wall-creator mt-3">
        <h4 class="mb-3">Dodaj sciane</h4>

        <div class="mb-3">
          <label class="form-label">Nazwa sciany *</label>
          <input
            v-model.trim="newWallName"
            class="form-control"
            placeholder="np. Sciana polnocna"
          />
        </div>

        <div class="canvas-container mb-3">
          <p class="canvas-instructions">
            <font-awesome-icon icon="info-circle" />
            Kliknij na canvas aby dodac punkty naroznikow sciany. Minimum 3 punkty.
          </p>
          <div style="position: relative">
            <canvas
              ref="wallCanvas"
              width="600"
              height="400"
              @click="addPointToCanvas"
              @contextmenu.prevent="undoLastPoint"
              @mousemove="updateMousePosition"
              @mouseleave="mousePosition = null"
              class="wall-canvas"
            ></canvas>
            <input
              v-if="editingDistanceIndex !== null && canvasInputPosition"
              ref="canvasDistanceInput"
              v-model="editDistanceValue"
              type="number"
              step="0.1"
              min="0.1"
              class="canvas-distance-input"
              :style="{
                left: canvasInputPosition.x + 'px',
                top: canvasInputPosition.y + 'px',
              }"
              @keyup.enter="applyDistanceEdit(editingDistanceIndex)"
              @keyup.escape="cancelDistanceEdit"
              @blur="applyDistanceEdit(editingDistanceIndex)"
            />
          </div>
          <div class="canvas-controls mt-2">
            <button
              type="button"
              class="btn btn-sm btn-secondary"
              @click="clearCanvasPoints"
              :disabled="wallPoints.length === 0"
            >
              <font-awesome-icon icon="undo" /> Wyczysc punkty ({{ wallPoints.length }})
            </button>
            <button
              type="button"
              class="btn btn-sm btn-primary"
              @click="addWall"
              :disabled="addingWall || !newWallName || wallPoints.length < 3"
            >
              <font-awesome-icon icon="plus" />
              <span v-if="addingWall">Dodawanie...</span>
              <span v-else>Dodaj sciane</span>
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
              <span class="distance-label">
                Punkt {{ index + 1 }} → {{ ((index + 1) % wallPoints.length) + 1 }}:
              </span>
              <template v-if="editingDistanceIndex === index">
                <input
                  v-model="editDistanceValue"
                  type="number"
                  step="0.1"
                  min="0.1"
                  class="form-control form-control-sm distance-input"
                  @keyup.enter="applyDistanceEdit(index)"
                  @blur="cancelDistanceEdit"
                />
                <span class="text-muted">cm</span>
              </template>
              <template v-else>
                <span class="distance-value" @click="startDistanceEdit(index)">
                  {{
                    calculateDistance(point, wallPoints[(index + 1) % wallPoints.length]!).toFixed(
                      1,
                    )
                  }}
                  cm
                </span>
              </template>
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
              type="checkbox"
              :id="'wall-' + wall.id"
              :value="wall.id"
              :checked="selectedWallIds.has(wall.id)"
              @change="toggleWallSelection(wall.id)"
              class="form-check-input m-0"
            />
            <label :for="'wall-' + wall.id" class="wall-info mb-0 cursor-pointer flex-grow-1">
              <strong>{{ wall.name }}</strong>
              <span class="text-muted">
                <template v-if="wall.points"> {{ wall.points.length }} punktów, </template>
                powierzchnia: {{ wall.area.toFixed(2) }} m²
              </span>
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
        <h4 class="mb-3">Dodaj kolejna sciane</h4>

        <div class="mb-3">
          <label class="form-label">Nazwa sciany *</label>
          <input
            v-model.trim="newWallName"
            class="form-control"
            placeholder="np. Sciana poludniowa"
          />
        </div>

        <div class="canvas-container mb-3">
          <p class="canvas-instructions">
            <font-awesome-icon icon="info-circle" />
            Kliknij na canvas aby dodac punkty naroznikow sciany. Minimum 3 punkty.
          </p>
          <div style="position: relative">
            <canvas
              ref="wallCanvas"
              width="600"
              height="400"
              @click="addPointToCanvas"
              @contextmenu.prevent="undoLastPoint"
              @mousemove="updateMousePosition"
              @mouseleave="mousePosition = null"
              class="wall-canvas"
            ></canvas>
            <input
              v-if="editingDistanceIndex !== null && canvasInputPosition"
              ref="canvasDistanceInput"
              v-model="editDistanceValue"
              type="number"
              step="0.1"
              min="0.1"
              class="canvas-distance-input"
              :style="{
                left: canvasInputPosition.x + 'px',
                top: canvasInputPosition.y + 'px',
              }"
              @keyup.enter="applyDistanceEdit(editingDistanceIndex)"
              @keyup.escape="cancelDistanceEdit"
              @blur="applyDistanceEdit(editingDistanceIndex)"
            />
          </div>
          <div class="canvas-controls mt-2">
            <button
              type="button"
              class="btn btn-sm btn-secondary"
              @click="clearCanvasPoints"
              :disabled="wallPoints.length === 0"
            >
              <font-awesome-icon icon="undo" /> Wyczysc punkty ({{ wallPoints.length }})
            </button>
            <button
              type="button"
              class="btn btn-sm btn-primary"
              @click="addWall"
              :disabled="addingWall || !newWallName || wallPoints.length < 3"
            >
              <font-awesome-icon icon="plus" />
              <span v-if="addingWall">Dodawanie...</span>
              <span v-else>Dodaj sciane</span>
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
              <span class="distance-label">
                Punkt {{ index + 1 }} → {{ ((index + 1) % wallPoints.length) + 1 }}:
              </span>
              <template v-if="editingDistanceIndex === index">
                <input
                  v-model="editDistanceValue"
                  type="number"
                  step="0.1"
                  min="0.1"
                  class="form-control form-control-sm distance-input"
                  @keyup.enter="applyDistanceEdit(index)"
                  @blur="cancelDistanceEdit"
                />
                <span class="text-muted">cm</span>
              </template>
              <template v-else>
                <span class="distance-value" @click="startDistanceEdit(index)">
                  {{
                    calculateDistance(point, wallPoints[(index + 1) % wallPoints.length]!).toFixed(
                      1,
                    )
                  }}
                  cm
                </span>
              </template>
            </div>
          </div>
        </div>
      </div>

      <div v-if="selectedWallIds.size > 0" class="calculation-params mb-4">
        <h4 class="mb-3">Parametry kalkulacji</h4>
        <div class="params-box">
          <div class="mb-3">
            <label class="form-label">Nazwa (materiał/praca) *</label>
            <input
              v-model.trim="calculationName"
              type="text"
              class="form-control"
              placeholder="np. Malowanie ścian, Tapetowanie"
              required
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Wydajność na m² *</label>
            <input
              v-model.number="efficiencyPerM2"
              type="number"
              step="0.01"
              min="0.01"
              class="form-control"
              placeholder="np. 0.5"
              required
            />
          </div>
          <div class="mb-3">
            <label class="form-label">Grubość warstwy wskazana przez producenta (mm)</label>
            <input
              v-model.number="manufacturerLayerThickness"
              type="number"
              step="0.1"
              min="0.1"
              class="form-control"
              placeholder="np. 2.0"
            />
            <small class="text-muted">
              Służy do przeliczenia zapotrzebowania materiału przy innej grubości
            </small>
          </div>
          <div class="mb-3">
            <label class="form-label">Grubość warstwy (mm)</label>
            <input
              v-model.number="layerThickness"
              type="number"
              step="0.1"
              min="0.1"
              class="form-control"
              placeholder="np. 2.5"
            />
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
            <span class="summary-label">Materiał/Praca:</span>
            <span class="summary-value">{{ calculationName }}</span>
          </div>
          <div v-if="efficiencyPerM2" class="summary-item">
            <span class="summary-label">Wydajność na m²:</span>
            <span class="summary-value">{{ efficiencyPerM2 }}</span>
          </div>
          <div v-if="layerThickness" class="summary-item">
            <span class="summary-label">Grubość warstwy:</span>
            <span class="summary-value">{{ layerThickness }} mm</span>
          </div>
          <div v-if="manufacturerLayerThickness" class="summary-item">
            <span class="summary-label">Grubość producenta:</span>
            <span class="summary-value">{{ manufacturerLayerThickness }} mm</span>
          </div>
          <div v-if="totalEfficiency > 0" class="summary-item total-item">
            <span class="summary-label">Łączna ilość:</span>
            <span class="summary-value">{{ totalEfficiency.toFixed(2) }}</span>
          </div>
        </div>
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
              !efficiencyPerM2 ||
              efficiencyPerM2 <= 0
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
        <div v-if="success" class="alert alert-success mt-3">
          Kalkulacja utworzona. Przekierowywanie...
        </div>
      </form>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import { ref, computed, watch, nextTick } from 'vue'
import { cm2ToM2, pixelToMeter } from '@/helpers/distanceFormatter'
import type { PointDTO, WallDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

interface Wall {
  id: string
  name: string
  area: number
  points?: PointDTO[]
}

interface Props {
  roomId: string
  submitting: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  back: []
  submit: [
    data: {
      calculationName: string
      totalEfficiency: number
    },
  ]
  error: [message: string]
  success: []
}>()

const walls = ref<Wall[]>([])
const loadingWalls = ref(false)
const showAddWallSection = ref(false)
const selectedWallIds = ref<Set<string>>(new Set())
const calculationName = ref('')
const efficiencyPerM2 = ref<number | null>(null)
const layerThickness = ref<number>(1)
const manufacturerLayerThickness = ref<number>(1)
const newWallName = ref('')
const wallPoints = ref<PointDTO[]>([])
const wallCanvas = ref<HTMLCanvasElement | null>(null)
const polygonClosed = ref(false)
const addingWall = ref(false)
const removingWall = ref(false)
const mousePosition = ref<{ x: number; y: number } | null>(null)
const editingDistanceIndex = ref<number | null>(null)
const editDistanceValue = ref<string>('')
const distanceLabelPositions = ref<
  Array<{ index: number; x: number; y: number; width: number; height: number }>
>([])
const canvasInputPosition = ref<{ x: number; y: number } | null>(null)
const canvasDistanceInput = ref<HTMLInputElement | null>(null)
const error = ref('')
const success = ref(false)

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

const totalEfficiency = computed(() => {
  if (!efficiencyPerM2.value || selectedWallsArea.value === 0) return 0

  let baseEfficiency = selectedWallsArea.value * efficiencyPerM2.value

  // Jeśli podano obie grubości, oblicz współczynnik
  if (
    layerThickness.value &&
    manufacturerLayerThickness.value &&
    manufacturerLayerThickness.value > 0
  ) {
    const thicknessRatio = layerThickness.value / manufacturerLayerThickness.value
    baseEfficiency *= thicknessRatio
  }

  return baseEfficiency
})

const loadWalls = async () => {
  if (!props.roomId) return

  try {
    loadingWalls.value = true

    const wallsData = await Backend.getWallsByRoomId(props.roomId)

    walls.value = (wallsData || []).map((wallDto: WallDTO) => ({
      id: wallDto.id || '',
      name: wallDto.name || 'Bez nazwy',
      area: cm2ToM2(wallDto.calculatedArea),
      points: undefined,
    }))
  } catch (e) {
    console.error('Error loading walls:', e)
    walls.value = []
  } finally {
    loadingWalls.value = false
  }
}

const addPointToCanvas = (event: MouseEvent) => {
  if (!wallCanvas.value) return

  const rect = wallCanvas.value.getBoundingClientRect()
  const clickX = event.clientX - rect.left
  const clickY = event.clientY - rect.top

  for (const label of distanceLabelPositions.value) {
    if (
      clickX >= label.x &&
      clickX <= label.x + label.width &&
      clickY >= label.y &&
      clickY <= label.y + label.height
    ) {
      startDistanceEdit(label.index)
      return
    }
  }

  if (polygonClosed.value) return

  if (wallPoints.value.length === 0) {
    wallPoints.value.push({ x: 0, y: 0 })
    polygonClosed.value = false
    if (!wallCanvas.value.dataset.originX) {
      wallCanvas.value.dataset.originX = clickX.toString()
      wallCanvas.value.dataset.originY = clickY.toString()
    }
  } else {
    const originX = parseFloat(wallCanvas.value.dataset.originX || '0')
    const originY = parseFloat(wallCanvas.value.dataset.originY || '0')

    if (wallPoints.value.length >= 3) {
      const distanceToFirst = Math.sqrt(
        Math.pow(clickX - originX, 2) + Math.pow(clickY - originY, 2),
      )

      if (distanceToFirst < 10) {
        polygonClosed.value = true
        drawCanvas()
        return
      }
    }

    const relativeX = clickX - originX
    const relativeY = clickY - originY
    wallPoints.value.push({ x: relativeX, y: relativeY })
  }

  drawCanvas()
}

const clearCanvasPoints = () => {
  wallPoints.value = []
  polygonClosed.value = false
  if (wallCanvas.value) {
    delete wallCanvas.value.dataset.originX
    delete wallCanvas.value.dataset.originY
  }
  mousePosition.value = null
  drawCanvas()
}

const undoLastPoint = () => {
  if (wallPoints.value.length > 0) {
    wallPoints.value.pop()
    polygonClosed.value = false

    if (wallPoints.value.length === 0 && wallCanvas.value) {
      delete wallCanvas.value.dataset.originX
      delete wallCanvas.value.dataset.originY
      mousePosition.value = null
    }

    drawCanvas()
  }
}

const updateMousePosition = (event: MouseEvent) => {
  if (!wallCanvas.value || wallPoints.value.length === 0) {
    mousePosition.value = null
    return
  }

  const rect = wallCanvas.value.getBoundingClientRect()
  const x = event.clientX - rect.left
  const y = event.clientY - rect.top

  let isOverLabel = false
  for (const label of distanceLabelPositions.value) {
    if (x >= label.x && x <= label.x + label.width && y >= label.y && y <= label.y + label.height) {
      isOverLabel = true
      wallCanvas.value.style.cursor = 'pointer'
      break
    }
  }

  if (!isOverLabel) {
    wallCanvas.value.style.cursor = polygonClosed.value ? 'default' : 'crosshair'
  }

  mousePosition.value = { x, y }
  drawCanvas()
}

const startDistanceEdit = (index: number) => {
  const point1 = wallPoints.value[index]
  const point2 = wallPoints.value[(index + 1) % wallPoints.value.length]
  if (point1 && point2 && wallCanvas.value) {
    editingDistanceIndex.value = index
    editDistanceValue.value = calculateDistance(point1, point2).toFixed(1)

    const originX = parseFloat(wallCanvas.value.dataset.originX || '0')
    const originY = parseFloat(wallCanvas.value.dataset.originY || '0')
    const midX = originX + ((point1.x || 0) + (point2.x || 0)) / 2
    const midY = originY + ((point1.y || 0) + (point2.y || 0)) / 2

    canvasInputPosition.value = {
      x: midX - 40,
      y: midY - 12,
    }

    nextTick(() => {
      canvasDistanceInput.value?.focus()
      canvasDistanceInput.value?.select()
    })

    drawCanvas()
  }
}

const cancelDistanceEdit = () => {
  editingDistanceIndex.value = null
  editDistanceValue.value = ''
  canvasInputPosition.value = null
  drawCanvas()
}

const applyDistanceEdit = (index: number) => {
  const newDistance = parseFloat(editDistanceValue.value)
  if (isNaN(newDistance) || newDistance <= 0) {
    cancelDistanceEdit()
    return
  }

  const point1 = wallPoints.value[index]
  const nextIndex = (index + 1) % wallPoints.value.length
  const point2 = wallPoints.value[nextIndex]
  if (!point1 || !point2) return

  const dx = (point2.x || 0) - (point1.x || 0)
  const dy = (point2.y || 0) - (point1.y || 0)
  const angle = Math.atan2(dy, dx)

  const newDistancePx = newDistance / 100 / pixelToMeter

  const newX = (point1.x || 0) + Math.cos(angle) * newDistancePx
  const newY = (point1.y || 0) + Math.sin(angle) * newDistancePx

  if (index === wallPoints.value.length - 1 && wallPoints.value.length > 2) {
    const offsetX = newX - (point2.x || 0)
    const offsetY = newY - (point2.y || 0)

    const firstPoint = wallPoints.value[0]
    if (firstPoint) {
      wallPoints.value[0] = {
        x: (firstPoint.x || 0) + offsetX,
        y: (firstPoint.y || 0) + offsetY,
      }
    }
  } else {
    const offsetX = newX - (point2.x || 0)
    const offsetY = newY - (point2.y || 0)

    for (let i = nextIndex; i < wallPoints.value.length; i++) {
      const point = wallPoints.value[i]
      if (point) {
        wallPoints.value[i] = {
          x: (point.x || 0) + offsetX,
          y: (point.y || 0) + offsetY,
        }
      }
    }
  }

  cancelDistanceEdit()
  drawCanvas()
}

const pixelsToCm = (pixels: number): number => {
  return pixels * pixelToMeter * 100
}

const calculateDistance = (p1: PointDTO, p2: PointDTO): number => {
  const dx = (p2.x || 0) - (p1.x || 0)
  const dy = (p2.y || 0) - (p1.y || 0)
  const distanceInPixels = Math.sqrt(dx * dx + dy * dy)
  return pixelsToCm(distanceInPixels)
}

const drawCanvas = () => {
  if (!wallCanvas.value) return

  const ctx = wallCanvas.value.getContext('2d')
  if (!ctx) return

  ctx.clearRect(0, 0, wallCanvas.value.width, wallCanvas.value.height)

  const gridSpacing = 30
  ctx.strokeStyle = '#e0e0e0'
  ctx.lineWidth = 1
  for (let i = 0; i < wallCanvas.value.width; i += gridSpacing) {
    ctx.beginPath()
    ctx.moveTo(i, 0)
    ctx.lineTo(i, wallCanvas.value.height)
    ctx.stroke()
  }
  for (let i = 0; i < wallCanvas.value.height; i += gridSpacing) {
    ctx.beginPath()
    ctx.moveTo(0, i)
    ctx.lineTo(wallCanvas.value.width, i)
    ctx.stroke()
  }

  if (wallPoints.value.length === 0) return

  const originX = parseFloat(wallCanvas.value.dataset.originX || '0')
  const originY = parseFloat(wallCanvas.value.dataset.originY || '0')

  ctx.strokeStyle = '#4A90E2'
  ctx.fillStyle = 'rgba(74, 144, 226, 0.2)'
  ctx.lineWidth = 2

  ctx.beginPath()
  const firstPoint = wallPoints.value[0]
  if (!firstPoint) return
  ctx.moveTo(originX + (firstPoint.x || 0), originY + (firstPoint.y || 0))
  for (let i = 1; i < wallPoints.value.length; i++) {
    const point = wallPoints.value[i]
    if (point) {
      ctx.lineTo(originX + (point.x || 0), originY + (point.y || 0))
    }
  }
  if (wallPoints.value.length > 2) {
    ctx.closePath()
    ctx.fill()
  }
  ctx.stroke()

  wallPoints.value.forEach((point: PointDTO, index: number) => {
    const canvasX = originX + (point.x || 0)
    const canvasY = originY + (point.y || 0)

    ctx.fillStyle = '#4A90E2'
    ctx.beginPath()
    ctx.arc(canvasX, canvasY, 5, 0, 2 * Math.PI)
    ctx.fill()

    ctx.fillStyle = '#000'
    ctx.font = '12px Arial'
    ctx.fillText(`${index + 1}`, canvasX + 10, canvasY - 10)
  })

  distanceLabelPositions.value = []
  ctx.fillStyle = '#E74C3C'
  ctx.font = 'bold 13px Arial'
  for (let i = 0; i < wallPoints.value.length; i++) {
    const currentPoint = wallPoints.value[i]
    const nextPoint = wallPoints.value[(i + 1) % wallPoints.value.length]

    if (
      currentPoint &&
      nextPoint &&
      (i < wallPoints.value.length - 1 || wallPoints.value.length > 2)
    ) {
      const distance = calculateDistance(currentPoint, nextPoint)
      const midX = originX + ((currentPoint.x || 0) + (nextPoint.x || 0)) / 2
      const midY = originY + ((currentPoint.y || 0) + (nextPoint.y || 0)) / 2

      const text = `${distance.toFixed(1)} cm`
      const metrics = ctx.measureText(text)
      const labelX = midX - metrics.width / 2 - 3
      const labelY = midY - 10
      const labelWidth = metrics.width + 6
      const labelHeight = 16

      distanceLabelPositions.value.push({
        index: i,
        x: labelX,
        y: labelY,
        width: labelWidth,
        height: labelHeight,
      })

      if (editingDistanceIndex.value === i) {
        ctx.fillStyle = 'rgba(74, 144, 226, 0.9)'
      } else {
        ctx.fillStyle = 'rgba(255, 255, 255, 0.8)'
      }
      ctx.fillRect(labelX, labelY, labelWidth, labelHeight)

      ctx.strokeStyle = editingDistanceIndex.value === i ? '#4A90E2' : '#E74C3C'
      ctx.lineWidth = 1
      ctx.strokeRect(labelX, labelY, labelWidth, labelHeight)

      ctx.fillStyle = editingDistanceIndex.value === i ? '#fff' : '#E74C3C'
      ctx.fillText(text, midX - metrics.width / 2, midY + 3)
    }
  }

  if (mousePosition.value && wallPoints.value.length > 0 && !polygonClosed.value) {
    const lastPoint = wallPoints.value[wallPoints.value.length - 1]
    if (lastPoint) {
      const lastCanvasX = originX + (lastPoint.x || 0)
      const lastCanvasY = originY + (lastPoint.y || 0)

      const relativeMouseX = mousePosition.value.x - originX
      const relativeMouseY = mousePosition.value.y - originY

      ctx.strokeStyle = '#999'
      ctx.lineWidth = 1
      ctx.setLineDash([5, 5])
      ctx.beginPath()
      ctx.moveTo(lastCanvasX, lastCanvasY)
      ctx.lineTo(mousePosition.value.x, mousePosition.value.y)
      ctx.stroke()
      ctx.setLineDash([])

      const previewDistance = calculateDistance(lastPoint, {
        x: relativeMouseX,
        y: relativeMouseY,
      })
      const midX = (lastCanvasX + mousePosition.value.x) / 2
      const midY = (lastCanvasY + mousePosition.value.y) / 2

      ctx.fillStyle = 'rgba(255, 255, 255, 0.9)'
      const previewText = `${previewDistance.toFixed(1)} cm`
      const previewMetrics = ctx.measureText(previewText)
      ctx.fillRect(midX - previewMetrics.width / 2 - 3, midY - 10, previewMetrics.width + 6, 16)

      ctx.fillStyle = '#666'
      ctx.font = 'bold 13px Arial'
      ctx.fillText(previewText, midX - previewMetrics.width / 2, midY + 3)
    }
  }
}

const calculatePolygonArea = (): number => {
  if (wallPoints.value.length < 3) return 0

  let area = 0
  const points = wallPoints.value

  for (let i = 0; i < points.length; i++) {
    const j = (i + 1) % points.length
    const pointI = points[i]
    const pointJ = points[j]
    if (pointI && pointJ) {
      area += (pointI.x || 0) * (pointJ.y || 0)
      area -= (pointJ.x || 0) * (pointI.y || 0)
    }
  }

  return Math.abs(area / 2) * pixelToMeter * pixelToMeter
}

const addWall = async () => {
  if (!newWallName.value || wallPoints.value.length < 3 || !props.roomId) {
    error.value = 'Podaj nazwę ściany i zaznacz minimum 3 punkty'
    return
  }

  try {
    addingWall.value = true
    error.value = ''

    const success = await Backend.addWall(props.roomId, newWallName.value, wallPoints.value)

    if (success) {
      const newWall: Wall = {
        id: `wall-${Date.now()}`,
        name: newWallName.value,
        area: calculatePolygonArea(),
        points: [...wallPoints.value],
      }
      walls.value.push(newWall)

      newWallName.value = ''
      wallPoints.value = []
      polygonClosed.value = false
      if (wallCanvas.value) {
        delete wallCanvas.value.dataset.originX
        delete wallCanvas.value.dataset.originY
      }
      drawCanvas()
    } else {
      error.value = 'Nie udało się dodać ściany'
    }
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
    error.value = 'Podaj nazwę materiału/pracy'
    return
  }
  if (!efficiencyPerM2.value || efficiencyPerM2.value <= 0) {
    error.value = 'Podaj poprawną wydajność na m²'
    return
  }

  emit('submit', {
    calculationName: calculationName.value,
    totalEfficiency: totalEfficiency.value,
  })
}

watch(wallPoints, () => {
  nextTick(() => drawCanvas())
})

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

<style scoped>
.walls-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.wall-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: var(--color-bg-light-gray);
  border-radius: 8px;
  border: 1px solid #ddd;
  transition: all 0.2s ease;
}

.wall-item:has(input[type='checkbox']:checked) {
  background: rgba(52, 152, 219, 0.08);
  border-color: var(--color-primary-blue);
}

.wall-item input[type='checkbox'] {
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.wall-item label {
  cursor: pointer;
  flex: 1;
  margin: 0;
}

.wall-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.wall-creator {
  background: var(--color-bg-light-gray);
  padding: 1.5rem;
  border-radius: 12px;
  border: 2px dashed #ddd;
}

.calculation-summary {
  background: var(--color-bg-blue-pale);
  padding: 1.5rem;
  border-radius: 12px;
}

.summary-box {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.summary-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: var(--color-bg-white);
  border-radius: 8px;
}

.total-item {
  border: 2px solid var(--color-primary-blue);
  background: rgba(52, 152, 219, 0.05);
}

.total-item .summary-value {
  font-size: 1.5rem;
}

.summary-label {
  font-weight: 600;
  color: var(--color-text-dark);
}

.summary-value {
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--color-primary-blue);
}

.canvas-container {
  background: var(--color-bg-white);
  padding: 1rem;
  border-radius: 12px;
  border: 1px solid #ddd;
}

.canvas-instructions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--color-text-medium);
  font-size: 0.9rem;
  margin-bottom: 0.75rem;
}

.wall-canvas {
  width: 100%;
  max-width: 600px;
  height: 400px;
  border: 2px solid var(--color-primary-blue);
  border-radius: 8px;
  cursor: crosshair;
  background: var(--color-bg-white);
  display: block;
}

.canvas-controls {
  display: flex;
  gap: 0.75rem;
  justify-content: flex-end;
  flex-wrap: wrap;
}

.points-info {
  margin-top: 1rem;
  padding: 1rem;
  background: var(--color-bg-light-gray);
  border-radius: 8px;
}

.distances-list {
  margin-top: 1rem;
}

.distances-list h6 {
  font-size: 0.9rem;
  font-weight: 600;
  color: var(--color-text-dark);
  margin-bottom: 0.5rem;
}

.distance-item {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.5rem 0;
  border-bottom: 1px solid #e0e0e0;
}

.distance-item:last-child {
  border-bottom: none;
}

.distance-label {
  font-size: 0.85rem;
  color: var(--color-text-medium);
  min-width: 100px;
}

.distance-value {
  font-weight: 600;
  color: var(--color-primary-blue);
  cursor: pointer;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  transition: background-color 0.2s;
}

.distance-value:hover {
  background: var(--color-bg-blue-pale);
}

.distance-input {
  width: 100px;
  display: inline-block;
  margin: 0 0.5rem;
}

.canvas-distance-input {
  position: absolute;
  width: 80px;
  padding: 4px 8px;
  border: 2px solid var(--color-primary-blue);
  border-radius: 4px;
  font-size: 13px;
  font-weight: bold;
  text-align: center;
  background: white;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
  z-index: 10;
}

.canvas-distance-input:focus {
  outline: none;
  border-color: var(--color-primary-blue);
  box-shadow: 0 0 0 3px rgba(74, 144, 226, 0.3);
}

@media (max-width: 768px) {
  .wall-item {
    flex-direction: column;
    align-items: stretch;
    gap: 0.5rem;
  }

  .wall-canvas {
    height: 300px;
  }

  .canvas-controls {
    justify-content: stretch;
  }

  .canvas-controls button {
    flex: 1;
  }
}
</style>
