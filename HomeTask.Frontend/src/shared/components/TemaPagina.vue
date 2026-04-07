<template>
  <button
    type="button"
    :title="'Tema: ' + modeLabel"
    class="p-2 rounded-lg text-muted hover:text-foreground hover:bg-surface transition-colors"
    @click="cycleMode"
  >
    <span class="material-symbols-rounded text-xl">{{ modeIcon }}</span>
  </button>
</template>

<script lang="ts" setup>
import { useTheme } from '@/composables/useTheme'

const { modeIcon, modeLabel, cycleMode } = useTheme()
</script>
