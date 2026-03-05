<template lang="pug">
dialog.left#sidebarTelaInicial
  header
    nav
      h4.max HomeTask
      button.transparent.circle.large(data-ui="#sidebarTelaInicial")
        i close
  ul.list
    li.wave.round(@click="utils.fecharSidenav('sidebarTelaInicial')")
      router-link.small-padding(to="/")
        i home
        span Início
    li.wave.round(@click="utils.fecharSidenav('sidebarTelaInicial')")
      router-link.small-padding(to="/servicos/buscar")
        i search
        span Buscar Serviços
    .space
    .divider
    template(v-if="auth.isLoggedIn")
      li
        button.transparent.no-hover
          i person
          span {{ auth.user?.nome }}
        button.transparent(@click="handleLogout")
          i.right-padding.small-padding logout
          span Sair
    template(v-else)
      li
        button.transparent(@click="utils.fecharSidenav('sidebarTelaInicial')")
          router-link(to="/login")
            i.right-padding.small-padding login
            span Entrar
        button.transparent(@click="utils.fecharSidenav('sidebarTelaInicial')")
          router-link(to="/cadastro")
            i.right-padding.small-padding person_add
            span Cadastrar
</template>
<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import utils from "../../shared/utils";
import { useRouter } from "vue-router";

const auth = useAuthStore();
const router = useRouter();

async function handleLogout() {
  utils.fecharSidenav("sidebarTelaInicial");
  await auth.logout();
  router.push("/login");
}
</script>
