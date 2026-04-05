<!-- client/src/views/AnalyzeView.vue -->
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../api'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()
const router = useRouter()

const jobTitle = ref('')
const company = ref('')
const jobPostingText = ref('')
const loading = ref(false)
const error = ref('')
const remaining = ref<number | null>(null)
const limit = ref(10)

onMounted(async () => {
  if (!auth.user?.hasResume) {
    router.replace('/resume')
    return
  }
  try {
    const info = await api.getRemaining()
    remaining.value = info.remaining
    limit.value = info.limit
  } catch { /* ignore */ }
})

async function analyze() {
  if (!jobPostingText.value.trim()) return
  loading.value = true
  error.value = ''
  try {
    const result = await api.analyze(jobPostingText.value, jobTitle.value || undefined, company.value || undefined)
    router.push(`/result/${result.id}`)
  } catch (e: any) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="max-w-[560px] mx-auto px-6 py-12">
    <h2 class="text-xl font-semibold text-[#cdd9e5] mb-1">Analyze a job posting</h2>
    <p class="text-[#768390] text-[13px] mb-6">
      Using saved resume ·
      <router-link to="/resume" class="text-[#768390] underline hover:text-[#4acaa8]">edit</router-link>
    </p>

    <label class="block text-[#768390] text-[13px] mb-1.5">Job title (optional)</label>
    <input
      v-model="jobTitle"
      placeholder="e.g. Senior .NET Developer"
      class="w-full bg-[#1c2128] border border-[#30363d] rounded-md px-3.5 py-2.5 text-[#cdd9e5] text-sm outline-none focus:border-[#4acaa8] placeholder-[#4d5566] mb-4"
    />

    <label class="block text-[#768390] text-[13px] mb-1.5">Company (optional)</label>
    <input
      v-model="company"
      placeholder="e.g. Acme Corp"
      class="w-full bg-[#1c2128] border border-[#30363d] rounded-md px-3.5 py-2.5 text-[#cdd9e5] text-sm outline-none focus:border-[#4acaa8] placeholder-[#4d5566] mb-4"
    />

    <label class="block text-[#768390] text-[13px] mb-1.5">Job posting</label>
    <textarea
      v-model="jobPostingText"
      placeholder="Paste the full job description here..."
      class="w-full min-h-[160px] bg-[#1c2128] border border-[#30363d] rounded-lg p-4 text-[#cdd9e5] text-sm resize-y outline-none focus:border-[#4acaa8] placeholder-[#4d5566] mb-5"
    />

    <p v-if="error" class="text-[#f47067] text-sm mb-3">{{ error }}</p>

    <div v-if="loading" class="flex flex-col items-center gap-3 py-6">
      <svg class="animate-spin h-8 w-8 text-[#4acaa8]" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" />
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
      </svg>
      <p class="text-[#768390] text-sm">Analyzing your resume against this posting...</p>
      <p class="text-[#4d5566] text-xs">This usually takes 10-15 seconds</p>
    </div>

    <button
      v-else
      :disabled="!jobPostingText.trim()"
      class="w-full bg-[#4acaa8] text-[#0d1117] font-semibold text-[15px] py-3 rounded-md disabled:opacity-40 hover:bg-[#3db896] transition-colors"
      @click="analyze"
    >
      Analyze match
    </button>

    <p v-if="remaining !== null" class="text-[#4d5566] text-xs font-mono text-center mt-2.5">
      {{ remaining }} of {{ limit }} analyses remaining today
    </p>
  </div>
</template>
