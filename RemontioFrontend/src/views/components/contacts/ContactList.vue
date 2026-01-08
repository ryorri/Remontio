<template>
  <MainLayout>
    <div class="contacts-container">
      <div class="contacts-header">
        <h1 class="contacts-title">Kontakty</h1>
        <button class="btn btn-primary" @click="createNewContact">
          <font-awesome-icon icon="plus" /> Nowy Kontakt
        </button>
      </div>

      <div v-if="!loading && contactList.length > 0" class="search-container">
        <input
          v-model="searchQuery"
          type="text"
          class="form-control search-input"
          placeholder="Szukaj kontaktów (nazwa, specjalizacja, dane kontaktowe, opis)..."
        />
        <font-awesome-icon icon="search" class="search-icon" />
      </div>

      <div v-if="loading" class="loading-state">
        <div class="spinner"></div>
        <p>Ładowanie kontaktów...</p>
      </div>

      <div v-else-if="contactList.length === 0" class="empty-state">
        <font-awesome-icon icon="address-book" style="font-size: 5rem" />
        <h2>Brak kontaktów</h2>
        <p>Dodaj kontakty do wykonawców i specjalistów</p>
        <button class="btn btn-primary btn-large" @click="createNewContact">
          Dodaj pierwszy kontakt
        </button>
      </div>

      <div v-else class="table-container">
        <table class="contacts-table">
          <thead>
            <tr>
              <th>Nazwa</th>
              <th>Specjalizacja</th>
              <th>Dane kontaktowe</th>
              <th>Opis</th>
              <th>Utworzono</th>
              <th>Akcje</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="contact in filteredContacts" :key="contact.id" class="contact-row">
              <td class="contact-name">{{ contact.name }}</td>
              <td class="contact-spec">
                <span class="spec-badge">{{ getSpecLabel(contact.spec) }}</span>
              </td>
              <td class="contact-details">{{ contact.contactDetails || '-' }}</td>
              <td class="contact-description">
                {{ contact.description || 'Brak opisu' }}
              </td>
              <td class="date-cell">
                {{ formatDate(contact.createdDate) }}
              </td>
              <td>
                <div class="action-buttons">
                  <button
                    class="btn btn-sm btn-primary"
                    @click="openContact(contact.id)"
                    title="Otwórz"
                  >
                    <font-awesome-icon icon="eye" />
                  </button>
                  <button class="btn btn-sm btn-primary" @click="onEdit(contact.id)" title="Edytuj">
                    <font-awesome-icon icon="edit" />
                  </button>
                  <button
                    class="btn btn-sm"
                    :class="contact.isPrivate ? 'btn-info' : 'btn-warning'"
                    @click="togglePrivacy(contact.id, !contact.isPrivate)"
                    :title="contact.isPrivate ? 'Ustaw publiczny' : 'Ustaw prywatny'"
                  >
                    <font-awesome-icon :icon="contact.isPrivate ? 'globe' : 'lock'" />
                  </button>
                  <button class="btn btn-sm btn-danger" @click="onDelete(contact.id)" title="Usuń">
                    <font-awesome-icon icon="trash-can" />
                  </button>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </MainLayout>
</template>

<script lang="ts" setup>
import { Backend } from '@/main'
import MainLayout from '@/views/layouts/MainLayout.vue'
import type { ContactDataDTO } from '@/backend/BackendBase'
import { onMounted, ref, computed } from 'vue'
import { getCurrentUserId } from '@/helpers/userHelpers'
import { formatDate } from '@/helpers/dateFormatter'
import { getSpecLabel } from '@/helpers/specEnumFormatter'
import { useRouter } from 'vue-router'
import { FontAwesomeIcon } from '@fortawesome/vue-fontawesome'

const contactList = ref<ContactDataDTO[]>([])
const loading = ref(true)
const searchQuery = ref('')
const router = useRouter()

const filteredContacts = computed(() => {
  if (!searchQuery.value.trim()) {
    return contactList.value
  }

  const query = searchQuery.value.toLowerCase()
  return contactList.value.filter((contact) => {
    const name = contact.name?.toLowerCase() || ''
    const spec = getSpecLabel(contact.spec).toLowerCase()
    const contactDetails = contact.contactDetails?.toLowerCase() || ''
    const description = contact.description?.toLowerCase() || ''

    return (
      name.includes(query) ||
      spec.includes(query) ||
      contactDetails.includes(query) ||
      description.includes(query)
    )
  })
})

const fetchContacts = async () => {
  try {
    loading.value = true
    const userId = getCurrentUserId()
    if (!userId) {
      console.error('No user ID found')
      return
    }
    contactList.value = await Backend.getContactListByUserId(userId)
  } catch (error) {
    console.error('Error fetching contacts:', error)
  } finally {
    loading.value = false
  }
}

