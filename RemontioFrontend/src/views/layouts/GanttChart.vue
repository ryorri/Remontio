<template>
  <div class="gantt-wrapper">
    <div class="gantt-header">
      <h2 class="gantt-title">
        <i class="fas fa-chart-gantt"></i>
        Wykres Gantta - Zadania
      </h2>
      <div class="gantt-legend">
        <div class="legend-item">
          <div class="legend-color" style="background: var(--color-primary-purple)"></div>
          <span>Aktywne</span>
        </div>
        <div class="legend-item">
          <div
            class="legend-color"
            style="
              background: var(--color-bg-light-gray);
              border: 2px dashed var(--color-primary-purple);
            "
          ></div>
          <span>W planach</span>
        </div>
        <div class="legend-item">
          <div class="legend-color" style="background: var(--color-green)"></div>
          <span>Zakończone</span>
        </div>
        <div class="legend-item">
          <div
            class="legend-color"
            style="
              background: var(--color-bg-yellow-light);
              border: 2px solid var(--color-bg-dark-gray);
            "
          ></div>
          <span>Wstrzymane</span>
        </div>
      </div>
    </div>

    <!-- Gantt chart for each room -->
    <div
      v-for="(room, roomIndex) in rooms"
      :key="'room-gantt-' + room.id"
      class="room-gantt-section"
    >
      <div class="room-section-header">
        <i class="fas fa-door-open"></i>
        <h3>{{ room.name }}</h3>
        <span class="task-count">{{ getTasksForRoom(room.id).length }} zadań</span>
        <button
          v-if="room.id"
          class="add-task-btn"
          @click="emit('createTask', room.id)"
          title="Dodaj zadanie"
        >
          <i class="fas fa-plus"></i>
          Nowe zadanie
        </button>
      </div>

      <div class="gantt-container">
        <!-- Timeline header -->
        <div class="gantt-timeline-header">
          <div class="gantt-sidebar-header">Zadanie</div>
          <div class="gantt-timeline-dates-wrapper">
            <div
              class="gantt-timeline-dates"
              :style="{
                width: totalTimelineWidth + 'px',
                transform: `translateX(${-(roomScrollPositions[roomIndex] || 0)}px)`,
              }"
            >
              <div
                v-for="(date, index) in allDates"
                :key="index"
                class="timeline-date"
                :style="{ width: dayWidth + 'px' }"
              >
                <div class="date-day">{{ formatDay(date) }}</div>
                <div class="date-month">{{ formatMonth(date) }}</div>
              </div>
              <!-- Today marker inside the translated dates so it moves with the timeline -->
              <div
                class="today-marker"
                :style="{ left: getDatePosition(today) + 'px' }"
                title="Dzisiaj"
              >
                <div class="today-label">Dziś</div>
              </div>
            </div>
          </div>
        </div>

        <!-- Scrollable content -->
        <div class="gantt-scroll-area" @scroll="handleScroll($event, roomIndex)">
          <div class="gantt-content">
            <!-- Sidebar with task names -->
            <div class="gantt-sidebar">
              <div
                v-for="task in getTasksForRoom(room.id)"
                :key="'task-' + task.id"
                class="gantt-row-label task-label"
                @dblclick="emit('editTask', task)"
              >
                <i class="fas fa-tasks"></i>
                <span class="task-name">{{ task.name }}</span>
                <button class="edit-btn" @click.stop="emit('editTask', task)" title="Edytuj">
                  <i class="fas fa-edit"></i>
                </button>
              </div>
              <div v-if="getTasksForRoom(room.id).length === 0" class="empty-tasks">Brak zadań</div>
            </div>

            <!-- Chart area -->
            <div
              class="gantt-chart"
              :style="{
                width: totalTimelineWidth + 'px',
                height: getChartHeightForTasks(getTasksForRoom(room.id)) + 'px',
              }"
            >
              <!-- Today marker in chart area -->
              <div
                class="today-marker"
                :style="{ left: getDatePosition(today) + 'px' }"
                title="Dzisiaj"
              ></div>
              <!-- Grid lines -->
              <div class="gantt-grid" :style="{ width: totalTimelineWidth + 'px' }">
                <div
                  v-for="(date, index) in allDates"
                  :key="'grid-' + index"
                  class="grid-line"
                  :class="{ weekend: isWeekend(date) }"
                  :style="{ left: index * dayWidth + 'px', width: dayWidth + 'px' }"
                ></div>
              </div>

              <!-- Bars -->
              <div class="gantt-bars">
                <div
                  v-for="(task, taskIndex) in getTasksForRoom(room.id)"
                  :key="'task-bar-' + task.id"
                  class="gantt-bar-row"
                  :style="{ top: taskIndex * taskRowHeight + 'px' }"
                >
                  <div
                    v-if="task.startAt"
                    class="gantt-bar task-bar"
                    :class="{
                      dragging: draggedItem?.id === task.id && draggedItem?.type === 'task',
                    }"
                    :style="getTaskBarStyle(task)"
                    @mousedown="startDrag($event, task, 'task')"
                    @dblclick="emit('editTask', task)"
                  >
                    <div class="bar-content">
                      <span class="bar-text">{{ task.name }}</span>
                      <span class="bar-duration">{{ calculateDuration(task) }}d</span>
                    </div>
                    <div
                      class="resize-handle resize-left"
                      @mousedown.stop="startResize($event, task, 'task', 'start')"
                    ></div>
                    <div
                      class="resize-handle resize-right"
                      @mousedown.stop="startResize($event, task, 'task', 'end')"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Orphan tasks section -->
    <div v-if="orphanTasks.length > 0" class="room-gantt-section">
      <div class="room-section-header orphan-header">
        <i class="fas fa-exclamation-triangle"></i>
        <h3>Zadania bez pokoju</h3>
        <span class="task-count">{{ orphanTasks.length }} zadań</span>
      </div>

      <div class="gantt-container">
        <!-- Timeline header -->
        <div class="gantt-timeline-header">
          <div class="gantt-sidebar-header">Zadanie</div>
          <div class="gantt-timeline-dates-wrapper">
            <div
              class="gantt-timeline-dates"
              :style="{
                width: totalTimelineWidth + 'px',
                transform: `translateX(${-orphanScrollPosition}px)`,
              }"
            >
              <div
                v-for="(date, index) in allDates"
                :key="index"
                class="timeline-date"
                :style="{ width: dayWidth + 'px' }"
              >
                <div class="date-day">{{ formatDay(date) }}</div>
                <div class="date-month">{{ formatMonth(date) }}</div>
              </div>
              <!-- Today marker for orphan timeline -->
              <div
                class="today-marker"
                :style="{ left: getDatePosition(today) + 'px' }"
                title="Dzisiaj"
              ></div>
            </div>
          </div>
        </div>

        <!-- Scrollable content -->
        <div class="gantt-scroll-area" @scroll="handleOrphanScroll($event)">
          <div class="gantt-content">
            <!-- Sidebar with task names -->
            <div class="gantt-sidebar">
              <div
                v-for="task in orphanTasks"
                :key="'orphan-' + task.id"
                class="gantt-row-label task-label"
                @dblclick="emit('editTask', task)"
              >
                <i class="fas fa-tasks"></i>
                <span class="task-name">{{ task.name }}</span>
                <button class="edit-btn" @click.stop="emit('editTask', task)" title="Edytuj">
                  <i class="fas fa-edit"></i>
                </button>
              </div>
            </div>

            <!-- Chart area -->
            <div
              class="gantt-chart"
              :style="{
                width: totalTimelineWidth + 'px',
                height: getChartHeightForTasks(orphanTasks) + 'px',
              }"
            >
              <!-- Grid lines -->
              <div class="gantt-grid" :style="{ width: totalTimelineWidth + 'px' }">
                <div
                  v-for="(date, index) in allDates"
                  :key="'grid-' + index"
                  class="grid-line"
                  :class="{ weekend: isWeekend(date) }"
                  :style="{ left: index * dayWidth + 'px', width: dayWidth + 'px' }"
                ></div>
              </div>

              <!-- Bars -->
              <div class="gantt-bars">
                <div
                  v-for="(task, taskIndex) in orphanTasks"
                  :key="'orphan-bar-' + task.id"
                  class="gantt-bar-row"
                  :style="{ top: taskIndex * taskRowHeight + 'px' }"
                >
                  <div
                    v-if="task.startAt"
                    class="gantt-bar task-bar"
                    :class="{
                      dragging: draggedItem?.id === task.id && draggedItem?.type === 'task',
                    }"
                    :style="getTaskBarStyle(task)"
                    @mousedown="startDrag($event, task, 'task')"
                    @dblclick="emit('editTask', task)"
                  >
                    <div class="bar-content">
                      <span class="bar-text">{{ task.name }}</span>
                      <span class="bar-duration">{{ calculateDuration(task) }}d</span>
                    </div>
                    <div
                      class="resize-handle resize-left"
                      @mousedown.stop="startResize($event, task, 'task', 'start')"
                    ></div>
                    <div
                      class="resize-handle resize-right"
                      @mousedown.stop="startResize($event, task, 'task', 'end')"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue'
