<template>
  <MainLayout>
    <div class="shoppinglists-container">
      <div class="shoppinglists-header">
        <h1 class="shoppinglists-title">
          Listy zakupowe dla
          {{ budgetName }}
        </h1>
        <button class="btn btn-primary" @click="createNewShoppingList">Nowa lista zakupowa</button>
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie list zakupowych...</p>
      </div>

      <div v-else-if="shoppingLists.length === 0" class="empty-state">
        <h2>Brak list zakupowych</h2>
        <p>Utwórz swoją pierwszą listę zakupową dla tego budżetu</p>
        <button class="btn btn-primary btn-large" @click="createNewShoppingList">
          Stwórz pierwszą listę
        </button>
      </div>

      <div v-else class="table-container">
        <table class="shoppinglists-table">
          <thead>
            <tr>
              <th>Nazwa listy</th>
              <th>Opis</th>
              <th>Liczba pozycji</th>
              <th>Utworzono</th>
              <th>Akcje</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="list in shoppingLists" :key="list.id" class="shoppinglist-row">
              <td class="list-name">{{ list.name }}</td>
              <td class="list-description">{{ list.description || 'Brak opisu' }}</td>
              <td class="list-count">{{ list.itemCount || 0 }}</td>
              <td class="date-cell">{{ formatDate(list.createAt) }}</td>
              <td>
                <div class="action-buttons">
                  <button
                    class="btn btn-sm btn-primary"
                    @click="editShoppingList(list.id)"
                    title="Edytuj"
                  >
                    <font-awesome-icon icon="edit" />
                  </button>
                  <button
                    class="btn btn-sm btn-danger"
                    @click="deleteShoppingList(list.id)"
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

      <div class="mt-4">
        <button class="btn btn-primary" @click="goBack">
          <i class="fas fa-arrow-left"></i> Powrót do budżetów
        </button>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { formatDate } from '@/helpers/dateFormatter'
import { Backend } from '@/main'
import { getCurrentUserId } from '@/helpers/userHelpers'

interface UiShoppingListRow {
  id: string
  name: string
  description?: string
  itemCount?: number
  createAt: Date
}

const router = useRouter()
const route = useRoute()
const shoppingLists = ref<UiShoppingListRow[]>([])
const budgetName = ref<string>('')
const loading = ref(true)
const budgetId = ref<string>(route.params.budgetId as string)

const fetchShoppingLists = async () => {
  loading.value = true
  try {
    const budget = await Backend.getBudgetById(budgetId.value)
    budgetName.value = budget.name || ''
    const projectId = budget.projectId || ''
    const roomId = budget.roomId || ''

    const userId = getCurrentUserId()
    if (!userId) {
      shoppingLists.value = []
      return
    }
    const lists = await Backend.getListByUserId(userId)
    const filtered = lists.filter(
      (l) => (l.projectId || '') === projectId && (l.roomId || '') === roomId,
    )

    // Fetch actual item counts from backend for each list
    const listsWithCounts = await Promise.all(
      filtered.map(async (l) => {
        let itemCount = 0
        try {
          const items = await Backend.getItemListByListId(l.id || '')
          itemCount = items.length
        } catch (e) {
          console.warn(`Nie udało się pobrać pozycji dla listy ${l.id}`, e)
        }
        return {
          id: l.id || '',
          name: l.name || '',
          description: l.description || undefined,
          itemCount,
          createAt: new Date(l.createAt),
        }
      }),
    )

    shoppingLists.value = listsWithCounts
  } catch (e) {
    console.error('Error loading shopping lists', e)
  } finally {
    loading.value = false
  }
}

const createNewShoppingList = () => {
  router.push({ name: 'ShoppingListCreate', params: { budgetId: budgetId.value } })
}

const editShoppingList = (listId: string) => {
  router.push({ name: 'ShoppingListEdit', params: { budgetId: budgetId.value, listId } })
}

const deleteShoppingList = async (listId: string) => {
  if (!confirm('Czy na pewno chcesz usunąć tę listę?')) return
  try {
    await Backend.deleteList(listId)
    await fetchShoppingLists()
  } catch (e) {
    console.error('Nie udało się usunąć listy', e)
  }
}

const goBack = () => {
  router.push({ name: 'BudgetList' })
}

onMounted(fetchShoppingLists)
</script>

<style scoped>
.shoppinglists-container {
  width: 100%;
  max-width: 1400px;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.shoppinglists-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  flex-wrap: wrap;
  gap: 1rem;
}

.shoppinglists-title {
  font-size: 2.5rem;
  font-weight: 700;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 1rem;
  flex-wrap: wrap;
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

.shoppinglists-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}

.shoppinglists-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--gradient-primary);
}

.shoppinglists-table th {
  padding: 1rem;
  text-align: left;
  color: var(--color-text-white);
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.shoppinglists-table td {
  padding: 1rem;
  color: var(--color-text-dark);
}

.shoppinglists-table tbody tr {
  border-bottom: 1px solid var(--color-bg-light-gray);
  transition: all 0.2s ease;
}

.shoppinglist-row:hover {
  background: var(--color-bg-light-gray);
}

.list-name {
  font-weight: 600;
  color: var(--color-primary-blue);
}

.list-description {
  max-width: 400px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-medium);
}

.list-count {
  font-weight: 500;
  color: var(--color-text-dark);
}

.date-cell {
  white-space: nowrap;
  color: var(--color-text-medium);
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
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
