<template>
  <MainLayout>
    <div class="shoppinglist-form-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="form-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-shopping-cart" aria-hidden="true"></i>
          {{ isEditMode ? 'Edytuj listę zakupową' : 'Nowa lista zakupowa' }}
        </h1>

        <form @submit.prevent="onSaveList" class="shoppinglist-form">
          <div class="mb-3">
            <label class="form-label">Nazwa listy *</label>
            <input
              v-model.trim="name"
              class="form-control"
              required
              :disabled="submitting"
              placeholder="np. Materiały do kuchni"
            />
          </div>

          <div class="mb-3">
            <label class="form-label">Opis</label>
            <textarea
              v-model.trim="description"
              class="form-control"
              rows="3"
              :disabled="submitting"
              placeholder="Krótki opis listy zakupowej..."
            ></textarea>
          </div>

          <div class="mb-4">
            <label class="form-label">Elementy listy</label>
            <div class="items-container">
              <div v-if="hasList && listItems.length > 0" class="mb-3">
                <div class="fw-semibold mb-2">Pozycje:</div>
                <div class="table-responsive">
                  <table class="table table-sm align-middle">
                    <thead>
                      <tr>
                        <th>Nazwa</th>
                        <th class="text-center" style="width: 110px">Ilość</th>
                        <th class="text-end" style="width: 140px">Cena jedn.</th>
                        <th class="text-end" style="width: 140px">Cena całk.</th>
                        <th class="text-center" style="width: 140px">Kupione</th>
                        <th class="text-end" style="width: 120px">Akcje</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="li in listItems" :key="li.id">
                        <td>{{ li.name }}</td>
                        <td class="text-center">{{ li.quantity }}</td>
                        <td class="text-end">{{ formatCurrency(li.price || 0) }}</td>
                        <td class="text-end">
                          {{ formatCurrency((li.quantity || 0) * (li.price || 0)) }}
                        </td>
                        <td class="text-center">
                          <input
                            class="form-check-input"
                            type="checkbox"
                            :checked="!!li.isBought"
                            @change="toggleBought(li)"
                            :disabled="submitting"
                            aria-label="Kupione"
                          />
                        </td>
                        <td class="text-end">
                          <button
                            class="btn btn-sm btn-danger delete-btn"
                            @click="removeExistingItem(li)"
                            :disabled="submitting"
                            title="Usuń pozycję"
                          >
                            <font-awesome-icon icon="trash-can" />
                          </button>
                        </td>
                      </tr>
                      <tr>
                        <td class="fw-semibold">Suma</td>
                        <td></td>
                        <td></td>
                        <td class="text-end fw-semibold">{{ formatCurrency(backendTotal) }}</td>
                        <td></td>
                        <td></td>
                      </tr>
                    </tbody>
                  </table>
                </div>
              </div>
              <div v-for="(item, index) in items" :key="index" class="item-row mb-2 d-flex gap-2">
                <input
                  v-model.trim="item.name"
                  class="form-control"
                  placeholder="Nazwa przedmiotu"
                  :disabled="submitting || !hasList"
                />
                <input
                  v-model.number="item.quantity"
                  type="number"
                  min="1"
                  class="form-control"
                  style="max-width: 100px"
                  placeholder="Ilość"
                  :disabled="submitting || !hasList"
                />
                <input
                  v-model.number="item.price"
                  type="number"
                  step="0.01"
                  min="0"
                  class="form-control"
                  style="max-width: 120px"
                  placeholder="Cena jedn. (PLN)"
                  :disabled="submitting || !hasList"
                />
                <div class="text-end" style="min-width: 140px; align-self: center">
                  {{ formatCurrency((item.quantity || 0) * (item.price || 0)) }}
                </div>
                <button
                  type="button"
                  class="btn btn-danger btn-sm"
                  @click="removeItem(index)"
                  :disabled="submitting || !hasList"
                  title="Usuń"
                >
                  <font-awesome-icon icon="trash-can" />
                </button>
              </div>
            </div>
            <button
              type="button"
              class="btn btn-primary mt-2"
              @click="addItem"
              :disabled="submitting || !hasList"
            >
              Dodaj element
            </button>
          </div>

          <div v-if="totalPrice > 0" class="alert alert-info">
            <strong>Łączna wartość:</strong> {{ formatCurrency(totalPrice) }}
          </div>

          <div class="d-flex flex-wrap gap-3">
            <button
              type="submit"
              class="btn btn-primary header-btn"
              :disabled="submitting || name.trim().length === 0"
            >
              <span v-if="!submitting">
                <i class="fas fa-save" aria-hidden="true"></i>
                {{ hasList ? 'Zapisz listę' : 'Utwórz listę' }}
              </span>
              <span v-else>{{ hasList ? 'Zapisywanie...' : 'Tworzenie...' }}</span>
            </button>
            <button
              type="button"
              class="btn btn-success header-btn"
              @click="onSaveItems"
              :disabled="submitting || !hasList || items.length === 0 || !hasAnyItem"
            >
              <i class="fas fa-check" aria-hidden="true"></i> Zapisz pozycje
            </button>
            <button
              type="button"
              class="btn btn-danger header-btn"
              @click="cancel"
              :disabled="submitting"
            >
              Anuluj
            </button>
          </div>

          <div v-if="error" class="alert alert-danger mt-3" role="alert">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3" role="alert">
            {{ isEditMode ? 'Lista zapisana.' : 'Lista utworzona.' }}
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, computed, onMounted } from 'vue'
import { debounce } from '@/helpers/debounce'
import { useRouter, useRoute } from 'vue-router'
import type { ListItemDataDTO } from '@/backend/BackendBase'
interface ShoppingListItem {
  name: string
  quantity: number
  price: number
}

