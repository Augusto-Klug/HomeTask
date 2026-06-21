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

      <div>
        <h2 class="mb-4 text-lg font-bold">Histórico concluído</h2>
        <div v-if="perfil.historicoConcluido.length" class="grid gap-3">
          <HtCard v-for="item in perfil.historicoConcluido" :key="item.agendamentoId">
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
          </HtCard>
        </div>
        <p v-else class="text-sm text-muted">Nenhum serviço concluído disponível.</p>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue"
import api from "@/services/api"
import { formatarData, formatarEstrelas } from "@/shared/utils"
import type { PrestadorPerfilPublico } from "@/types"
import HtCard from "@/components/ui/HtCard.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"

const props = defineProps<{ id: string }>()

const perfil = ref<PrestadorPerfilPublico | null>(null)
const carregando = ref(true)

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
</script>
