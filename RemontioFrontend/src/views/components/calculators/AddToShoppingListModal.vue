<template>
  <div
    v-if="isOpen"
    class="modal-overlay"
    @click.self="closeModal"
    role="dialog"
    aria-modal="true"
    aria-labelledby="modal-title"
  >
    <div class="modal-dialog">
      <div class="modal-content">
        <div class="modal-header">
          <h3 id="modal-title" class="modal-title">
            <font-awesome-icon icon="shopping-cart" />
            Dodaj do listy zakupowej
          </h3>
          <button
            type="button"
            class="btn-close"
            @click="closeModal"
            aria-label="Zamknij"
            :disabled="submitting"
          ></button>
        </div>

        <div class="modal-body">
          <div v-if="loadingLists" class="loading-state">
            <div class="spinner"></div>
            <p>Ładowanie list zakupowych...</p>
          </div>

          <form v-else @submit.prevent="handleAddToList">
            <div class="mb-3">
              <label class="form-label">Lista zakupowa *</label>
              <select v-model="selectedListId" class="form-select" required :disabled="submitting">
                <option value="">Wybierz listę</option>
                <option v-for="list in shoppingLists" :key="list.id" :value="list.id">
                  {{ list.name }}
                </option>
              </select>
              <small class="text-muted">
                Nie widzisz swojej listy?
                <a href="#" @click.prevent="showCreateNewList" class="text-primary">
                  Utwórz nową
                </a>
              </small>
            </div>

            <div class="mb-3">
              <label class="form-label">Nazwa pozycji *</label>
              <input
                v-model.trim="itemName"
                type="text"
                class="form-control"
                required
                minlength="3"
                maxlength="100"
                :disabled="submitting"
                placeholder="np. Farba biała"
              />
              <small class="text-muted">Minimum 3 znaki</small>
            </div>

            <div class="row">
              <div class="col-md-6 mb-3">
                <label class="form-label">Ilość *</label>
                <input
                  v-model.number="quantity"
                  type="number"
                  step="1"
                  min="1"
                  class="form-control"
                  required
                  :disabled="submitting"
                  placeholder="1"
                />
              </div>

              <div class="col-md-6 mb-3">
                <label class="form-label">Cena jednostkowa (PLN)</label>
                <input
                  v-model.number="price"
                  type="number"
                  step="0.01"
                  min="0"
                  class="form-control"
                  :disabled="submitting"
                  placeholder="0.00"
                />
              </div>
            </div>

            <div v-if="calculationData" class="calculation-info mb-3">
              <div class="info-box">
                <h6 class="mb-2">Dane z kalkulacji:</h6>
                <div class="info-item">
                  <span class="info-label">Nazwa kalkulacji:</span>
                  <span class="info-value">{{ calculationData.name }}</span>
                </div>
                <div v-if="calculationData.totalEfficiency" class="info-item">
                  <span class="info-label">Łączna ilość:</span>
                  <span class="info-value">{{ calculationData.totalEfficiency.toFixed(2) }}</span>
                </div>
              </div>
            </div>

            <div v-if="error" class="alert alert-danger" role="alert">{{ error }}</div>
            <div v-if="success" class="alert alert-success" role="alert">
              Pozycja dodana do listy zakupowej!
            </div>

            <div class="modal-footer">
              <button
                type="button"
                class="btn btn-secondary modal-btn"
                @click="closeModal"
                :disabled="submitting"
              >
                <font-awesome-icon icon="times" />
                Anuluj
              </button>
              <button
                type="submit"
                class="btn btn-primary modal-btn"
                :disabled="submitting || !selectedListId"
              >
                <font-awesome-icon icon="plus" />
                {{ submitting ? 'Dodawanie...' : 'Dodaj do listy' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import { ref, watch } from 'vue'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

interface ShoppingListSummary {
  id: string
  name: string
  description?: string
}

interface CalculationData {
  name: string
  totalEfficiency?: number
  value?: number
}

const props = defineProps<{
  isOpen: boolean
  calculationData?: CalculationData
  roomId: string
}>()

const emit = defineEmits<{
  close: []
}>()

const shoppingLists = ref<ShoppingListSummary[]>([])
const selectedListId = ref('')
const itemName = ref('')
const quantity = ref<number>(1)
const price = ref<number>(0)

const loadingLists = ref(false)
const submitting = ref(false)
const error = ref('')
const success = ref(false)

watch(
  () => props.isOpen,
  async (isOpen) => {
    if (isOpen) {
      await loadShoppingLists()
      resetForm()

      // Pre-fill item name from calculation data
      if (props.calculationData?.name) {
        itemName.value = props.calculationData.name
      }

      // Pre-fill quantity from calculation (rounded to whole number)
      if (props.calculationData?.totalEfficiency) {
        quantity.value = Math.round(props.calculationData.totalEfficiency)
      } else if (props.calculationData?.value) {
        quantity.value = Math.round(props.calculationData.value)
      }
    }
  },
)

const loadShoppingLists = async () => {
  try {
    loadingLists.value = true
    error.value = ''

    const userId = getCurrentUserId()
    if (!userId) {
      error.value = 'Nie znaleziono użytkownika'
      return
    }

    // Get all lists for user
    const allLists = await Backend.getListByUserId(userId)

    // Filter lists - could filter by room/project if needed
    shoppingLists.value = allLists
      .filter((list) => list.id && list.name)
      .map((list) => ({
        id: list.id!,
        name: list.name!,
        description: list.description,
      }))
  } catch (e) {
    console.error('Error fetching shopping lists:', e)
    error.value = 'Nie udało się załadować list zakupowych'
  } finally {
    loadingLists.value = false
  }
}

const handleAddToList = async () => {
  if (!selectedListId.value) return

  // Validate name length
  if (itemName.value.trim().length < 3) {
    error.value = 'Nazwa pozycji musi mieć co najmniej 3 znaki'
    return
  }

  try {
    submitting.value = true
    error.value = ''
    success.value = false

    // Round quantity to whole number, price to 2 decimal places
    const roundedQuantity = Math.round(quantity.value)
    const roundedPrice = Math.round(price.value * 100) / 100

    await Backend.addListItem(selectedListId.value, itemName.value, roundedQuantity, roundedPrice)

    success.value = true

    setTimeout(() => {
      closeModal()
    }, 1500)
  } catch (e: any) {
    console.error('Error adding to shopping list:', e)
    error.value = e?.message || 'Nie udało się dodać pozycji do listy'
  } finally {
    submitting.value = false
  }
}

const resetForm = () => {
  selectedListId.value = ''
  itemName.value = ''
  quantity.value = 1
  price.value = 0
  error.value = ''
  success.value = false
}

const closeModal = () => {
  if (!submitting.value) {
    emit('close')
  }
}

const showCreateNewList = () => {
  // TODO: Could open a nested modal or redirect to create list page
  alert('Funkcja tworzenia nowej listy będzie dostępna wkrótce')
}
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 9999;
  animation: fadeIn 0.2s ease;
}

.modal-dialog {
  max-width: 600px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  animation: slideUp 0.3s ease;
}

.modal-content {
  background: white;
  border-radius: 12px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e0e0e0;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.modal-title {
  margin: 0;
  font-size: 1.5rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  color: var(--color-text-dark);
}

.modal-title svg {
  color: var(--color-primary-purple);
}

.btn-close {
  background: transparent;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  padding: 0.5rem;
  line-height: 1;
  color: #666;
  transition: color 0.2s ease;
}

.btn-close:hover:not(:disabled) {
  color: #333;
}

.btn-close::before {
  content: '×';
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  display: flex;
  gap: 1rem;
  justify-content: flex-end;
  padding-top: 1rem;
  margin-top: 1rem;
  border-top: 1px solid #e0e0e0;
}

.calculation-info {
  padding: 1rem;
  background: var(--color-bg-light-gray);
  border-radius: 8px;
}

.info-box h6 {
  font-weight: 600;
  color: var(--color-text-dark);
  margin-bottom: 0.75rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  padding: 0.5rem 0;
  border-bottom: 1px solid #e0e0e0;
}

.info-item:last-child {
  border-bottom: none;
}

.info-label {
  color: #666;
  font-weight: 500;
}

.info-value {
  color: var(--color-text-dark);
  font-weight: 600;
}

.modal-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  font-weight: 600;
  border-radius: 8px;
  transition: all 0.2s ease;
  border: none;
}

.btn-secondary.modal-btn {
  background: #6c757d;
  color: white;
}

.btn-secondary.modal-btn:hover:not(:disabled) {
  background: #5a6268;
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.btn-primary.modal-btn {
  background: var(--color-primary-purple);
  color: white;
}

.btn-primary.modal-btn:hover:not(:disabled) {
  background: var(--color-primary-pink);
  transform: translateY(-1px);
  box-shadow: 0 2px 8px rgba(143, 159, 230, 0.3);
}

.modal-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slideUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}
</style>
