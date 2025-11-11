<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1 class="login-title">🏠 Remontio</h1>
          <p class="login-subtitle">Zaloguj się do swojego konta</p>
        </div>

        <form class="login-form" @submit.prevent="handleLogin">
          <div v-if="authStore.sessionExpiredMessage" class="info-message">
            <span class="info-icon">ℹ️</span>
            <span>{{ authStore.sessionExpiredMessage }}</span>
          </div>

          <div class="form-group">
            <label for="username">Login</label>
            <input
              type="text"
              id="username"
              v-model="username"
              placeholder="Twój login"
              required
              :disabled="authStore.isLoading"
            />
          </div>

          <div class="form-group">
            <label for="password">Hasło</label>
            <input
              type="password"
              id="password"
              v-model="password"
              placeholder="••••••••"
              required
              :disabled="authStore.isLoading"
            />
          </div>

          <div v-if="authStore.error" class="error-message">
            <span class="error-icon">⚠️</span>
            <span>{{ authStore.error }}</span>
          </div>

          <button
            type="submit"
            class="btn btn-primary btn-large btn-block"
            :disabled="authStore.isLoading"
          >
            {{ authStore.isLoading ? 'Logowanie...' : 'Zaloguj się' }}
          </button>
        </form>

        <div class="login-footer">
          <p>Nie masz konta? <a href="#" class="register-link">Zarejestruj się</a></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/authStore'
import router from '@/router'
import { ref } from 'vue'
import { Backend } from '@/main'

const authStore = useAuthStore()

const username = ref('')
const password = ref('')

const handleLogin = async () => {
  authStore.clearError()
  authStore.clearSessionExpiredMessage()

  const success = await authStore.login(username.value, password.value)

  if (success) {
    router.push({ name: 'DashboardPage' })
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  background: var(--gradient-primary);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 20px;
}

.login-container {
  width: 100%;
  max-width: 450px;
}

.login-card {
  background: var(--color-bg-white);
  border-radius: 16px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.12);
  padding: 40px;
  animation: fadeInUp 0.6s ease;
}

.login-header {
  text-align: center;
  margin-bottom: 40px;
}

.login-title {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--color-text-dark);
  margin-bottom: 10px;
}

.login-subtitle {
  font-size: 1.1rem;
  color: var(--color-text-medium);
}

.login-form {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.form-group label {
  font-size: 0.95rem;
  font-weight: 600;
  color: var(--color-text-dark);
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
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.form-group input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
  opacity: 0.6;
}

.error-message {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  background-color: #fee;
  border: 1px solid #fcc;
  border-radius: 8px;
  color: #c33;
  font-size: 0.9rem;
  animation: shake 0.3s ease;
}

.error-icon {
  font-size: 1.2rem;
}

.info-message {
  display: flex;
  align-items: center;
  gap: 10px;
  padding: 12px 16px;
  background-color: #e7f3ff;
  border: 1px solid #b3d9ff;
  border-radius: 8px;
  color: #0066cc;
  font-size: 0.9rem;
}

.info-icon {
  font-size: 1.2rem;
}

@keyframes shake {
  0%,
  100% {
    transform: translateX(0);
  }
  25% {
    transform: translateX(-5px);
  }
  75% {
    transform: translateX(5px);
  }
}

.form-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 5px;
}

.remember-me {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.9rem;
  color: var(--color-text-medium);
  cursor: pointer;
}

.remember-me input[type='checkbox'] {
  cursor: pointer;
  width: 18px;
  height: 18px;
}

.forgot-password {
  font-size: 0.9rem;
  color: var(--color-primary-blue);
  text-decoration: none;
  font-weight: 600;
}

.forgot-password:hover {
  text-decoration: underline;
}

.btn-block {
  width: 100%;
  margin-top: 10px;
  border: solid 1px lightgrey;
  transition: all 0.3s ease;
}

.btn-block:disabled {
  background-color: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}

.btn-block:disabled:hover {
  transform: none;
  box-shadow: none;
}

.login-footer {
  margin-top: 30px;
  text-align: center;
  padding-top: 25px;
  border-top: 1px solid #e0e0e0;
}

.login-footer p {
  font-size: 0.95rem;
  color: var(--color-text-medium);
}

.register-link {
  color: var(--color-primary-blue);
  font-weight: 600;
  text-decoration: none;
}

.register-link:hover {
  text-decoration: underline;
}
</style>
