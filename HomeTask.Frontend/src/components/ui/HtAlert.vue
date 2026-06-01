<template>
  <div
    v-if="show"
    role="alert"
    :class="['alert alert-soft rounded-[4px]', variantClass]"
  >
    <span class="material-symbols-rounded text-lg shrink-0">{{ icon }}</span>
    <div class="flex-1">
      <p v-if="title" class="font-semibold mb-0.5">{{ title }}</p>
      <slot>{{ message }}</slot>
    </div>
    <button
      v-if="dismissible"
      type="button"
      class="btn btn-ghost btn-xs btn-square shrink-0"
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
    case 'success': return 'alert-success'
    case 'info':    return 'alert-info'
    case 'warning': return 'alert-warning'
    default:        return 'alert-error'
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
