<template>
  <span
    class="inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium"
    :class="variantClass"
  >
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
    case 'primary': return 'bg-primary-light text-primary'
    case 'success': return 'bg-success-light text-success'
    case 'error':   return 'bg-error-light text-error'
    case 'outline': return 'border border-border text-foreground'
    default:        return 'bg-surface text-muted'
  }
})
</script>
