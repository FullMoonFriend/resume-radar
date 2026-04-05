<!-- client/src/views/ResumeView.vue -->
<script setup lang="ts">
import { useRouter } from 'vue-router'
import ResumeUpload from '../components/ResumeUpload.vue'
import { useAuthStore } from '../stores/auth'

const router = useRouter()
const auth = useAuthStore()

function onSaved() {
  auth.fetchUser()
  router.push('/analyze')
}
</script>

<template>
  <div class="max-w-[560px] mx-auto px-6 py-12">
    <h2 class="text-xl font-semibold text-[#cdd9e5] mb-2">
      {{ auth.user?.hasResume ? 'Update your resume' : 'First, add your resume' }}
    </h2>
    <p class="text-[#768390] text-sm mb-6 leading-relaxed">
      Upload a PDF or paste your resume text. You can edit it anytime.
    </p>
    <ResumeUpload @saved="onSaved" />
  </div>
</template>
