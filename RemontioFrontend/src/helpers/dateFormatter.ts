export function formatDate(date: Date | undefined): string {
  if (!date) return ''
  const dateObj = new Date(date)
  if (dateObj.getFullYear() === 1) return '' // .NET MinValue
  return dateObj.toLocaleDateString('pl-PL', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
  })
}

export function getTodayString(): string {
  const d = new Date()
  d.setHours(0, 0, 0, 0)
  const pad = (n: number) => n.toString().padStart(2, '0')
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
}

// Przyjmuje Date lub string 'YYYY-MM-DD' i ustawia godzinę na 12:00 lokalnie.
export function makeLocalMidday(input: Date | string | undefined): Date | undefined {
  if (!input) return undefined
  if (input instanceof Date) {
    const nd = new Date(input)
    nd.setHours(12, 0, 0, 0)
    return nd
  }
  if (typeof input === 'string') {
    const parts = input.split('-')
    if (parts.length !== 3) return undefined
    const [yStr, mStr, dStr] = parts
    const y = Number(yStr)
    const m = Number(mStr)
    const d = Number(dStr)
    if ([y, m, d].some((n) => isNaN(n))) return undefined
    return new Date(y, m - 1, d, 12, 0, 0, 0)
  }
  return undefined
}
