<template>
  <HtSidenav v-model:open="open">
    <template #title>HomeTask</template>

    <nav class="flex flex-col gap-1 px-3">
      <router-link
        to="/"
        class="ht-sidenav-link flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm text-foreground hover:bg-surface transition-colors"
        @click="open = false"
      >
        <span class="material-symbols-rounded text-xl">home</span>
        Início
      </router-link>

      <router-link
        to="/servicos/buscar"
        class="ht-sidenav-link flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm text-foreground hover:bg-surface transition-colors"
        @click="open = false"
      >
        <span class="material-symbols-rounded text-xl">search</span>
        Buscar Serviços
      </router-link>
    </nav>

    <template #footer>
      <template v-if="auth.isLoggedIn">
        <div class="flex items-center gap-3 px-1 mb-3">
          <span class="material-symbols-rounded text-xl text-muted">account_circle</span>
          <span class="text-sm font-medium text-foreground truncate">{{ auth.user?.nome }}</span>
        </div>
        <HtButton variant="outline" size="sm" class="w-full" @click="handleLogout">
          <span class="material-symbols-rounded text-base">logout</span>
          Sair
        </HtButton>
      </template>

      <template v-else>
        <div class="flex flex-col gap-2">
          <router-link to="/login" @click="open = false">
            <HtButton variant="outline" size="sm" class="w-full">
              <span class="material-symbols-rounded text-base">login</span>
              Entrar
            </HtButton>
          </router-link>
          <router-link to="/cadastro" @click="open = false">
            <HtButton size="sm" class="w-full">
              <span class="material-symbols-rounded text-base">person_add</span>
              Cadastrar
            </HtButton>
          </router-link>
        </div>
      </template>
    </template>
  </HtSidenav>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import HtSidenav from '@/components/ui/HtSidenav.vue'
import HtButton from '@/components/ui/HtButton.vue'

const open = defineModel<boolean>('open', { default: false })

const auth = useAuthStore()
const router = useRouter()

async function handleLogout() {
  open.value = false
  await auth.logout()
  router.push('/login')
}
</script>