import type { RoomDataDTO, TaskDataDTO } from '@/backend/BackendBase'
import { StatusEnum } from '@/backend/BackendBase'
import { makeLocalMidday } from '@/helpers/dateFormatter'
import { Backend } from '@/main'

interface Props {
  rooms: RoomDataDTO[]
  tasks: TaskDataDTO[]
}

const props = defineProps<Props>()

// Constants
const emit = defineEmits<{
  createTask: [roomId: string]
  editTask: [task: TaskDataDTO]
  updateTaskDates: [payload: TaskDataDTO]
}>()

// Constants
const dayWidth = 60 // pixels per day
const taskRowHeight = 45
const sidebarWidth = 200

// Reactive visible days based on window width
const windowWidth = ref(window.innerWidth)
const visibleDaysCount = computed(() => {
  const availableWidth = windowWidth.value - sidebarWidth - 100 // Subtract sidebar and padding
  const days = Math.floor(availableWidth / dayWidth)
  return Math.max(7, Math.min(days, 60)) // Between 7 and 60 days
})

// Timeline
const today = new Date()
today.setHours(0, 0, 0, 0)

// (reverted) Use native Date parsing/normalization below

const startDate = computed(() => {
  let earliest = new Date(today)
  earliest.setDate(earliest.getDate() - 7) // Start 1 week before today

  // Check if we have any earlier dates in tasks
  for (const task of props.tasks) {
    if (task.startAt) {
      const taskDate = new Date(task.startAt)
      taskDate.setHours(0, 0, 0, 0)
      if (taskDate < earliest) {
        earliest = taskDate
      }
    }
  }

  return earliest
})

