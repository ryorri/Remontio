<template>
  <MainLayout>
    <div class="details-wrapper" v-if="!loading && budget">
      <div class="details-header">
        <div class="title-block">
          <h1 class="details-title">Edytuj budżet: {{ budget.name }}</h1>
        </div>
        <div class="header-actions">
          <button class="btn btn-primary header-btn" @click="saveChanges" :disabled="saving">
            <font-awesome-icon :icon="['fas', 'bars']" /> Zapisz zmiany
          </button>
          <button class="btn btn-danger header-btn" @click="goBack" :disabled="saving">
            Anuluj
          </button>
        </div>
      </div>

      <div class="card-section">
        <div class="mb-3">
          <label class="form-label">Nazwa budżetu</label>
          <input type="text" class="form-control" v-model="form.name" />
        </div>
        <div class="mb-3">
          <label class="form-label">Opis</label>
          <textarea class="form-control" rows="4" v-model="form.description"></textarea>
        </div>
        <div class="mb-3">
          <label class="form-label">Szacowana cena (PLN)</label>
          <input
            type="number"
            step="0.01"
            class="form-control"
            v-model.number="form.estimatedPrice"
          />
        </div>
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
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { BudgetDataDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@/assets/styles/fortawesome'

const route = useRoute()
const router = useRouter()
const budgetId = computed(() => String(route.params.budgetId || ''))

const loading = ref(true)
const saving = ref(false)
const budget = ref<BudgetDataDTO | null>(null)

const form = ref<{ name: string; description: string; estimatedPrice: number | null }>({
  name: '',
  description: '',
  estimatedPrice: null,
})

const loadBudget = async () => {
  try {
    loading.value = true
    budget.value = await Backend.getBudgetById(budgetId.value)
    form.value = {
      name: budget.value.name || '',
      description: budget.value.description || '',
      estimatedPrice: budget.value.estimatedPrice ?? null,
    }
  } catch (e) {
    console.error('Error loading budget:', e)
  } finally {
    loading.value = false
  }
}

const saveChanges = async () => {
  if (!budget.value) return
  try {
    saving.value = true
    await Backend.editBudget({
      id: budget.value.id!,
      name: form.value.name,
      description: form.value.description,
      estimatedPrice: form.value.estimatedPrice ?? undefined,
      total: budget.value.total,
      spent: budget.value.spent,
      createAt: budget.value.createAt,
      projectId: budget.value.projectId,
      roomId: budget.value.roomId,
      userId: budget.value.userId,
    })
    router.push({ name: 'BudgetDetails', params: { budgetId: budgetId.value } })
  } catch (e) {
    console.error('Error saving budget:', e)
  } finally {
    saving.value = false
  }
}

const goBack = () => {
  router.push({ name: 'BudgetList' })
}

onMounted(async () => {
  await loadBudget()
})
</script>
