<template lang="pug">
.padding
  .row.padding
    router-link.button.transparent.circle(to="/servicos/buscar")
      i arrow_back

  .center-align.padding(v-if="!servicoId || !prestadorId")
    p Parâmetros inválidos.
    router-link.button(to="/servicos/buscar") Voltar à busca

  template(v-else)
    h4 Novo Agendamento

    .center-align.padding(v-if="carregandoServico")
      .progress.circle

    article.padding(v-else-if="servico")
      h5 {{ servico.titulo }}
      p {{ servico.prestadorNome }} · R$ {{ formatarPreco(servico.preco) }}/h

      .divider

      .field.border(v-if="erro")
        p.error {{ erro }}

      form(@submit.prevent="handleAgendar")
        .row
          .field.label.border.max
            input(v-model="form.data" type="date" :min="hoje" required)
            label Data

          .field.label.border.max
            input(v-model="form.hora" type="time" required)
            label Horário

        .field.label.border
          input(v-model="form.endereco" type="text" required)
          label Endereço do serviço

        .field.label.textarea.border
          textarea(v-model="form.observacoes" rows="3")
          label Observações

        button.responsive(:disabled="carregando" type="submit")
          .progress.circle.small(v-if="carregando")
          span(v-else) Confirmar Agendamento
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '@/services/api'
import { useAuthStore } from '@/stores/auth'
import type { Servico, AgendamentoForm } from '@/types'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const servicoId = computed(() => route.query.servicoId as string | undefined)
const prestadorId = computed(() => route.query.prestadorId as string | undefined)

const servico = ref<Servico | null>(null)
const carregandoServico = ref(true)
const carregando = ref(false)
const erro = ref<string | null>(null)
const hoje = new Date().toISOString().split('T')[0]

const form = reactive<AgendamentoForm>({
  data: '',
  hora: '',
  endereco: '',
  observacoes: '',
})

onMounted(async () => {
  if (!servicoId.value) return
  try {
    const { data } = await api.get<Servico>('/api/ServicoOferecido/ObterServicoPorId', {
      params: { id: servicoId.value },
    })
    servico.value = data
  } catch {
    servico.value = null
  } finally {
    carregandoServico.value = false
  }
})

async function handleAgendar() {
  erro.value = null
  carregando.value = true
  try {
    const { data: cliente } = await api.get('/api/Cliente/ObterClientesPorUsuarioId', {
      params: { usuarioId: auth.user?.userId },
    })

    const dataHora = new Date(`${form.data}T${form.hora}:00`).toISOString()

    await api.post('/api/Agendamento/CriarAgendamento', {
      clienteId: cliente.id,
      prestadorId: prestadorId.value,
      servicoOferecidoId: servicoId.value,
      dataHoraAgendada: dataHora,
      enderecoServico: form.endereco,
      observacoes: form.observacoes,
    })

    router.push('/agendamento/sucesso')
  } catch (err: unknown) {
    const e = err as { response?: { data?: unknown } }
    const msg = e.response?.data
    erro.value =
      typeof msg === 'string' && msg
        ? msg
        : 'Erro ao criar agendamento. Verifique os dados e tente novamente.'
  } finally {
    carregando.value = false
  }
}

function formatarPreco(valor: number): string {
  return Number(valor).toFixed(2).replace('.', ',')
}
</script>
