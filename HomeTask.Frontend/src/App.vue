<template lang="pug">
Cabecalho
main.responsive
  RouterView
Rodape
</template>

<script setup lang="ts">
import { onMounted } from "vue";
import { useAuthStore } from "@/stores/auth";
import Cabecalho from "./views/Cabecalho.vue";
import Rodape from "./views/telaInicial/Rodape.vue";

const auth = useAuthStore();

onMounted(() => auth.fetchMe());
</script>

<style>
main.responsive {
  padding-top: 1rem;
}
</style>
