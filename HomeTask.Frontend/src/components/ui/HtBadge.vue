<template>
  <span :class="['badge', variantClass]">
    <slot />
  </span>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{ variant?: 'default' | 'primary' | 'success' | 'error' | 'outline' }>(),
  { variant: 'default' },
)

const variantClass = computed(() => {
  switch (props.variant) {
    case 'primary': return 'badge-primary badge-soft'
    case 'success': return 'badge-success badge-soft'
    case 'error':   return 'badge-error badge-soft'
    case 'outline': return 'badge-outline'
    default:        return 'badge-neutral badge-soft'
  }
})
</script>
