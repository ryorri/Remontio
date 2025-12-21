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
            </tr>
          </thead>
          <tbody>
            <template v-for="budget in budgetList" :key="budget.id">
              <tr class="budget-row" @click="openBudget(budget.id)">
                <td class="budget-name">{{ budget.name }}</td>
                <td class="budget-description">
                  {{ budget.description || 'Brak opisu' }}
                </td>
                <td class="budget-meta">
                  {{ projectNames[budget.id!] || '-' }}
                </td>
                <td class="budget-meta">
                  {{ roomNames[budget.id!] || '-' }}
                </td>

                <td class="budget-amount">
                  {{ formatCurrency(budget.spent ?? 0, (budget as any)?.currency || 'PLN') }}
                </td>
                <td class="budget-amount">
                  {{ formatCurrency(budget.estimatedPrice, (budget as any)?.currency || 'PLN') }}
                </td>
                <td class="date-cell">
                  {{ formatDate(budget.createAt) }}
                </td>
              </tr>
              <tr class="toggle-arrow" @click.stop="toggleMenu(budget.id)">
                <td colspan="8">{{ openMenuId === budget.id ? '▲' : '▼' }} Akcje</td>
              </tr>
              <tr v-if="openMenuId === budget.id" class="actions-dropdown-row" @click.stop>
                <td colspan="8">
                  <div class="actions-panel">
                    <div class="panel-actions">
                      <button
                        class="panel-item"
                        @click="handleAction(() => viewShoppingLists(budget.id))"
                      >
                        <i class="fas fa-shopping-cart"></i> Listy zakupowe
                      </button>
                      <button class="panel-item" @click="handleAction(() => openBudget(budget.id))">
                        Otwórz
                      </button>
                      <button class="panel-item" @click="handleAction(() => onEdit(budget.id))">
                        Edytuj
                      </button>
                      <button
                        class="panel-item danger"
                        @click="handleAction(() => onDelete(budget.id))"
                      >
                        Usuń
                      </button>
                    </div>
                  </div>
                </td>
              </tr>
            </template>
          </tbody>
        </table>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '@/views/layouts/MainLayout.vue'
import type { BudgetDataDTO } from '@/backend/BackendBase'
import { onMounted, onUnmounted, ref } from 'vue'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { formatDate } from '@/helpers/dateFormatter'
import { formatCurrency } from '@/helpers/currencyFormatter'
import { useRouter } from 'vue-router'

const budgetList = ref<BudgetDataDTO[]>([])
const loading = ref(true)
const openMenuId = ref<string | null>(null)
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

const toggleMenu = (budgetId: string | undefined) => {
  if (!budgetId) return
  openMenuId.value = openMenuId.value === budgetId ? null : budgetId
}

const handleAction = (action: () => void) => {
  action()
  openMenuId.value = null
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

const handleOutsideClick = () => {
  openMenuId.value = null
}

onMounted(async () => {
  window.addEventListener('click', handleOutsideClick)
  await fetchBudgets()
})

onUnmounted(() => {
  window.removeEventListener('click', handleOutsideClick)
})
</script>

<style scoped>
.actions-panel {
  background: var(--color-bg-light-gray);
  border-top: 1px solid var(--color-bg-light-gray);
  box-shadow: inset 0 1px 0 var(--shadow-light);
  padding: 12px 16px;
}

.panel-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 10px;
  justify-content: center;
}

.panel-item {
  padding: 10px 14px;
  background: var(--color-bg-light-gray);
  border: 1px solid var(--color-bg-light-gray);
  border-radius: 8px;
  color: var(--color-text-dark);
  cursor: pointer;
  transition: all 0.2s ease;
}

.panel-item:hover {
  background: var(--color-bg-white);
  transform: translateY(-1px);
  box-shadow: 0 4px 10px var(--shadow-light);
}

.panel-item.danger {
  color: var(--color-red);
}
</style>
