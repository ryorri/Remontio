<template>
  <MainLayout>
    <div class="details-wrapper" v-if="!loading && budget">
      <div class="details-header">
        <div class="title-block">
          <h1 class="details-title">Budżet: {{ budget.name }}</h1>
          <span class="badge">Utworzono: {{ formatDate(budget.createAt) }}</span>
        </div>
        <div class="header-actions">
          <button class="btn btn-success header-btn" @click="goToAddItem">
            <font-awesome-icon :icon="['fas', 'plus']" /> Dodaj pozycję
          </button>
          <button class="btn btn-primary header-btn" @click="goToEdit">
            <font-awesome-icon :icon="['fas', 'edit']" /> Edytuj
          </button>
        </div>
      </div>

      <div class="details-content">
        <section class="content-section">
          <h3 class="section-title">
            <i class="fas fa-wallet"></i>
            Podsumowanie budżetu
          </h3>
          <div class="meta-grid">
            <div class="meta-item">
              <i class="fas fa-clipboard-list"></i>
              <div class="meta-text">
                <span class="meta-label">Projekt</span>
                <span class="meta-value">{{ projectName || '-' }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-door-open"></i>
              <div class="meta-text">
                <span class="meta-label">Pokój</span>
                <span class="meta-value">{{ roomName || '-' }}</span>
              </div>
            </div>

            <div class="meta-item">
              <i class="fas fa-calculator"></i>
              <div class="meta-text">
                <span class="meta-label">Szacowana cena</span>
                <span class="meta-value">{{
                  formatCurrency(budget.estimatedPrice, (budget as any)?.currency || 'PLN')
                }}</span>
              </div>
            </div>
          </div>
        </section>

        <section class="content-section">
          <h3 class="section-title">
            <i class="fas fa-file-alt"></i>
            Opis
          </h3>
          <div class="description-text">
            {{ budget.description || 'Brak opisu' }}
          </div>
        </section>
      </div>

      <div class="details-content mt-4">
        <BudgetItemsList
          :items="budgetItems"
          :loading="itemsLoading"
          @add-item="goToAddItem"
          @edit-item="goToEditItem"
          @edit-shopping-list="goToEditShoppingList"
          @mark-completed="markItemCompleted"
          @delete-item="deleteItem"
          @delete-shopping-list="deleteShoppingList"
        />
      </div>
    </div>

    <div v-else-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Ładowanie budżetu...</p>
    </div>

    <div v-else class="error-state">
      <i class="fas fa-exclamation-triangle"></i>
      <h2>Nie udało się wczytać budżetu</h2>
      <p>Spróbuj ponownie później.</p>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '../../layouts/MainLayout.vue'
import BudgetItemsList from './BudgetItemsList.vue'
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { BudgetDataDTO, BudgetItemDataDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@/assets/styles/fortawesome'
import { formatDate } from '@/helpers/dateFormatter'
import { formatCurrency } from '@/helpers/currencyFormatter'

const route = useRoute()
const budgetId = computed(() => String(route.params.budgetId || ''))

const loading = ref(true)
const itemsLoading = ref(false)

const budget = ref<BudgetDataDTO | null>(null)
const budgetItems = ref<BudgetItemDataDTO[]>([])
const projectName = ref<string>('')
const roomName = ref<string>('')

const router = useRouter()

const goToEdit = () => {
  router.push({ name: 'BudgetEdit', params: { budgetId: budgetId.value } })
}

const goToAddItem = () => {
  router.push({ name: 'BudgetItemAdd', params: { budgetId: budgetId.value } })
}

const goToEditItem = (itemId: string | undefined) => {
  if (!itemId) return
  router.push({ name: 'BudgetItemEdit', params: { budgetId: budgetId.value, itemId } })
}

const goToEditShoppingList = async (item: BudgetItemDataDTO) => {
  try {
    // Extract shopping list name by removing "(Lista zakupów)" suffix
    const listName = item.name?.replace(' (Lista zakupów)', '') || ''

    // Find shopping list by name
    const allLists = await Backend.getListByUserId(budget.value?.userId || '')
    const shoppingList = allLists.find((list) => list.name === listName)

    if (shoppingList && shoppingList.id) {
      router.push({
        name: 'ShoppingListEdit',
        params: { budgetId: budgetId.value, listId: shoppingList.id },
      })
    } else {
      alert('Nie znaleziono listy zakupów')
    }
  } catch (e) {
    console.error('Error finding shopping list:', e)
    alert('Nie udało się otworzyć listy zakupów')
  }
}

const deleteShoppingList = async (item: BudgetItemDataDTO) => {
  if (!confirm('Czy na pewno chcesz usunąć tę listę zakupów? Usunie to również item z budżetu.'))
    return

  try {
    // Extract shopping list name by removing "(Lista zakupów)" suffix
    const listName = item.name?.replace(' (Lista zakupów)', '') || ''

    // Find shopping list by name
    const allLists = await Backend.getListByUserId(budget.value?.userId || '')
    const shoppingList = allLists.find((list) => list.name === listName)

    if (shoppingList && shoppingList.id) {
      // Delete the shopping list (which should also remove the budget item)
      await Backend.deleteList(shoppingList.id)
      // Also remove the budget item to be sure
      if (item.id) {
        await Backend.removeBudgetItem(budgetId.value, item.id)
      }
      await loadBudgetItems()
      await loadBudget()
    } else {
      alert('Nie znaleziono listy zakupów')
    }
  } catch (e: any) {
    console.error('Error deleting shopping list:', e)
    alert('Nie udało się usunąć listy zakupów.')
  }
}

onMounted(async () => {
  await loadBudget()
})

const loadBudget = async () => {
  try {
    loading.value = true
    budget.value = await Backend.getBudgetById(budgetId.value)

    if (budget.value.projectId) {
      try {
        const p = await Backend.getProjectById(budget.value.projectId)
        projectName.value = p.name || ''
      } catch {}
    }
    if (budget.value.roomId) {
      try {
        const r = await Backend.getRoomById(budget.value.roomId)
        roomName.value = r.name || ''
      } catch {}
    }

    await loadBudgetItems()
  } catch (e) {
    console.error('Error loading budget:', e)
  } finally {
    loading.value = false
  }
}

const loadBudgetItems = async () => {
  try {
    itemsLoading.value = true
    let items = await Backend.getBudgetItems(budgetId.value)

    // Usuń itemy powiązane z listą zakupową, jeśli lista nie istnieje
    const userId = budget.value?.userId || ''
    const allLists = await Backend.getListByUserId(userId)
    const listNames = allLists.map((list) => list.name)
    for (const item of items) {
      if (item.name?.endsWith(' (Lista zakupów)')) {
        const baseName = item.name.replace(' (Lista zakupów)', '')
        if (!listNames.includes(baseName) && item.id) {
          // Usuń item z budżetu jeśli nie ma już listy zakupowej
          await Backend.removeBudgetItem(budgetId.value, item.id)
        }
      }
    }
    // Pobierz ponownie po ewentualnych usunięciach
    budgetItems.value = await Backend.getBudgetItems(budgetId.value)
    // Aktualizuj pole "wydano" po każdej zmianie pozycji
    await updateBudgetSpent()
  } catch (e) {
    console.error('Error loading budget items:', e)
    budgetItems.value = []
  } finally {
    itemsLoading.value = false
  }
}

const markItemCompleted = async (itemId: string | undefined) => {
  if (!itemId) return

  try {
    // Mark item as completed
    await Backend.markItemCompleted(budgetId.value, itemId, true)

    // Reload items to get updated state
    await loadBudgetItems()

    // Update spent amount in budget based on completed items
    await updateBudgetSpent()
  } catch (e: any) {
    console.error('Error marking item as completed:', e)
    alert('Nie udało się oznaczyć pozycji jako zakończonej.')
  }
}

const updateBudgetSpent = async () => {
  try {
    if (!budget.value) return

    // Calculate total spent from completed items
    const totalSpent = budgetItems.value
      .filter((item) => item.isCompleted)
      .reduce((sum, item) => sum + (item.estimatedPrice || 0), 0)

    // Update budget with new spent amount
    await Backend.editBudget({
      ...budget.value,
      spent: totalSpent,
    })

    // Update local budget state immediately for UI reactivity
    budget.value.spent = totalSpent
  } catch (e) {
    console.error('Error updating budget spent:', e)
  }
}

const deleteItem = async (itemId: string | undefined) => {
  if (!itemId) return

  if (!confirm('Czy na pewno chcesz usunąć tę pozycję?')) return

  try {
    await Backend.removeBudgetItem(budgetId.value, itemId)
    await loadBudgetItems()
    await loadBudget()
  } catch (e: any) {
    console.error('Error deleting item:', e)
    alert('Nie udało się usunąć pozycji.')
  }
}
</script>