const router = useRouter()
const route = useRoute()

const budgetId = ref<string>(route.params.budgetId as string)
const listId = ref<string | undefined>(route.params.listId as string)
const isEditMode = computed(() => !!listId.value)
const hasList = computed(() => !!listId.value)

const name = ref('')
const description = ref('')
const items = ref<ShoppingListItem[]>([{ name: '', quantity: 1, price: 0 }])

const listItems = ref<ListItemDataDTO[]>([])
const backendTotal = computed(() =>
  listItems.value.reduce((sum, it) => sum + (it.quantity || 0) * (it.price || 0), 0),
)
const existingItemCount = ref<number>(0)

const submitting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

const totalPrice = computed(() => {
  return items.value.reduce((sum, item) => {
    return sum + (item.quantity || 0) * (item.price || 0)
  }, 0)
})

const hasAnyItem = computed(() => items.value.some((item) => item.name.trim().length > 0))

const addItem = () => {
  items.value.push({ name: '', quantity: 1, price: 0 })
}

const removeItem = (index: number) => {
  if (items.value.length > 1) {
    items.value.splice(index, 1)
  }
}

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('pl-PL', {
    style: 'currency',
    currency: 'PLN',
  }).format(value)
}

// Local summary (without items) retained for listing; items persisted via backend addItem2
interface PersistedShoppingListSummary {
  id: string
  budgetId: string
  name: string
  description?: string
  createAt: string
  itemCount: number
}

function loadAllLists(): PersistedShoppingListSummary[] {
  try {
    const raw = localStorage.getItem('remontio_shopping_lists')
    if (!raw) return []
    return JSON.parse(raw) as PersistedShoppingListSummary[]
  } catch {
    return []
  }
}

function saveAllLists(lists: PersistedShoppingListSummary[]) {
  localStorage.setItem('remontio_shopping_lists', JSON.stringify(lists))
}

const existingListData = ref<any | null>(null)
const budgetProjectId = ref<string>('')
const budgetRoomId = ref<string>('')
const userId = ref<string | null>(null)

import { getCurrentUserId } from '@/helpers/userHelpers'

