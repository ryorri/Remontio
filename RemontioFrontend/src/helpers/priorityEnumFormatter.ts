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
