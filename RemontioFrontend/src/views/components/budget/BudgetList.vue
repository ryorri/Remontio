<template>
  <MainLayout>
    <div class="budgets-container">
      <div class="budgets-header">
        <h1 class="budgets-title">Budżety</h1>
        <button class="btn btn-primary" @click="createNewBudget">
          <i class="fas fa-plus"></i> Nowy Budżet
        </button>
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie budżetów...</p>
      </div>

      <div v-else-if="budgetList.length === 0" class="empty-state">
        <i class="fas fa-wallet"></i>
        <h2>Brak budżetów</h2>
        <p>Utwórz swój pierwszy budżet, aby zarządzać kosztami remontu</p>
        <button class="btn btn-primary btn-large" @click="createNewBudget">
          Stwórz swój pierwszy budżet
        </button>
      </div>

      <div v-else class="table-container">
        <table class="budgets-table">
          <thead>
            <tr>
              <th>Nazwa budżetu</th>
              <th>Opis</th>
              <th>Projekt</th>
              <th>Pokój</th>
              <th>Wydano</th>
              <th>Szacowana cena</th>
              <th>Utworzono</th>
              <th>Akcje</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="budget in budgetList" :key="budget.id" class="budget-row">
              <td class="budget-name" @click="openBudget(budget.id)">{{ budget.name }}</td>
              <td class="budget-description" @click="openBudget(budget.id)">
                {{ budget.description || 'Brak opisu' }}
              </td>
              <td class="budget-meta" @click="openBudget(budget.id)">
                {{ projectNames[budget.id!] || '-' }}
              </td>
              <td class="budget-meta" @click="openBudget(budget.id)">
                {{ roomNames[budget.id!] || '-' }}
              </td>

              <td class="budget-amount" @click="openBudget(budget.id)">
                {{ formatCurrency(budget.spent ?? 0, (budget as any)?.currency || 'PLN') }}
              </td>
              <td class="budget-amount" @click="openBudget(budget.id)">
                {{ formatCurrency(budget.estimatedPrice, (budget as any)?.currency || 'PLN') }}
              </td>
              <td class="date-cell" @click="openBudget(budget.id)">
                {{ formatDate(budget.createAt) }}
              </td>
              <td>
                <div class="action-buttons">
                  <button
                    class="btn btn-sm btn-primary"
                    @click="openBudget(budget.id)"
                    title="Otwórz"
                  >
                    <font-awesome-icon :icon="['fas', 'folder-open']" />
                  </button>
                  <button
                    class="btn btn-sm btn-info"
                    @click="viewShoppingLists(budget.id)"
                    title="Listy zakupowe"
                  >
                    <font-awesome-icon :icon="['fas', 'shopping-cart']" />
                  </button>
                  <button class="btn btn-sm btn-primary" @click="onEdit(budget.id)" title="Edytuj">
                    <font-awesome-icon :icon="['fas', 'edit']" />
                  </button>
                  <button
                    class="btn btn-sm btn-success"
                    @click="openExportModal(budget.id)"
                    title="Eksportuj"
                  >
                    <font-awesome-icon :icon="['fas', 'file-export']" />
                  </button>
                  <button class="btn btn-sm btn-danger" @click="onDelete(budget.id)" title="Usuń">
                    <font-awesome-icon :icon="['fas', 'trash-can']" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <BudgetExportModal
        v-if="showExportModal && exportBudgetData"
        :budget="exportBudgetData"
        @close="closeExportModal"
      />
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import BudgetExportModal from './BudgetExportModal.vue'
const showExportModal = ref(false)
const exportBudgetData = ref<any | null>(null)

const openExportModal = (budgetId: string | undefined) => {
  if (!budgetId) return
  const budget = budgetList.value.find((b) => b.id === budgetId)
  if (!budget) return
  exportBudgetData.value = {
    ...budget,
    projectName: projectNames.value[budget.id!] || '-',
    roomName: roomNames.value[budget.id!] || '-',
  }
  showExportModal.value = true
}