const loadShoppingList = async () => {
  userId.value = getCurrentUserId()
  // Load budget to obtain project/room association needed for list creation
  try {
    if (budgetId.value) {
      const budget = await Backend.getBudgetById(budgetId.value)
      budgetProjectId.value = budget.projectId || ''
      budgetRoomId.value = budget.roomId || ''
    }
  } catch (e) {
    console.warn('Nie udało się pobrać budżetu', e)
  }

  if (!isEditMode.value || !listId.value) return
  try {
    existingListData.value = await Backend.getListById(listId.value)
    if (existingListData.value) {
      name.value = existingListData.value.name || ''
      description.value = existingListData.value.description || ''
      // Items cannot be loaded (API provides only itemIds); keep blank row for adding new ones
      items.value = [{ name: '', quantity: 1, price: 0 }]
      // Show count of existing items from backend
      existingItemCount.value = (existingListData.value.itemIds?.length || 0) as number
      // Fetch backend items details (name, etc.)
      try {
        listItems.value = (await Backend.getItemListByListId(listId.value)) as any
      } catch (e) {
        console.warn('Nie udało się pobrać pozycji listy', e)
        listItems.value = []
      }
    }
  } catch (e) {
    console.warn('Nie udało się pobrać listy', e)
  }
}

const cancel = () => {
  router.push({
    name: 'ShoppingListsByBudget',
    params: { budgetId: budgetId.value },
  })
}

const findCreatedListId = async (): Promise<string | null> => {
  if (!userId.value) return null
  try {
    const allLists = await Backend.getListByUserId(userId.value)
    // Filter by name + project/room
    const candidates = allLists.filter(
      (l) =>
        l.name === name.value &&
        (l.projectId || '') === budgetProjectId.value &&
        (l.roomId || '') === budgetRoomId.value,
    )
    if (!candidates.length) return null
    // Sort by createAt descending
    candidates.sort((a, b) => new Date(b.createAt).getTime() - new Date(a.createAt).getTime())
    const first = candidates[0]
    return first && first.id ? first.id : null
  } catch (e) {
    console.warn('Nie udało się zidentyfikować nowej listy', e)
    return null
  }
}

const onSaveList = async () => {
  if (name.value.trim().length === 0) return
  submitting.value = true
  error.value = null
  success.value = false
  try {
    if (isEditMode.value && listId.value) {
      // Edit list metadata only
      if (existingListData.value) {
        await Backend.editList({
          id: listId.value,
          name: name.value,
          description: description.value || undefined,
          createAt: existingListData.value.createAt,
          roomId: existingListData.value.roomId,
          projectId: existingListData.value.projectId,
          userId: existingListData.value.userId,
        })
      }
      // Update local summary (no change in itemCount here)
      const summaries = loadAllLists()
      const idx = summaries.findIndex((s) => s.id === listId.value)
      if (idx >= 0) {
        const existing = summaries[idx]!
        summaries[idx] = {
          id: existing.id,
          budgetId: existing.budgetId,
          createAt: existing.createAt,
          name: name.value,
          description: description.value || undefined,
          itemCount: existing.itemCount || 0,
        }
        saveAllLists(summaries)
      }
    } else {
      // Create new list via backend
      if (!userId.value) throw new Error('Brak identyfikatora użytkownika')
      await Backend.createList({
        name: name.value,
        description: description.value || undefined,
        createAt: new Date(),
        roomId: budgetRoomId.value,
        projectId: budgetProjectId.value,
        userId: userId.value,
      })
      const newListId = await findCreatedListId()
      if (!newListId) throw new Error('Nie udało się pobrać ID nowej listy')
      // Attach list to budget (snapshot=false)
      try {
        await Backend.addShoppingListAsItem(budgetId.value, newListId, false)
      } catch (e) {
        console.warn('Nie udało się podpiąć listy do budżetu', e)
      }
      // Persist local summary for UI listing
      const summaries = loadAllLists()
      summaries.push({
        id: newListId,
        budgetId: budgetId.value,
        name: name.value,
        description: description.value || undefined,
        createAt: new Date().toISOString(),
        itemCount: 0,
      })
      saveAllLists(summaries)
      // Set listId so that items become active
      listId.value = newListId
      existingListData.value = await Backend.getListById(newListId)
    }

    success.value = true
    // Do not redirect automatically; allow adding items after creating list
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się zapisać listy zakupowej'
  } finally {
    submitting.value = false
  }
}

