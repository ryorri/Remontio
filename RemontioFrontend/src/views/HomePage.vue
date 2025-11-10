<template>
  <div class="homepage">
    <section class="hero">
      <div class="container">
        <h1 class="hero-title">🏠 Remontio</h1>
        <p class="hero-subtitle">Zarządzaj swoim remontem profesjonalnie</p>
        <p class="hero-description">
          Kompleksowe narzędzie do planowania, organizacji i kontroli projektów remontowych
        </p>
        <div class="hero-buttons">
          <button class="btn btn-primary">Zacznij teraz</button>
          <button class="btn btn-secondary">Dowiedz się więcej</button>
        </div>
      </div>
    </section>

    <section class="features">
      <div class="container">
        <h2 class="section-title">Funkcjonalności</h2>
        <div class="features-grid">
          <div class="feature-card">
            <div class="feature-icon">📋</div>
            <h3>Projekty i Pomieszczenia</h3>
            <p>Organizuj remont według projektów i pomieszczeń z pełną kontrolą statusów</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">✅</div>
            <h3>Harmonogram Zadań</h3>
            <p>Planuj zadania, ustalaj priorytety i śledź postępy prac remontowych</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">💰</div>
            <h3>Kosztorys</h3>
            <p>Kontroluj budżet z podziałem na kategorie i monitoruj wydatki</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">🛒</div>
            <h3>Lista Zakupów</h3>
            <p>Twórz listy materiałów z podziałem na pomieszczenia i kategorie</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">🧮</div>
            <h3>Kalkulatory</h3>
            <p>Oblicz ilość farby, podłóg, płytek i innych materiałów</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">📸</div>
            <h3>Dokumentacja Zdjęciowa</h3>
            <p>Archiwizuj postępy prac z galerią zdjęć przed i po</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">🔔</div>
            <h3>Alerty i Powiadomienia</h3>
            <p>Otrzymuj powiadomienia o terminach</p>
          </div>

          <div class="feature-card">
            <div class="feature-icon">📤</div>
            <h3>Eksport Danych</h3>
            <p>Eksportuj kosztorysy i listy do PDF/CSV</p>
          </div>
        </div>
      </div>
    </section>

    <section class="cta">
      <div class="container">
        <h2>Gotowy do rozpoczęcia remontu?</h2>
        <p>Dołącz do tysięcy zadowolonych użytkowników, którzy efektywnie zarządzają swoimi projektami</p>
        <button class="btn btn-primary btn-large">Rozpocznij za darmo</button>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
// Homepage component for Remontio renovation management application
</script>

<style scoped>
.homepage {
  min-height: 100vh;
  background: linear-gradient(to bottom, #f8f9fa, #ffffff);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}

/* Hero Section */
.hero {
  padding: 80px 20px;
  text-align: center;
  background: linear-gradient(135deg, #8f9fe6 0%, #b18bd8 100%);
  color: white;
}

.hero-title {
  font-size: 4rem;
  font-weight: 800;
  margin-bottom: 20px;
  animation: fadeInDown 0.8s ease;
}

.hero-subtitle {
  font-size: 1.8rem;
  font-weight: 500;
  margin-bottom: 15px;
  animation: fadeInDown 1s ease;
}

.hero-description {
  font-size: 1.2rem;
  margin-bottom: 40px;
  opacity: 0.95;
  animation: fadeInUp 1.2s ease;
}

.hero-buttons {
  display: flex;
  gap: 20px;
  justify-content: center;
  flex-wrap: wrap;
  animation: fadeInUp 1.4s ease;
}

/* Features Section */
.features {
  padding: 80px 20px;
}

.section-title {
  font-size: 2.5rem;
  font-weight: 700;
  text-align: center;
  margin-bottom: 60px;
  color: #333;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 30px;
  margin-bottom: 40px;
}

.feature-card {
  background: white;
  padding: 40px 30px;
  border-radius: 16px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.07);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  text-align: center;
}

.feature-card:hover {
  transform: translateY(-10px);
  box-shadow: 0 12px 24px rgba(0, 0, 0, 0.15);
}

.feature-icon {
  font-size: 3.5rem;
  margin-bottom: 20px;
}

.feature-card h3 {
  font-size: 1.4rem;
  font-weight: 600;
  margin-bottom: 15px;
  color: #333;
}

.feature-card p {
  font-size: 1rem;
  color: #666;
  line-height: 1.6;
}

/* CTA Section */
.cta {
  padding: 80px 20px;
  text-align: center;
  background: linear-gradient(135deg, #8f9fe6 0%, #b18bd8 100%);
  color: white;
}


.cta h2 {
  font-size: 2.5rem;
  font-weight: 700;
  margin-bottom: 20px;
}

.cta p {
  font-size: 1.2rem;
  margin-bottom: 40px;
  opacity: 0.95;
}

/* Animations */
@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

/* Responsive */
@media (max-width: 768px) {
  .hero-title {
    font-size: 2.5rem;
  }

  .hero-subtitle {
    font-size: 1.3rem;
  }

  .hero-description {
    font-size: 1rem;
  }

  .section-title {
    font-size: 2rem;
  }

  .features-grid {
    grid-template-columns: 1fr;
  }

  .cta h2 {
    font-size: 2rem;
  }
}
</style>