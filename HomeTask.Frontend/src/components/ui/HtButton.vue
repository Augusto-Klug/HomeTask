<template>
  <button
    v-bind="$attrs"
    :type="type"
    :disabled="disabled || loading"
    :class="['btn', variantClass, sizeClass]"
  >
    <span v-if="loading" :class="['loading loading-spinner', spinnerSizeClass]" />
    <slot />
  </button>
</template>

<script setup lang="ts">
import { computed } from 'vue'

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

const variantClass = computed(() => {
  switch (props.variant) {
    case 'primary': return 'btn-primary'
    case 'outline': return 'btn-outline'
    case 'ghost':   return 'btn-ghost'
    case 'danger':  return 'btn-error'
    default:        return 'btn-primary'
  }
})

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'btn-sm'
    case 'lg': return 'btn-lg'
    default:   return ''
  }
})

const spinnerSizeClass = computed(() => {
  switch (props.size) {
    case 'lg': return 'loading-md'
    default:   return 'loading-sm'
  }
})
</script>
