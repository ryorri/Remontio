import { Backend } from '@/main'

export function getUserData() {
  let rawData = localStorage.getItem('remontio_user_data')
  if (!rawData) return null

  let allUserData = JSON.parse(rawData)
  return allUserData || null
}

export function getCurrentUserId() {
  let rawData = localStorage.getItem('remontio_user_data')
  if (!rawData) return null
  let allUserData = JSON.parse(rawData)
  return allUserData ? allUserData.id : null
}

export async function getCurrentUserProjects() {
  let userId = getCurrentUserId()
  if (!userId) return null

  let projects = await Backend.getProjectListByUserId(userId)
  return projects
}
