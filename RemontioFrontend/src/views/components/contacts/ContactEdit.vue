<template>
  <MainLayout>
    <div class="contacts-container">
      <div class="details-wrapper p-4" style="max-width: 980px; margin: 0 auto">
        <h1 class="contacts-title mb-4 d-flex align-items-center gap-2">
          <i class="fas fa-edit"></i>
          Edytuj kontakt
        </h1>

        <div v-if="loading" class="loading-state">
          <div class="spinner"></div>
          <p>Ładowanie kontaktu...</p>
        </div>

        <form v-else @submit.prevent="onSubmit">
          <div class="mb-3">
            <label class="form-label">Nazwa kontaktu *</label>
            <input
              v-model.trim="name"
              class="form-control"
              required
              :disabled="submitting"
              placeholder="np. Jan Kowalski"
            />
          </div>

          <div class="mb-3">
            <label class="form-label">Specjalizacja *</label>
            <select v-model="spec" class="form-select" required :disabled="submitting">
              <option value="">Wybierz specjalizację</option>
              <option v-for="s in specializations" :key="s.value" :value="s.value">
                {{ s.label }}
              </option>
            </select>
          </div>

          <div class="mb-3">
            <label class="form-label">Opis</label>
            <textarea
              v-model.trim="description"
              class="form-control"
              rows="3"
              :disabled="submitting"
              placeholder="Krótki opis kontaktu..."
            ></textarea>
          </div>

          <div class="mb-3">
            <label class="form-label">Dane kontaktowe *</label>
            <textarea
              v-model.trim="contactDetails"
              class="form-control"
              rows="4"
              required
              :disabled="submitting"
              placeholder="Telefon, email, adres itp."
            ></textarea>
          </div>

          <div class="mb-4">
            <div class="form-check">
              <input
                v-model="isPrivate"
                type="checkbox"
                class="form-check-input"
                id="isPrivateCheck"
                :disabled="submitting"
              />
              <label class="form-check-label" for="isPrivateCheck">Kontakt prywatny</label>
            </div>
          </div>

          <div class="d-flex flex-wrap gap-3">
            <button
              type="submit"
              class="btn btn-primary header-btn"
              :disabled="submitting || !canSubmit"
            >
              <i class="fas fa-save"></i>
              {{ submitting ? 'Zapisywanie...' : 'Zapisz zmiany' }}
            </button>
            <button
              type="button"
              class="btn btn-secondary header-btn"
              @click="cancel"
              :disabled="submitting"
            >
              <i class="fas fa-arrow-left"></i> Anuluj
            </button>
          </div>

          <div v-if="error" class="alert alert-danger mt-3">{{ error }}</div>
          <div v-if="success" class="alert alert-success mt-3">
            Kontakt zaktualizowany. Przekierowywanie...
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import MainLayout from '@/views/layouts/MainLayout.vue'
import { Backend } from '@/main'
import { ref, computed, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { SpecEnum } from '@/backend/BackendBase'
import { getAllSpecOptions } from '@/helpers/specEnumFormatter'

const router = useRouter()
const route = useRoute()
const contactId = computed(() => String(route.params.contactId || ''))
const specializations = getAllSpecOptions()

const name = ref('')
const description = ref('')
const contactDetails = ref('')
const spec = ref<SpecEnum | ''>('')
const isPrivate = ref(false)
const userId = ref<string>('')
const createdDate = ref<Date>(new Date())
const loading = ref(true)
const submitting = ref(false)
const error = ref<string | null>(null)
const success = ref(false)

const canSubmit = computed(
  () => name.value.trim().length > 0 && contactDetails.value.trim().length > 0 && spec.value !== '',
)

async function loadContact() {
  try {
    loading.value = true
    const contact = await Backend.getContactById(contactId.value)
    name.value = contact.name || ''
    description.value = contact.description || ''
    contactDetails.value = contact.contactDetails || ''
    spec.value = contact.spec || ''
    isPrivate.value = contact.isPrivate || false
    userId.value = contact.userId || ''
    createdDate.value = contact.createdDate || new Date()
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się załadować kontaktu'
  } finally {
    loading.value = false
  }
}

const cancel = () => router.push({ name: 'ContactDetails', params: { contactId: contactId.value } })

async function onSubmit() {
  if (!canSubmit.value) return
  submitting.value = true
  error.value = null
  success.value = false

  try {
    await Backend.editContact({
      id: contactId.value,
      name: name.value,
      description: description.value || undefined,
      contactDetails: contactDetails.value,
      spec: spec.value as SpecEnum,
      createdDate: createdDate.value,
      isPrivate: isPrivate.value,
      userId: userId.value,
    })

    success.value = true
    setTimeout(
      () => router.push({ name: 'ContactDetails', params: { contactId: contactId.value } }),
      1000,
    )
  } catch (e: any) {
    error.value = e?.message || 'Nie udało się zaktualizować kontaktu.'
  } finally {
    submitting.value = false
  }
}

onMounted(() => loadContact())
</script>
