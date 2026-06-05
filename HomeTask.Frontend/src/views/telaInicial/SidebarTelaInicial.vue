<template>
  <HtSidenav v-model:open="open">
    <template #title>HomeTask</template>

    <ul class="menu menu-sm px-2">
      <li>
        <router-link to="/" class="ht-sidenav-link" @click="open = false">
          <span class="material-symbols-rounded text-xl">home</span>
          Inicio
        </router-link>
      </li>

      <li>
        <router-link to="/servicos/buscar" class="ht-sidenav-link" @click="open = false">
          <span class="material-symbols-rounded text-xl">search</span>
          Buscar Serviços
        </router-link>
      </li>

      <template v-if="auth.isLoggedIn">
        <li v-if="auth.user?.tipo === 1 || auth.user?.tipo === 3">
          <router-link to="/perfil/agendamentos-cliente" class="ht-sidenav-link" @click="open = false">
            <span class="material-symbols-rounded text-xl">calendar_month</span>
            Agendamentos
          </router-link>
        </li>

        <li v-if="auth.user?.tipo === 2 || auth.user?.tipo === 3">
          <router-link to="/perfil/agendamentos-prestador" class="ht-sidenav-link" @click="open = false">
            <span class="material-symbols-rounded text-xl">work_history</span>
            Minha Operação
          </router-link>
        </li>

        <li v-if="auth.user?.tipo === 1 || auth.user?.tipo === 3">
          <router-link to="/servicos/novo-cliente" class="ht-sidenav-link" @click="open = false">
            <span class="material-symbols-rounded text-xl">campaign</span>
            Solicitar serviço
          </router-link>
        </li>

        <li v-if="auth.user?.tipo === 2 || auth.user?.tipo === 3">
          <router-link to="/servicos/novo-prestador" class="ht-sidenav-link" @click="open = false">
            <span class="material-symbols-rounded text-xl">work</span>
            Anunciar serviço
          </router-link>
        </li>
      </template>
    </ul>

    <template #footer>
      <template v-if="auth.isLoggedIn">
        <div class="flex items-center gap-3 mb-3">
          <span class="material-symbols-rounded text-xl text-base-content/50">account_circle</span>
          <span class="text-sm font-medium text-base-content truncate">{{ auth.user?.nome }}</span>
        </div>
        <button class="btn btn-outline btn-sm w-full" @click="handleLogout">
          <span class="material-symbols-rounded text-base">logout</span>
          Sair
        </button>
      </template>

      <template v-else>
        <div class="flex flex-col gap-2">
          <router-link to="/login" @click="open = false">
            <button class="btn btn-outline btn-sm w-full">
              <span class="material-symbols-rounded text-base">login</span>
              Entrar
            </button>
          </router-link>
          <router-link to="/cadastro" @click="open = false">
            <button class="btn btn-primary btn-sm w-full">
              <span class="material-symbols-rounded text-base">person_add</span>
              Cadastrar
            </button>
          </router-link>
        </div>
      </template>
    </template>
  </HtSidenav>
</template>

<script setup lang="ts">
import { useRouter } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import HtSidenav from "@/components/ui/HtSidenav.vue";
import { logoutHandler } from "@/shared/utils";

const open = defineModel<boolean>("open", { default: false });

const auth = useAuthStore();
const router = useRouter();

async function handleLogout() {
  await logoutHandler(auth.logout, () => router.replace("/"), () => {
    open.value = false;
  });
}
</script>
