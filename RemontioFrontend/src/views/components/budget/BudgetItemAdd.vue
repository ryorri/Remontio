<template>
  <MainLayout>
    <div class="budgets-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="budgets-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-plus-circle" aria-hidden="true"></i>
          Dodaj pozycję do budżetu
        </h1>

        <div v-if="loading" class="loading-state mb-4">
          <div class="spinner"></div>
          <p>Ładowanie budżetu...</p>
        </div>

        <div
          v-else-if="budget"
          class="budget-info mb-4 p-3"
          style="background: #f8f9fa; border-radius: 8px"
        >
          <h5 class="mb-2">{{ budget.name }}</h5>
        </div>

        <form @submit.prevent="onSubmit" class="budget-form">
          <div class="mb-3">
            <label class="form-label">Nazwa pozycji *</label>
            <input
              v-model.trim="name"
              class="form-control"
              required
              :disabled="submitting"
              placeholder="np. Farba biała"
            />
          </div>

          <div class="mb-3">
            <label class="form-label">Opis</label>
            <textarea
              v-model.trim="description"
              class="form-control"
              rows="3"
              :disabled="submitting"
              placeholder="Krótki opis pozycji..."
            ></textarea>
          </div>

          <div class="mb-3">
            <label class="form-label">Kategoria</label>
            <select v-model.number="category" class="form-select" :disabled="submitting">
              <option :value="undefined">Wybierz kategorię</option>
              <option v-for="cat in categories" :key="cat.value" :value="cat.value">
                {{ cat.label }}
              </option>
            </select>
          </div>

          <div class="mb-3">
            <label class="form-label">Szacowana cena (PLN)</label>
            <input
              v-model.number="estimatedPrice"
              type="number"
              step="0.01"
              min="0"
              class="form-control"
              :disabled="submitting"
              placeholder="0.00"
            />
          </div>

          <div class="d-flex flex-wrap gap-3">
            <button
              type="submit"
              class="btn btn-primary header-btn"
              :disabled="submitting || !canSubmit"
            >
              <i class="fas fa-save" aria-hidden="true"></i>
              {{ submitting ? 'Dodawanie...' : 'Dodaj pozycję' }}
            </button>
            <button
              type="button"
              class="btn btn-secondary header-btn"
              @click="cancel"
              :disabled="submitting"
            >
              <i class="fas fa-arrow-left" aria-hidden="true"></i> Anuluj
            </button>
          </div>

          <div v-if="error" class="alert alert-danger mt-3" role="alert">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3" role="alert">
            Pozycja dodana. Przekierowywanie...
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '../../layouts/MainLayout.vue'
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import {
  type BudgetDataDTO,
  type CreateBudgetItemDTO,
  BudgetItemCategory,
} from '@/backend/BackendBase'
import { formatCurrency } from '@/helpers/currencyFormatter'
import { getAllBudgetItemCategories } from '@/helpers/budgetItemCategoryEnumFormatter'

const route = useRoute()
const router = useRouter()

const budgetId = computed(() => String(route.params.budgetId || ''))

const loading = ref(true)
const submitting = ref(false)
const error = ref('')
const success = ref(false)

const budget = ref<BudgetDataDTO | null>(null)
const name = ref('')
const description = ref('')
const category = ref<BudgetItemCategory | undefined>(undefined)
const estimatedPrice = ref<number>(0)

const categories = getAllBudgetItemCategories()

const canSubmit = computed(() => {
  return name.value.trim() !== ''
})

onMounted(async () => {
  await loadBudget()
})

const loadBudget = async () => {
  try {
    loading.value = true
    budget.value = await Backend.getBudgetById(budgetId.value)
  } catch (e) {
    console.error('Error loading budget:', e)
    error.value = 'Nie udało się wczytać budżetu'
  } finally {
    loading.value = false
  }
}

const onSubmit = async () => {
  if (!canSubmit.value) return

  submitting.value = true
  error.value = ''
  success.value = false

  try {
    const itemDto: CreateBudgetItemDTO = {
      name: name.value,
      description: description.value || undefined,
      category: category.value,
      estimatedPrice: estimatedPrice.value || 0,
      isCompleted: false,
    }

    console.log('Creating budget item with estimatedPrice:', itemDto.estimatedPrice)

    const result = await Backend.addBudgetItem(budgetId.value, itemDto)

    if (result) {
      success.value = true
      setTimeout(() => {
        router.push({ name: 'BudgetDetails', params: { budgetId: budgetId.value } })
      }, 1500)
    } else {
      error.value = 'Nie udało się dodać pozycji. Spróbuj ponownie.'
    }
  } catch (e: any) {
    console.error('Error adding budget item:', e)
    error.value = e?.message || 'Wystąpił błąd podczas dodawania pozycji.'
  } finally {
    submitting.value = false
  }
}

const cancel = () => {
  router.push({ name: 'BudgetDetails', params: { budgetId: budgetId.value } })
}
</script>

<style scoped>
.budgets-container {
  min-height: 100vh;
  padding: 2rem 1rem;
}

.details-wrapper {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.budgets-title {
  font-size: 1.75rem;
  font-weight: 600;
  color: #2c3e50;
}

.budget-info {
  border: 1px solid #e9ecef;
}

.budget-info h5 {
  color: #2c3e50;
  font-weight: 600;
  margin-bottom: 0.5rem;
}

.form-label {
  font-weight: 500;
  color: #495057;
  margin-bottom: 0.5rem;
}

.form-control,
.form-select {
  border: 1px solid #ced4da;
  border-radius: 6px;
  padding: 0.625rem 0.875rem;
  font-size: 0.9375rem;
  transition:
    border-color 0.15s ease-in-out,
    box-shadow 0.15s ease-in-out;
}

.form-control:focus,
.form-select:focus {
  border-color: #80bdff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
  outline: 0;
}

.btn {
  padding: 0.625rem 1.25rem;
  font-size: 0.9375rem;
  border-radius: 6px;
  font-weight: 500;
  transition: all 0.2s ease;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn-primary:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.btn-secondary {
  background: #6c757d;
  border: none;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #5a6268;
  transform: translateY(-1px);
}

.header-btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.loading-state {
  text-align: center;
  padding: 2rem;
  color: #6c757d;
}

.spinner {
  border: 3px solid #f3f3f3;
  border-top: 3px solid #667eea;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

.alert {
  border-radius: 6px;
  padding: 0.875rem 1rem;
  margin-top: 1rem;
}

.alert-danger {
  background-color: #f8d7da;
  border: 1px solid #f5c6cb;
  color: #721c24;
}

.alert-success {
  background-color: #d4edda;
  border: 1px solid #c3e6cb;
  color: #155724;
}

.alert-info {
  background-color: #d1ecf1;
  border: 1px solid #bee5eb;
  color: #0c5460;
}

@media (max-width: 768px) {
  .budgets-container {
    padding: 1rem 0.5rem;
  }

  .budgets-title {
    font-size: 1.5rem;
  }
}
</style>
