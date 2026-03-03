<template lang="pug">
dialog.left#sidebarTelaInicial
  header
    nav
      router-link.button.transparent.circle(to="/" @click="utils.fecharSidenav('sidebarTelaInicial')")
        i home
      h5.max HomeTask
  //- Links de navegação
  a(@click="utils.fecharSidenav('sidebarTelaInicial')")
    router-link(to="/")
      i home
      span Início
  a(@click="utils.fecharSidenav('sidebarTelaInicial')")
    router-link(to="/servicos/buscar")
      i search
      span Buscar Serviços
  .divider
  //- Auth links
  template(v-if="auth.isLoggedIn")
    a.no-hover
      i person
      span {{ auth.user?.nome }}
    a(@click="handleLogout")
      i logout
      span Sair
  template(v-else)
    a(@click="utils.fecharSidenav('sidebarTelaInicial')")
      router-link(to="/login")
        i login
        span Entrar
    a(@click="utils.fecharSidenav('sidebarTelaInicial')")
      router-link(to="/cadastro")
        i person_add
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
