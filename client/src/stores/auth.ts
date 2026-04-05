import { defineStore } from 'pinia'
import { ref } from 'vue'
import { api, type UserInfo } from '../api'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<UserInfo | null>(null)
  const loading = ref(true)

  async function fetchUser() {
    loading.value = true
    try {
      user.value = await api.getMe()
    } catch {
      user.value = null
    } finally {
      loading.value = false
    }
  }

  async function logout() {
    await api.logout()
    user.value = null
  }

  return { user, loading, fetchUser, logout }
})
