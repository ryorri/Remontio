<template>
  <MainLayout>
    <div class="contacts-container">
      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie danych użytkownika...</p>
      </div>

      <div v-else-if="error" class="error-state">
        <font-awesome-icon icon="exclamation-triangle" />
        <h2>Wystąpił błąd</h2>
        <p>{{ error }}</p>
        <button @click="loadUserData" class="btn btn-primary">
          <font-awesome-icon icon="undo" />
          Spróbuj ponownie
        </button>
      </div>

      <div v-else class="details-wrapper">
        <div class="details-header">
          <div class="title-block">
            <h1 class="details-title">
              <font-awesome-icon icon="user" />
              Ustawienia konta
            </h1>
          </div>
        </div>

        <div class="details-content">
          <div v-if="successMessage" class="alert alert-success">
            <font-awesome-icon icon="check-circle" />
            {{ successMessage }}
          </div>

          <div v-if="errorMessage" class="alert alert-danger">
            <font-awesome-icon icon="times-circle" />
            {{ errorMessage }}
          </div>

          <div class="card-section">
            <div class="section-header">
              <font-awesome-icon icon="user" />
              <h3>Informacje o koncie</h3>
            </div>

            <form @submit.prevent="handleSubmit">
              <div class="form-group mb-3">
                <label for="name" class="form-label">Imię</label>
                <input
                  id="name"
                  v-model="formData.name"
                  type="text"
                  class="form-control"
                  required
                  :disabled="saving"
                />
              </div>

              <div class="form-group mb-3">
                <label for="surname" class="form-label">Nazwisko</label>
                <input
                  id="surname"
                  v-model="formData.surname"
                  type="text"
                  class="form-control"
                  required
                  :disabled="saving"
                />
              </div>

              <div class="form-group mb-3">
                <label for="email" class="form-label">Email</label>
                <input
                  id="email"
                  v-model="formData.email"
                  type="email"
                  class="form-control"
                  required
                  :disabled="saving"
                />
              </div>

              <div class="form-group mb-4">
                <label class="form-label">Data utworzenia konta</label>
                <input
                  type="text"
                  :value="formatDateTime(userData?.createdAt)"
                  disabled
                  class="form-control"
                  style="background-color: #f5f5f5; cursor: not-allowed"
                />
              </div>

              <div class="d-flex gap-2 flex-wrap">
                <button type="submit" class="btn btn-primary" :disabled="saving || !isFormDirty">
                  <font-awesome-icon v-if="saving" icon="spinner" spin />
                  <font-awesome-icon v-else icon="save" />
                  {{ saving ? 'Zapisywanie...' : 'Zapisz zmiany' }}
                </button>
                <button
                  type="button"
                  @click="resetForm"
                  class="btn btn-primary"
                  :disabled="saving || !isFormDirty"
                >
                  <font-awesome-icon icon="undo" />
                  Resetuj
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import MainLayout from '@/views/layouts/MainLayout.vue'
import { Backend } from '@/main'
import type { UserDataDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import { getCurrentUserId, getUserData } from '@/helpers/userHelpers'
import { formatDateTime } from '@/helpers/dateFormatter'

const router = useRouter()

const loading = ref(false)
const saving = ref(false)
const error = ref<string | null>(null)
const successMessage = ref<string | null>(null)
const errorMessage = ref<string | null>(null)
const userData = ref<UserDataDTO | null>(null)
const formData = ref({
  userName: '',
  name: '',
  surname: '',
  email: '',
  role: '',
})

const isFormDirty = computed(() => {
  if (!userData.value) return false
  return (
    formData.value.userName !== userData.value.userName ||
    formData.value.name !== userData.value.name ||
    formData.value.surname !== userData.value.surname ||
    formData.value.email !== userData.value.email
  )
})

const loadUserData = async () => {
  loading.value = true
  error.value = null
  successMessage.value = null
  errorMessage.value = null

  try {
    const userId = getCurrentUserId()

    if (!userId) {
      throw new Error('Nie znaleziono danych użytkownika. Zaloguj się ponownie.')
    }

    const user = await Backend.getUser(userId)
    userData.value = user

    formData.value = {
      userName: user.userName || '',
      name: user.name || '',
      surname: user.surname || '',
      email: user.email || '',
      role: user.role || '',
    }
  } catch (err: any) {
    console.error('Error loading user data:', err)
    error.value = err.message || 'Nie udało się załadować danych użytkownika'
  } finally {
    loading.value = false
  }
}

const handleSubmit = async () => {
  if (!userData.value?.id) {
    errorMessage.value = 'Brak ID użytkownika'
    return
  }

  saving.value = true
  successMessage.value = null
  errorMessage.value = null

  try {
    const updatedUser: UserDataDTO = {
      ...userData.value,
      userName: formData.value.userName,
      name: formData.value.name,
      surname: formData.value.surname,
      email: formData.value.email,
    }

    const success = await Backend.updateUser(userData.value.id, updatedUser)

    if (success) {
      userData.value = updatedUser

      const storedUserData = getUserData() || {}
      const updatedStoredData = { ...storedUserData, ...updatedUser }
      localStorage.setItem('remontio_user_data', JSON.stringify(updatedStoredData))

      successMessage.value = 'Dane zostały pomyślnie zaktualizowane!'

      setTimeout(() => {
        successMessage.value = null
      }, 5000)
    } else {
      errorMessage.value = 'Nie udało się zaktualizować danych'
    }
  } catch (err: any) {
    console.error('Error updating user:', err)
    errorMessage.value = err.message || 'Wystąpił błąd podczas zapisywania danych'
  } finally {
    saving.value = false
  }
}

const resetForm = () => {
  if (userData.value) {
    formData.value = {
      userName: userData.value.userName || '',
      name: userData.value.name || '',
      surname: userData.value.surname || '',
      email: userData.value.email || '',
      role: userData.value.role || '',
    }
  }
  successMessage.value = null
  errorMessage.value = null
}

onMounted(() => {
  loadUserData()
})
</script>
