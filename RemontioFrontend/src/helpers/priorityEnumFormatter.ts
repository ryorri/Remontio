import { PriorityEnum } from '@/backend/BackendBase'

export function getPriorityLabel(priority: PriorityEnum | undefined): string {
  switch (priority) {
    case PriorityEnum._0:
      return 'Niski'
    case PriorityEnum._1:
      return 'Średni'
    case PriorityEnum._2:
      return 'Wysoki'
    case PriorityEnum._3:
      return 'Krytyczny'
    default:
      return 'Nieznany'
  }
}

export function getPriorityIcon(priority: PriorityEnum | undefined): string {
  switch (priority) {
    case PriorityEnum._0:
      return 'fa-arrow-down'
    case PriorityEnum._1:
      return 'fa-minus'
    case PriorityEnum._2:
      return 'fa-arrow-up'
    case PriorityEnum._3:
      return 'fa-exclamation'
    default:
      return ''
  }
}

export function getPriorityColor(priority: PriorityEnum | undefined): string {
  switch (priority) {
    case PriorityEnum._0:
      return '#10b981'
    case PriorityEnum._1:
      return '#3b82f6'
    case PriorityEnum._2:
      return '#f59e0b'
    case PriorityEnum._3:
      return '#ef4444'
    default:
      return '#6b7280'
  }
}

export interface PriorityOption {
  value: PriorityEnum
  label: string
  description: string
}

export function getExtendedPriorityLabel(): PriorityOption[] {
  const priorityOptions: PriorityOption[] = [
    {
      value: PriorityEnum._0,
      label: getPriorityLabel(PriorityEnum._0),
      description: 'Niski priorytet – może poczekać.',
    },
    {
      value: PriorityEnum._1,
      label: getPriorityLabel(PriorityEnum._1),
      description: 'Średni priorytet – standardowe zadanie.',
    },
    {
      value: PriorityEnum._2,
      label: getPriorityLabel(PriorityEnum._2),
      description: 'Wysoki priorytet – wymaga szybkiej realizacji.',
    },
    {
      value: PriorityEnum._3,
      label: getPriorityLabel(PriorityEnum._3),
      description: 'Krytyczny priorytet – pilne i niezbędne.',
    },
  ]
  return priorityOptions
}
