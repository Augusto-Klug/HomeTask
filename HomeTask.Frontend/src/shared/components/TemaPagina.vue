<template>
  <button
    type="button"
    :title="'Tema: ' + modeLabel"
    class="btn btn-ghost btn-sm btn-square"
    @click="cycleMode"
  >
    <span class="material-symbols-rounded text-xl">{{ modeIcon }}</span>
  </button>
</template>

<script lang="ts" setup>
import { useTheme } from '@/composables/useTheme'

const { modeIcon, modeLabel, cycleMode } = useTheme()
</script>
