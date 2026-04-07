<template>
  <button
    v-bind="$attrs"
    :type="type"
    :disabled="disabled || loading"
    :class="[baseClass, variantClass, sizeClass, 'disabled:opacity-50 disabled:cursor-not-allowed']"
  >
    <HtSpinner v-if="loading" :size="size === 'lg' ? 'md' : 'sm'" class="mr-2" />
    <slot />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import HtSpinner from './HtSpinner.vue'

const props = withDefaults(
  defineProps<{
    variant?: 'primary' | 'outline' | 'ghost' | 'danger'
    size?: 'sm' | 'md' | 'lg'
    loading?: boolean
    disabled?: boolean
    type?: 'button' | 'submit' | 'reset'
  }>(),
  {
    variant: 'primary',
    size: 'md',
    loading: false,
    disabled: false,
    type: 'button',
  },
)

const baseClass = 'inline-flex items-center justify-center font-medium rounded-lg transition-colors focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary cursor-pointer'

const variantClass = computed(() => {
  switch (props.variant) {
    case 'primary':
      return 'bg-primary text-primary-fg hover:bg-primary-hover'
    case 'outline':
      return 'border border-border bg-card text-foreground hover:bg-surface'
    case 'ghost':
      return 'text-foreground hover:bg-surface'
    case 'danger':
      return 'bg-error text-error-fg hover:opacity-90'
    default:
      return 'bg-primary text-primary-fg hover:bg-primary-hover'
  }
})

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'h-8 px-3 text-sm gap-1.5'
    case 'lg': return 'h-12 px-6 text-base gap-2'
    default:   return 'h-10 px-4 text-sm gap-2'
  }
})
</script>
