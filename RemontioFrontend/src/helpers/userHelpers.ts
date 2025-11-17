export function getUserData() {
  let rawData = localStorage.getItem('remontio_user_data')
  if (!rawData) return null

  let allUserData = JSON.parse(rawData)
  return allUserData || null
}

export function getUserInitials(userId: string) {
  let rawData = localStorage.getItem('remontio_user_data')
  if (!rawData) return null
  let allUserData = JSON.parse(rawData)
}
