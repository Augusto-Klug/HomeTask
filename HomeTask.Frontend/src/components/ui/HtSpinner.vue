<template>
  <span
    class="inline-block rounded-full border-2 border-current border-t-transparent animate-spin"
    :class="sizeClass"
    role="status"
    aria-label="Carregando"
  />
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = withDefaults(
  defineProps<{ size?: 'sm' | 'md' | 'lg' }>(),
  { size: 'md' },
)

const sizeClass = computed(() => ({
  'w-3 h-3 border': props.size === 'sm',
  'w-4 h-4 border-2': props.size === 'md',
  'w-6 h-6 border-2': props.size === 'lg',
}))
</script>
