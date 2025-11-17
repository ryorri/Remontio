import { StatusEnum } from '@/backend/BackendBase'

export function getStatusLabel(status: StatusEnum | undefined): string {
  switch (status) {
    case StatusEnum._0:
      return 'W planach'
    case StatusEnum._1:
      return 'W trakcie'
    case StatusEnum._2:
      return 'Wstrzymany'
    case StatusEnum._3:
      return 'Zakończony'
    default:
      return 'Nieznany'
  }
}
