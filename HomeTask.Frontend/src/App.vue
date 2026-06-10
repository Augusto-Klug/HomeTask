<template>
  <div class="flex flex-col min-h-screen bg-base-100 text-base-content">
    <Cabecalho />
    <main class="flex-1">
      <RouterView />
    </main>
    <Rodape />
  </div>
</template>

<script setup lang="ts">
import Cabecalho from './views/Cabecalho.vue'
import Rodape from './views/telaInicial/Rodape.vue'
</script>