const endDate = computed(() => {
  let latest = new Date(today)
  latest.setDate(latest.getDate() + 30) // End 30 days from today

  // Check if we have any later dates
  for (const task of props.tasks) {
    if (task.closedAt) {
      const taskDate = new Date(task.closedAt)
      taskDate.setHours(0, 0, 0, 0)
      if (taskDate > latest) {
        latest = taskDate
      }
    }
  }

  return latest
})

const allDates = computed(() => {
  const dates: Date[] = []
  const current = new Date(startDate.value)
  const end = new Date(endDate.value)

  // Limit to prevent infinite loops
  const maxDays = 365
  let dayCount = 0

  while (current <= end && dayCount < maxDays) {
    dates.push(new Date(current))
    current.setDate(current.getDate() + 1)
    dayCount++
  }

  return dates
})

const visibleDates = computed(() => {
  return allDates.value.slice(0, visibleDaysCount.value)
})

const timelineWidth = computed(() => visibleDaysCount.value * dayWidth)
const totalTimelineWidth = computed(() => allDates.value.length * dayWidth)

// Scroll state for each room
const roomScrollPositions = ref<number[]>([])
const orphanScrollPosition = ref(0)

// Initialize scroll positions when rooms change
watch(
  () => props.rooms.length,
  (newLength) => {
    roomScrollPositions.value = new Array(newLength).fill(0)
  },
  { immediate: true },
)

// Drag and drop state
const draggedItem = ref<{
  id: string
  type: 'room' | 'task'
  data: RoomDataDTO | TaskDataDTO
} | null>(null)
const dragStartX = ref(0)
const dragStartDate = ref<Date | null>(null)
const resizeMode = ref<{
  type: 'start' | 'end'
  item: RoomDataDTO | TaskDataDTO
  itemType: 'room' | 'task'
} | null>(null)

// Helpers
const orphanTasks = computed(() => {
  const roomIds = new Set(props.rooms.map((r) => r.id))
  return props.tasks.filter((t) => !t.roomId || !roomIds.has(t.roomId))
})

const getTasksForRoom = (roomId: string | undefined) => {
  if (!roomId) return []
  return props.tasks.filter((t) => t.roomId === roomId)
}

