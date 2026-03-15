<template lang="pug">
button.transparent.circle(@click="cycleMode" :title="'Tema: ' + modeLabel")
  i {{ modeIcon }}
</template>
<script lang="ts" setup>
import { useTheme } from "@/composables/useTheme";

const { modeIcon, modeLabel, cycleMode } = useTheme();
</script>
