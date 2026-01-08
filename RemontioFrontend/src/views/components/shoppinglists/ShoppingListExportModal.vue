<template>
  <div class="export-modal-overlay" @click.self="onClose">
    <div class="export-modal">
      <h3>Eksport listy zakupowej</h3>
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

const props = defineProps<{ shoppingList: Record<string, any> & { items?: any[] } }>()
const emit = defineEmits(['close'])

const onClose = () => emit('close')

const handleExport = (format: 'pdf' | 'csv') => {
  if (format === 'pdf') exportToPDF()
  else exportToCSV()
  emit('close')
}

const exportToPDF = async () => {
  const doc = new jsPDF()
  registerFonts(doc)
  doc.setFont('Roboto', 'normal')

  let y = 10
  doc.setFontSize(14)
  doc.text(`Lista zakupowa: ${props.shoppingList.name ?? ''}`, 10, y)
  y += 8
  doc.setFontSize(11)
  doc.text(`Opis: ${props.shoppingList.description ?? ''}`, 10, y)
  y += 8
  doc.text(`Utworzono: ${props.shoppingList.createAt ?? '-'}`, 10, y)
  y += 12

  let items: any[] = []
  try {
    if (props.shoppingList.id) {
      items = await Backend.getItemListByListId(props.shoppingList.id)
    } else if (Array.isArray(props.shoppingList.items)) {
      items = props.shoppingList.items
    }
  } catch (e) {
    console.warn('Nie udało się pobrać pozycji listy', e)
    items = Array.isArray(props.shoppingList.items) ? props.shoppingList.items : []
  }

  console.log(items)
  if (items.length > 0) {
    const head = [['Nazwa', 'Ilość', 'Cena jedn.', 'Cena', 'Kupione']]
    const body = items.map((it: any) => [
      it.name ?? '-',
      it.quantity ?? '-',
      it.price,
      it.total ?? it.totalPrice ?? (it.quantity && it.price ? `${it.quantity * it.price}` : '-'),
      it.isBought ? 'Tak' : 'Nie',
    ])

    autoTable(doc, {
      startY: y,
      head,
      body,
      styles: { fontSize: 10, font: 'Roboto' },
      headStyles: { font: 'Roboto', fillColor: [41, 128, 185], textColor: 255 },
      theme: 'striped',
      margin: { left: 10, right: 10 },
    })
  } else {
    doc.setFontSize(11)
    doc.text('Brak pozycji na liście.', 10, y)
  }

  doc.save(`lista_zakupowa_${props.shoppingList.name ?? 'export'}.pdf`)
}

const exportToCSV = async () => {
  const header = ['Nazwa listy', 'Opis', 'Utworzono']
  const mainRow = [
    props.shoppingList.name ?? '',
    props.shoppingList.description ?? '',
    props.shoppingList.createAt ?? '-',
  ]
  const rows: string[][] = [header, mainRow]

  let items: any[] = []
  if (props.shoppingList.id) {
    try {
      items = await Backend.getItemListByListId(props.shoppingList.id)
    } catch (e) {
      console.warn('Nie udało się pobrać pozycji listy', e)
      items = Array.isArray(props.shoppingList.items) ? props.shoppingList.items : []
    }
  } else {
    items = Array.isArray(props.shoppingList.items) ? props.shoppingList.items : []
  }

  if (items.length > 0) {
    rows.push([])
    const itemHeader = ['Nazwa', 'Ilość', 'Cena jedn.', 'Cena', 'Kupione']
    rows.push(itemHeader)
    for (const it of items) {
      const row = [
        it.name ?? '-',
        it.quantity ?? it.qty ?? '-',
        it.price != null ? `${it.unitPrice}` : it.price != null ? `${it.price}` : '-',
        it.total ?? it.totalPrice ?? (it.quantity && it.price ? `${it.quantity * it.price}` : '-'),
        it.isBought ? 'Tak' : 'Nie',
      ]
      rows.push(row)
    }
  }

  const csvContent = rows.map((r) => r.join(';')).join('\n')
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' })
  const link = document.createElement('a')
  link.href = URL.createObjectURL(blob)
  link.setAttribute('download', `lista_zakupowa_${props.shoppingList.name ?? 'export'}.csv`)
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
