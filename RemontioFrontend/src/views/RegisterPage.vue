<template>
  <div class="register-page">
    <div class="register-container">
      <div class="register-card">
        <div class="register-header">
          <h1 class="register-title">🏠 Remontio</h1>
          <p class="register-subtitle">Stwórz nowe konto</p>
        </div>

        <form class="register-form" @submit.prevent="handleRegister">
          <div class="form-group">
            <label for="username">
              <font-awesome-icon icon="user" class="label-icon" />
              Login
            </label>
            <input
              type="text"
              id="username"
              v-model="formData.userName"
              placeholder="Twój login"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="name">
              <font-awesome-icon icon="user" class="label-icon" />
              Imię
            </label>
            <input
              type="text"
              id="name"
              v-model="formData.name"
              placeholder="Twoje imię"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="surname">
              <font-awesome-icon icon="user" class="label-icon" />
              Nazwisko (opcjonalne)
            </label>
            <input
              type="text"
              id="surname"
              v-model="formData.surname"
              placeholder="Twoje nazwisko"
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="email">
              <font-awesome-icon icon="globe" class="label-icon" />
              Email
            </label>
            <input
              type="email"
              id="email"
              v-model="formData.email"
              placeholder="twoj@email.com"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="password">
              <font-awesome-icon icon="lock" class="label-icon" />
              Hasło
            </label>
            <input
              type="password"
              id="password"
              v-model="formData.password"
              placeholder="••••••••"
              required
              :disabled="isLoading"
            />
          </div>

          <div class="form-group">
            <label for="confirmPassword">
              <font-awesome-icon icon="lock" class="label-icon" />
              Potwierdź hasło
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
            {{ isLoading ? 'Rejestracja...' : 'Zarejestruj się' }}
          </button>
        </form>

        <div class="register-footer">
          <p>
            Masz już konto?
            <a @click="goToLogin" class="login-link">Zaloguj się</a>
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
import type { CreateUserDTO } from '@/backend/BackendBase'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const router = useRouter()

const goToLogin = () => {
  router.push({ name: 'LoginPage' })
}

const formData = ref<CreateUserDTO>({
  userName: '',
  name: '',
  surname: '',
  email: '',
  password: '',
  role: 'User',
})

const confirmPassword = ref('')
const error = ref<string | null>(null)
const successMessage = ref<string | null>(null)
const isLoading = ref(false)

const validateForm = (): boolean => {
  if (
    !formData.value.userName ||
    !formData.value.name ||
    !formData.value.email ||
    !formData.value.password
  ) {
    error.value = 'Wszystkie wymagane pola muszą być wypełnione'
    return false
  }

  if (formData.value.password !== confirmPassword.value) {
    error.value = 'Hasła nie są identyczne'
    return false
  }

  if (formData.value.password.length < 6) {
    error.value = 'Hasło musi mieć co najmniej 6 znaków'
    return false
  }

  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!emailRegex.test(formData.value.email || '')) {
    error.value = 'Nieprawidłowy format adresu email'
    return false
  }

  return true
}

const handleRegister = async () => {
  error.value = null
  successMessage.value = null

  if (!validateForm()) {
    return
  }

  isLoading.value = true

  try {
    const response = await Backend.register(formData.value)

    if (response) {
      successMessage.value = 'Konto zostało utworzone! Przekierowanie do logowania...'
      setTimeout(() => {
        router.push({ name: 'LoginPage' })
      }, 2000)
    } else {
      error.value = 'Nie udało się utworzyć konta. Spróbuj ponownie.'
    }
  } catch (err: any) {
    console.error('Registration error:', err)
    if (err.message?.includes('already exists')) {
      error.value = 'Użytkownik o podanej nazwie lub emailu już istnieje'
    } else {
      error.value = 'Wystąpił błąd podczas rejestracji. Spróbuj ponownie.'
    }
  } finally {
    isLoading.value = false
  }
}
</script>

<style scoped>
.register-page {
  min-height: 100vh;
  background: var(--gradient-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.register-container {
  width: 100%;
  max-width: 500px;
}

.register-card {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  padding: 40px;
  animation: fadeInUp 0.6s ease;
}

.register-header {
  text-align: center;
  margin-bottom: 32px;
}

.register-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--color-text-dark);
  margin-bottom: 10px;
}

.register-subtitle {
  font-size: 1.1rem;
  color: var(--color-text-medium);
}

.register-form {
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

.register-footer {
  text-align: center;
  margin-top: 24px;
  padding-top: 24px;
  border-top: 1px solid #e0e0e0;
}

.register-footer p {
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
  .register-card {
    padding: 30px 24px;
  }

  .register-title {
    font-size: 2rem;
  }

  .register-subtitle {
    font-size: 1rem;
  }
}
</style>
