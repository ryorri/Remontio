import { StatusEnum } from '@/backend/BackendBase'

export function getStatusLabel(status: StatusEnum | undefined): string {
  switch (status) {
    case StatusEnum._0:
      return 'W planach'
    case StatusEnum._1:
      return 'W trakcie'
    case StatusEnum._2:
      return 'Zakończony'
    case StatusEnum._3:
      return 'Wstrzymany'
    default:
      return 'Nieznany'
  }
}

export interface StatusOption {
  value: StatusEnum
  label: string
  description: string
}

export function getExtendedStatusLabel(): StatusOption[] {
  return [
    {
      value: StatusEnum._0,
      label: getStatusLabel(StatusEnum._0),
      description: 'Projekt zaplanowany, prace jeszcze się nie rozpoczęły.',
    },
    {
      value: StatusEnum._1,
      label: getStatusLabel(StatusEnum._1),
      description: 'Prace trwają, zadania są aktywnie realizowane.',
    },
    {
      value: StatusEnum._2,
      label: getStatusLabel(StatusEnum._2),
      description: 'Projekt zakończony – wszystkie główne zadania ukończone.',
    },
    {
      value: StatusEnum._3,
      label: getStatusLabel(StatusEnum._3),
      description: 'Projekt tymczasowo wstrzymany – brak postępów.',
    },
  ]
}
