<template>
  <MainLayout>
    <div class="contacts-container">
      <div class="contacts-header">
        <h1 class="contacts-title">Kalkulacje</h1>
        <button class="btn btn-primary" @click="createNewCalculation">
          <font-awesome-icon icon="plus" /> Nowa Kalkulacja
        </button>
      </div>

      <div v-if="!loading && calculationList.length > 0" class="search-container">
        <input
          v-model="searchQuery"
          type="text"
          class="form-control search-input"
          placeholder="Szukaj kalkulacji (nazwa, typ, projekt, pokój, wartość)..."
        />
        <font-awesome-icon icon="search" class="search-icon" />
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie kalkulacji...</p>
      </div>

      <div v-else-if="calculationList.length === 0" class="empty-state">
        <font-awesome-icon icon="calculator" style="font-size: 5rem" />
        <h2>Brak kalkulacji</h2>
        <p>Dodaj kalkulacje do swoich projektów i pokoi</p>
        <button class="btn btn-primary btn-large" @click="createNewCalculation">
          Dodaj pierwszą kalkulację
        </button>
      </div>

      <div v-else class="table-container">
        <table class="contacts-table">
          <thead>
            <tr>
              <th>Nazwa</th>
              <th>Typ kalkulacji</th>
              <th>Projekt</th>
              <th>Pokój</th>
              <th>Wartość</th>
              <th>Akcje</th>
            </tr>
          </thead>
          <tbody>
            <tr
              v-for="calculation in filteredCalculations"
              :key="calculation.id"
              class="contact-row"
            >
              <td class="calculation-name">
                <strong>{{ calculation.name || 'Bez nazwy' }}</strong>
              </td>
              <td class="contact-spec">
                <span class="spec-badge">{{ getTypeLabel(calculation.type) }}</span>
              </td>
              <td class="project-name">
                {{ getProjectName(calculation.projectId) }}
              </td>
              <td class="room-name">
                {{ getRoomName(calculation.roomId) }}
              </td>
              <td class="calculation-value">
                {{ formatValue(calculation.value) }}
              </td>
              <td>
                <div class="action-buttons">
                  <button
                    class="btn btn-sm btn-primary"
                    @click="openCalculation(calculation.id)"
                    title="Otwórz"
                  >
                    <font-awesome-icon icon="eye" />
                  </button>
                  <button
                    class="btn btn-sm btn-primary"
                    @click="onEdit(calculation.id)"
                    title="Edytuj"
                  >
                    <font-awesome-icon icon="edit" />
                  </button>
                  <button
                    class="btn btn-sm btn-danger"
                    @click="onDelete(calculation.id)"
                    title="Usuń"
                  >
                    <font-awesome-icon icon="trash-can" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '@/views/layouts/MainLayout.vue'
import type { CalculationDataDTO, ProjectDataDTO, RoomDataDTO } from '@/backend/BackendBase'
import { onMounted, ref, computed } from 'vue'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { getSpecLabel } from '@/helpers/calculationTypeEnumFormatter'
import { useRouter } from 'vue-router'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const calculationList = ref<CalculationDataDTO[]>([])
const projects = ref<Map<string, ProjectDataDTO>>(new Map())
const rooms = ref<Map<string, RoomDataDTO>>(new Map())
const loading = ref(true)
const searchQuery = ref('')
const router = useRouter()

const filteredCalculations = computed(() => {
  if (!searchQuery.value.trim()) {
    return calculationList.value
  }

  const query = searchQuery.value.toLowerCase()
  return calculationList.value.filter((calculation) => {
    const name = (calculation.name || '').toLowerCase()
    const type = getSpecLabel(calculation.type).toLowerCase()
    const value = calculation.value.toString().toLowerCase()
    const projectName = getProjectName(calculation.projectId).toLowerCase()
    const roomName = getRoomName(calculation.roomId).toLowerCase()

    return (
      name.includes(query) ||
      type.includes(query) ||
      value.includes(query) ||
      projectName.includes(query) ||
      roomName.includes(query)
    )
  })
})

const formatValue = (value: number): string => {
  return value.toFixed(2)
}

const getTypeLabel = (type: any): string => {
  return getSpecLabel(type)
}

const getProjectName = (projectId: string | undefined): string => {
  if (!projectId) return '-'
  const project = projects.value.get(projectId)
  return project?.name || 'Ładowanie...'
}

const getRoomName = (roomId: string | undefined): string => {
  if (!roomId) return '-'
  const room = rooms.value.get(roomId)
  return room?.name || 'Ładowanie...'
}

