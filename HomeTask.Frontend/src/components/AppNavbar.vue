<template lang="pug">
nav.top
  .max
    router-link.button.transparent(to="/")
      i home
      span HomeTask

  router-link.button.transparent(to="/servicos/buscar")
    i search
    span Buscar Serviços

  template(v-if="auth.isLoggedIn")
    .button.transparent
      i person
      span {{ auth.user?.nome }}
    button.border.small(@click="handleLogout")
      i logout
      span Sair

  template(v-else)
    router-link.button.border.small(to="/login") Entrar
    router-link.button.small(to="/cadastro") Cadastrar
</template>

<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import { useRouter } from "vue-router";

const auth = useAuthStore();
const router = useRouter();

async function handleLogout() {
  await auth.logout();
  router.push("/login");
}
</script>
