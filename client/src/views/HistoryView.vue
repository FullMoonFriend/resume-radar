<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api, type AnalysisSummary } from '../api'
import AnalysisListItem from '../components/AnalysisListItem.vue'

const analyses = ref<AnalysisSummary[]>([])
const loading = ref(true)

onMounted(async () => {
  try {
    analyses.value = await api.getHistory()
  } catch { /* ignore */ }
  finally { loading.value = false }
})
</script>

<template>
  <div class="max-w-[600px] mx-auto px-6 py-9">
    <h2 class="text-xl font-semibold text-[#cdd9e5] mb-6">Past analyses</h2>

    <p v-if="loading" class="text-[#768390] text-center py-12">Loading...</p>

    <div v-else-if="analyses.length" class="border border-[#30363d] rounded-xl overflow-hidden">
      <AnalysisListItem v-for="a in analyses" :key="a.id" :analysis="a" />
    </div>

    <p v-else class="text-[#768390] text-center py-12">No analyses yet.</p>

    <div class="text-center mt-6">
      <router-link
        to="/analyze"
        class="inline-block bg-[#4acaa8] text-[#0d1117] font-semibold px-6 py-2.5 rounded-md no-underline hover:bg-[#3db896] transition-colors"
      >
        New analysis
      </router-link>
    </div>
  </div>
</template>
