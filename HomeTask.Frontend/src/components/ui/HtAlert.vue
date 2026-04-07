<template>
  <div
    v-if="show"
    role="alert"
    class="flex items-start gap-3 rounded-lg border px-4 py-3 text-sm"
    :class="variantClass"
  >
    <span class="material-symbols-rounded text-lg shrink-0 mt-0.5">{{ icon }}</span>
    <div class="flex-1">
      <p v-if="title" class="font-semibold mb-0.5">{{ title }}</p>
      <slot>{{ message }}</slot>
    </div>
    <button
      v-if="dismissible"
      type="button"
      class="shrink-0 p-0.5 hover:opacity-70 transition-opacity"
      @click="show = false"
    >
      <span class="material-symbols-rounded text-sm">close</span>
    </button>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

const props = withDefaults(
  defineProps<{
    variant?: 'error' | 'success' | 'info' | 'warning'
    title?: string
    message?: string
    dismissible?: boolean
  }>(),
  { variant: 'error', dismissible: false },
)

const show = ref(true)

const variantClass = computed(() => {
  switch (props.variant) {
    case 'success': return 'border-success/30 bg-success-light text-success'
    case 'info':    return 'border-primary/30 bg-primary-light text-primary'
    case 'warning': return 'border-yellow-400/30 bg-yellow-50 text-yellow-800'
    default:        return 'border-error/30 bg-error-light text-error'
  }
})

const icon = computed(() => {
  switch (props.variant) {
    case 'success': return 'check_circle'
    case 'info':    return 'info'
    case 'warning': return 'warning'
    default:        return 'error'
  }
})

function reset() { show.value = true }

defineExpose({ reset })
</script>