const onSaveItems = async () => {
  if (!hasList.value) return
  submitting.value = true
  error.value = null
  success.value = false
  try {
    const validItems = items.value.filter((item) => item.name.trim().length > 0)
    for (const item of validItems) {
      await Backend.addListItem(listId.value, item.name, item.quantity, item.price)
    }
    // Update local summary count
    const summaries = loadAllLists()
    const idx = summaries.findIndex((s) => s.id === listId.value)
    if (idx >= 0) {
      const existing = summaries[idx]!
      summaries[idx] = {
        id: existing.id,
        budgetId: existing.budgetId,
        createAt: existing.createAt,
        name: existing.name,
        description: existing.description,
        itemCount: (existing.itemCount || 0) + validItems.length,
      }
      saveAllLists(summaries)
    }
    existingItemCount.value += validItems.length
    // Refresh backend items list
    try {
      listItems.value = await Backend.getItemListByListId(listId.value!)
    } catch {}
    success.value = true
    // Clear inputs after save
    items.value = [{ name: '', quantity: 1, price: 0 }]
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się zapisać pozycji listy'
  } finally {
    submitting.value = false
  }
}

onMounted(() => {
  loadShoppingList()
})

const debouncedMarkBought = debounce(
  async (listIdParam: string, itemId: string, newState: boolean) => {
    try {
      await Backend.markListItemBought(listIdParam, itemId, newState)
    } catch (e) {
      console.warn('Nie udało się zmienić statusu kupione', e)
    }
  },
  300,
)

function toggleBought(li: ListItemDataDTO) {
  if (!listId.value) return
  const next = !li.isBought
  // Optimistic UI update
  li.isBought = next
  // Debounced backend update
  debouncedMarkBought(listId.value, li.id!, next)
}

async function removeExistingItem(li: ListItemDataDTO) {
  if (!listId.value) return
  if (!confirm('Usunąć pozycję z listy?')) return
  try {
    submitting.value = true
    await Backend.removeListItem(listId.value, li.id)
    listItems.value = listItems.value.filter((x) => x.id !== li.id)
    existingItemCount.value = Math.max(0, existingItemCount.value - 1)
    // Update summary count
    const summaries = loadAllLists()
    const idx = summaries.findIndex((s) => s.id === listId.value)
    if (idx >= 0) {
      const s = summaries[idx]!
      summaries[idx] = { ...s, itemCount: Math.max(0, (s.itemCount || 0) - 1) }
      saveAllLists(summaries)
    }
  } catch (e) {
    console.warn('Nie udało się usunąć pozycji', e)
  } finally {
    submitting.value = false
  }
}

// Usuwamy lokalny cache — dane pozycji pobierane z backendu
</script>

<style scoped>
.shoppinglist-form-container {
  width: 100%;
  margin: 0 auto;
  padding: 2rem;
  animation: fadeInUp 0.6s ease;
}

.form-title {
  font-size: 2.5rem;
  font-weight: 700;
  background: var(--gradient-primary);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  margin: 0;
}

.shoppinglist-form .form-label {
  font-weight: 600;
  color: var(--color-text-dark);
  margin-bottom: 0.5rem;
}

.shoppinglist-form .form-control,
.shoppinglist-form .form-select {
  border: 1px solid var(--color-bg-light-gray);
  border-radius: 8px;
  padding: 0.75rem;
  transition: all 0.2s ease;
}

.shoppinglist-form .form-control:focus,
.shoppinglist-form .form-select:focus {
  border-color: var(--color-primary-blue);
  box-shadow: 0 0 0 0.2rem rgba(59, 130, 246, 0.1);
}

.items-container {
  background: var(--color-bg-light-gray);
  padding: 1rem;
  border-radius: 8px;
}

.item-row {
  align-items: center;
}

/* Styled checkbox for bought state */
/* default checkbox styling handled by Bootstrap's form-check-input */

/* Prominent delete button */
.delete-btn {
  background: #dc3545;
  color: #fff;
  border: none;
  box-shadow: 0 2px 6px rgba(220, 53, 69, 0.3);
}
.delete-btn:hover {
  filter: brightness(0.95);
}
.delete-btn i {
  color: #fff;
}

.header-btn {
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  border-radius: 8px;
  transition: all 0.2s ease;
}

.btn-sm {
  padding: 0.5rem 0.75rem;
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
</style>
