<template lang="pug">
div
  //- Hero
  .padding.center-align
    h1 Encontre o profissional ideal para sua casa
    p.large-text.padding-bottom Conectamos você aos melhores prestadores de serviços domésticos da sua região.
    .row.center-align.wrap
      router-link.button(to="/servicos/buscar")
        i search
        span Buscar Serviços
      router-link.button.border(to="/cadastro" v-if="!auth.isLoggedIn")
        span Criar Conta

  //- Categorias
  .padding
    h4.center-align Categorias
    .grid.padding-top
      article.s4.m3.l2(
        v-for="cat in categorias"
        :key="cat.id"
      )
        router-link.card.padding.center-align(
          :to="`/servicos/buscar?categoria=${cat.id}`"
          style="display: block; text-decoration: none;"
        )
          i.large {{ cat.icone }}
          .small.bold {{ cat.nome }}

  //- CTA prestador
  .padding.center-align.border-top
    h5 Você é um profissional?
    p.padding Cadastre-se como prestador e comece a receber agendamentos hoje mesmo.
    router-link.button(to="/cadastro") Quero ser prestador
</template>

<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import type { Categoria } from "@/types";

const auth = useAuthStore();

const categorias: Categoria[] = [
  { id: 1, nome: "Faxina", icone: "cleaning_services" },
  { id: 2, nome: "Jardinagem", icone: "yard" },
  { id: 3, nome: "Reparos", icone: "handyman" },
  { id: 4, nome: "Lavanderia", icone: "local_laundry_service" },
  { id: 5, nome: "Passadoria", icone: "iron" },
  { id: 6, nome: "Babysitter", icone: "child_care" },
  { id: 7, nome: "Cuidador de Idosos", icone: "elderly" },
  { id: 8, nome: "Pet Sitter", icone: "pets" },
  { id: 9, nome: "Cozinheiro", icone: "restaurant" },
  { id: 10, nome: "Serviços Gerais", icone: "build" },
];
</script>

<style scoped></style>
