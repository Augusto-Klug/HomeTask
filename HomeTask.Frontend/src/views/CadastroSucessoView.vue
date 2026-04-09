<template>
  <div class="min-h-[calc(100vh-7rem)] flex items-center justify-center px-4">
    <div class="text-center flex flex-col items-center gap-4 max-w-sm">
      <span class="material-symbols-rounded text-6xl text-success">check_circle</span>
      <h1 class="text-2xl font-bold text-foreground">Cadastro realizado com sucesso!</h1>
      <p class="text-sm text-muted">Sua conta foi criada. Faça login para começar.</p>
      <router-link to="/login">
        <HtButton class="mt-2">
          <span class="material-symbols-rounded text-base">login</span>
          Fazer Login
        </HtButton>
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import HtButton from '@/components/ui/HtButton.vue'
</script>
