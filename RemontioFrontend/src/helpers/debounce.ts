export function debounce<T extends (...args: any[]) => any>(fn: T, wait: number) {
  let timeout: number | undefined
  return (...args: Parameters<T>) => {
    if (timeout !== undefined) {
      clearTimeout(timeout)
    }
    timeout = window.setTimeout(() => {
      fn(...args)
    }, wait)
  }
}
