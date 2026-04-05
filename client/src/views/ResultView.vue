<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { api, type AnalysisDetail } from '../api'
import AnalysisResult from '../components/AnalysisResult.vue'

const props = defineProps<{ id: string }>()

const analysis = ref<AnalysisDetail | null>(null)
const error = ref('')

onMounted(async () => {
  try {
    analysis.value = await api.getAnalysis(Number(props.id))
  } catch (e: any) {
    error.value = e.message
  }
})
</script>

<template>
  <div class="max-w-[600px] mx-auto px-6 py-9">
    <p v-if="error" class="text-[#f47067] text-sm">{{ error }}</p>
    <AnalysisResult v-else-if="analysis" :analysis="analysis" />
    <p v-else class="text-[#768390] text-center py-12">Loading...</p>
  </div>
</template>
