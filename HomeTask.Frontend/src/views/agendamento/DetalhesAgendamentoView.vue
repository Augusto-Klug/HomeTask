<template>
  <div class="container mx-auto max-w-3xl px-4 py-8">
    <router-link
      :to="rotaVolta"
      class="mb-6 inline-flex items-center gap-1.5 text-sm text-muted transition-colors hover:text-primary"
    >
      <span class="material-symbols-rounded text-base">arrow_back</span>
      {{ textoVolta }}
    </router-link>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <div v-else-if="erro" class="py-16 text-center">
      <p class="mb-4 text-error">{{ erro }}</p>
      <HtButton @click="buscarDetalhes">Tentar novamente</HtButton>
    </div>

    <template v-else-if="agendamento">
      <div class="flex flex-col gap-6">
        <HtAlert v-if="checkoutRetorno" :variant="checkoutRetorno.variant" :title="checkoutRetorno.title">
          {{ checkoutRetorno.message }}
        </HtAlert>

        <HtCard>
          <div class="flex items-center justify-between">
            <div>
              <p class="mb-1 text-xs uppercase tracking-wider text-muted">Status do agendamento</p>
              <HtBadge :variant="obterVariantStatus(agendamento.status)" class="px-3 py-1 text-base">
                {{ statusLabel }}
              </HtBadge>
            </div>
            <div class="text-right">
              <p class="mb-1 text-xs uppercase tracking-wider text-muted">Valor total</p>
              <p class="text-xl font-bold">{{ formatarMoeda(agendamento.valorTotal) }}</p>
            </div>
          </div>

          <div
            v-if="agendamento.motivoRecusa"
            class="mt-4 rounded-lg border border-error/20 bg-error/10 p-3"
          >
            <p class="text-sm font-semibold text-error">Motivo:</p>
            <p class="text-sm text-error/80">{{ agendamento.motivoRecusa }}</p>
          </div>
        </HtCard>

        <HtCard v-if="agendamento.status === StatusAgendamento.AguardandoPagamento">
          <div class="flex items-start justify-between gap-4">
            <div>
              <p class="mb-1 text-xs uppercase tracking-wider text-muted">Pagamento</p>
              <p class="font-semibold">{{ labelPagamento }}</p>
              <p class="mt-1 text-sm text-muted">{{ descricaoPagamento }}</p>
            </div>
            <HtBadge :variant="badgePagamento">{{ labelPagamento }}</HtBadge>
          </div>
        </HtCard>

        <HtCard v-if="avaliacaoEnviada" class="border-success/30 bg-success/5">
          <p class="font-semibold text-success">Avaliação enviada</p>
          <p class="mt-1 text-sm text-muted">Este agendamento já foi avaliado e não aceita novo envio.</p>
        </HtCard>

        <HtCard>
          <h2 class="mb-4 text-lg font-bold">Serviços agendados</h2>
          <div
            v-for="servico in agendamento.servicos"
            :key="servico.id"
            class="flex items-center justify-between border-b py-2 last:border-0"
          >
            <div>
              <p class="font-semibold">{{ servico.titulo }}</p>
            </div>
            <p class="font-medium">{{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}</p>
          </div>
        </HtCard>

        <HtCard>
          <h2 class="mb-4 text-lg font-bold">Quando e onde</h2>
          <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
            <div class="flex items-start gap-3">
              <span class="material-symbols-rounded mt-0.5 text-primary">calendar_today</span>
              <div>
                <p class="text-sm font-semibold">Data e horário</p>
                <p class="text-sm text-muted">
                  {{ formatarDataLonga(agendamento.dataHoraAgendada) }} às
                  {{ formatarHora(agendamento.dataHoraAgendada) }}
                </p>
                <p class="mt-1 text-xs text-muted">Duração estimada: {{ agendamento.duracaoMinutos }} min</p>
              </div>
            </div>
            <div class="flex items-start gap-3">
              <span class="material-symbols-rounded mt-0.5 text-primary">location_on</span>
              <div>
                <p class="text-sm font-semibold">Local de realização</p>
                <p class="text-sm text-muted">{{ formatarEndereco(agendamento.endereco) }}</p>
              </div>
            </div>
          </div>
          <div v-if="agendamento.observacoes" class="mt-4 rounded-lg bg-muted/20 p-3">
            <p class="mb-1 text-sm font-semibold">Observações:</p>
            <p class="text-sm text-muted">{{ agendamento.observacoes }}</p>
          </div>
        </HtCard>

        <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
          <HtCard>
            <p class="mb-2 text-xs uppercase tracking-wider text-muted">Cliente</p>
            <router-link v-if="souOPrestador" :to="`/clientes/${agendamento.clienteId}`" class="flex items-center gap-3 hover:opacity-90">
              <div class="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 font-bold text-primary">
                {{ agendamento.clienteNome.charAt(0) }}
              </div>
              <p class="font-semibold">{{ agendamento.clienteNome }}</p>
            </router-link>
            <div v-else class="flex items-center gap-3">
              <div class="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10 font-bold text-primary">
                {{ agendamento.clienteNome.charAt(0) }}
              </div>
              <p class="font-semibold">{{ agendamento.clienteNome }}</p>
            </div>
          </HtCard>
          <HtCard>
            <p class="mb-2 text-xs uppercase tracking-wider text-muted">Prestador</p>
            <router-link :to="`/prestadores/${agendamento.prestadorId}`" class="flex items-center gap-3 hover:opacity-90">
              <div class="flex h-10 w-10 items-center justify-center rounded-full bg-secondary/10 font-bold text-secondary">
                {{ agendamento.prestadorNome.charAt(0) }}
              </div>
              <p class="font-semibold">{{ agendamento.prestadorNome }}</p>
            </router-link>
          </HtCard>
        </div>

        <div class="mt-4 flex flex-wrap gap-3">
          <template v-if="podeAceitar">
            <HtButton class="flex-1" :loading="carregandoAcao" @click="acaoAgendamento('Aceitar')">
              {{ agendamento.aguardandoRespostaDe === TipoUsuario.Cliente ? "Aceitar proposta" : "Aceitar agendamento" }}
            </HtButton>
            <HtButton
              variant="outline"
              class="flex-1 border-error text-error hover:bg-error/10"
              @click="mostrarModalRecusa = true"
            >
              Recusar
            </HtButton>
          </template>

          <template v-if="souOPrestador">
            <HtButton
              v-if="agendamento.status === StatusAgendamento.Aceito"
              class="flex-1"
              :loading="carregandoAcao"
              @click="acaoAgendamento('Iniciar')"
            >
              Iniciar serviço
            </HtButton>
            <HtButton
              v-if="agendamento.status === StatusAgendamento.EmAndamento"
              class="flex-1"
              variant="success"
              :loading="carregandoAcao"
              @click="acaoAgendamento('Concluir')"
            >
              Concluir serviço
            </HtButton>
          </template>

          <HtButton
            v-if="podePagar"
            class="flex-1"
            variant="success"
            :loading="carregandoPagamento"
            @click="realizarPagamento"
          >
            {{ textoBotaoPagamento }}
          </HtButton>

          <HtButton
            v-if="podeCancelar"
            class="flex-1"
            variant="outline"
            @click="mostrarModalCancelamento = true"
          >
            Cancelar agendamento
          </HtButton>

          <HtButton
            v-if="podeAbrirChat"
            class="flex-1"
            variant="outline"
            @click="abrirChat"
          >
            <span class="material-symbols-rounded text-base">chat</span>
            Abrir chat
          </HtButton>
        </div>
      </div>

      <AgendamentoChat
        v-if="podeAbrirChat"
        v-model:aberto="chatAberto"
        :agendamento-id="agendamento.id"
        :usuario-atual-id="auth.user?.userId ?? ''"
        :nome-participante="nomeParticipanteChat"
        :titulo-servico="tituloServicoChat"
      />
    </template>

    <div v-if="mostrarModalRecusa" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <HtCard class="w-full max-w-md">
        <h3 class="mb-4 text-lg font-bold">Recusar agendamento</h3>
        <HtTextarea v-model="motivo" label="Motivo da recusa" placeholder="Explique o motivo..." required />
        <div class="mt-6 flex gap-3">
          <HtButton variant="outline" class="flex-1" @click="mostrarModalRecusa = false">Voltar</HtButton>
          <HtButton variant="error" class="flex-1" :loading="carregandoAcao" @click="acaoAgendamento('Recusar')">
            Confirmar recusa
          </HtButton>
        </div>
      </HtCard>
    </div>

    <div v-if="mostrarModalAvaliacaoCliente" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <HtCard class="w-full max-w-xl">
        <h3 class="mb-2 text-lg font-bold">Avaliar atendimento</h3>
        <p class="mb-6 text-sm text-muted">Envie uma avaliação única para o serviço e para o prestador.</p>

        <div class="space-y-5">
          <div>
            <p class="mb-2 text-sm font-semibold">Avalie o serviço</p>
            <div class="flex gap-2">
              <button
                v-for="nota in 5"
                :key="`servico-${nota}`"
                type="button"
                class="text-2xl"
                @click="notaServico = nota"
              >
                {{ nota <= notaServico ? "★" : "☆" }}
              </button>
            </div>
          </div>

          <div>
            <p class="mb-2 text-sm font-semibold">Avalie o prestador</p>
            <div class="flex gap-2">
              <button
                v-for="nota in 5"
                :key="`prestador-${nota}`"
                type="button"
                class="text-2xl"
                @click="notaPrestador = nota"
              >
                {{ nota <= notaPrestador ? "★" : "☆" }}
              </button>
            </div>
          </div>

          <HtTextarea v-model="comentarioAvaliacao" label="Comentário opcional" placeholder="Conte como foi a experiência..." />
        </div>

        <div class="mt-6 flex gap-3">
          <HtButton variant="outline" class="flex-1" @click="mostrarModalAvaliacao = false">Agora não</HtButton>
          <HtButton class="flex-1" :loading="carregandoAvaliacao" @click="enviarAvaliacao">Enviar avaliação</HtButton>
        </div>
      </HtCard>
    </div>

    <div v-if="mostrarModalCancelamento" class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <HtCard class="w-full max-w-md">
        <h3 class="mb-4 text-lg font-bold">Cancelar agendamento</h3>
        <HtTextarea v-model="motivo" label="Motivo do cancelamento" placeholder="Por que deseja cancelar?" required />
        <div class="mt-6 flex gap-3">
          <HtButton variant="outline" class="flex-1" @click="mostrarModalCancelamento = false">Voltar</HtButton>
          <HtButton variant="error" class="flex-1" :loading="carregandoAcao" @click="acaoAgendamento('Cancelar')">
            Confirmar cancelamento
          </HtButton>
        </div>
      </HtCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue"
import { useRoute } from "vue-router"
import { useAuthStore } from "@/stores/auth"
import api from "@/services/api"
import {
  StatusAgendamento,
  StatusPagamento,
  TipoUsuario,
  type AgendamentoResumo,
  type Avaliacao,
  type AvaliacaoCliente,
  type PagamentoResumo,
} from "@/types"
import { formatarDataLonga, formatarHora, formatarMoeda, formatarPrecoServico } from "@/shared/utils"
import HtAlert from "@/components/ui/HtAlert.vue"
import HtCard from "@/components/ui/HtCard.vue"
import HtBadge, { HtBadgeVariant } from "@/components/ui/HtBadge.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"
import HtButton from "@/components/ui/HtButton.vue"
import HtTextarea from "@/components/ui/HtTextarea.vue"
import AgendamentoChat from "./components/AgendamentoChat.vue"

const route = useRoute()
const auth = useAuthStore()

const id = route.params.id as string
const agendamento = ref<AgendamentoResumo | null>(null)
const pagamento = ref<PagamentoResumo | null>(null)
const avaliacaoPrestadorExistente = ref<Avaliacao | null>(null)
const avaliacaoClienteExistente = ref<AvaliacaoCliente | null>(null)
const carregando = ref(true)
const carregandoAcao = ref(false)
const carregandoPagamento = ref(false)
const carregandoAvaliacaoCliente = ref(false)
const carregandoAvaliacaoPrestador = ref(false)
const erro = ref<string | null>(null)
const motivo = ref("")
const comentarioAvaliacao = ref("")
const comentarioAvaliacaoCliente = ref("")
const notaServico = ref(0)
const notaPrestador = ref(0)
const notaCliente = ref(0)
const mostrarModalRecusa = ref(false)
const mostrarModalCancelamento = ref(false)
const mostrarModalAvaliacao = ref(false)
const chatAberto = ref(false)
const clienteAtualId = ref<string | null>(null)
const prestadorAtualId = ref<string | null>(null)
const retornoCheckoutVisivel = ref(false)
const statusRetornoCheckout = ref<string | null>(null)
const clienteAvaliacaoEnviada = ref(false)
const prestadorAvaliacaoEnviada = ref(false)

const rotaVolta = computed(() => {
  if (route.query.origem === "operacao") return "/perfil/agendamentos-prestador"
  if (route.query.origem === "pendentes") return "/perfil/agendamentos-prestador"
  return "/perfil/agendamentos-cliente"
})

const textoVolta = computed(() => {
  if (route.query.origem === "operacao") return "Voltar para minha operação"
  if (route.query.origem === "pendentes") return "Voltar para minha operação"
  return "Voltar para meus agendamentos"
})

const souOPrestador = computed(() =>
  !!agendamento.value && prestadorAtualId.value === String(agendamento.value.prestadorId),
)

const souOCliente = computed(() =>
  !!agendamento.value && clienteAtualId.value === String(agendamento.value.clienteId),
)

const podeAceitar = computed(() => {
  if (!agendamento.value || agendamento.value.status !== StatusAgendamento.Solicitado) return false
  if (agendamento.value.aguardandoRespostaDe === TipoUsuario.Cliente) return souOCliente.value
  return souOPrestador.value
})

const podeCancelar = computed(() => {
  if (!agendamento.value) return false
  return agendamento.value.status === StatusAgendamento.Solicitado || agendamento.value.status === StatusAgendamento.Aceito
})

const podePagar = computed(() =>
  !!agendamento.value &&
  souOCliente.value &&
  agendamento.value.status === StatusAgendamento.AguardandoPagamento &&
  pagamento.value?.status !== StatusPagamento.Aprovado,
)

const podeAbrirChat = computed(() => {
  if (!agendamento.value) return false

  return [
    StatusAgendamento.Aceito,
    StatusAgendamento.EmAndamento,
    StatusAgendamento.AguardandoPagamento,
    StatusAgendamento.Concluido,
  ].includes(agendamento.value.status)
})

const nomeParticipanteChat = computed(() => {
  if (!agendamento.value) return ""
  return souOPrestador.value ? agendamento.value.clienteNome : agendamento.value.prestadorNome
})

const tituloServicoChat = computed(() => {
  if (!agendamento.value) return "Serviço agendado"
  return agendamento.value.servicos[0]?.titulo ?? "Serviço agendado"
})

const checkoutRetorno = computed(() => {
  if (!retornoCheckoutVisivel.value) return null

  if (pagamento.value?.status === StatusPagamento.Aprovado || agendamento.value?.status === StatusAgendamento.Concluido) {
    return {
      variant: "success",
      title: "Pagamento aprovado",
      message: "Pagamento confirmado com sucesso. O agendamento foi concluído.",
    } as const
  }

  switch (statusRetornoCheckout.value) {
    case "rejected":
    case "cancelled":
    case "failure":
      return {
        variant: "error",
        title: "Pagamento não concluído",
        message: "O Mercado Pago informou que o pagamento não foi concluído. Você pode tentar novamente abaixo.",
      } as const
    case "pending":
    case "in_process":
      return {
        variant: "info",
        title: "Pagamento em processamento",
        message: "O pagamento foi iniciado, mas a confirmação oficial ainda depende do gateway.",
      } as const
    default:
      return {
        variant: "info",
        title: "Retorno do checkout recebido",
        message: "O retorno do checkout não confirma o pagamento sozinho. A conclusão oficial depende do gateway.",
      } as const
  }
})

const statusLabel = computed(() => {
  if (!agendamento.value) return ""
  if (agendamento.value.status === StatusAgendamento.Solicitado && agendamento.value.aguardandoRespostaDe === TipoUsuario.Cliente) {
    return "Pendente de aprovação do cliente"
  }
  if (agendamento.value.status === StatusAgendamento.Solicitado && agendamento.value.aguardandoRespostaDe === TipoUsuario.Prestador) {
    return "Pendente de resposta do prestador"
  }
  return STATUS_LABEL[agendamento.value.status]
})

const labelPagamento = computed(() => {
  if (!pagamento.value) return "Pagamento pendente"
  return STATUS_PAGAMENTO_LABEL[pagamento.value.status]
})

const descricaoPagamento = computed(() => {
  if (pagamento.value?.status === StatusPagamento.Aprovado) {
    return "Pagamento confirmado. O serviço foi concluído com sucesso."
  }

  if (pagamento.value?.status === StatusPagamento.Recusado || statusRetornoCheckout.value === "rejected") {
    return "O pagamento foi recusado. Revise os dados e tente iniciar o checkout novamente."
  }

  if (pagamento.value?.status === StatusPagamento.Processando) {
    return "O serviço foi concluído pelo prestador e aguarda confirmação oficial do gateway."
  }

  return "O serviço foi concluído pelo prestador e aguarda pagamento do cliente."
})

const textoBotaoPagamento = computed(() => {
  if (pagamento.value?.status === StatusPagamento.Recusado || statusRetornoCheckout.value === "rejected") {
    return "Tentar novamente"
  }

  return "Realizar pagamento"
})

const badgePagamento = computed(() => {
  if (!pagamento.value) return HtBadgeVariant.Outline

  switch (pagamento.value.status) {
    case StatusPagamento.Aprovado:
      return HtBadgeVariant.Success
    case StatusPagamento.Recusado:
      return HtBadgeVariant.Error
    case StatusPagamento.Processando:
      return HtBadgeVariant.Primary
    default:
      return HtBadgeVariant.Outline
  }
})

onMounted(async () => {
  retornoCheckoutVisivel.value = route.query.retornoCheckout === "1"
  statusRetornoCheckout.value = obterStatusRetornoCheckout()
  await Promise.all([carregarAtorAtual(), reconciliarRetornoCheckout()])
  await buscarDetalhes()
})

async function carregarAtorAtual() {
  if (!auth.user?.userId) return

  const [cliente, prestador] = await Promise.allSettled([
    api.get("/api/Cliente/ObterClientesPorUsuarioId", { params: { usuarioId: auth.user.userId } }),
    api.get("/api/Prestador/ObterPrestadorPorUsuarioId", { params: { usuarioId: auth.user.userId } }),
  ])

  if (cliente.status === "fulfilled") {
    clienteAtualId.value = String((cliente.value.data as { id?: string }).id ?? "")
  }

  if (prestador.status === "fulfilled") {
    prestadorAtualId.value = String((prestador.value.data as { id?: string }).id ?? "")
  }
}

async function buscarDetalhes() {
  carregando.value = true
  erro.value = null
  try {
    const { data } = await api.get<AgendamentoResumo>("/api/Agendamento/ObterAgendamentoPorId", { params: { id } })
    agendamento.value = data
    await Promise.all([buscarPagamento(), buscarAvaliacoes()])
    abrirAvaliacoesAutomaticamente()
  } catch {
    erro.value = "Não foi possível carregar os detalhes do agendamento."
  } finally {
    carregando.value = false
  }
}

async function buscarPagamento() {
  try {
    const { data } = await api.get<PagamentoResumo>("/api/Pagamento/ObterPagamentoPorAgendamento", {
      params: { agendamentoId: id },
    })
    pagamento.value = data
  } catch {
    pagamento.value = null
  }
}

async function buscarAvaliacoes() {
  const [avaliacaoPrestador, avaliacaoCliente] = await Promise.allSettled([
    api.get<Avaliacao>("/api/Avaliacao/ObterAvaliacaoPorAgendamento", {
      params: { agendamentoId: id },
    }),
    api.get<AvaliacaoCliente>("/api/Avaliacao/ObterAvaliacaoClientePorAgendamento", {
      params: { agendamentoId: id },
    }),
  ])

  if (avaliacaoPrestador.status === "fulfilled") {
    avaliacaoPrestadorExistente.value = avaliacaoPrestador.value.data
    clienteAvaliacaoEnviada.value = true
  } else {
    avaliacaoPrestadorExistente.value = null
    clienteAvaliacaoEnviada.value = false
  }

  if (avaliacaoCliente.status === "fulfilled") {
    avaliacaoClienteExistente.value = avaliacaoCliente.value.data
    prestadorAvaliacaoEnviada.value = true
  } else {
    avaliacaoClienteExistente.value = null
    prestadorAvaliacaoEnviada.value = false
  }
}

async function reconciliarRetornoCheckout() {
  if (!retornoCheckoutVisivel.value) return

  const pagamentoExternoId = String(route.query.payment_id ?? "") || String(route.query.collection_id ?? "")
  if (!pagamentoExternoId) return

  try {
    await api.post("/api/Pagamento/ReconciliarPagamento", {
      pagamentoExternoId,
    })
  } catch {
    // O retorno do checkout melhora a UX, mas o webhook continua sendo a fonte oficial.
  }
}

async function acaoAgendamento(acao: string) {
  carregandoAcao.value = true
  try {
    const payload: Record<string, unknown> = { id }
    if (acao === "Recusar" || acao === "Cancelar") {
      payload.motivoRecusa = motivo.value
    }

    await api.post(`/api/Agendamento/${acao}Agendamento`, payload)
    mostrarModalRecusa.value = false
    mostrarModalCancelamento.value = false
    motivo.value = ""
    await buscarDetalhes()
  } catch {
    alert("Erro ao realizar a ação. Tente novamente.")
  } finally {
    carregandoAcao.value = false
  }
}

async function realizarPagamento() {
  carregandoPagamento.value = true
  try {
    const { data } = await api.post<PagamentoResumo>("/api/Pagamento/IniciarPagamento", { agendamentoId: id })
    pagamento.value = data
    if (data.checkoutUrl) {
      window.location.href = data.checkoutUrl
      return
    }
    alert("Não foi possível iniciar o checkout.")
  } catch {
    alert("Erro ao iniciar pagamento. Tente novamente.")
  } finally {
    carregandoPagamento.value = false
  }
}

function abrirChat() {
  chatAberto.value = true
}

async function enviarAvaliacao() {
  if (!agendamento.value || notaServico.value < 1 || notaPrestador.value < 1) {
    alert("Informe as duas notas antes de enviar.")
    return
  }

  carregandoAvaliacaoCliente.value = true
  try {
    const { data } = await api.post<Avaliacao>("/api/Avaliacao/CriarAvaliacao", {
      agendamentoId: agendamento.value.id,
      clienteId: agendamento.value.clienteId,
      prestadorId: agendamento.value.prestadorId,
      notaServico: notaServico.value,
      notaPrestador: notaPrestador.value,
      comentario: comentarioAvaliacao.value || null,
    })

    avaliacaoPrestadorExistente.value = data
    clienteAvaliacaoEnviada.value = true
    mostrarModalAvaliacaoCliente.value = false
    comentarioAvaliacao.value = ""
    notaServico.value = 0
    notaPrestador.value = 0
    await buscarDetalhes()
  } catch {
    alert("Erro ao enviar avaliação. Tente novamente.")
  } finally {
    carregandoAvaliacaoCliente.value = false
  }
}

function obterStatusRetornoCheckout() {
  const statusQuery =
    String(route.query.status ?? "") ||
    String(route.query.collection_status ?? "") ||
    String(route.query.payment_status ?? "")

  return statusQuery ? statusQuery.toLowerCase() : null
}

function formatarEndereco(endereco: AgendamentoResumo["endereco"]) {
  if (endereco.descricao) {
    const cidadeEstado = [endereco.cidade, endereco.estado].filter(Boolean).join("/")
    return cidadeEstado ? `${endereco.descricao} - ${cidadeEstado}` : endereco.descricao
  }

  const partes = [endereco.logradouro, endereco.bairro].filter(Boolean).join(", ")
  const cidadeEstado = [endereco.cidade, endereco.estado].filter(Boolean).join("/")
  return [partes, cidadeEstado].filter(Boolean).join(" - ")
}

async function enviarAvaliacaoPrestador() {
  if (!agendamento.value || notaCliente.value < 1) {
    alert("Informe a nota antes de enviar.")
    return
  }

  carregandoAvaliacaoPrestador.value = true
  try {
    const { data } = await api.post<AvaliacaoCliente>("/api/Avaliacao/CriarAvaliacaoCliente", {
      agendamentoId: agendamento.value.id,
      clienteId: agendamento.value.clienteId,
      prestadorId: agendamento.value.prestadorId,
      nota: notaCliente.value,
      comentario: comentarioAvaliacaoCliente.value || null,
    })

    avaliacaoClienteExistente.value = data
    prestadorAvaliacaoEnviada.value = true
    mostrarModalAvaliacaoPrestador.value = false
    comentarioAvaliacaoCliente.value = ""
    notaCliente.value = 0
    await buscarDetalhes()
  } catch {
    alert("Erro ao enviar avaliacao. Tente novamente.")
  } finally {
    carregandoAvaliacaoPrestador.value = false
  }
}

function abrirAvaliacoesAutomaticamente() {
  if (!agendamento.value || pagamento.value?.status !== StatusPagamento.Aprovado) return
  if (agendamento.value.status !== StatusAgendamento.Concluido) return

  if (
    souOCliente.value &&
    !avaliacaoPrestadorExistente.value &&
    !clienteAvaliacaoEnviada.value &&
    agendamento.value.podeClienteAvaliarPrestador !== false
  ) {
    mostrarModalAvaliacaoCliente.value = true
  }

  if (
    souOPrestador.value &&
    !avaliacaoClienteExistente.value &&
    !prestadorAvaliacaoEnviada.value &&
    agendamento.value.podePrestadorAvaliarCliente !== false
  ) {
    mostrarModalAvaliacaoPrestador.value = true
  }
}

const STATUS_LABEL: Record<StatusAgendamento, string> = {
  [StatusAgendamento.Aceito]: "Aceito",
  [StatusAgendamento.Solicitado]: "Solicitado",
  [StatusAgendamento.EmAndamento]: "Em andamento",
  [StatusAgendamento.Concluido]: "Concluído",
  [StatusAgendamento.Cancelado]: "Cancelado",
  [StatusAgendamento.Recusado]: "Recusado",
  [StatusAgendamento.AguardandoPagamento]: "Aguardando pagamento",
}

const STATUS_PAGAMENTO_LABEL: Record<StatusPagamento, string> = {
  [StatusPagamento.Pendente]: "Pagamento pendente",
  [StatusPagamento.Processando]: "Checkout iniciado",
  [StatusPagamento.Aprovado]: "Pagamento aprovado",
  [StatusPagamento.Recusado]: "Pagamento recusado",
  [StatusPagamento.Estornado]: "Pagamento estornado",
}

function obterVariantStatus(status: StatusAgendamento): HtBadgeVariant {
  switch (status) {
    case StatusAgendamento.Aceito:
      return HtBadgeVariant.Primary
    case StatusAgendamento.Concluido:
      return HtBadgeVariant.Success
    case StatusAgendamento.AguardandoPagamento:
      return HtBadgeVariant.Default
    case StatusAgendamento.Cancelado:
    case StatusAgendamento.Recusado:
      return HtBadgeVariant.Error
    case StatusAgendamento.Solicitado:
      return HtBadgeVariant.Outline
    default:
      return HtBadgeVariant.Default
  }
}
</script>

