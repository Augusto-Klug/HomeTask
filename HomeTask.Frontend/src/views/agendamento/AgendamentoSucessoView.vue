<template>
  <div class="min-h-[calc(100vh-7rem)] flex items-center justify-center px-4">
    <div class="text-center flex flex-col items-center gap-4 max-w-sm">
      <span class="material-symbols-rounded text-6xl text-success">check_circle</span>
      <h1 class="text-2xl font-bold text-foreground">Agendamento realizado!</h1>
      <p class="text-sm text-muted">
        Seu agendamento foi criado com sucesso. O prestador entrará em contato em breve.
      </p>
      <div class="flex flex-wrap justify-center gap-3 mt-2">
        <router-link to="/servicos/buscar">
          <HtButton variant="outline">
            <span class="material-symbols-rounded text-base">search</span>
            Buscar mais serviços
          </HtButton>
        </router-link>
        <router-link to="/">
          <HtButton>
            <span class="material-symbols-rounded text-base">home</span>
            Início
          </HtButton>
        </router-link>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import HtButton from '@/components/ui/HtButton.vue'
</script>