const formatDay = (date: Date) => {
  return date.getDate().toString().padStart(2, '0')
}

const formatMonth = (date: Date) => {
  const months = [
    'Sty',
    'Lut',
    'Mar',
    'Kwi',
    'Maj',
    'Cze',
    'Lip',
    'Sie',
    'Wrz',
    'Paź',
    'Lis',
    'Gru',
  ]
  return months[date.getMonth()]
}

const isWeekend = (date: Date) => {
  const day = date.getDay()
  return day === 0 || day === 6
}

const getDatePosition = (date: Date | string) => {
  const d = new Date(date)
  d.setHours(0, 0, 0, 0)
  const diffTime = d.getTime() - startDate.value.getTime()
  const diffDays = diffTime / (1000 * 60 * 60 * 24)

  return diffDays * dayWidth
}

// ===== NOWA LOGIKA BLOCZKÓW GANTTA =====
// Każdy pasek bazuje wyłącznie na startAt (data początku) oraz estimatedTime (data końca lub liczba dni).
// closedAt ignorujemy przy rysowaniu (możesz dodać osobną stylizację później).

// Helper moved to dateFormatter.ts (makeLocalMidday)

function normalizeTask(task: TaskDataDTO): { start: Date; end: Date; durationDays: number } | null {
  if (!task.startAt) return null
  const start = new Date(task.startAt)
  start.setHours(0, 0, 0, 0)
  const raw = (task as any).estimatedTime
  let end: Date
  if (!raw) {
    end = new Date(start)
  } else {
    end = new Date(raw)
  }
  end.setHours(0, 0, 0, 0)
  // Jeśli końcowa data jest niepoprawna lub wcześniejsza/równa – traktuj jako 1 dzień
  if (isNaN(end.getTime()) || end.getFullYear() === 1 || end < start) {
    return { start, end: start, durationDays: 1 }
  }
  // Liczenie długości INKLUZYWNIE: jeśli end == start -> 1 dzień, jeśli start=20 a end=21 -> 2 dni
  const diffDaysInclusive =
    Math.floor((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24)) + 1
  return { start, end, durationDays: diffDaysInclusive }
}

const getTaskBarStyle = (task: TaskDataDTO) => {
  const norm = normalizeTask(task)
  if (!norm) return {}
  const leftPx = getDatePosition(norm.start)
  const widthPx = norm.durationDays * dayWidth
  const status = task.status
  const locked = status === StatusEnum._0 || status === StatusEnum._2 || status === StatusEnum._3

  let background = 'var(--color-primary-purple)'
  let border: string | undefined = 'none'
  let color = 'white'
  let opacity = '1'

  if (status === StatusEnum._0) {
    // planned
    background = 'var(--color-bg-light-gray)'
    border = '2px dashed var(--color-primary-purple)'
    color = 'var(--color-text-dark)'
    opacity = '0.6'
  } else if (status === StatusEnum._2) {
    // completed
    background = 'var(--color-green)'
    color = 'white'
  } else if (status === StatusEnum._3) {
    // paused
    background = 'var(--color-bg-yellow-light)'
    border = '2px solid var(--color-bg-dark-gray)'
    color = 'var(--color-text-dark)'
  }

  return {
    left: leftPx + 'px',
    width: widthPx + 'px',
    background,
    opacity,
    border,
    color,
    cursor: locked ? 'default' : 'move',
  }
}

const calculateDuration = (task: TaskDataDTO) => {
  const norm = normalizeTask(task)
  return norm ? norm.durationDays : 0
}

// Chart height helper: ensure at least one row height even if no tasks
const getChartHeightForTasks = (taskList: TaskDataDTO[]) =>
  Math.max(taskList.length, 1) * taskRowHeight

// Drag and drop handlers
const startDrag = (event: MouseEvent, item: RoomDataDTO | TaskDataDTO, type: 'room' | 'task') => {
  // don't preventDefault here — allow dblclick to propagate for editing
  if (type === 'task') {
    const t = item as TaskDataDTO
    if (t.status === StatusEnum._0 || t.status === StatusEnum._2 || t.status === StatusEnum._3) {
      // Planned / Zakończony / Wstrzymany tasks nie podlegają drag
      return
    }
  }
  draggedItem.value = { id: item.id!, type, data: item }
  dragStartX.value = event.clientX
  if (type === 'task' && (item as TaskDataDTO).startAt) {
    dragStartDate.value = new Date((item as TaskDataDTO).startAt!)
  }
  document.addEventListener('mousemove', handleDrag)
  document.addEventListener('mouseup', stopDrag)
}

