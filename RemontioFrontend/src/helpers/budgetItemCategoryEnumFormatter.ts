import { BudgetItemCategory } from '@/backend/BackendBase'

export function formatBudgetItemCategory(category: BudgetItemCategory): string {
  switch (category) {
    case BudgetItemCategory._0:
      return 'Materiały budowlane'
    case BudgetItemCategory._1:
      return 'Narzędzia'
    case BudgetItemCategory._2:
      return 'Usługi'
    case BudgetItemCategory._3:
      return 'Wyposażenie'
    case BudgetItemCategory._4:
      return 'Elektryka'
    case BudgetItemCategory._5:
      return 'Hydraulika'
    case BudgetItemCategory._6:
      return 'Wykończenie'
    case BudgetItemCategory._7:
      return 'Transport'
    case BudgetItemCategory._8:
      return 'Inne'
    default:
      return 'Nieznana kategoria'
  }
}

export function getAllBudgetItemCategories(): { value: BudgetItemCategory; label: string }[] {
  return [
    { value: BudgetItemCategory._0, label: 'Materiały budowlane' },
    { value: BudgetItemCategory._1, label: 'Narzędzia' },
    { value: BudgetItemCategory._2, label: 'Usługi' },
    { value: BudgetItemCategory._3, label: 'Wyposażenie' },
    { value: BudgetItemCategory._4, label: 'Elektryka' },
    { value: BudgetItemCategory._5, label: 'Hydraulika' },
    { value: BudgetItemCategory._6, label: 'Wykończenie' },
    { value: BudgetItemCategory._7, label: 'Transport' },
    { value: BudgetItemCategory._8, label: 'Inne' },
  ]
}
