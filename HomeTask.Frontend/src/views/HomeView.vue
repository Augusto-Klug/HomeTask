<template>
  <div>
    <!-- Hero -->
    <section
      class="bg-primary-light border-b border-border py-16 px-4 text-center"
    >
      <div class="max-w-2xl mx-auto">
        <h1 class="text-3xl font-bold text-foreground mb-3">
          Encontre o profissional ideal para sua casa
        </h1>
        <p class="text-base text-muted mb-8">
          Conectamos você aos melhores prestadores de serviços domésticos da sua
          região.
        </p>
        <div class="flex flex-wrap justify-center gap-3">
          <router-link to="/servicos/buscar">
            <HtButton size="lg">
              <span class="material-symbols-rounded">search</span>
              Buscar Serviços
            </HtButton>
          </router-link>
          <router-link v-if="!auth.isLoggedIn" to="/cadastro">
            <HtButton variant="outline" size="lg"> Criar Conta </HtButton>
          </router-link>
          <!-- Cliente logado: anunciar serviço desejado -->
          <router-link
            v-if="auth.isLoggedIn && (auth.user?.tipo === 1 || auth.user?.tipo === 3)"
            to="/servicos/novo-cliente"
          >
            <HtButton variant="outline" size="lg">
              <span class="material-symbols-rounded">campaign</span>
              Anunciar Serviço
            </HtButton>
          </router-link>
        </div>
      </div>
    </section>

    <!-- Categorias -->
    <section class="max-w-5xl mx-auto px-4 py-12">
      <h2 class="text-title font-semibold text-foreground text-center mb-8">
        Categorias
      </h2>
      <div
        class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-4"
      >
        <router-link
          v-for="cat in categorias"
          :key="cat.id"
          :to="`/servicos/buscar?categoria=${cat.id}`"
          class="group flex flex-col items-center gap-2 p-4 rounded-xl border border-border bg-card hover:border-primary hover:bg-primary-light transition-colors text-center cursor-pointer"
        >
          <span class="material-symbols-rounded text-3xl text-primary">{{
            cat.icone
          }}</span>
          <span
            class="text-xs font-medium text-foreground group-hover:text-primary"
            >{{ cat.nome }}</span
          >
        </router-link>
      </div>
    </section>

    <!-- CTA Prestador -->
    <section class="border-t border-border bg-surface py-12 px-4 text-center">
      <div class="max-w-xl mx-auto">
        <template v-if="auth.isLoggedIn && (auth.user?.tipo === 2 || auth.user?.tipo === 3)">
          <h2 class="text-title font-semibold text-foreground mb-2">
            Ofereça seus serviços
          </h2>
          <p class="text-sm text-muted mb-6">
            Publique os serviços que você oferece e seja encontrado por clientes da sua região.
          </p>
          <router-link to="/servicos/novo-prestador">
            <HtButton>
              <span class="material-symbols-rounded">work</span>
              Anunciar Meu Serviço
            </HtButton>
          </router-link>
        </template>
        <template v-else-if="!auth.isLoggedIn">
          <h2 class="text-title font-semibold text-foreground mb-2">
            Você é um profissional?
          </h2>
          <p class="text-sm text-muted mb-6">
            Cadastre-se como prestador e comece a receber agendamentos hoje mesmo.
          </p>
          <router-link to="/cadastro">
            <HtButton>Quero ser prestador</HtButton>
          </router-link>
        </template>
      </div>
    </section>
  </div>
</template>

<script setup lang="ts">
import { useAuthStore } from "@/stores/auth";
import type { Categoria } from "@/types";
import HtButton from "@/components/ui/HtButton.vue";

const auth = useAuthStore();

const categorias: Categoria[] = [
  { id: 1, nome: "Faxina", icone: "cleaning_services" },
  { id: 2, nome: "Jardinagem", icone: "yard" },
  { id: 3, nome: "Reparos", icone: "handyman" },
  { id: 4, nome: "Lavanderia", icone: "local_laundry_service" },
  { id: 6, nome: "Babysitter", icone: "child_care" },
  { id: 7, nome: "Cuidador de Idosos", icone: "elderly" },
  { id: 8, nome: "Pet Sitter", icone: "pets" },
  { id: 9, nome: "Cozinheiro", icone: "restaurant" },
  { id: 10, nome: "Serviços Gerais", icone: "build" },
];
</script>