const handleDrag = (event: MouseEvent) => {
  if (!draggedItem.value || !dragStartDate.value) return
  const deltaX = event.clientX - dragStartX.value
  const deltaDays = Math.round(deltaX / dayWidth)
  if (deltaDays === 0) return
  const task = draggedItem.value.data as TaskDataDTO
  // Przesuwamy start (południe lokalne)
  const newStart = new Date(dragStartDate.value)
  newStart.setDate(newStart.getDate() + deltaDays)
  task.startAt = makeLocalMidday(newStart)
  // Przesuwamy koniec jeśli istnieje (też południe)
  if ((task as any).estimatedTime) {
    const end = new Date((task as any).estimatedTime)
    if (!isNaN(end.getTime()) && end.getFullYear() !== 1) {
      end.setDate(end.getDate() + deltaDays)
      ;(task as any).estimatedTime = makeLocalMidday(end)
    }
  }
  dragStartX.value = event.clientX
  dragStartDate.value = newStart
}

const stopDrag = async () => {
  if (draggedItem.value && draggedItem.value.type === 'task') {
    const task = draggedItem.value.data as TaskDataDTO
    const dto: TaskDataDTO = {
      id: task.id!,
      name: task.name,
      description: task.description,
      status: task.status,
      priority: task.priority,
      createAt: task.createAt,
      startAt: task.startAt ? makeLocalMidday(new Date(task.startAt)) : undefined,
      closedAt: task.closedAt,
      roomId: task.roomId,
      projectId: task.projectId,
      userId: task.userId,
      estimatedTime: (task as any).estimatedTime
        ? makeLocalMidday(new Date((task as any).estimatedTime))
        : undefined,
    }
    try {
      const ok = await Backend.editTask(dto)
      if (!ok) {
        console.warn('[Gantt] Nie udało się zapisać zmian zadania (drag).')
      }
      emit('updateTaskDates', dto)
    } catch (e) {
      console.error('[Gantt] Błąd zapisu zadania po drag:', e)
    }
  }
  draggedItem.value = null
  dragStartDate.value = null
  document.removeEventListener('mousemove', handleDrag)
  document.removeEventListener('mouseup', stopDrag)
}

const startResize = (
  event: MouseEvent,
  item: RoomDataDTO | TaskDataDTO,
  itemType: 'room' | 'task',
  type: 'start' | 'end',
) => {
  event.preventDefault()
  event.stopPropagation()
  if (itemType === 'task') {
    const t = item as TaskDataDTO
    if (t.status === StatusEnum._0 || t.status === StatusEnum._2 || t.status === StatusEnum._3) {
      // Planned / Zakończony / Wstrzymany tasks nie podlegają resize
      return
    }
  }
  resizeMode.value = { type, item, itemType }
  dragStartX.value = event.clientX
  document.addEventListener('mousemove', handleResize)
  document.addEventListener('mouseup', stopResize)
}

const handleResize = (event: MouseEvent) => {
  if (!resizeMode.value) return
  const deltaX = event.clientX - dragStartX.value
  const deltaDays = Math.round(deltaX / dayWidth)
  if (deltaDays === 0) return
  const { item, type } = resizeMode.value
  const task = item as TaskDataDTO
  if (!task.startAt) return
  if (type === 'start') {
    const newStart = new Date(task.startAt)
    newStart.setDate(newStart.getDate() + deltaDays)
    task.startAt = makeLocalMidday(newStart)
  } else if (type === 'end') {
    if ((task as any).estimatedTime) {
      const end = new Date((task as any).estimatedTime)
      if (!isNaN(end.getTime()) && end.getFullYear() !== 1) {
        end.setDate(end.getDate() + deltaDays)
        ;(task as any).estimatedTime = makeLocalMidday(end)
      }
    } else if (task.startAt) {
      const end = new Date(task.startAt)
      end.setDate(end.getDate() + Math.max(deltaDays, 1))
      ;(task as any).estimatedTime = makeLocalMidday(end)
    }
  }
  dragStartX.value = event.clientX
}

