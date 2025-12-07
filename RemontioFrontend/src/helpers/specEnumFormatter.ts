import { SpecEnum } from '@/backend/BackendBase'

export function getSpecLabel(spec: SpecEnum | undefined): string {
  switch (spec) {
    case SpecEnum._0:
      return 'Ogólnobudowlany'
    case SpecEnum._1:
      return 'Wykończeniowiec'
    case SpecEnum._2:
      return 'Elektryk'
    case SpecEnum._3:
      return 'Hydraulik'
    case SpecEnum._4:
      return 'Montażysta okien'
    case SpecEnum._5:
      return 'Stolarz'
    case SpecEnum._6:
      return 'Złota rączka'
    case SpecEnum._7:
      return 'Inne'
    default:
      return 'Nieznany'
  }
}

export interface SpecOption {
  value: SpecEnum
  label: string
}

export function getAllSpecOptions(): SpecOption[] {
  return [
    { value: SpecEnum._0, label: getSpecLabel(SpecEnum._0) },
    { value: SpecEnum._1, label: getSpecLabel(SpecEnum._1) },
    { value: SpecEnum._2, label: getSpecLabel(SpecEnum._2) },
    { value: SpecEnum._3, label: getSpecLabel(SpecEnum._3) },
    { value: SpecEnum._4, label: getSpecLabel(SpecEnum._4) },
    { value: SpecEnum._5, label: getSpecLabel(SpecEnum._5) },
    { value: SpecEnum._6, label: getSpecLabel(SpecEnum._6) },
    { value: SpecEnum._7, label: getSpecLabel(SpecEnum._7) },
  ]
}
