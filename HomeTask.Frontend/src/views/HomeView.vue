<template>
  <div>
    <!-- Hero -->
    <section class="border-b border-border bg-surface/60 px-4 py-10 sm:py-14">
      <div
        data-testid="home-hero"
        :style="heroBackgroundStyle"
        class="relative mx-auto flex min-h-[26rem] max-w-6xl items-center overflow-hidden rounded-[2rem] border border-border/70 bg-base-200 bg-center bg-no-repeat shadow-[0_24px_80px_color-mix(in_oklch,var(--color-base-content)_10%,transparent)] sm:min-h-[30rem] lg:min-h-[34rem]"
      >
        <div
          data-testid="home-hero-overlay"
          class="absolute inset-0 bg-[color-mix(in_oklch,var(--color-base-100)_72%,transparent)]"
        />
        <div class="relative flex w-full justify-center px-4 py-10 sm:justify-start sm:px-8 sm:py-14 lg:px-14 lg:py-16">
          <div
            data-testid="home-hero-panel"
            class="w-full max-w-xl rounded-[1.75rem] border border-border/60 bg-[color-mix(in_oklch,var(--color-base-100)_82%,transparent)] px-5 py-7 text-center shadow-[0_16px_48px_color-mix(in_oklch,var(--color-base-content)_9%,transparent)] backdrop-blur-md sm:px-8 sm:py-8 sm:text-left"
          >
            <h1 class="mb-3 text-3xl font-bold text-foreground sm:text-4xl lg:text-[2.8rem]">
              Encontre o profissional ideal para sua casa
            </h1>
            <p class="mb-8 text-base leading-7 text-foreground/80 sm:text-lg">
              Conectamos você aos melhores prestadores de serviços domésticos da sua
              região.
            </p>
            <div class="flex flex-wrap justify-center gap-3 sm:justify-start">
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
        </div>
      </div>
    </section>

    <!-- Benefícios -->
    <section class="mx-auto max-w-6xl px-4 py-12 sm:py-14">
      <div class="rounded-[2rem] border border-border bg-card px-6 py-8 shadow-[0_18px_56px_color-mix(in_oklch,var(--color-base-content)_7%,transparent)] sm:px-8 lg:px-10">
        <div class="mx-auto max-w-3xl text-center">
          <p class="mb-3 text-sm font-semibold uppercase tracking-[0.18em] text-primary">
            HomeTask
          </p>
          <h2 class="mb-4 text-2xl font-semibold text-foreground sm:text-3xl">
            Tudo para contratar ou oferecer serviços com mais segurança
          </h2>
          <p class="text-base leading-7 text-muted">
            A plataforma conecta clientes e prestadores com recursos pensados para
            dar mais confiança em cada etapa da negociação.
          </p>
        </div>
        <div class="mt-8 grid gap-4 md:grid-cols-2 xl:grid-cols-4">
          <article class="rounded-[1.5rem] border border-border bg-base-100 px-5 py-5">
            <span class="material-symbols-rounded text-3xl text-primary">verified_user</span>
            <h3 class="mt-4 text-lg font-semibold text-foreground">
              Pagamento com mais segurança pela plataforma
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted">
              Mais tranquilidade para contratar e receber com um fluxo centralizado.
            </p>
          </article>
          <article class="rounded-[1.5rem] border border-border bg-base-100 px-5 py-5">
            <span class="material-symbols-rounded text-3xl text-primary">campaign</span>
            <h3 class="mt-4 text-lg font-semibold text-foreground">
              Publique o serviço que você precisa como cliente
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted">
              Descreva sua necessidade e receba contato de profissionais compatíveis.
            </p>
          </article>
          <article class="rounded-[1.5rem] border border-border bg-base-100 px-5 py-5">
            <span class="material-symbols-rounded text-3xl text-primary">storefront</span>
            <h3 class="mt-4 text-lg font-semibold text-foreground">
              Anuncie seus serviços e alcance novos clientes
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted">
              O prestador divulga seu trabalho e aumenta as chances de ser encontrado.
            </p>
          </article>
          <article class="rounded-[1.5rem] border border-border bg-base-100 px-5 py-5">
            <span class="material-symbols-rounded text-3xl text-primary">chat</span>
            <h3 class="mt-4 text-lg font-semibold text-foreground">
              Converse pelo chat com clientes e prestadores
            </h3>
            <p class="mt-2 text-sm leading-6 text-muted">
              Negocie detalhes, alinhe expectativas e mantenha tudo em um só lugar.
            </p>
          </article>
        </div>
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
import HtButton from "@/components/ui/HtButton.vue";
import toolsBackground from "@/assets/postura-plana-de-varias-ferramentas-tecnicas-isoladas-no-fundo-branco.png";

const auth = useAuthStore();

const heroBackgroundStyle = {
  backgroundImage: `url(${toolsBackground})`,
  backgroundSize: "cover",
  backgroundPosition: "center",
  backgroundRepeat: "no-repeat",
};
</script>
