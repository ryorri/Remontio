## START!

# 📋 Plan zadań – Remontio

## 🧱 1. Konfiguracja projektu

- [ ] Inicjalizacja repozytorium (frontend + backend)

## 🎨 2. Frontend (Vue.js 3 + Bootstrap 5)

**Ogólne:**

- [ ] Konfiguracja projektu Vue (Vite, Bootstrap, Routing)
- [ ] Layout ogólny (sidebar + topbar)
- [ ] Routing dynamiczny na podstawie projektu

**Widoki:**

- [ ] Dashboard – komponenty do alertów, zadań i podsumowań
- [ ] Harmonogram prac – lista + formularz edycji
- [ ] Kosztorys – tabela z kategoriami i edytowalnymi polami
- [ ] Lista zakupów – dodawanie/odhaczanie/dzielenie na pomieszczenia
- [ ] Kalkulatory – farby, podłogi, ściany (formularz + wynik)
- [ ] Dokumentacja zdjęciowa – galeria + kategorie
- [ ] Formularz logowania/rejestracji + obsługa sesji

## 🧠 3. Backend (ASP.NET Core Web API + EF Core)

**Struktura i infrastruktura:**

- [ ] Konfiguracja projektu ASP.NET Core
- [ ] Entity Framework Core
- [ ] AutoMapper + FluentValidation
- [ ] API dokumentacja (Swagger)

**Moduły:**

- [ ] Autoryzacja użytkownika (Identity, JWT)
- [ ] Obsługa projektów i pomieszczeń
- [ ] Harmonogram zadań (CRUD + przypisanie do pokoju)
- [ ] Alerty i powiadomienia (np. przekroczenie budżetu)
- [ ] Kosztorysy – model kategorii + sumowanie
- [ ] Lista zakupów (powiązanie z kalkulatorami)
- [ ] Kalkulatory (algorytmy do farb, podłóg, napraw)
- [ ] Obsługa uploadu zdjęć i ich kategoryzacja

## 📂 4. Baza danych i testy

- [ ] Projekt bazy danych (SQLite – schemat + migracje)
- [ ] Dodanie danych testowych (np. szablony zadań)
- [ ] Jednostkowe testy backendu (np. walidacje, kalkulacje)
- [ ] Testy integracyjne API (np. autoryzacja, dodawanie zadań)

## 🌍 5. Integracje i eksporty

- [ ] Eksport kosztorysu i listy zakupowej do PDF/CSV
- [ ] Integracja z Google Calendar / Apple Calendar (iCal)
- [ ] Powiadomienia e-mail (opcjonalnie w przyszłości)

## 📦 6. Finalizacja i dokumentacja

- [ ] Instrukcja wdrożeniowa i lokalnego uruchomienia
- [ ] README z podziałem projektów
- [ ] Test końcowy aplikacji z checklistą
