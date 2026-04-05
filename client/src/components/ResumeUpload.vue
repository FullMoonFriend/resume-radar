<!-- client/src/components/ResumeUpload.vue -->
<script setup lang="ts">
import { ref } from 'vue'
import { api } from '../api'

const emit = defineEmits<{ saved: [] }>()

const resumeText = ref('')
const loading = ref(false)
const error = ref('')
const extracting = ref(false)

async function loadExisting() {
  try {
    const data = await api.getResume()
    if (data.resumeText) resumeText.value = data.resumeText
  } catch { /* no resume yet */ }
}
loadExisting()

async function handleFile(file: File) {
  if (file.size > 5 * 1024 * 1024) {
    error.value = 'File must be under 5 MB.'
    return
  }
  if (!file.name.toLowerCase().endsWith('.pdf')) {
    error.value = 'Only PDF files are accepted.'
    return
  }

  extracting.value = true
  error.value = ''
  try {
    const result = await api.extractPdf(file)
    if (result.success && result.text) {
      resumeText.value = result.text
    } else {
      error.value = result.error || 'Could not extract text. Please paste your resume instead.'
    }
  } catch {
    error.value = 'Upload failed. Please try again.'
  } finally {
    extracting.value = false
  }
}

function onDrop(e: DragEvent) {
  const file = e.dataTransfer?.files[0]
  if (file) handleFile(file)
}

function onFileInput(e: Event) {
  const file = (e.target as HTMLInputElement).files?.[0]
  if (file) handleFile(file)
}

async function save() {
  if (!resumeText.value.trim()) return
  loading.value = true
  error.value = ''
  try {
    await api.saveResume(resumeText.value)
    emit('saved')
  } catch (e: any) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div>
    <!-- Drop zone -->
    <div
      class="border-2 border-dashed border-[#30363d] rounded-xl p-10 text-center mb-4 hover:border-[#4acaa8] transition-colors"
      @dragover.prevent
      @drop.prevent="onDrop"
    >
      <p class="text-[#768390] text-sm mb-3">
        {{ extracting ? 'Extracting text...' : 'Drag and drop a PDF here' }}
      </p>
      <label class="inline-block bg-[#161b22] text-[#cdd9e5] border border-[#30363d] px-3.5 py-1.5 rounded-md text-[13px] cursor-pointer hover:border-[#4acaa8]">
        Browse files
        <input type="file" accept=".pdf" class="hidden" @change="onFileInput" />
      </label>
    </div>

    <p class="text-center text-[#4d5566] text-[13px] mb-4">or</p>

    <!-- Text area -->
    <textarea
      v-model="resumeText"
      placeholder="Paste your resume text here..."
      class="w-full min-h-[200px] bg-[#1c2128] border border-[#30363d] rounded-lg p-4 text-[#cdd9e5] text-sm resize-y outline-none focus:border-[#4acaa8] placeholder-[#4d5566]"
    />

    <p v-if="error" class="text-[#f47067] text-sm mt-2">{{ error }}</p>

    <div class="mt-4 flex justify-end">
      <button
        :disabled="!resumeText.trim() || loading"
        class="bg-[#4acaa8] text-[#0d1117] font-semibold px-6 py-2.5 rounded-md disabled:opacity-40 hover:bg-[#3db896] transition-colors"
        @click="save"
      >
        {{ loading ? 'Saving...' : 'Save resume' }}
      </button>
    </div>
  </div>
</template>
