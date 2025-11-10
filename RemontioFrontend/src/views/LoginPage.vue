<template>
  <div class="login-page">
    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h1 class="login-title">🏠 Remontio</h1>
          <p class="login-subtitle">Zaloguj się do swojego konta</p>
        </div>

        <form class="login-form" @submit.prevent="handleLogin">
          <div class="form-group">
            <label for="email">Login</label>
            <input
              type="login"
              id="email"
              v-model="email"
              placeholder="Twój login"
              required
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
            />
          </div>

          <button type="submit" class="btn btn-primary btn-large btn-block">
            Zaloguj się
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
import { Backend } from '@/main'
import { ref } from 'vue'

const email = ref('')
const password = ref('')


const handleLogin = async () => {
  console.log('Login attempt:', { email: email.value})

}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  background: linear-gradient(135deg, #8f9fe6 0%, #b18bd8 100%);
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
  background: white;
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
  color: #333;
  margin-bottom: 10px;
}

.login-subtitle {
  font-size: 1.1rem;
  color: #666;
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
  color: #333;
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
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
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
  color: #666;
  cursor: pointer;
}

.remember-me input[type="checkbox"] {
  cursor: pointer;
  width: 18px;
  height: 18px;
}

.forgot-password {
  font-size: 0.9rem;
  color: #667eea;
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
}

.login-footer {
  margin-top: 30px;
  text-align: center;
  padding-top: 25px;
  border-top: 1px solid #e0e0e0;
}

.login-footer p {
  font-size: 0.95rem;
  color: #666;
}

.register-link {
  color: #667eea;
  font-weight: 600;
  text-decoration: none;
}

.register-link:hover {
  text-decoration: underline;
}
</style>