const stopResize = async () => {
  if (resizeMode.value && resizeMode.value.itemType === 'task') {
    const task = resizeMode.value.item as TaskDataDTO
    const dto: TaskDataDTO = {
      id: task.id!,
      name: task.name,
      description: task.description,
      status: task.status,
      priority: task.priority,
      createAt: task.createAt,
      startAt: task.startAt ? makeLocalMidday(new Date(task.startAt)) : undefined,
      closedAt: task.closedAt,
      roomId: task.roomId,
      projectId: task.projectId,
      userId: task.userId,
      estimatedTime: (task as any).estimatedTime
        ? makeLocalMidday(new Date((task as any).estimatedTime))
        : undefined,
    }
    try {
      const ok = await Backend.editTask(dto)
      if (!ok) {
        console.warn('[Gantt] Nie udało się zapisać zmian zadania (resize).')
      }
      emit('updateTaskDates', dto)
    } catch (e) {
      console.error('[Gantt] Błąd zapisu zadania po resize:', e)
    }
  }
  resizeMode.value = null
  document.removeEventListener('mousemove', handleResize)
  document.removeEventListener('mouseup', stopResize)
}

const handleScroll = (event: Event, roomIndex: number) => {
  const target = event.target as HTMLElement
  if (target) {
    roomScrollPositions.value[roomIndex] = target.scrollLeft
  }
}

const handleOrphanScroll = (event: Event) => {
  const target = event.target as HTMLElement
  if (target) {
    orphanScrollPosition.value = target.scrollLeft
  }
}

// Handle window resize
const handleWindowResize = () => {
  windowWidth.value = window.innerWidth
}

let resizeTimeout: number | null = null
const debouncedHandleResize = () => {
  if (resizeTimeout) {
    clearTimeout(resizeTimeout)
  }
  resizeTimeout = window.setTimeout(() => {
    handleWindowResize()
  }, 150)
}

onMounted(() => {
  window.addEventListener('resize', debouncedHandleResize)
})

onUnmounted(() => {
  if (resizeTimeout) {
    clearTimeout(resizeTimeout)
  }
  window.removeEventListener('resize', debouncedHandleResize)
  document.removeEventListener('mousemove', handleDrag)
  document.removeEventListener('mouseup', stopDrag)
  document.removeEventListener('mousemove', handleResize)
  document.removeEventListener('mouseup', stopResize)
})
</script>

<style scoped>
.gantt-wrapper {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.gantt-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 1rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid var(--color-bg-light-gray);
}

.gantt-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 12px;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.gantt-legend {
  display: flex;
  gap: 1.5rem;
  flex-wrap: wrap;
}

.legend-item {
  display: flex;
  align-items: center;
  gap: 8px;
}

.legend-color {
  width: 24px;
  height: 12px;
  border-radius: 4px;
}

.room-gantt-section {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 4px 16px var(--shadow-light);
  padding: 1.5rem;
  overflow: hidden;
}

.room-section-header {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid var(--color-bg-light-gray);
}

.room-section-header i {
  font-size: 1.5rem;
  color: var(--color-primary-blue);
}

.room-section-header h3 {
  font-size: 1.5rem;
  font-weight: 700;
  margin: 0;
  color: var(--color-text-dark);
  flex: 1;
}

.room-section-header.orphan-header i {
  color: var(--color-red);
}

.task-count {
  font-size: 0.9rem;
  color: var(--color-text-medium);
  background: var(--color-bg-light-gray);
  padding: 4px 12px;
  border-radius: 12px;
  font-weight: 600;
}

.gantt-container {
  border: 1px solid var(--color-bg-light-gray);
  border-radius: 12px;
  overflow: hidden;
  background: var(--color-bg-white);
}

.gantt-timeline-header {
  display: flex;
  background: var(--color-bg-light-gray);
  border-bottom: 2px solid #ddd;
  position: sticky;
  top: 0;
  z-index: 10;
}

.gantt-sidebar-header {
  width: 200px;
  min-width: 200px;
  padding: 12px 16px;
  font-weight: 700;
  color: var(--color-text-dark);
  border-right: 2px solid #ddd;
  background: var(--color-bg-light-gray);
}

.gantt-timeline-dates-wrapper {
  flex: 1;
  overflow: hidden;
  position: relative;
}

.gantt-timeline-dates {
  display: flex;
  position: relative;
  transition: transform 0.05s linear;
}

.timeline-date {
  border-right: 1px solid #e0e0e0;
  padding: 8px 4px;
  text-align: center;
  flex-shrink: 0;
}

