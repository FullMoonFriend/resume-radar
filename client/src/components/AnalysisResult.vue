<script setup lang="ts">
import type { AnalysisDetail } from '../api'

defineProps<{ analysis: AnalysisDetail }>()

function scoreColor(score: number): string {
  if (score >= 70) return '#4acaa8'
  if (score >= 50) return '#f0883e'
  return '#f47067'
}

function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })
}
</script>

<template>
  <div>
    <div class="flex justify-between items-baseline mb-1">
      <h2 class="text-lg font-semibold text-[#cdd9e5]">{{ analysis.jobTitle || 'Untitled' }}</h2>
      <span class="font-mono text-xs text-[#4d5566]">{{ formatDate(analysis.createdAt) }}</span>
    </div>
    <p v-if="analysis.company" class="text-[#768390] text-sm mb-7">{{ analysis.company }}</p>

    <div class="bg-[#161b22] border border-[#30363d] rounded-xl p-6 mb-5 text-center">
      <div class="text-5xl font-bold" :style="{ color: scoreColor(analysis.matchScore) }">
        {{ analysis.matchScore }}%
      </div>
      <div class="text-[#768390] text-[13px] mt-1">Match Score</div>
    </div>

    <div v-if="analysis.strengthMatches.length" class="bg-[#161b22] border border-[#30363d] rounded-xl p-5 mb-4">
      <h3 class="text-[#4acaa8] font-semibold text-sm mb-3">Strong matches</h3>
      <div class="space-y-2">
        <div v-for="match in analysis.strengthMatches" :key="match" class="flex gap-2 text-[13px] text-[#cdd9e5]">
          <span class="text-[#4acaa8] shrink-0">&#10003;</span>
          <span>{{ match }}</span>
        </div>
      </div>
    </div>

    <div v-if="analysis.skillGaps.length" class="bg-[#161b22] border border-[#30363d] rounded-xl p-5 mb-4">
      <h3 class="text-[#f47067] font-semibold text-sm mb-3">Gaps to address</h3>
      <div class="space-y-2">
        <div v-for="gap in analysis.skillGaps" :key="gap.skill" class="flex gap-2 text-[13px] text-[#cdd9e5]">
          <span class="text-[#f47067] shrink-0">&#9679;</span>
          <span><strong>{{ gap.skill }}</strong> - {{ gap.note }}</span>
        </div>
      </div>
    </div>

    <div v-if="analysis.tailoredBullets.length" class="bg-[#161b22] border border-[#30363d] rounded-xl p-5">
      <h3 class="text-[#f0883e] font-semibold text-sm mb-3">Tailored talking points</h3>
      <div class="space-y-2">
        <div
          v-for="bullet in analysis.tailoredBullets"
          :key="bullet"
          class="bg-[#1c2128] rounded-md px-3 py-2 text-[13px] text-[#cdd9e5] leading-relaxed"
        >
          "{{ bullet }}"
        </div>
      </div>
    </div>
  </div>
</template>
