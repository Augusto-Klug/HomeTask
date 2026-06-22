<template>
  <div class="mx-auto max-w-5xl px-4 py-8">
    <div v-if="carregando" class="flex justify-center py-20">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <div v-else-if="!perfil" class="py-16 text-center">
      <p class="text-sm text-muted">Prestador não encontrado.</p>
    </div>

    <template v-else>
      <router-link
        to="/servicos/buscar"
        class="mb-6 inline-flex items-center gap-1.5 text-sm text-muted transition-colors hover:text-primary"
      >
        <span class="material-symbols-rounded text-base">arrow_back</span>
        Voltar
      </router-link>

      <HtCard class="mb-6">
        <div class="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
          <div>
            <h1 class="text-2xl font-bold text-foreground">{{ perfil.nome }}</h1>
            <p v-if="perfil.cidade || perfil.estado" class="mt-1 text-sm text-muted">
              {{ [perfil.cidade, perfil.estado].filter(Boolean).join("/") }}
            </p>
            <p v-if="perfil.descricao" class="mt-3 max-w-2xl text-sm text-foreground/80">
              {{ perfil.descricao }}
            </p>
          </div>

          <div class="grid grid-cols-1 gap-3 text-sm md:min-w-72">
            <div class="rounded-xl border border-border/60 bg-muted/10 p-4">
              <p class="text-xs uppercase tracking-wider text-muted">Avaliação do prestador</p>
              <p class="mt-1 text-lg font-semibold text-yellow-500">{{ estrelas(perfil.mediaAvaliacoes) }}</p>
              <p class="text-sm text-foreground">{{ formatarNota(perfil.mediaAvaliacoes) }} de 5</p>
              <p class="text-xs text-muted">{{ perfil.totalAvaliacoes }} avaliação(ões)</p>
            </div>
            <div class="rounded-xl border border-border/60 bg-muted/10 p-4">
              <p class="text-xs uppercase tracking-wider text-muted">Serviços concluídos</p>
              <p class="mt-1 text-2xl font-bold text-foreground">{{ perfil.totalServicosConcluidos }}</p>
            </div>
          </div>
        </div>
      </HtCard>

      <HtCard v-if="perfil.servicosOferecidos.length" class="mb-6">
        <h2 class="mb-4 text-lg font-bold">Serviços ofertados</h2>
        <div class="grid gap-3 md:grid-cols-2">
          <router-link
            v-for="servico in perfil.servicosOferecidos"
            :key="servico.id"
            :to="`/servicos/detalhes/${servico.id}`"
            class="rounded-xl border border-border/60 p-4 transition-colors hover:border-primary"
          >
            <div class="flex items-start justify-between gap-3">
              <div>
                <p class="font-semibold text-foreground">{{ servico.titulo }}</p>
                <p class="mt-1 text-sm text-muted">{{ servico.descricao }}</p>
              </div>
              <span class="text-sm text-yellow-500">{{ formatarEstrelas(servico.mediaAvaliacoes ?? 0) }}</span>
            </div>
          </router-link>
        </div>
      </HtCard>

      <HtCard class="mb-6">
        <h2 class="mb-4 text-lg font-bold">Histórico concluído</h2>
        <div v-if="perfil.historicoConcluido.length" class="grid gap-3">
          <div
            v-for="item in perfil.historicoConcluido"
            :key="item.agendamentoId"
            class="rounded-xl border border-border/60 p-4"
          >
            <div class="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
              <div>
                <p class="font-semibold text-foreground">{{ item.tituloServico }}</p>
                <p class="mt-1 text-sm text-muted">
                  {{ formatarData(item.dataHoraAgendada) }} · {{ item.cidade }}/{{ item.estado }}
                </p>
              </div>
              <div class="text-sm">
                <p class="text-yellow-500">Serviço: {{ estrelas(item.notaServico ?? 0) }}</p>
                <p class="text-yellow-500">Prestador: {{ estrelas(item.notaPrestador ?? 0) }}</p>
              </div>
            </div>
          </div>
        </div>
        <p v-else class="text-sm text-muted">Nenhum serviço concluído disponível.</p>
      </HtCard>

      <HtCard class="mb-6">
        <div class="mb-4 flex items-center justify-between gap-3">
          <div>
            <h2 class="text-lg font-bold">Certificações</h2>
            <p class="text-sm text-muted">Comprovantes e formações compartilhados pelo prestador.</p>
          </div>
          <span class="rounded-full bg-base-200 px-3 py-1 text-xs font-medium text-base-content/70">
            {{ perfil.certificacoes.length }} item(ns)
          </span>
        </div>

        <div v-if="perfil.certificacoes.length" class="grid gap-3 md:grid-cols-2">
          <button
            v-for="certificacao in perfil.certificacoes"
            :key="certificacao.id"
            type="button"
            class="rounded-xl border border-border/60 p-4 text-left transition-all hover:-translate-y-0.5 hover:border-primary hover:shadow-sm"
            @click="abrirCertificacao(certificacao)"
          >
            <div class="flex items-start justify-between gap-3">
              <div>
                <p class="font-semibold text-foreground">{{ certificacao.nome }}</p>
                <p v-if="certificacao.instituicao" class="mt-1 text-sm text-muted">
                  {{ certificacao.instituicao }}
                </p>
              </div>
              <span
                class="rounded-full px-2.5 py-1 text-xs font-medium"
                :class="certificacao.verificada ? 'bg-success/15 text-success' : 'bg-base-200 text-base-content/70'"
              >
                {{ certificacao.verificada ? "Verificada" : "Informada" }}
              </span>
            </div>
            <p class="mt-3 text-xs text-base-content/60">
              {{ obterResumoDatasCertificacao(certificacao) }}
            </p>
          </button>
        </div>
        <p v-else class="text-sm text-muted">Nenhuma certificação pública disponível.</p>
      </HtCard>

      <HtCard>
        <div class="mb-4 flex items-center justify-between gap-3">
          <div>
            <h2 class="text-lg font-bold">Portfólio</h2>
            <p class="text-sm text-muted">Fotos de trabalhos e resultados publicados pelo prestador.</p>
          </div>
          <span class="rounded-full bg-base-200 px-3 py-1 text-xs font-medium text-base-content/70">
            {{ perfil.portfolios.length }} foto(s)
          </span>
        </div>

        <div v-if="perfil.portfolios.length" class="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          <article
            v-for="portfolio in perfil.portfolios"
            :key="portfolio.id"
            class="overflow-hidden rounded-2xl border border-border/60 bg-base-100 shadow-sm transition-shadow hover:shadow-md"
          >
            <button
              type="button"
              class="block h-52 w-full overflow-hidden bg-base-200 text-left"
              @click="abrirPortfolio(portfolio)"
            >
              <img
                :src="resolverUrlArquivo(portfolio.urlImagem)"
                :alt="portfolio.titulo || 'Imagem do portfólio'"
                class="h-full w-full object-cover transition-transform duration-300 hover:scale-[1.03]"
              />
            </button>
            <div class="space-y-2 p-4">
              <div class="flex items-start justify-between gap-3">
                <div>
                  <p class="font-semibold text-foreground">{{ portfolio.titulo || "Trabalho publicado" }}</p>
                  <p class="text-xs text-base-content/60">{{ formatarData(portfolio.dataCadastro) }}</p>
                </div>
                <button
                  type="button"
                  class="btn btn-ghost btn-xs"
                  @click="abrirPortfolio(portfolio)"
                >
                  Ver foto
                </button>
              </div>
              <p v-if="portfolio.descricao" class="text-sm text-base-content/75">
                {{ portfolio.descricao }}
              </p>
            </div>
          </article>
        </div>
        <p v-else class="text-sm text-muted">Nenhuma foto de portfólio publicada ainda.</p>
      </HtCard>

      <HtCertificacaoDetail
        :certificacao="certificacaoSelecionada"
        :is-open="mostrarCertificacao"
        @close="mostrarCertificacao = false"
      />

      <div v-if="mostrarPortfolio && portfolioSelecionado" class="modal modal-open">
        <div class="modal-box max-w-5xl p-0">
          <button
            type="button"
            class="btn btn-sm btn-circle btn-ghost absolute right-3 top-3 z-10 bg-black/50 text-white hover:bg-black/70"
            @click="fecharPortfolio"
          >
            ×
          </button>

          <div class="grid gap-0 lg:grid-cols-[minmax(0,1.5fr)_minmax(280px,1fr)]">
            <div class="bg-black">
              <img
                :src="resolverUrlArquivo(portfolioSelecionado.urlImagem)"
                :alt="portfolioSelecionado.titulo || 'Imagem do portfólio'"
                class="max-h-[75vh] w-full object-contain"
              />
            </div>
            <div class="space-y-4 p-6">
              <div>
                <p class="text-lg font-bold text-foreground">
                  {{ portfolioSelecionado.titulo || "Trabalho publicado" }}
                </p>
                <p class="text-sm text-muted">{{ formatarData(portfolioSelecionado.dataCadastro) }}</p>
              </div>
              <p v-if="portfolioSelecionado.descricao" class="text-sm leading-6 text-base-content/80">
                {{ portfolioSelecionado.descricao }}
              </p>
              <a
                :href="resolverUrlArquivo(portfolioSelecionado.urlImagem)"
                target="_blank"
                rel="noopener noreferrer"
                class="btn btn-primary btn-sm"
              >
                Abrir imagem
              </a>
            </div>
          </div>
        </div>
        <div class="modal-backdrop" @click="fecharPortfolio"></div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue"
