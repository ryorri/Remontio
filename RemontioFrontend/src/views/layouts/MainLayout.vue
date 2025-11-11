<template>
  <div class="main-layout">
    <header class="header">
      <div class="header-inner">
        <h1 class="title">🏠 Remontio</h1>
      </div>
    </header>
    <div class="container">
      <div v-if="showMenu" class="nav-column">
        <button class="menu-toggle-btn" @click="showMenu = !showMenu">
          {{ showMenu ? '⟨ Ukryj menu' : '☰ Pokaż menu' }}
        </button>
        <nav class="nav-menu">
          <ul class="nav-list">
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">📋</span>
                <span class="nav-text">Projekty</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">✅</span>
                <span class="nav-text">Zadania</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">💰</span>
                <span class="nav-text">Budżet</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">🛒</span>
                <span class="nav-text">Zakupy</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">🧮</span>
                <span class="nav-text">Kalkulatory</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">📸</span>
                <span class="nav-text">Galeria</span>
              </a>
            </li>
            <li>
              <a href="#" class="cst-btn">
                <span class="nav-icon">⚙️</span>
                <span class="nav-text">Ustawienia</span>
              </a>
            </li>
          </ul>
        </nav>
      </div>
      <button
        v-else
        class="menu-toggle-btn-floating"
        @click="showMenu = !showMenu"
        title="Pokaż menu"
      >
        ☰
      </button>
      <main class="content">
        <slot></slot>
      </main>
    </div>
    <footer class="footer">
      <p>&copy; {{ year }} Remontio. All rights reserved.</p>
    </footer>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue'
const year = new Date().getFullYear()
const showMenu = ref(true)
</script>

<style scoped lang="css">
.main-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.header {
  width: 100%;
  background: var(--gradient-primary);
  color: var(--color-text-white);
  padding: 20px 0;
  box-shadow: 0 2px 8px var(--shadow-light);
}

.header-inner {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 12px;
}

.title {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
  font-size: 2rem;
  font-weight: 700;
}

.container {
  max-width: 100%;
  width: 100%;
  margin: 0;
  padding: 0 20px;
  flex: 1;
  display: flex;
  flex-direction: row;
  gap: 30px;
  position: relative;
}

.nav-column {
  width: 250px;
  flex-shrink: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.menu-toggle-btn {
  width: 100%;
  padding: 10px 16px;
  margin-top: 10px;
  font-size: 0.9rem;
  border-radius: 8px;
  background: var(--color-bg-white);
  color: var(--color-text-dark);
  border: 1px solid rgba(0, 0, 0, 0.1);
  box-shadow: 0 2px 6px var(--shadow-light);
  opacity: 0.85;
  cursor: pointer;
  transition:
    opacity 0.2s ease,
    box-shadow 0.2s ease;
  font-weight: 500;
}

.menu-toggle-btn:hover {
  opacity: 1;
  box-shadow: 0 4px 8px var(--shadow-medium);
}

.menu-toggle-btn-floating {
  position: absolute;
  top: 10px;
  left: 10px;
  width: 36px;
  height: 36px;
  padding: 0;
  font-size: 1.2rem;
  border-radius: 50%;
  background: var(--color-bg-white);
  color: var(--color-text-dark);
  border: 1px solid rgba(0, 0, 0, 0.1);
  box-shadow: 0 2px 6px var(--shadow-light);
  opacity: 0.6;
  cursor: pointer;
  transition:
    opacity 0.2s ease,
    box-shadow 0.2s ease,
    transform 0.2s ease;
  font-weight: 500;
  z-index: 10;
  display: flex;
  align-items: center;
  justify-content: center;
}

.menu-toggle-btn-floating:hover {
  opacity: 1;
  box-shadow: 0 4px 10px var(--shadow-medium);
  transform: scale(1.1);
}

.nav-menu {
  width: 100%;
  padding: 30px 20px;
  flex-shrink: 0;
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  max-height: calc(100vh - 180px);
  overflow-y: auto;
}

.nav-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.content {
  flex: 1;
  padding: 30px 0 30px 40px;
  min-width: 0;
  display: flex;
  flex-direction: column;
  position: relative;
}

.footer {
  width: 100%;
  padding: 20px;
  text-align: center;
  border-top: 1px solid #e0e0e0;
  color: var(--color-text-medium);
  font-size: 0.9rem;
  background: var(--color-bg-white);
  margin-top: auto;
}

/* Responsive */
@media (max-width: 768px) {
  .container {
    flex-direction: column;
    gap: 0;
  }

  .nav-column {
    width: 100%;
  }

  .menu-toggle-btn {
    margin-bottom: 10px;
  }

  .nav-menu {
    width: 100%;
    padding: 20px;
    height: 125px;
    position: static;
    max-height: none;
    border-radius: 0;
    box-shadow: 0 2px 8px var(--shadow-light);
  }

  .nav-list {
    flex-direction: row;
    overflow-x: auto;
    gap: 4px;
  }

  .content {
    padding: 20px 0;
  }
}
</style>
