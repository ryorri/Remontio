export function formatCurrency(value?: number | null, currency = 'PLN', locale = 'pl-PL'): string {
  if (value === undefined || value === null) return '-'
  try {
    return new Intl.NumberFormat(locale, {
      style: 'currency',
      currency,
    }).format(value)
  } catch (e) {
    // Fallback: plain number with currency code
    return `${value.toFixed(2)} ${currency}`
  }
}

export default formatCurrency
