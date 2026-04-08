<template>
  <span
    :class="['loading loading-spinner', sizeClass]"
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

const sizeClass = computed(() => {
  switch (props.size) {
    case 'sm': return 'loading-sm'
    case 'lg': return 'loading-lg'
    default:   return 'loading-md'
  }
})
</script>
