<!-- client/src/views/LandingView.vue -->
<script setup lang="ts">
import { useAuthStore } from '../stores/auth'
import { useRouter } from 'vue-router'
import { watch } from 'vue'

const auth = useAuthStore()
const router = useRouter()

// Redirect logged-in users to analyze page
watch(() => auth.user, (user) => {
  if (user) {
    router.replace(user.hasResume ? '/analyze' : '/resume')
  }
}, { immediate: true })
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-[#0d1117]">
    <div class="text-center px-6">
      <h1 class="text-[28px] font-bold text-[#cdd9e5] mb-3 leading-tight">
        Paste a job posting.<br />See how you match.
      </h1>
      <p class="text-[#768390] text-[15px] max-w-[440px] mx-auto mb-8 leading-relaxed">
        Upload your resume and any job description. AI analyzes the match,
        identifies skill gaps, and drafts tailored talking points.
      </p>
      <a
        href="/api/auth/login"
        class="inline-block bg-[#4acaa8] text-[#0d1117] font-semibold text-[15px] px-7 py-3 rounded-lg no-underline hover:bg-[#3db896] transition-colors"
      >
        Get started - Sign in with GitHub
      </a>
      <p class="text-[#4d5566] text-xs font-mono mt-4">Free - 10 analyses per day</p>
    </div>
  </div>
</template>
