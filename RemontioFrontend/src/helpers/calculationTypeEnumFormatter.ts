import { CalculationsTypeEnum } from '@/backend/BackendBase'

export function getSpecLabel(spec: CalculationsTypeEnum | undefined): string {
  switch (spec) {
    case CalculationsTypeEnum._0:
      return 'Ściana'
    case CalculationsTypeEnum._1:
      return 'Podłoga'
    case CalculationsTypeEnum._2:
      return 'Naprawa ściany'
    case CalculationsTypeEnum._3:
      return 'Farba'
    case CalculationsTypeEnum._4:
      return 'Inne'
    default:
      return 'Nieznany'
  }
}

export interface SpecOption {
  value: CalculationsTypeEnum
  label: string
}

export function getAllTypesOptions(): SpecOption[] {
  return [
    { value: CalculationsTypeEnum._0, label: getSpecLabel(CalculationsTypeEnum._0) },
    { value: CalculationsTypeEnum._1, label: getSpecLabel(CalculationsTypeEnum._1) },
    { value: CalculationsTypeEnum._2, label: getSpecLabel(CalculationsTypeEnum._2) },
    { value: CalculationsTypeEnum._3, label: getSpecLabel(CalculationsTypeEnum._3) },
    { value: CalculationsTypeEnum._4, label: getSpecLabel(CalculationsTypeEnum._4) },
  ]
}
