<template>
  <div>
    <h3 class="section-title mb-3">
      <font-awesome-icon icon="calculator" />
      Kalkulacja niestandardowa
    </h3>

    <div class="custom-calculation-form">
      <div class="params-box">
        <div class="mb-3">
          <label class="form-label">Co będzie kalkulowane? *</label>
          <input
            v-model.trim="calculationName"
            type="text"
            class="form-control"
            placeholder="np. Panele podłogowe, Gładź gipsowa, Uszczelniacz"
            required
          />
          <small class="text-muted"> Nazwa materiału, pracy lub usługi </small>
        </div>

        <div class="mb-3">
          <label class="form-label">Ilość (wartość liczbowa) *</label>
          <input
            v-model.number="quantity"
            type="number"
            step="0.01"
            min="0"
            class="form-control"
            placeholder="np. 15.5"
            required
          />
          <small class="text-muted">
            Wprowadź ilość w odpowiednich jednostkach (m², l, kg, szt., itp.)
          </small>
        </div>

        <div class="mb-3">
          <label class="form-label">Jednostka *</label>
          <select v-model="unit" class="form-control" required>
            <option value="">-- Wybierz jednostkę --</option>
            <option value="m²">m² (metry kwadratowe)</option>
            <option value="m³">m³ (metry sześcienne)</option>
            <option value="m">m (metry)</option>
            <option value="l">l (litry)</option>
            <option value="kg">kg (kilogramy)</option>
            <option value="g">g (gramy)</option>
            <option value="szt">szt (sztuki)</option>
            <option value="opak">opak (opakowania)</option>
            <option value="godz">godz (godziny pracy)</option>
            <option value="inne">inne</option>
          </select>
          <small class="text-muted"> Wybierz odpowiednią jednostkę dla wprowadzonej ilości </small>
        </div>

        <div v-if="unit === 'inne'" class="mb-3">
          <label class="form-label">Nazwa jednostki *</label>
          <input
            v-model.trim="customUnit"
            type="text"
            class="form-control"
            placeholder="np. worki, puszki, rolki"
            required
          />
        </div>

        <hr class="my-4" />

        <h5 class="mb-3">Parametry dodatkowe (opcjonalne)</h5>

        <div class="mb-3">
          <label class="form-label">Liczba warstw</label>
          <input
            v-model.number="numberOfLayers"
            type="number"
            step="1"
            min="1"
            class="form-control"
            placeholder="np. 2"
          />
          <small class="text-muted"> Jeśli materiał nakładany jest w warstwach </small>
        </div>

        <div class="mb-3">
          <label class="form-label">Grubość warstwy (mm)</label>
          <input
            v-model.number="layerThickness"
            type="number"
            step="0.1"
            min="0"
            class="form-control"
            placeholder="np. 2.5"
          />
          <small class="text-muted"> Grubość pojedynczej warstwy w milimetrach </small>
        </div>

        <div class="mb-3">
          <label class="form-label">Powierzchnia (m²)</label>
          <input
            v-model.number="area"
            type="number"
            step="0.01"
            min="0"
            class="form-control"
            placeholder="np. 25.5"
          />
          <small class="text-muted"> Powierzchnia na której materiał będzie użyty </small>
        </div>

        <div class="mb-3">
          <label class="form-label">Notatki</label>
          <textarea
            v-model.trim="notes"
            class="form-control"
            rows="3"
            placeholder="Dodatkowe informacje, uwagi, specyfikacja..."
          ></textarea>
          <small class="text-muted"> Dowolne dodatkowe informacje dotyczące kalkulacji </small>
        </div>
      </div>

      <div
        v-if="calculationName && quantity && (unit !== 'inne' || customUnit)"
        class="calculation-summary mb-4"
      >
        <h4 class="mb-3">Podsumowanie</h4>
        <div class="summary-box">
          <div class="summary-item">
            <span class="summary-label">Materiał/Usługa:</span>
            <span class="summary-value">{{ calculationName }}</span>
          </div>
          <div class="summary-item total-item">
            <span class="summary-label">Ilość:</span>
            <span class="summary-value">
              {{ quantity.toFixed(2) }} {{ unit === 'inne' ? customUnit : unit }}
            </span>
          </div>
          <div v-if="numberOfLayers" class="summary-item">
            <span class="summary-label">Liczba warstw:</span>
            <span class="summary-value">{{ numberOfLayers }}</span>
          </div>
          <div v-if="layerThickness" class="summary-item">
            <span class="summary-label">Grubość warstwy:</span>
            <span class="summary-value">{{ layerThickness }} mm</span>
          </div>
          <div v-if="area" class="summary-item">
            <span class="summary-label">Powierzchnia:</span>
            <span class="summary-value">{{ area.toFixed(2) }} m²</span>
          </div>
          <div v-if="totalValue > 0" class="summary-item total-item">
            <span class="summary-label">Wartość całkowita:</span>
            <span class="summary-value">
              {{ totalValue.toFixed(2) }} {{ unit === 'inne' ? customUnit : unit }}
            </span>
          </div>
        </div>

        <button
          type="button"
          class="btn btn-success mt-3 w-100"
          @click="openShoppingListModal"
          :disabled="submitting"
        >
          <font-awesome-icon icon="shopping-cart" />
          Dodaj do listy zakupowej
        </button>
      </div>

      <form @submit.prevent="handleSubmit">
        <div class="d-flex flex-wrap gap-3">
          <button
            type="submit"
            class="btn btn-primary header-btn"
            :disabled="
              submitting ||
              !calculationName.trim() ||
              !quantity ||
              quantity <= 0 ||
              !unit ||
              (unit === 'inne' && !customUnit.trim())
            "
          >
            <font-awesome-icon icon="save" />
            {{ submitting ? 'Zapisywanie...' : 'Zapisz kalkulację' }}
          </button>
          <button
            type="button"
            class="btn btn-secondary header-btn"
            @click="$emit('back')"
            :disabled="submitting"
          >
            <font-awesome-icon icon="arrow-left" /> Wstecz
          </button>
        </div>

        <div v-if="error" class="alert alert-danger mt-3">{{ error }}</div>
      </form>
    </div>

    <AddToShoppingListModal
      :is-open="isShoppingListModalOpen"
      :calculation-data="{
        name: calculationName,
        totalEfficiency: totalValue,
      }"
      :room-id="roomId"
      @close="isShoppingListModalOpen = false"
    />
  </div>
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'
import AddToShoppingListModal from '../AddToShoppingListModal.vue'

