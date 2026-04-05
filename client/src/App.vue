<script setup lang="ts">
import { onMounted } from 'vue'
import { useAuthStore } from './stores/auth'
import AppNav from './components/AppNav.vue'

const auth = useAuthStore()
onMounted(() => auth.fetchUser())
</script>

<template>
  <AppNav />
  <main class="pt-[60px]">
    <router-view />
  </main>
</template>
