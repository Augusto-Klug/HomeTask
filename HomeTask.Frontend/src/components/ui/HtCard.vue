<template>
  <div
    class="rounded-xl border border-border bg-card"
    :class="[padding ? 'p-5' : '', shadow ? 'shadow-sm' : '', $attrs.class]"
    v-bind="{ ...$attrs, class: undefined }"
  >
    <slot />
  </div>
</template>

<script setup lang="ts">
defineOptions({ inheritAttrs: false })

withDefaults(
  defineProps<{ padding?: boolean; shadow?: boolean }>(),
  { padding: true, shadow: false },
)
</script>