const createNewContact = () => {
  router.push({ name: 'ContactCreate' })
}

const openContact = (contactId: string | undefined) => {
  if (!contactId) return
  router.push({ name: 'ContactDetails', params: { contactId } })
}

const onEdit = (contactId: string | undefined) => {
  if (!contactId) return
  router.push({ name: 'ContactEdit', params: { contactId } })
}

const onDelete = async (contactId: string | undefined) => {
  if (!contactId) return
  if (!confirm('Czy na pewno chcesz usunąć ten kontakt?')) return
  try {
    await Backend.deleteContact(contactId)
    await fetchContacts()
  } catch (e) {
    console.error('Nie udało się usunąć kontaktu', e)
  }
}

const togglePrivacy = async (contactId: string | undefined, isPrivate: boolean) => {
  if (!contactId) return
  try {
    await Backend.changePrivacy(contactId, isPrivate)
    await fetchContacts()
  } catch (e) {
    console.error('Nie udało się zmienić prywatności', e)
  }
}

onMounted(() => fetchContacts())
</script>

<style scoped>
.contacts-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
}

.contacts-header .btn {
  display: flex;
  align-items: center;
  gap: 8px;
}

.search-container {
  position: relative;
  margin-bottom: 1.5rem;
  max-width: 600px;
  margin-left: auto;
}

.search-input {
  width: 100%;
  padding: 0.75rem 2.5rem 0.75rem 1rem;
  border: 2px solid var(--color-bg-light-gray);
  border-radius: 12px;
  font-size: 1rem;
  transition: all 0.2s ease;
}

.search-input:focus {
  outline: none;
  border-color: var(--color-primary-blue);
  box-shadow: 0 0 0 3px rgba(74, 144, 226, 0.1);
}

.search-icon {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: var(--color-text-medium);
  pointer-events: none;
}

.empty-state {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  text-align: center;
  gap: 1rem;
}

.empty-state i {
  font-size: 5rem;
  color: var(--color-primary-purple);
  opacity: var(--opacity-medium);
}

.empty-state h2 {
  font-size: 2rem;
  color: var(--color-text-dark);
  margin: 0;
}

.empty-state p {
  font-size: 1.1rem;
  color: var(--color-text-medium);
  max-width: 600px;
}

.table-container {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 12px var(--shadow-light);
  overflow: hidden auto;
}

.contacts-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.95rem;
}

.contacts-table thead {
  position: sticky;
  top: 0;
  z-index: 10;
  background: var(--gradient-primary);
}

.contacts-table th {
  padding: 1rem;
  text-align: left;
  color: var(--color-text-white);
  font-weight: 600;
  text-transform: uppercase;
  font-size: 0.85rem;
  letter-spacing: 0.5px;
  white-space: nowrap;
}

.contacts-table td {
  padding: 1rem;
  color: var(--color-text-dark);
}

.contacts-table tbody tr {
  border-bottom: 1px solid var(--color-bg-light-gray);
  transition: all 0.2s ease;
}

.contact-row:hover {
  background: var(--color-bg-light-gray);
}

.contact-name {
  font-weight: 600;
  color: var(--color-primary-blue);
}

.contact-spec {
  font-size: 0.9rem;
}

.spec-badge {
  display: inline-block;
  padding: 4px 12px;
  border-radius: 12px;
  background: var(--color-bg-blue-pale);
  color: var(--color-primary-blue);
  font-weight: 600;
  font-size: 0.85rem;
}

.contact-details {
  font-size: 0.9rem;
  color: var(--color-text-medium);
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.contact-description {
  max-width: 400px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  color: var(--color-text-medium);
}

.date-cell {
  font-size: 0.9rem;
  color: var(--color-text-medium);
  white-space: nowrap;
}

.action-buttons {
  display: flex;
  gap: 0.5rem;
}

.btn-info {
  background-color: #17a2b8;
  color: white;
}

.btn-info:hover {
  background-color: #138496;
}

.btn-warning {
  background-color: #ffc107;
  color: #212529;
}

.btn-warning:hover {
  background-color: #e0a800;
}

@media (max-width: 768px) {
  .contacts-container {
    padding: 1rem;
  }

  .contacts-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }

  .contacts-title {
    font-size: 2rem;
    text-align: center;
  }

  .contacts-header .btn {
    width: 100%;
    justify-content: center;
  }

  .contacts-table {
    font-size: 0.85rem;
  }

  .contacts-table th,
  .contacts-table td {
    padding: 0.75rem 0.5rem;
  }

  .contact-description,
  .contact-details {
    max-width: 120px;
  }

  .action-buttons {
    flex-direction: column;
  }
}
</style>
