<template lang="pug">
header.no-padding
  nav.primary-container
    SidebarTelaInicial
    button.transparent.circle.left-margin.small-margin(data-ui="#sidebarTelaInicial")
      i menu
    router-link.transparent(to="/")
      h5.no-margin HomeTask
    .max
    router-link.button.transparent.circle(to="/servicos/buscar")
      i search
    TemaPagina
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
    .right-margin(v-else)
      router-link.button.transparent.small-round.no-padding(to="login")
        i person
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
