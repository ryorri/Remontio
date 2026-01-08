export function formatDate(date: Date | undefined): string {
  if (!date) return ''
  const dateObj = new Date(date)
  if (dateObj.getFullYear() === 1) return ''
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

export function formatDay(date: Date): string {
  return date.getDate().toString().padStart(2, '0')
}

export function formatMonth(date: Date): string {
  const months = [
    'Sty',
    'Lut',
    'Mar',
    'Kwi',
    'Maj',
    'Cze',
    'Lip',
    'Sie',
    'Wrz',
    'Paź',
    'Lis',
    'Gru',
  ]
  return months[date.getMonth()] || ''
}

export function toDateInputValue(d: unknown, startFallback?: Date | string | undefined): string {
  if (d === undefined || d === null) return ''
  let dateObj: Date
  if (typeof d === 'number') {
    if (startFallback) {
      const base = startFallback instanceof Date ? new Date(startFallback) : new Date(startFallback)
      base.setHours(0, 0, 0, 0)
      base.setDate(base.getDate() + d)
      dateObj = base
    } else {
      dateObj = new Date(d)
    }
  } else if (d instanceof Date) {
    dateObj = d
  } else if (typeof d === 'string') {
    dateObj = new Date(d)
  } else {
    return ''
  }
  if (isNaN(dateObj.getTime()) || dateObj.getFullYear() === 1) return ''
  const pad = (n: number) => n.toString().padStart(2, '0')
  return `${dateObj.getFullYear()}-${pad(dateObj.getMonth() + 1)}-${pad(dateObj.getDate())}`
}

export function getDateRangeError(
  start: Date | string | undefined,
  end: Date | string | undefined,
): string {
  if (!start || !end) return ''
  const startDate = typeof start === 'string' ? new Date(start) : start
  const endDate = typeof end === 'string' ? new Date(end) : end
  if (isNaN(startDate.getTime()) || isNaN(endDate.getTime())) return ''
  if (endDate < startDate) {
    return 'Data zakończenia nie może być wcześniejsza niż data rozpoczęcia.'
  }
  return ''
}

export function formatDateTime(date: Date | undefined): string {
  if (!date) return 'N/A'
  return new Date(date).toLocaleString('pl-PL', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

export function formatRelativeTime(date: Date): string {
  const now = new Date()
  const diff = Math.floor((now.getTime() - date.getTime()) / 1000)

  if (diff < 60) return 'przed chwilą'
  if (diff < 3600) return `${Math.floor(diff / 60)} min temu`
  if (diff < 86400) return `${Math.floor(diff / 3600)} godz. temu`

  return date.toLocaleString('pl-PL', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  })
}
