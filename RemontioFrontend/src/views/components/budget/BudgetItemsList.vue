<template>
  <div class="budget-items-section">
    <div class="section-header">
      <h3 class="section-title">
        <i class="fas fa-list"></i>
        Pozycje budżetu
      </h3>
      <button class="btn btn-sm btn-success" @click="$emit('add-item')">
        <i class="fas fa-plus"></i> Dodaj pozycję
      </button>
    </div>

    <div v-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Ładowanie pozycji...</p>
    </div>

    <div v-else-if="items.length === 0" class="empty-state">
      <i class="fas fa-inbox"></i>
      <h4>Brak pozycji w budżecie</h4>
      <p>Dodaj pierwszą pozycję, aby śledzić wydatki</p>
      <button class="btn btn-primary" @click="$emit('add-item')">
        <i class="fas fa-plus"></i> Dodaj pierwszą pozycję
      </button>
    </div>

    <div v-else class="items-list">
      <div class="items-table-container">
        <table class="items-table">
          <thead>
            <tr>
              <th>Nazwa</th>
              <th>Kategoria</th>
              <th>Opis</th>
              <th>Szacowana cena</th>
              <th>Status</th>
              <th class="actions-col">Akcje</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in items" :key="item.id" :class="{ completed: item.isCompleted }">
              <td class="item-name">
                <i class="fas fa-tag item-icon"></i>
                {{ item.name }}
              </td>
              <td class="item-category">
                <span v-if="item.category !== undefined" class="category-badge">
                  {{ formatCategory(item.category) }}
                </span>
                <span v-else class="text-muted">-</span>
              </td>
              <td class="item-description">
                {{ item.description || '-' }}
              </td>
              <td class="item-price">
                {{ formatCurrency(item.estimatedPrice || 0, 'PLN') }}
              </td>
              <td class="item-status">
                <span
                  class="status-badge"
                  :class="item.isCompleted ? 'status-completed' : 'status-pending'"
                >
                  <i :class="item.isCompleted ? 'fas fa-check-circle' : 'fas fa-clock'"></i>
                  {{ item.isCompleted ? 'Zakończone' : 'W trakcie' }}
                </span>
              </td>
              <td class="actions-col">
                <div class="action-buttons">
                  <button
                    class="btn-icon btn-edit"
                    @click="handleEditClick(item)"
                    :title="isShoppingList(item) ? 'Edytuj listę zakupów' : 'Edytuj pozycję'"
                  >
                    <font-awesome-icon :icon="['fas', 'edit']" />
                  </button>
                  <button
                    v-if="!item.isCompleted"
                    class="btn-icon btn-complete"
                    @click="$emit('mark-completed', item.id)"
                    title="Oznacz jako zakończone"
                  >
                    <font-awesome-icon :icon="['fas', 'check-circle']" />
                  </button>
                  <button
                    class="btn-icon btn-delete"
                    @click="handleDeleteClick(item)"
                    :title="isShoppingList(item) ? 'Usuń listę zakupów' : 'Usuń pozycję'"
                  >
                    <font-awesome-icon :icon="['fas', 'trash-can']" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="items-summary">
        <div class="summary-item">
          <span class="summary-label">Łączna szacowana cena:</span>
          <span class="summary-value">{{ formatCurrency(totalEstimated, 'PLN') }}</span>
        </div>
        <div class="summary-item">
          <span class="summary-label">Liczba pozycji:</span>
          <span class="summary-value">{{ items.length }}</span>
        </div>
        <div class="summary-item">
          <span class="summary-label">Zakończone:</span>
          <span class="summary-value">{{ completedCount }} / {{ items.length }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed } from 'vue'
import type { BudgetItemDataDTO } from '@/backend/BackendBase'
import { formatCurrency } from '@/helpers/currencyFormatter'
import { formatBudgetItemCategory } from '@/helpers/budgetItemCategoryEnumFormatter'
import { FontAwesomeIcon } from '@/assets/styles/fortawesome'

interface Props {
  items: BudgetItemDataDTO[]
  loading?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
})

const emit = defineEmits<{
  'add-item': []
  'edit-item': [itemId: string | undefined]
  'edit-shopping-list': [item: BudgetItemDataDTO]
  'mark-completed': [itemId: string | undefined]
  'delete-item': [itemId: string | undefined]
  'delete-shopping-list': [item: BudgetItemDataDTO]
}>()

const isShoppingList = (item: BudgetItemDataDTO): boolean => {
  return item.name?.endsWith('(Lista zakupów)') ?? false
}

const handleEditClick = (item: BudgetItemDataDTO) => {
  if (isShoppingList(item)) {
    emit('edit-shopping-list', item)
  } else {
    emit('edit-item', item.id)
  }
}

