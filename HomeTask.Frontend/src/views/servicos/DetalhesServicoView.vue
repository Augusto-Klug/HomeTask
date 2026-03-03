<template lang="pug">
.padding
  .center-align.padding(v-if="carregando")
    .progress.circle

  .center-align.padding(v-else-if="!servico")
    i.extra error
    p Serviço não encontrado.
    router-link.button(to="/servicos/buscar") Voltar à busca

  template(v-else)
    .row.padding
      router-link.button.transparent.circle(to="/servicos/buscar")
        i arrow_back

    article.padding
      .row.wrap
        .max
          h4 {{ servico.titulo }}
          p {{ servico.prestadorNome }}
        span.chip {{ servico.categoria }}

      .divider
      .row
        i location_on
        span {{ servico.cidade }}/{{ servico.estado }}
      .row.padding-top
        h5 R$ {{ formatarPreco(servico.preco) }}/h
        .max
        span {{ estrelas(servico.mediaAvaliacoes) }}

      p.padding-top {{ servico.descricao }}

      button.padding-top(v-if="auth.isLoggedIn" @click="irParaAgendamento")
        i calendar_month
        span Agendar
      router-link.button.border.padding-top(v-else :to="`/login?redirect=/servicos/detalhes/${id}`")
        i login
        span Faça login para agendar

    //- Avaliações
    .padding-top(v-if="avaliacoes.length")
      h5 Avaliações
      article.padding.margin(v-for="av in avaliacoes" :key="av.id")
        .row
          span.bold {{ av.clienteNome }}
          .max
          span {{ estrelas(av.nota) }}
        p.small {{ av.comentario }}
        p.small {{ formatarData(av.data) }}
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import { useAuthStore } from '@/stores/auth'
import type { Servico, Avaliacao } from '@/types'

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
    query: { servicoId: props.id, prestadorId: servico.value?.prestadorId },
  })
}

function formatarPreco(valor: number): string {
  return Number(valor).toFixed(2).replace('.', ',')
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0)
  return '★'.repeat(cheias) + '☆'.repeat(5 - cheias)
}

function formatarData(dataStr: string): string {
  return new Date(dataStr).toLocaleDateString('pt-BR')
}
</script>