const fetchProjectAndRoomData = async () => {
  const projectIds = new Set<string>()
  const roomIds = new Set<string>()

  calculationList.value.forEach((calc) => {
    if (calc.projectId) projectIds.add(calc.projectId)
    if (calc.roomId) roomIds.add(calc.roomId)
  })

  // Pobierz projekty
  for (const projectId of projectIds) {
    try {
      const project = await Backend.getProjectById(projectId)
      if (project) {
        projects.value.set(projectId, project)
      }
    } catch (error) {
      console.error(`Error fetching project ${projectId}:`, error)
    }
  }

  // Pobierz pokoje
  for (const roomId of roomIds) {
    try {
      const room = await Backend.getRoomById(roomId)
      if (room) {
        rooms.value.set(roomId, room)
      }
    } catch (error) {
      console.error(`Error fetching room ${roomId}:`, error)
    }
  }
}

const fetchCalculations = async () => {
  try {
    loading.value = true
    const userId = getCurrentUserId()
    if (!userId) {
      console.error('No user ID found')
      return
    }
    calculationList.value = await Backend.getCalculationListByUserId(userId)
    await fetchProjectAndRoomData()
  } catch (error) {
    console.error('Error fetching calculations:', error)
  } finally {
    loading.value = false
  }
}

const createNewCalculation = () => {
  router.push({ name: 'CalculatorCreate' })
}

const openCalculation = (calculationId: string | undefined) => {
  if (!calculationId) return
  router.push({ name: 'CalculatorDetails', params: { calculationId } })
}

const onEdit = (calculationId: string | undefined) => {
  if (!calculationId) return
  router.push({ name: 'CalculatorEdit', params: { calculationId } })
}

const onDelete = async (calculationId: string | undefined) => {
  if (!calculationId) return
  if (!confirm('Czy na pewno chcesz usunąć tę kalkulację?')) return
  try {
    await Backend.deleteCalculation(calculationId)
    await fetchCalculations()
  } catch (e) {
    console.error('Nie udało się usunąć kalkulacji', e)
  }
}

onMounted(() => fetchCalculations())
</script>

<style scoped>
.contacts-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.contacts-header .btn {
  display: flex;
  align-items: center;
  gap: 8px;
}

.search-container {
  position: relative;
  margin-bottom: 1.5rem;
  max-width: 600px;
  margin-left: auto;
}

.search-input {
  width: 100%;
  padding: 0.75rem 2.5rem 0.75rem 1rem;
  border: 2px solid var(--color-bg-light-gray);
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.2s ease;
}

.search-input:focus {
  outline: none;
  border-color: var(--color-primary-blue);
  box-shadow: 0 0 0 3px rgba(74, 144, 226, 0.1);
}

.search-icon {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: var(--color-text-medium);
  pointer-events: none;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  gap: 1rem;
}

.empty-state i {
  font-size: 5rem;
  color: var(--color-primary-purple);
  opacity: var(--opacity-medium);
}

.empty-state h2 {
  font-size: 2rem;
  color: var(--color-text-dark);
  margin: 0;
}

.empty-state p {
  font-size: 1.1rem;
  color: var(--color-text-medium);
  max-width: 600px;
}

.table-container {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  overflow: hidden auto;
}

.contacts-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}

.contacts-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--gradient-primary);
}

.contacts-table th {
  padding: 1rem;
  text-align: left;
  color: var(--color-text-white);
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.contacts-table td {
  padding: 1rem;
  color: var(--color-text-dark);
}

.contacts-table tbody tr {
  border-bottom: 1px solid var(--color-bg-light-gray);
  transition: all 0.2s ease;
}

.contact-row:hover {
  background: var(--color-bg-light-gray);
}

.contact-spec {
  font-size: 0.9rem;
}

.spec-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  background: var(--color-bg-blue-pale);
  color: var(--color-primary-blue);
  font-weight: 600;
  font-size: 0.85rem;
}

.project-name,
.room-name {
  color: var(--color-text-dark);
  font-weight: 500;
}

.calculation-name {
  color: var(--color-text-dark);
  font-weight: 600;
  font-size: 1rem;
}

.calculation-value {
  font-weight: 600;
  color: var(--color-primary-blue);
  font-size: 1.1rem;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

@media (max-width: 768px) {
  .contacts-container {
    padding: 1rem;
  }

  .contacts-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }

  .contacts-title {
    font-size: 2rem;
    text-align: center;
  }

  .contacts-header .btn {
    width: 100%;
    justify-content: center;
  }

  .contacts-table {
    font-size: 0.85rem;
  }

  .contacts-table th,
  .contacts-table td {
    padding: 0.75rem 0.5rem;
  }

  .action-buttons {
    flex-direction: column;
  }
}
</style>
