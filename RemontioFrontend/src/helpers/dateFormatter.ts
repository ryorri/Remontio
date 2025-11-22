export function formatDate(date: Date | undefined): string {
  if (!date) return ''
  const dateObj = new Date(date)
  // Check if date is the default "0001-01-01T00:00:00"
  if (dateObj.getFullYear() === 1) return ''
  return dateObj.toLocaleDateString('pl-PL', {
    year: 'numeric',
    month: 'short',
    day: '2-digit',
  })
}
