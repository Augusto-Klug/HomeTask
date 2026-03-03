<template lang="pug">
nav.top.primary-container
  SidebarTelaInicial
  button.transparent.circle.lef-margin.small-margin(data-ui="#sidebarTelaInicial")
    i menu
  router-link.button.transparent(to="/")
    h5.no-margin HomeTask
  .max
  //- Atalho de busca
  router-link.button.transparent.circle(to="/servicos/buscar")
    i search
  //- Toggle de tema: cicla auto → light → dark
  TemaPagina
  //- Menu de conta (desktop)
  .right-margin.small-margin(v-if="auth.isLoggedIn")
    button.transparent.circle(data-ui="#user-menu")
      i account_circle
    menu.left#user-menu
      li
        a.no-hover
          i person
          span {{ auth.user?.nome }}
      li.divider
      li
        a(@click="handleLogout")
          i logout
          span Sair
  .right-margin.small-margin(v-else)
    router-link.button.border.small.no-margin(to="login") Entrar
</template>
<script lang="ts" setup>
import router from "@/router";
import TemaPagina from "@/shared/components/TemaPagina.vue";
import { useAuthStore } from "@/stores/auth";
import SidebarTelaInicial from "./telaInicial/SidebarTelaInicial.vue";

const auth = useAuthStore();

async function handleLogout() {
  await auth.logout();
  router.push("/login");
}
</script>
