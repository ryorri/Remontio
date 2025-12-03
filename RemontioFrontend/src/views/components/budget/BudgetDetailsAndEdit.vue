<template>
  <MainLayout>
    <div class="details-wrapper" v-if="!loading && budget">
      <div class="details-header">
        <div class="title-block">
          <h1 class="details-title">Budżet: {{ budget?.name || '' }}</h1>
          <span class="badge">Utworzono: {{ formatDate(budget?.createAt) }}</span>
        </div>
        <div class="header-actions">
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
              <i class="fas fa-coins"></i>
              <div class="meta-text">
                <span class="meta-label">Suma</span>
                <span class="meta-value">{{ formatCurrency(budget.total) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-money-bill-wave"></i>
              <div class="meta-text">
                <span class="meta-label">Wydano</span>
                <span class="meta-value">{{ formatCurrency(budget.spent) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-calculator"></i>
              <div class="meta-text">
                <span class="meta-label">Szacowana cena</span>
                <span class="meta-value">{{ formatCurrency(budget.estimatedPrice) }}</span>
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
import { formatDate } from '@/helpers/dateFormatter'

const route = useRoute()
const budgetId = computed(() => String(route.params.budgetId || ''))

const loading = ref(true)

const budget = ref<BudgetDataDTO | null>(null)
const projectName = ref<string>('')
const roomName = ref<string>('')

const router = useRouter()

const formatCurrency = (value?: number | null) => {
  if (value === undefined || value === null) return '-'
  return new Intl.NumberFormat('pl-PL', { style: 'currency', currency: 'PLN' }).format(value)
}

const goToEdit = () => {
  router.push({ name: 'BudgetEdit', params: { budgetId: budgetId.value } })
}

const loadBudget = async () => {
  try {
    loading.value = true
    const b = await Backend.getBudgetById(budgetId.value)
    budget.value = b
    // Fetch related project/room names
    if (b?.projectId) {
      try {
        const p = await Backend.getProjectById(b.projectId)
        projectName.value = p?.name || ''
      } catch {}
    } else {
      projectName.value = ''
    }
    if (b?.roomId) {
      try {
        const r = await Backend.getRoomById(b.roomId)
        roomName.value = r?.name || ''
      } catch {}
    } else {
      roomName.value = ''
    }
  } catch (e) {
    console.error('Error loading budget:', e)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await loadBudget()
})
</script>

<style scoped>
.mb-3 {
  margin-bottom: 1rem;
}
</style>
