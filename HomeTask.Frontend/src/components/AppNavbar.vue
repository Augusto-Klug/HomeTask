<template>
  <nav class="flex items-center h-14 px-4 border-b border-border bg-card gap-4">
    <router-link to="/" class="flex items-center gap-2 text-primary font-bold">
      <span class="material-symbols-rounded">home</span>
      HomeTask
    </router-link>

    <router-link
      to="/servicos/buscar"
      class="flex items-center gap-1.5 text-sm text-muted hover:text-primary transition-colors"
    >
      <span class="material-symbols-rounded text-base">search</span>
      Buscar Serviços
    </router-link>

    <div class="flex-1" />

    <template v-if="auth.isLoggedIn">
      <span class="flex items-center gap-1.5 text-sm text-foreground">
        <span class="material-symbols-rounded text-base">person</span>
        {{ auth.user?.nome }}
      </span>
      <HtButton variant="outline" size="sm" @click="handleLogout">
        <span class="material-symbols-rounded text-base">logout</span>
        Sair
      </HtButton>
    </template>

    <template v-else>
      <router-link to="/login">
        <HtButton variant="outline" size="sm">Entrar</HtButton>
      </router-link>
      <router-link to="/cadastro">
        <HtButton size="sm">Cadastrar</HtButton>
      </router-link>
    </template>
  </nav>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/stores/auth'
import { useRouter } from 'vue-router'
import HtButton from '@/components/ui/HtButton.vue'

const auth = useAuthStore()
const router = useRouter()

async function handleLogout() {
  await auth.logout()
  router.push('/login')
}
</script>
