<template>
  <header
    class="sticky top-0 z-30 h-14 border-b border-base-300 bg-base-100/95 backdrop-blur-sm"
  >
    <div class="flex items-center h-full px-3 gap-2">
      <router-link to="/" class="text-title font-bold text-primary"
        >HomeTask</router-link
      >

      <nav
        v-if="auth.isLoggedIn"
        class="flex flex-1 justify-center overflow-x-auto px-4"
      >
        <div
          class="flex min-w-max items-center gap-1 rounded-full border border-base-300 bg-base-200/70 p-1"
        >
          <router-link
            v-for="item in navItems"
            :key="item.to"
            :to="item.to"
            active-class="border-primary/20 bg-base-100 text-primary shadow-sm"
            class="rounded-full border border-transparent px-4 py-1.5 text-sm font-medium text-base-content/75 transition-colors hover:border-base-300 hover:bg-base-100 hover:text-primary"
          >
            {{ item.label }}
          </router-link>
        </div>
      </nav>

      <div v-else class="flex-1" />

      <TemaPagina />

      <template v-if="auth.isLoggedIn">
        <div class="relative" ref="userMenuContainer">
          <button
            type="button"
            class="flex h-9 w-9 items-center justify-center rounded-full border border-base-300 bg-base-200 text-sm font-semibold text-base-content transition-colors hover:border-primary hover:text-primary"
            :aria-label="`Abrir menu do usuario ${auth.user?.nome ?? ''}`"
            @click="userMenuOpen = !userMenuOpen"
          >
            {{ inicialUsuario }}
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
              <span class="material-symbols-rounded text-base"
                >manage_accounts</span
              >
              Minha conta
            </router-link>
            <router-link
              v-if="auth.user?.tipo === 2 || auth.user?.tipo === 3"
              to="/perfil/agendamentos-prestador"
              class="flex items-center gap-3 w-full px-4 py-2.5 text-sm text-base-content hover:bg-base-200 transition-colors"
              @click="userMenuOpen = false"
            >
              <span class="material-symbols-rounded text-base"
                >work_history</span
              >
              Minha operacao
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
  </header>
</template>

<script lang="ts" setup>
import { computed, onMounted, onUnmounted, ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import TemaPagina from "@/shared/components/TemaPagina.vue";
import { logoutHandler, obterInicialNome } from "@/shared/utils";

const auth = useAuthStore();
const router = useRouter();

const userMenuOpen = ref(false);
const userMenuContainer = ref<HTMLElement | null>(null);
const inicialUsuario = computed(() => obterInicialNome(auth.user?.nome));
const navItems = computed(() => {
  const items = [{ label: "Buscar serviços", to: "/servicos/buscar" }];

  if (auth.user?.tipo === 1 || auth.user?.tipo === 3) {
    items.push({ label: "Agendamentos", to: "/perfil/agendamentos-cliente" });
    items.push({ label: "Solicitar serviço", to: "/servicos/novo-cliente" });
    return items;
  }

  if (auth.user?.tipo === 2) {
    items.push({ label: "Agendamentos", to: "/perfil/agendamentos-prestador" });
    items.push({ label: "Anunciar serviço", to: "/servicos/novo-prestador" });
  }

  return items;
});

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
  await logoutHandler(auth.logout, () => router.replace("/"), () => {
    userMenuOpen.value = false;
  });
}
</script>
