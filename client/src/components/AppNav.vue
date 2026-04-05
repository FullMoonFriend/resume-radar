<!-- client/src/components/AppNav.vue -->
<script setup lang="ts">
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
</script>

<template>
  <nav class="fixed top-0 left-0 right-0 h-[60px] bg-[#0d1117]/92 backdrop-blur-xl border-b border-[#30363d] z-50 flex items-center">
    <div class="max-w-[860px] w-full mx-auto px-6 flex items-center justify-between">
      <router-link to="/" class="font-mono font-bold text-[15px] text-[#4acaa8] no-underline">
        ResumeRadar
      </router-link>

      <div v-if="auth.user" class="flex items-center gap-4">
        <router-link to="/history" class="text-[13px] text-[#768390] hover:text-[#cdd9e5] no-underline">
          History
        </router-link>
        <router-link to="/resume" class="text-[13px] text-[#768390] hover:text-[#cdd9e5] no-underline">
          Resume
        </router-link>
        <img
          v-if="auth.user.avatarUrl"
          :src="auth.user.avatarUrl"
          :alt="auth.user.username"
          class="w-7 h-7 rounded-full border border-[#30363d] cursor-pointer"
          @click="auth.logout().then(() => $router.push('/'))"
        />
        <div v-else class="w-7 h-7 rounded-full bg-[#30363d]" />
      </div>

      <a v-else href="/api/auth/login" class="bg-[#161b22] text-[#cdd9e5] border border-[#30363d] px-3.5 py-1.5 rounded-md text-[13px] no-underline hover:border-[#4acaa8]">
        Sign in with GitHub
      </a>
    </div>
  </nav>
</template>
