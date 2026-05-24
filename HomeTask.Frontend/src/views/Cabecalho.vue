<template>
  <header class="sticky top-0 z-30 h-14 border-b border-base-300 bg-base-100/95 backdrop-blur-sm">
    <div class="flex items-center h-full px-3 gap-2">
      <button
        type="button"
        class="btn btn-ghost btn-sm btn-square"
        @click="sidebarOpen = true"
      >
        <span class="material-symbols-rounded text-xl">menu</span>
      </button>

      <router-link to="/" class="text-title font-bold text-primary">
        HomeTask
      </router-link>

      <div class="flex-1" />

      <router-link
        to="/servicos/buscar"
        class="btn btn-ghost btn-sm btn-square"
        title="Buscar serviços"
      >
        <span class="material-symbols-rounded text-xl">search</span>
      </router-link>

      <TemaPagina />

      <template v-if="auth.isLoggedIn">
        <div class="relative" ref="userMenuContainer">
          <button
            type="button"
            class="btn btn-ghost btn-sm btn-square"
            @click="userMenuOpen = !userMenuOpen"
          >
            <span class="material-symbols-rounded text-xl">account_circle</span>
          </button>

          <div
            v-if="userMenuOpen"
            class="absolute right-0 top-12 w-52 rounded-box border border-base-300 bg-base-100 shadow-lg z-50 py-1"
          >
            <div class="px-4 py-2.5 border-b border-base-300">
              <p class="text-xs text-base-content/50">Logado como</p>
              <p class="text-sm font-medium text-base-content truncate">
                {{ auth.user?.nome }}
              </p>
            </div>

            <router-link
              to="/perfil/minha-conta"
              class="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-base-content hover:bg-base-200 transition-colors"
              @click="userMenuOpen = false"
            >
              <span class="material-symbols-rounded text-base">manage_accounts</span>
              Minha conta
            </router-link>

            <router-link
              v-if="auth.user?.tipo === 1 || auth.user?.tipo === 3"
              to="/perfil/agendamentos"
              class="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-base-content hover:bg-base-200 transition-colors"
              @click="userMenuOpen = false"
            >
              <span class="material-symbols-rounded text-base">calendar_month</span>
              Agendamentos
            </router-link>

            <router-link
              v-if="auth.user?.tipo === 2 || auth.user?.tipo === 3"
              to="/perfil/operacao"
              class="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-base-content hover:bg-base-200 transition-colors"
              @click="userMenuOpen = false"
            >
              <span class="material-symbols-rounded text-base">checklist</span>
              Minha operação
            </router-link>

            <div class="border-t border-base-300 my-1" />

            <button
              type="button"
              class="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-base-content hover:bg-base-200 transition-colors"
              @click="handleLogout"
            >
              <span class="material-symbols-rounded text-base">logout</span>
              Sair
            </button>
          </div>
        </div>
      </template>

      <template v-else>
        <router-link
          to="/login"
          class="btn btn-ghost btn-sm btn-square"
          title="Entrar"
        >
          <span class="material-symbols-rounded text-xl">person</span>
        </router-link>
      </template>
    </div>

    <SidebarTelaInicial v-model:open="sidebarOpen" />
  </header>
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import TemaPagina from "@/shared/components/TemaPagina.vue";
import SidebarTelaInicial from "./telaInicial/SidebarTelaInicial.vue";

const auth = useAuthStore();
const router = useRouter();

const sidebarOpen = ref(false);
const userMenuOpen = ref(false);
const userMenuContainer = ref<HTMLElement | null>(null);

function handleClickOutside(e: MouseEvent) {
  if (
    userMenuContainer.value &&
    !userMenuContainer.value.contains(e.target as Node)
  ) {
    userMenuOpen.value = false;
  }
}

onMounted(() => document.addEventListener("click", handleClickOutside));
onUnmounted(() => document.removeEventListener("click", handleClickOutside));

async function handleLogout() {
  userMenuOpen.value = false;
  await auth.logout();
  router.push("/login");
}
</script>
