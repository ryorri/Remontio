<template>
  <MainLayout>
    <div class="details-wrapper" v-if="!loading && contact">
      <div class="details-header">
        <div class="title-block">
          <h1 class="details-title">{{ contact.name }}</h1>
          <span class="badge">Utworzono: {{ formatDate(contact.createdDate) }}</span>
        </div>
        <div class="header-actions">
          <button class="btn btn-primary header-btn" @click="goToEdit">
            <font-awesome-icon icon="edit" /> Edytuj
          </button>
          <button class="btn btn-secondary header-btn" @click="goBack">
            <i class="fas fa-arrow-left"></i> Powrót
          </button>
        </div>
      </div>

      <div class="details-content">
        <section class="content-section">
          <h3 class="section-title">
            <i class="fas fa-info-circle"></i>
            Informacje podstawowe
          </h3>
          <div class="meta-grid">
            <div class="meta-item">
              <i class="fas fa-user"></i>
              <div class="meta-text">
                <span class="meta-label">Nazwa</span>
                <span class="meta-value">{{ contact.name }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i class="fas fa-briefcase"></i>
              <div class="meta-text">
                <span class="meta-label">Specjalizacja</span>
                <span class="meta-value">{{ getSpecLabel(contact.spec) }}</span>
              </div>
            </div>
            <div class="meta-item">
              <i :class="contact.isPrivate ? 'fas fa-lock' : 'fas fa-globe'"></i>
              <div class="meta-text">
                <span class="meta-label">Prywatność</span>
                <span class="meta-value">{{ contact.isPrivate ? 'Prywatny' : 'Publiczny' }}</span>
              </div>
            </div>
          </div>
        </section>

        <section class="content-section" v-if="contact.description">
          <h3 class="section-title">
            <i class="fas fa-file-alt"></i>
            Opis
          </h3>
          <div class="description-text">
            {{ contact.description }}
          </div>
        </section>

        <section class="content-section">
          <h3 class="section-title">
            <i class="fas fa-address-card"></i>
            Dane kontaktowe
          </h3>
          <div class="description-text" style="white-space: pre-wrap">
            {{ contact.contactDetails }}
          </div>
        </section>
      </div>
    </div>

    <div v-else-if="loading" class="loading-state">
      <div class="spinner"></div>
      <p>Ładowanie kontaktu...</p>
    </div>

    <div v-else class="error-state">
      <i class="fas fa-exclamation-triangle"></i>
      <h2>Nie udało się wczytać kontaktu</h2>
      <p>Spróbuj ponownie później.</p>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '@/views/layouts/MainLayout.vue'
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { ContactDataDTO } from '@/backend/BackendBase'
import { formatDate } from '@/helpers/dateFormatter'
import { getSpecLabel } from '@/helpers/specEnumFormatter'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const route = useRoute()
const router = useRouter()
const contactId = computed(() => String(route.params.contactId || ''))

const loading = ref(true)
const contact = ref<ContactDataDTO | null>(null)

const goToEdit = () => {
  router.push({ name: 'ContactEdit', params: { contactId: contactId.value } })
}

const goBack = () => {
  router.push({ name: 'ContactList' })
}

const loadContact = async () => {
  try {
    loading.value = true
    contact.value = await Backend.getContactById(contactId.value)
  } catch (e) {
    console.error('Error loading contact:', e)
  } finally {
    loading.value = false
  }
}

onMounted(async () => {
  await loadContact()
})
</script>

<style scoped>
/* Używamy globalnych styli z main.css */
</style>
