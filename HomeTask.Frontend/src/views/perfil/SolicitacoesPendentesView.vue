<template>
  <div class="container mx-auto px-4 py-8 max-w-4xl">
    <div class="flex flex-wrap items-center justify-between gap-3 mb-6">
      <div>
        <h1 class="text-2xl font-bold">Solicitacoes pendentes</h1>
        <p class="text-sm text-base-content/60">Pedidos aguardando sua resposta como prestador.</p>
      </div>
      <router-link to="/perfil/agendamentos-prestador">
        <HtButton variant="outline">Ver minha operacao</HtButton>
      </router-link>
    </div>
  </div>
</template>

<script setup lang="ts">
import HtButton from "@/components/ui/HtButton.vue";
</script>