interface Props {
  roomId: string
  submitting: boolean
}

const props = defineProps<Props>()

const emit = defineEmits<{
  back: []
  submit: [data: { calculationName: string; value: number }]
}>()

const calculationName = ref('')
const quantity = ref<number | null>(null)
const unit = ref('')
const customUnit = ref('')
const numberOfLayers = ref<number | null>(null)
const layerThickness = ref<number | null>(null)
const area = ref<number | null>(null)
const notes = ref('')
const error = ref('')
const isShoppingListModalOpen = ref(false)

const totalValue = computed(() => {
  if (!quantity.value) return 0

  let total = quantity.value

  // Jeśli podano liczbę warstw, pomnóż ilość
  if (numberOfLayers.value && numberOfLayers.value > 1) {
    total *= numberOfLayers.value
  }

  return total
})

const openShoppingListModal = () => {
  isShoppingListModalOpen.value = true
}

const handleSubmit = () => {
  if (!calculationName.value.trim()) {
    error.value = 'Podaj nazwę materiału/usługi'
    return
  }
  if (!quantity.value || quantity.value <= 0) {
    error.value = 'Podaj poprawną ilość'
    return
  }
  if (!unit.value) {
    error.value = 'Wybierz jednostkę'
    return
  }
  if (unit.value === 'inne' && !customUnit.value.trim()) {
    error.value = 'Podaj nazwę jednostki'
    return
  }

  emit('submit', {
    calculationName: calculationName.value,
    value: totalValue.value,
  })
}
</script>

<style scoped>
.section-title {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 1.5rem;
  font-weight: 700;
  color: var(--color-text-dark);
}

.custom-calculation-form {
  max-width: 800px;
}

.params-box {
  margin-bottom: 1.5rem;
}

hr {
  border: none;
  border-top: 1px solid #ddd;
  margin: 1.5rem 0;
}

h5 {
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--color-text-dark);
}

textarea.form-control {
  resize: vertical;
  min-height: 80px;
}

@media (max-width: 768px) {
  .custom-calculation-form {
    max-width: 100%;
  }
}
</style>
