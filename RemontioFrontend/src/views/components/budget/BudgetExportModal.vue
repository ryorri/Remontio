<template>
  <div class="export-modal-overlay" @click.self="onClose">
    <div class="export-modal">
      <h3>Eksport budżetu</h3>
      <p>Wybierz format eksportu:</p>
      <div class="export-options">
        <button class="btn btn-primary" @click="handleExport('pdf')">PDF</button>
        <button class="btn btn-primary" @click="handleExport('csv')">CSV</button>
      </div>
      <button class="btn btn-secondary" @click="onClose">Anuluj</button>
    </div>
  </div>
</template>

<script lang="ts" setup>
import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'
import { Backend } from '@/main'
import { registerFonts } from '@/helpers/fontRegisterToExports'

const props = defineProps<{ budget: Record<string, any> & { items?: any[] } }>()
const emit = defineEmits(['close'])

const onClose = () => {
  emit('close')
}

const handleExport = (format: 'pdf' | 'csv') => {
  if (format === 'pdf') {
    exportToPDF()
  } else {
    exportToCSV()
  }
  emit('close')
}

const exportToPDF = async () => {
  const doc = new jsPDF()

  registerFonts(doc)

  doc.setFont('Roboto', 'normal')
  let y = 10
  const currency = props.budget.currency ?? 'PLN'

  doc.setFontSize(14)
  doc.text(`Budżet: ${props.budget.name ?? ''}`, 10, y)
  y += 8
  doc.setFontSize(11)
  doc.text(`Opis: ${props.budget.description ?? ''}`, 10, y)
  y += 8
  doc.text(`Projekt: ${props.budget.projectName ?? '-'}`, 10, y)
  y += 8
  doc.text(`Pokój: ${props.budget.roomName ?? '-'}`, 10, y)
  y += 8
  doc.text(`Wydano: ${props.budget.spent ?? 0} ${currency}`, 10, y)
  y += 8
  doc.text(`Szacowana cena: ${props.budget.estimatedPrice ?? '-'} ${currency}`, 10, y)
  y += 8
  doc.text(`Utworzono: ${props.budget.createAt ?? '-'}`, 10, y)
  y += 12

  const budgetItems = await Backend.getBudgetItems(props.budget.id)

  const items = Array.isArray(budgetItems) ? budgetItems : []

  if (items.length > 0) {
    const head = [['Nazwa', 'Opis', 'Kategoria', 'Cena szacunkowa', 'Czy zakończono']]
    const body = items.map((it: any) => [
      it.name ?? '-',
      it.description ?? '-',
      it.category ?? '-',
      it.estimatedPrice != null ? `${it.estimatedPrice} ${currency}` : '-',
      it.isCompleted ? 'Tak' : 'Nie',
    ])

    autoTable(doc, {
      startY: y,
      head: head,
      body: body,
      styles: { fontSize: 10, font: 'Roboto' },
      headStyles: { font: 'Roboto', fillColor: [41, 128, 185], textColor: 255 },
      theme: 'striped',
      margin: { left: 10, right: 10 },
    })
  } else {
    doc.setFontSize(11)
    doc.text('Brak pozycji budżetu.', 10, y)
  }

  doc.save(`budzet_${props.budget.name ?? 'export'}.pdf`)
}

const exportToCSV = async () => {
  const currency = props.budget.currency ?? 'PLN'
  const header = ['Nazwa budżetu', 'Projekt', 'Pokój', 'Wydano', 'Szacowana cena', 'Utworzono']
  const mainRow = [
    props.budget.name ?? '',
    props.budget.projectName ?? '-',
    props.budget.roomName ?? '-',
    `${props.budget.spent ?? 0} ${currency}`,
    `${props.budget.estimatedPrice ?? '-'} ${currency}`,
    props.budget.createAt ?? '-',
  ]

  const rows = [header, mainRow]

  const budgetItems = await Backend.getBudgetItems(props.budget.id)

  const items = Array.isArray(budgetItems) ? budgetItems : []
  if (items.length > 0) {
    rows.push([])
    const itemHeader = [['Nazwa', 'Opis', 'Kategoria', 'Cena szacunkowa', 'Czy zakończono']]
    rows.push(itemHeader)
    for (const it of items) {
      const row = [
        it.name ?? '-',
        it.description ?? '-',
        it.category ?? '-',
        it.estimatedPrice != null ? `${it.estimatedPrice} ${currency}` : '-',
        it.isCompleted ? 'Tak' : 'Nie',
      ]
      rows.push(row)
    }
  }

  const csvContent = rows.map((r) => r.join(';')).join('\n')
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.setAttribute('download', `budzet_${props.budget.name ?? 'export'}.csv`)
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}
</script>

<style scoped>
.export-modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background: rgba(0, 0, 0, 0.3);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}
.export-modal {
  background: var(--color-bg-white);
  border-radius: 12px;
  box-shadow: 0 4px 24px var(--shadow-light);
  padding: 2rem 2.5rem;
  min-width: 320px;
  max-width: 90vw;
  text-align: center;
}
.export-options {
  display: flex;
  gap: 1rem;
  justify-content: center;
  margin: 1.5rem 0;
}
</style>
