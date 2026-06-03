<template>
  <span :class="['badge', variantClass]">
    <slot />
  </span>
</template>

<script setup lang="ts">
import { computed } from "vue";

export enum HtBadgeVariant {
  Default = "default",
  Primary = "primary",
  Success = "success",
  Error = "error",
  Outline = "outline",
}

const props = withDefaults(defineProps<{ variant?: HtBadgeVariant }>(), {
  variant: HtBadgeVariant.Default,
});

const variantClass = computed(() => {
  switch (props.variant) {
    case HtBadgeVariant.Primary:
      return "badge-primary badge-soft";
    case HtBadgeVariant.Success:
      return "badge-success badge-soft";
    case HtBadgeVariant.Error:
      return "badge-error badge-soft";
    case HtBadgeVariant.Outline:
      return "badge-outline";
    default:
      return "badge-neutral badge-soft";
  }
});
</script>
