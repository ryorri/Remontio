<template>
  <MainLayout>
    <div class="details-wrapper" v-if="!loading && budget">
      <div class="details-header">
        <div class="title-block">
          <h1 class="details-title">Budżet: {{ budget.name }}</h1>
          <span class="badge">Utworzono: {{ formatDate(budget.createAt) }}</span>
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
                <span class="meta-value">{{
                  formatCurrency(budget.total, (budget as any)?.currency || 'PLN')
                }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-money-bill-wave"></i>
              <div class="meta-text">
                <span class="meta-label">Wydano</span>
                <span class="meta-value">{{
                  formatCurrency(budget.spent, (budget as any)?.currency || 'PLN')
                }}</span>
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
import { formatCurrency } from '@/helpers/currencyFormatter'

const route = useRoute()
const budgetId = computed(() => String(route.params.budgetId || ''))

const loading = ref(true)

const budget = ref<BudgetDataDTO | null>(null)
const projectName = ref<string>('')
const roomName = ref<string>('')

const router = useRouter()

const goToEdit = () => {
  router.push({ name: 'BudgetEdit', params: { budgetId: budgetId.value } })
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
  } catch (e) {
    console.error('Error loading budget:', e)
  } finally {
    loading.value = false
  }
}
</script>
