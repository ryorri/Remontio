<template>
  <div class="reset-password-page">
    <div class="reset-password-container">
      <div class="reset-password-card">
        <div class="reset-password-header">
          <h1 class="reset-password-title">🔒 Zmiana hasła</h1>
          <p class="reset-password-subtitle">Ustaw nowe hasło dla swojego konta</p>
        </div>

        <form class="reset-password-form" @submit.prevent="handleResetPassword">
          <div class="form-group">
            <label for="email">
              <font-awesome-icon icon="globe" class="label-icon" />
              Email
            </label>
            <input
              type="email"
              id="email"
              v-model="email"
              placeholder="twoj@email.com"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="oldPassword">
              <font-awesome-icon icon="lock" class="label-icon" />
              Stare hasło
            </label>
            <input
              type="password"
              id="oldPassword"
              v-model="oldPassword"
              placeholder="••••••••"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="newPassword">
              <font-awesome-icon icon="lock" class="label-icon" />
              Nowe hasło
            </label>
            <input
              type="password"
              id="newPassword"
              v-model="newPassword"
              placeholder="••••••••"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="confirmPassword">
              <font-awesome-icon icon="lock" class="label-icon" />
              Potwierdź nowe hasło
            </label>
            <input
              type="password"
              id="confirmPassword"
              v-model="confirmPassword"
              placeholder="••••••••"
              required
              :disabled="isLoading"
            />
          </div>

          <div v-if="error" class="alert alert-danger">
            <span class="error-icon">⚠️</span>
            <span>{{ error }}</span>
          </div>

          <div v-if="successMessage" class="alert alert-success">
            <font-awesome-icon icon="check-circle" class="success-icon" />
            <span>{{ successMessage }}</span>
          </div>

          <button type="submit" class="btn btn-primary btn-large btn-block" :disabled="isLoading">
            <font-awesome-icon v-if="isLoading" icon="spinner" spin class="btn-icon" />
            {{ isLoading ? 'Zmiana hasła...' : 'Zmień hasło' }}
          </button>
        </form>

        <div class="reset-password-footer">
          <p>
            <router-link to="/login" class="login-link">Powrót do logowania</router-link>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { Backend } from '@/main'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const router = useRouter()

const email = ref('')
const oldPassword = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const error = ref<string | null>(null)
const successMessage = ref<string | null>(null)
const isLoading = ref(false)

const validateForm = (): boolean => {
  if (!email.value || !oldPassword.value || !newPassword.value || !confirmPassword.value) {
    error.value = 'Wszystkie pola muszą być wypełnione'
    return false
  }

  if (newPassword.value !== confirmPassword.value) {
    error.value = 'Nowe hasła nie są identyczne'
    return false
  }

  if (newPassword.value.length < 6) {
    error.value = 'Nowe hasło musi mieć co najmniej 6 znaków'
    return false
  }

  if (oldPassword.value === newPassword.value) {
    error.value = 'Nowe hasło musi być inne niż stare hasło'
    return false
  }

  return true
}

const handleResetPassword = async () => {
  error.value = null
  successMessage.value = null

  if (!validateForm()) {
    return
  }

  isLoading.value = true

  try {
    const response = await Backend.changeUserPassword(
      email.value,
      oldPassword.value,
      newPassword.value,
    )

    if (response) {
      successMessage.value = 'Hasło zostało zmienione! Przekierowanie do logowania...'
      setTimeout(() => {
        router.push({ name: 'LoginPage' })
      }, 2000)
    } else {
      error.value = 'Nie udało się zmienić hasła. Spróbuj ponownie.'
    }
  } catch (err: any) {
    console.error('Password change error:', err)
    if (err.message?.includes('Invalid') || err.message?.includes('incorrect')) {
      error.value = 'Nieprawidłowy email lub stare hasło'
    } else if (err.message?.includes('not found')) {
      error.value = 'Użytkownik nie został znaleziony'
    } else {
      error.value = 'Wystąpił błąd podczas zmiany hasła. Spróbuj ponownie.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.reset-password-page {
  min-height: 100vh;
  background: var(--gradient-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.reset-password-container {
  width: 100%;
  max-width: 500px;
}

.reset-password-card {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  padding: 40px;
  animation: fadeInUp 0.6s ease;
}

.reset-password-header {
  text-align: center;
  margin-bottom: 32px;
}

.reset-password-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--color-text-dark);
  margin-bottom: 10px;
}

.reset-password-subtitle {
  font-size: 1.1rem;
  color: var(--color-text-medium);
}

.reset-password-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group label {
  display: flex;
  align-items: center;
  gap: 8px;
}

.label-icon {
  color: var(--color-primary-blue);
  font-size: 0.9rem;
}

.form-group input {
  padding: 14px 16px;
  font-size: 1rem;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.form-group input:focus {
  outline: none;
  border-color: var(--color-primary-blue);
  box-shadow: 0 0 0 3px rgba(74, 144, 226, 0.1);
}

.form-group input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.error-icon {
  font-size: 1.2rem;
}

.success-icon {
  font-size: 1.2rem;
  color: var(--color-green);
}

.btn-block {
  width: 100%;
}

.btn-icon {
  margin-right: 8px;
}

.reset-password-footer {
  text-align: center;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #e0e0e0;
}

.reset-password-footer p {
  color: var(--color-text-medium);
  font-size: 0.95rem;
}

.login-link {
  color: var(--color-primary-blue);
  font-weight: 600;
  text-decoration: none;
  transition: color 0.2s;
}

.login-link:hover {
  color: var(--color-primary-purple);
  text-decoration: underline;
}

@media (max-width: 768px) {
  .reset-password-card {
    padding: 30px 24px;
  }

  .reset-password-title {
    font-size: 2rem;
  }

  .reset-password-subtitle {
    font-size: 1rem;
  }
}
</style>