const handleDeleteClick = (item: BudgetItemDataDTO) => {
  if (isShoppingList(item)) {
    emit('delete-shopping-list', item)
  } else {
    emit('delete-item', item.id)
  }
}

const totalEstimated = computed(() => {
  return props.items.reduce((sum, item) => sum + (item.estimatedPrice || 0), 0)
})

const completedCount = computed(() => {
  return props.items.filter((item) => item.isCompleted).length
})

const formatCategory = (category: number) => {
  return formatBudgetItemCategory(category)
}
</script>

<style scoped>
.budget-items-section {
  background: white;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
}

.section-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  margin: 0;
}

.section-title i {
  color: #667eea;
}

.loading-state {
  text-align: center;
  padding: 3rem 1rem;
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

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #6c757d;
}

.empty-state i {
  font-size: 3rem;
  color: #dee2e6;
  margin-bottom: 1rem;
}

.empty-state h4 {
  font-size: 1.25rem;
  color: #495057;
  margin-bottom: 0.5rem;
}

.empty-state p {
  margin-bottom: 1.5rem;
}

.items-table-container {
  overflow-x: auto;
  margin-bottom: 1.5rem;
}

.items-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.9rem;
}

.items-table thead {
  background: #f8f9fa;
  border-bottom: 2px solid #dee2e6;
}

.items-table th {
  padding: 0.75rem 1rem;
  text-align: left;
  font-weight: 600;
  color: #495057;
  font-size: 0.875rem;
  text-transform: uppercase;
  letter-spacing: 0.025em;
}

.items-table tbody tr {
  border-bottom: 1px solid #e9ecef;
  transition: background-color 0.2s ease;
}

.items-table tbody tr:hover {
  background-color: #f8f9fa;
}

.items-table tbody tr.completed {
  opacity: 0.7;
}

.items-table td {
  padding: 1rem;
}

.item-name {
  font-weight: 500;
  color: #2c3e50;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.item-icon {
  color: #667eea;
  font-size: 0.875rem;
}

.item-category {
  white-space: nowrap;
}

.category-badge {
  display: inline-block;
  padding: 0.25rem 0.75rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: 500;
}

.item-description {
  color: #6c757d;
  max-width: 300px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.item-price {
  font-weight: 600;
  color: #2c3e50;
  white-space: nowrap;
}

.item-status {
  white-space: nowrap;
}

.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 0.375rem;
  padding: 0.375rem 0.75rem;
  border-radius: 12px;
  font-size: 0.8125rem;
  font-weight: 500;
}

.status-completed {
  background-color: #d4edda;
  color: #155724;
}

.status-pending {
  background-color: #fff3cd;
  color: #856404;
}

.actions-col {
  width: 150px;
  text-align: center;
}

.action-buttons {
  display: flex;
  gap: 0.375rem;
  justify-content: center;
  align-items: center;
}

.btn-icon {
  border: 1px solid;
  cursor: pointer;
  padding: 0.5rem 0.625rem;
  border-radius: 6px;
  transition: all 0.2s ease;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 0.875rem;
  min-width: 36px;
  min-height: 36px;
}

.btn-icon:hover {
  transform: translateY(-2px);
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.btn-edit {
  color: #667eea;
  background-color: #f0f1ff;
  border-color: #667eea;
}

.btn-edit:hover {
  background-color: #667eea;
  color: white;
}

.btn-complete {
  color: #28a745;
  background-color: #e8f5e9;
  border-color: #28a745;
}

.btn-complete:hover {
  background-color: #28a745;
  color: white;
}

.btn-delete {
  color: #dc3545;
  background-color: #ffebee;
  border-color: #dc3545;
}

.btn-delete:hover {
  background-color: #dc3545;
  color: white;
}

.items-summary {
  display: flex;
  gap: 2rem;
  padding: 1rem;
  background: #f8f9fa;
  border-radius: 8px;
  flex-wrap: wrap;
}

.summary-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.summary-label {
  font-size: 0.875rem;
  color: #6c757d;
  font-weight: 500;
}

.summary-value {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2c3e50;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
}

.btn-sm {
  padding: 0.375rem 0.75rem;
  font-size: 0.875rem;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover {
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn-success {
  background: #28a745;
  color: white;
}

.btn-success:hover {
  background: #218838;
  transform: translateY(-1px);
}

@media (max-width: 768px) {
  .section-header {
    flex-direction: column;
    gap: 1rem;
    align-items: stretch;
  }

  .items-table {
    font-size: 0.8125rem;
  }

  .items-table th,
  .items-table td {
    padding: 0.5rem;
  }

  .item-description {
    max-width: 150px;
  }

  .items-summary {
    flex-direction: column;
    gap: 1rem;
  }
}
</style>