import api from "@/services/api"
import { formatarData, formatarEstrelas, resolverUrlArquivo } from "@/shared/utils"
import type { Certificacao, Portfolio, PrestadorPerfilPublico } from "@/types"
import HtCard from "@/components/ui/HtCard.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"
import HtCertificacaoDetail from "./components/HtCertificacaoDetail.vue"

const props = defineProps<{ id: string }>()

const perfil = ref<PrestadorPerfilPublico | null>(null)
const carregando = ref(true)
const certificacaoSelecionada = ref<Certificacao | null>(null)
const mostrarCertificacao = ref(false)
const portfolioSelecionado = ref<Portfolio | null>(null)
const mostrarPortfolio = ref(false)

onMounted(async () => {
  try {
    const { data } = await api.get<PrestadorPerfilPublico>("/api/Prestador/ObterPerfilPublico", {
      params: { prestadorId: props.id },
    })
    perfil.value = data
  } catch {
    perfil.value = null
  } finally {
    carregando.value = false
  }
})

function formatarNota(nota: number): string {
  return Number(nota ?? 0).toFixed(1)
}

function estrelas(nota: number): string {
  return formatarEstrelas(nota, "round")
}

function abrirCertificacao(certificacao: Certificacao) {
  certificacaoSelecionada.value = certificacao
  mostrarCertificacao.value = true
}

function abrirPortfolio(portfolio: Portfolio) {
  portfolioSelecionado.value = portfolio
  mostrarPortfolio.value = true
}

function fecharPortfolio() {
  mostrarPortfolio.value = false
  portfolioSelecionado.value = null
}

function obterResumoDatasCertificacao(certificacao: Certificacao): string {
  if (certificacao.dataEmissao && certificacao.dataValidade) {
    return `Emitido em ${formatarData(certificacao.dataEmissao)} · válido até ${formatarData(certificacao.dataValidade)}`
  }

  if (certificacao.dataEmissao) {
    return `Emitido em ${formatarData(certificacao.dataEmissao)}`
  }

  if (certificacao.dataValidade) {
    return `Válido até ${formatarData(certificacao.dataValidade)}`
  }

  return "Sem datas informadas"
}
</script>
