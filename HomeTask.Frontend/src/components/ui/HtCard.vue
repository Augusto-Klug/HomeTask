<template>
  <div
    class="card card-border bg-base-100"
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
