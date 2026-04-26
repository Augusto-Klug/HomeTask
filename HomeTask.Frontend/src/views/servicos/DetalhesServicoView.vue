<template>
  <div class="max-w-3xl mx-auto px-4 py-8">

    <!-- Loading -->
    <div v-if="carregando" class="flex justify-center py-20">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <!-- Não encontrado -->
    <div v-else-if="!servico" class="text-center py-16 flex flex-col items-center gap-4">
      <span class="material-symbols-rounded text-5xl text-error">error</span>
      <p class="text-sm text-muted">Serviço não encontrado.</p>
      <router-link to="/servicos/buscar">
        <HtButton variant="outline">Voltar à busca</HtButton>
      </router-link>
    </div>

    <template v-else>
      <!-- Voltar -->
      <router-link
        to="/servicos/buscar"
        class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
      >
        <span class="material-symbols-rounded text-base">arrow_back</span>
        Voltar à busca
      </router-link>

      <!-- Detalhes -->
      <HtCard class="mb-6">
        <div class="flex flex-wrap items-start justify-between gap-3 mb-4">
          <div>
            <h1 class="text-xl font-bold text-foreground">{{ servico.titulo }}</h1>
            <p class="text-sm text-muted mt-0.5">
              {{ 'prestadorId' in servico ? 'Prestador: ' : 'Solicitante: ' }}
              {{ (servico as any).prestadorNome || (servico as any).clienteNome || 'N/A' }}
            </p>
          </div>
          <HtBadge variant="primary">{{ servico.categoria }}</HtBadge>
        </div>

        <HtDivider />

        <div class="flex items-center gap-2 text-sm text-muted mb-2">
          <span class="material-symbols-rounded text-base">location_on</span>
          {{ (servico as any).cidade || 'N/A' }}/{{ (servico as any).estado || 'N/A' }}
        </div>

        <div class="flex items-center justify-between mb-4">
          <div class="flex flex-col">
            <span class="text-lg font-bold text-primary">
              {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
            </span>
          </div>
          <span v-if="'mediaAvaliacoes' in servico" class="text-sm text-yellow-500">
            {{ estrelas((servico as any).mediaAvaliacoes) }}
          </span>
        </div>

        <div v-if="(servico as any).dataDesejada" class="mb-4 p-3 bg-primary/5 rounded-lg border border-primary/10">
          <p class="text-xs text-primary font-semibold uppercase mb-1">Data Desejada para Execução</p>
          <p class="text-sm text-foreground flex items-center gap-2">
            <span class="material-symbols-rounded text-base">event</span>
            {{ formatarData((servico as any).dataDesejada) }} às {{ formatarHora((servico as any).dataDesejada) }}
          </p>
        </div>

        <p v-if="servico.descricao" class="text-sm text-foreground mb-6 leading-relaxed">
          {{ servico.descricao }}
        </p>

        <HtButton v-if="auth.isLoggedIn" @click="irParaAgendamento">
          <span class="material-symbols-rounded text-base">calendar_month</span>
          {{ 'prestadorId' in servico ? 'Agendar agora' : 'Enviar Proposta' }}
        </HtButton>
        <router-link v-else :to="`/login?redirect=/servicos/detalhes/${props.id}`">
          <HtButton variant="outline">
            <span class="material-symbols-rounded text-base">login</span>
            Faça login para prosseguir
          </HtButton>
        </router-link>
      </HtCard>

      <!-- Avaliações -->
      <div v-if="avaliacoes.length">
        <h2 class="text-title font-semibold text-foreground mb-4">Avaliações</h2>
        <div class="flex flex-col gap-3">
          <HtCard v-for="av in avaliacoes" :key="av.id">
            <div class="flex items-center justify-between mb-1">
              <span class="text-sm font-semibold text-foreground">{{ av.clienteNome }}</span>
              <span class="text-sm text-yellow-500">{{ estrelas(av.nota) }}</span>
            </div>
            <p class="text-sm text-foreground mb-1">{{ av.comentario }}</p>
            <p class="text-xs text-muted">{{ formatarData(av.data) }}</p>
          </HtCard>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import { formatarPrecoServico } from '@/shared/utils'
import { useAuthStore } from '@/stores/auth'
import type { Servico, Avaliacao } from '@/types'
import HtButton from '@/components/ui/HtButton.vue'
import HtCard from '@/components/ui/HtCard.vue'
import HtBadge from '@/components/ui/HtBadge.vue'
import HtSpinner from '@/components/ui/HtSpinner.vue'
import HtDivider from '@/components/ui/HtDivider.vue'

const props = defineProps<{ id: string }>()

const auth = useAuthStore()
const router = useRouter()

const servico = ref<Servico | null>(null)
const avaliacoes = ref<Avaliacao[]>([])
const carregando = ref(true)

onMounted(async () => {
  try {
    const [servicoRes] = await Promise.all([
      api.get<Servico>('/api/ServicoOferecido/ObterServicoPorId', { params: { id: props.id } }),
    ])
    servico.value = servicoRes.data

    if (servico.value?.prestadorId) {
      const av = await api
        .get<Avaliacao[]>('/api/Avaliacao/ObterAvaliacoesPorPrestador', {
          params: { prestadorId: servico.value.prestadorId },
        })
        .catch(() => ({ data: [] as Avaliacao[] }))
      avaliacoes.value = av.data ?? []
    }
  } catch {
    servico.value = null
  } finally {
    carregando.value = false
  }
})

function irParaAgendamento() {
  router.push({
    name: 'agendamento-novo',
    params: { servicoId: props.id },
    query: { prestadorId: servico.value?.prestadorId },
  })
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0)
  return '★'.repeat(cheias) + '☆'.repeat(5 - cheias)
}

function formatarData(dataStr: string): string {
  return new Date(dataStr).toLocaleDateString('pt-BR')
}

function formatarHora(dataStr: string): string {
  return new Date(dataStr).toLocaleTimeString('pt-BR', {
    hour: '2-digit',
    minute: '2-digit',
  })
}

</script>