const closeExportModal = () => {
  showExportModal.value = false
  exportBudgetData.value = null
}
import { Backend } from '@/main'
import MainLayout from '@/views/layouts/MainLayout.vue'
import type { BudgetDataDTO } from '@/backend/BackendBase'
import { onMounted, ref } from 'vue'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { formatDate } from '@/helpers/dateFormatter'
import { formatCurrency } from '@/helpers/currencyFormatter'
import { useRouter } from 'vue-router'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const budgetList = ref<BudgetDataDTO[]>([])
const loading = ref(true)
const projectNames = ref<Record<string, string>>({})
const roomNames = ref<Record<string, string>>({})
const router = useRouter()

const fetchBudgets = async () => {
  try {
    loading.value = true
    budgetList.value = await Backend.getBudgetListByUserId(getCurrentUserId()!)

    // Fetch project and room names for each budget
    const projectMap: Record<string, string> = {}
    const roomMap: Record<string, string> = {}

    await Promise.all(
      budgetList.value.map(async (budget) => {
        if (budget.projectId) {
          try {
            const project = await Backend.getProjectById(budget.projectId)
            projectMap[budget.id!] = project.name || 'Nieznany projekt'
          } catch {
            projectMap[budget.id!] = 'Błąd ładowania'
          }
        }
        if (budget.roomId) {
          try {
            const room = await Backend.getRoomById(budget.roomId)
            roomMap[budget.id!] = room.name || 'Nieznany pokój'
          } catch {
            roomMap[budget.id!] = 'Błąd ładowania'
          }
        }
      }),
    )

    projectNames.value = projectMap
    roomNames.value = roomMap
  } catch (error) {
    console.error('Error fetching budgets:', error)
  } finally {
    loading.value = false
  }
}

const createNewBudget = () => {
  router.push({ name: 'BudgetCreate' })
}

const openBudget = (budgetId: string | undefined) => {
  if (!budgetId) return
  router.push({ name: 'BudgetDetails', params: { budgetId } })
}

const onEdit = (budgetId: string | undefined) => {
  if (!budgetId) return
  router.push({ name: 'BudgetEdit', params: { budgetId } })
}

const onDelete = async (budgetId: string | undefined) => {
  if (!budgetId) return
  if (!confirm('Czy na pewno chcesz usunąć ten budżet?')) return
  try {
    await Backend.deleteBudget(budgetId)
    await fetchBudgets()
  } catch (e) {
    console.error('Nie udało się usunąć budżetu', e)
  }
}

const viewShoppingLists = (budgetId: string | undefined) => {
  if (!budgetId) return
  router.push({ name: 'ShoppingListsByBudget', params: { budgetId } })
}

onMounted(async () => {
  await fetchBudgets()
})
</script>

<style scoped>
.budgets-container {
  width: 100%;
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.budgets-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
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

.budgets-header .btn {
  display: flex;
  align-items: center;
  gap: 8px;
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
  margin: 0;
}

.table-container {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  overflow: hidden auto;
}

.budgets-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}

.budgets-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--gradient-primary);
}

.budgets-table th {
  padding: 1rem;
  text-align: center;
  color: var(--color-text-white);
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.budgets-table td {
  padding: 1rem;
  color: var(--color-text-dark);
}

.budgets-table tbody tr {
  border-bottom: 1px solid var(--color-bg-light-gray);
  transition: all 0.2s ease;
}

.budget-row:hover {
  background: var(--color-bg-light-gray);
}

.budget-name {
  font-weight: 600;
  color: var(--color-primary-blue);
  cursor: pointer;
}

.budget-description {
  max-width: 300px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-medium);
  cursor: pointer;
}

.budget-meta {
  color: var(--color-text-medium);
  font-size: 0.9rem;
  white-space: nowrap;
  cursor: pointer;
}

.budget-amount {
  font-weight: 500;
  color: var(--color-text-dark);
  white-space: nowrap;
  cursor: pointer;
}

.date-cell {
  white-space: nowrap;
  color: var(--color-text-medium);
  cursor: pointer;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
  justify-content: center;
}

.loading-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  gap: 1rem;
}
</style>
