<script setup lang="ts">
import type { AnalysisSummary } from '../api'

defineProps<{ analysis: AnalysisSummary }>()

function scoreColor(score: number): string {
  if (score >= 70) return 'bg-[rgba(74,202,168,0.12)] text-[#4acaa8]'
  if (score >= 50) return 'bg-[rgba(240,136,62,0.12)] text-[#f0883e]'
  return 'bg-[rgba(244,112,103,0.12)] text-[#f47067]'
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}
</script>

<template>
  <router-link
    :to="`/result/${analysis.id}`"
    class="flex justify-between items-center px-5 py-4 border-b border-[#30363d] last:border-b-0 hover:bg-[#161b22] transition-colors no-underline"
  >
    <div>
      <div class="text-[#cdd9e5] font-medium text-sm">{{ analysis.jobTitle || 'Untitled' }}</div>
      <div class="text-[#768390] text-xs">
        {{ analysis.company ? `${analysis.company} · ` : '' }}{{ formatDate(analysis.createdAt) }}
      </div>
    </div>
    <span class="px-3 py-1 rounded-full text-[13px] font-semibold" :class="scoreColor(analysis.matchScore)">
      {{ analysis.matchScore }}%
    </span>
  </router-link>
</template>