.date-day {
  font-weight: 700;
  font-size: 1rem;
  color: var(--color-text-dark);
}

.date-month {
  font-size: 0.75rem;
  color: var(--color-text-medium);
  text-transform: uppercase;
}

.gantt-scroll-area {
  overflow-x: auto;
  overflow-y: auto;
  max-height: 400px;
  position: relative;
}

.gantt-content {
  display: flex;
  position: relative;
  min-width: fit-content;
}

.gantt-sidebar {
  width: 200px;
  min-width: 200px;
  background: var(--color-bg-white);
  border-right: 2px solid #ddd;
  position: sticky;
  left: 0;
  z-index: 5;
  align-self: flex-start;
}

.gantt-row-label {
  height: 45px;
  padding: 12px 16px;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9rem;
  background: var(--color-bg-white);
}

.task-label {
  color: var(--color-text-dark);
  font-size: 0.85rem;
}

.task-name {
  margin-left: 8px;
  flex: 1 1 auto;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.edit-btn {
  background: transparent;
  border: none;
  color: var(--color-primary-blue);
  cursor: pointer;
  padding: 6px;
  margin-left: 8px;
  border-radius: 6px;
}

.edit-btn:hover {
  background: rgba(0, 0, 0, 0.04);
}

.empty-tasks {
  height: 45px;
  padding: 12px 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: var(--color-text-medium);
  font-style: italic;
  font-size: 0.85rem;
}

.gantt-chart {
  position: relative;
}

.gantt-grid {
  position: absolute;
  top: 0;
  left: 0;
  height: 100%;
  display: flex;
}

.grid-line {
  position: absolute;
  top: 0;
  height: 100%;
  border-right: 1px solid #f0f0f0;
}

.grid-line.weekend {
  background: rgba(0, 0, 0, 0.02);
}

/* Today marker */
.today-marker {
  position: absolute;
  top: 0;
  bottom: 0;
  width: 2px;
  background: var(--color-red, #e53935);
  z-index: 15;
  pointer-events: none;
}

.today-marker .today-label {
  position: absolute;
  top: -20px;
  left: -10px;
  background: var(--color-red, #e53935);
  color: white;
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 0.7rem;
  white-space: nowrap;
}

.gantt-bars {
  position: relative;
  width: 100%;
  height: 100%;
}

.gantt-bar-row {
  position: absolute;
  left: 0;
  right: 0;
  height: 45px;
  border-bottom: 1px solid #e0e0e0;
}

.gantt-bar {
  position: absolute;
  height: 30px;
  top: 7px;
  border-radius: 8px;
  cursor: move;
  transition:
    transform 0.2s ease,
    box-shadow 0.2s ease;
  display: flex;
  align-items: center;
  padding: 0 12px;
  color: white;
  font-weight: 600;
  font-size: 0.8rem;
  user-select: none;
}

.gantt-bar:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.gantt-bar.dragging {
  opacity: 0.8;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.3);
  cursor: grabbing;
}

.task-bar {
  background: var(--color-primary-purple);
}
.task-bar.planned {
  cursor: default;
}

.bar-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  overflow: hidden;
  gap: 8px;
}

.bar-text {
  flex: 1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.bar-duration {
  font-size: 0.7rem;
  opacity: 0.9;
  white-space: nowrap;
}

.resize-handle {
  position: absolute;
  top: 0;
  bottom: 0;
  width: 8px;
  cursor: ew-resize;
  opacity: 0;
  transition: opacity 0.2s;
}

.gantt-bar:hover .resize-handle {
  opacity: 1;
}

.resize-left {
  left: 0;
  background: linear-gradient(to right, rgba(255, 255, 255, 0.4), transparent);
}

.resize-right {
  right: 0;
  background: linear-gradient(to left, rgba(255, 255, 255, 0.4), transparent);
}

@media (max-width: 768px) {
  .gantt-wrapper {
    gap: 1.5rem;
  }

  .gantt-title {
    font-size: 1.5rem;
  }

  .room-gantt-section {
    padding: 1rem;
  }

  .room-section-header h3 {
    font-size: 1.25rem;
  }

  .gantt-sidebar,
  .gantt-sidebar-header {
    width: 150px;
    min-width: 150px;
  }

  .gantt-scroll-area {
    max-height: 300px;
  }
}
</style>
