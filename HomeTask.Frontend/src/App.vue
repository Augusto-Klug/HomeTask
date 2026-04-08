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
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Cabecalho from './views/Cabecalho.vue'
import Rodape from './views/telaInicial/Rodape.vue'

const auth = useAuthStore()

onMounted(() => auth.fetchMe())
</script>
