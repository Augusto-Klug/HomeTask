<template>
  <div class="container mx-auto px-4 py-8 max-w-3xl">
    <router-link
      :to="rotaVolta"
      class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
    >
      <span class="material-symbols-rounded text-base">arrow_back</span>
      {{ textoVolta }}
    </router-link>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <div v-else-if="erro" class="text-center py-16">
      <p class="text-error mb-4">{{ erro }}</p>
      <HtButton @click="buscarDetalhes">Tentar novamente</HtButton>
    </div>

    <template v-else-if="agendamento">
      <div class="flex flex-col gap-6">
        <HtAlert
          v-if="checkoutRetorno"
          :variant="checkoutRetorno.variant"
          :title="checkoutRetorno.title"
        >
          {{ checkoutRetorno.message }}
        </HtAlert>

        <HtCard>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs text-muted uppercase tracking-wider mb-1">Status do agendamento</p>
              <HtBadge :variant="obterVariantStatus(agendamento.status)" class="text-base px-3 py-1">
                {{ statusLabel }}
              </HtBadge>
            </div>
            <div class="text-right">
              <p class="text-xs text-muted uppercase tracking-wider mb-1">Valor total</p>
              <p class="text-xl font-bold">{{ formatarMoeda(agendamento.valorTotal) }}</p>
            </div>
          </div>

          <div
            v-if="agendamento.motivoRecusa"
            class="mt-4 p-3 bg-error/10 border border-error/20 rounded-lg"
          >
            <p class="text-sm font-semibold text-error">Motivo:</p>
            <p class="text-sm text-error/80">{{ agendamento.motivoRecusa }}</p>
          </div>
        </HtCard>

        <HtCard v-if="agendamento.status === StatusAgendamento.AguardandoPagamento">
          <div class="flex items-start justify-between gap-4">
            <div>
              <p class="text-xs text-muted uppercase tracking-wider mb-1">Pagamento</p>
              <p class="font-semibold">{{ labelPagamento }}</p>
              <p class="text-sm text-muted mt-1">
                {{ descricaoPagamento }}
              </p>
            </div>
            <HtBadge :variant="badgePagamento">{{ labelPagamento }}</HtBadge>
          </div>
        </HtCard>

        <HtCard>
          <h2 class="text-lg font-bold mb-4">Servicos agendados</h2>
          <div
            v-for="s in agendamento.servicos"
            :key="s.id"
            class="flex justify-between items-center py-2 border-b last:border-0"
          >
            <div>
              <p class="font-semibold">{{ s.titulo }}</p>
            </div>
            <p class="font-medium">{{ formatarPrecoServico(s.precoBase, s.unidadeCobranca) }}</p>
          </div>
        </HtCard>

        <HtCard>
          <h2 class="text-lg font-bold mb-4">Quando e onde</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="flex items-start gap-3">
              <span class="material-symbols-rounded text-primary mt-0.5">calendar_today</span>
              <div>
                <p class="text-sm font-semibold">Data e horario</p>
                <p class="text-sm text-muted">
                  {{ formatarDataLonga(agendamento.dataHoraAgendada) }} as
                  {{ formatarHora(agendamento.dataHoraAgendada) }}
                </p>
                <p class="text-xs text-muted mt-1">Duracao estimada: {{ agendamento.duracaoMinutos }} min</p>
              </div>
            </div>
            <div class="flex items-start gap-3">
              <span class="material-symbols-rounded text-primary mt-0.5">location_on</span>
              <div>
                <p class="text-sm font-semibold">Local de realizacao</p>
                <p class="text-sm text-muted">{{ formatarEndereco(agendamento.endereco) }}</p>
              </div>
            </div>
          </div>
          <div v-if="agendamento.observacoes" class="mt-4 p-3 bg-muted/20 rounded-lg">
            <p class="text-sm font-semibold mb-1">Observacoes:</p>
            <p class="text-sm text-muted">{{ agendamento.observacoes }}</p>
          </div>
        </HtCard>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <HtCard>
            <p class="text-xs text-muted uppercase tracking-wider mb-2">Cliente</p>
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-primary/10 flex items-center justify-center text-primary font-bold">
                {{ agendamento.clienteNome.charAt(0) }}
              </div>
              <p class="font-semibold">{{ agendamento.clienteNome }}</p>
            </div>
          </HtCard>
          <HtCard>
            <p class="text-xs text-muted uppercase tracking-wider mb-2">Prestador</p>
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-full bg-secondary/10 flex items-center justify-center text-secondary font-bold">
                {{ agendamento.prestadorNome.charAt(0) }}
              </div>
              <p class="font-semibold">{{ agendamento.prestadorNome }}</p>
            </div>
          </HtCard>
        </div>

        <div class="flex flex-wrap gap-3 mt-4">
          <template v-if="podeAceitar">
            <HtButton @click="acaoAgendamento('Aceitar')" :loading="carregandoAcao" class="flex-1">
              {{ agendamento.aguardandoRespostaDe === TipoUsuario.Cliente ? "Aceitar proposta" : "Aceitar agendamento" }}
            </HtButton>
            <HtButton
              @click="mostrarModalRecusa = true"
              variant="outline"
              class="flex-1 border-error text-error hover:bg-error/10"
            >
              Recusar
            </HtButton>
          </template>

          <template v-if="souOPrestador">
            <HtButton
              v-if="agendamento.status === StatusAgendamento.Aceito"
              @click="acaoAgendamento('Iniciar')"
              :loading="carregandoAcao"
              class="flex-1"
            >
              Iniciar servico
            </HtButton>
            <HtButton
              v-if="agendamento.status === StatusAgendamento.EmAndamento"
              @click="acaoAgendamento('Concluir')"
              :loading="carregandoAcao"
              class="flex-1"
              variant="success"
            >
              Concluir servico
            </HtButton>
          </template>

          <HtButton
            v-if="podePagar"
            @click="realizarPagamento"
            :loading="carregandoPagamento"
            class="flex-1"
            variant="success"
          >
            {{ textoBotaoPagamento }}
          </HtButton>

          <HtButton
            v-if="podeCancelar"
            @click="mostrarModalCancelamento = true"
            variant="outline"
            class="flex-1"
          >
            Cancelar agendamento
          </HtButton>
        </div>
      </div>
    </template>

    <div
      v-if="mostrarModalRecusa"
      class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50"
    >
      <HtCard class="w-full max-w-md">
        <h3 class="text-lg font-bold mb-4">Recusar agendamento</h3>
        <HtTextarea v-model="motivo" label="Motivo da recusa" placeholder="Explique o motivo..." required />
        <div class="flex gap-3 mt-6">
          <HtButton variant="outline" @click="mostrarModalRecusa = false" class="flex-1">Voltar</HtButton>
          <HtButton variant="error" @click="acaoAgendamento('Recusar')" :loading="carregandoAcao" class="flex-1">
            Confirmar recusa
          </HtButton>
        </div>
      </HtCard>
    </div>

    <div
      v-if="mostrarModalCancelamento"
      class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50"
    >
      <HtCard class="w-full max-w-md">
        <h3 class="text-lg font-bold mb-4">Cancelar agendamento</h3>
        <HtTextarea v-model="motivo" label="Motivo do cancelamento" placeholder="Por que deseja cancelar?" required />
        <div class="flex gap-3 mt-6">
          <HtButton variant="outline" @click="mostrarModalCancelamento = false" class="flex-1">Voltar</HtButton>
          <HtButton variant="error" @click="acaoAgendamento('Cancelar')" :loading="carregandoAcao" class="flex-1">
            Confirmar cancelamento
          </HtButton>
        </div>
      </HtCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute } from "vue-router";
import { useAuthStore } from "@/stores/auth";
import api from "@/services/api";
import { StatusAgendamento, StatusPagamento, TipoUsuario, type AgendamentoResumo, type PagamentoResumo } from "@/types";
import { formatarDataLonga, formatarHora, formatarMoeda, formatarPrecoServico } from "@/shared/utils";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtBadge, { HtBadgeVariant } from "@/components/ui/HtBadge.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";

const route = useRoute();
const auth = useAuthStore();

const id = route.params.id as string;
const agendamento = ref<AgendamentoResumo | null>(null);
const pagamento = ref<PagamentoResumo | null>(null);
const carregando = ref(true);
const carregandoAcao = ref(false);
const carregandoPagamento = ref(false);
const erro = ref<string | null>(null);
const motivo = ref("");
const mostrarModalRecusa = ref(false);
const mostrarModalCancelamento = ref(false);
const clienteAtualId = ref<string | null>(null);
const prestadorAtualId = ref<string | null>(null);
const retornoCheckoutVisivel = ref(false);
const statusRetornoCheckout = ref<string | null>(null);

const rotaVolta = computed(() => {
  if (route.query.origem === "operacao") return "/perfil/agendamentos-prestador";
  if (route.query.origem === "pendentes") return "/perfil/agendamentos-prestador";
  return "/perfil/agendamentos-cliente";
});

const textoVolta = computed(() => {
  if (route.query.origem === "operacao") return "Voltar para minha operacao";
  if (route.query.origem === "pendentes") return "Voltar para minha operacao";
  return "Voltar para meus agendamentos";
});

const souOPrestador = computed(() =>
  !!agendamento.value && prestadorAtualId.value === String(agendamento.value.prestadorId),
);

const souOCliente = computed(() =>
  !!agendamento.value && clienteAtualId.value === String(agendamento.value.clienteId),
);

const podeAceitar = computed(() => {
  if (!agendamento.value || agendamento.value.status !== StatusAgendamento.Solicitado) return false;
  if (agendamento.value.aguardandoRespostaDe === TipoUsuario.Cliente) return souOCliente.value;
  return souOPrestador.value;
});

const podeCancelar = computed(() => {
  if (!agendamento.value) return false;
  return agendamento.value.status === StatusAgendamento.Solicitado || agendamento.value.status === StatusAgendamento.Aceito;
});

const podePagar = computed(() =>
  !!agendamento.value &&
  souOCliente.value &&
  agendamento.value.status === StatusAgendamento.AguardandoPagamento &&
  pagamento.value?.status !== StatusPagamento.Aprovado,
);

const checkoutRetorno = computed(() => {
  if (!retornoCheckoutVisivel.value) return null;

  if (pagamento.value?.status === StatusPagamento.Aprovado || agendamento.value?.status === StatusAgendamento.Concluido) {
    return {
      variant: "success",
      title: "Pagamento aprovado",
      message: "Pagamento confirmado com sucesso. O agendamento foi concluido.",
    } as const;
  }

  switch (statusRetornoCheckout.value) {
    case "rejected":
    case "cancelled":
    case "failure":
      return {
        variant: "error",
        title: "Pagamento nao concluido",
        message: "O Mercado Pago informou que o pagamento nao foi concluido. Voce pode tentar novamente abaixo.",
      } as const;
    case "pending":
    case "in_process":
      return {
        variant: "info",
        title: "Pagamento em processamento",
        message: "O pagamento foi iniciado, mas a confirmacao oficial ainda depende do gateway.",
      } as const;
    default:
      return {
        variant: "info",
        title: "Retorno do checkout recebido",
        message: "O retorno do checkout nao confirma o pagamento sozinho. A conclusao oficial depende do gateway.",
      } as const;
  }
});

const statusLabel = computed(() => {
  if (!agendamento.value) return "";
  if (agendamento.value.status === StatusAgendamento.Solicitado && agendamento.value.aguardandoRespostaDe === TipoUsuario.Cliente) {
    return "Pendente de aprovacao do cliente";
  }
  if (agendamento.value.status === StatusAgendamento.Solicitado && agendamento.value.aguardandoRespostaDe === TipoUsuario.Prestador) {
    return "Pendente de resposta do prestador";
  }
  return STATUS_LABEL[agendamento.value.status];
});

const labelPagamento = computed(() => {
  if (!pagamento.value) return "Pagamento pendente";
  return STATUS_PAGAMENTO_LABEL[pagamento.value.status];
});

const descricaoPagamento = computed(() => {
  if (pagamento.value?.status === StatusPagamento.Aprovado) {
    return "Pagamento confirmado. O servico foi concluido com sucesso.";
  }

  if (pagamento.value?.status === StatusPagamento.Recusado || statusRetornoCheckout.value === "rejected") {
    return "O pagamento foi recusado. Revise os dados e tente iniciar o checkout novamente.";
  }

  if (pagamento.value?.status === StatusPagamento.Processando) {
    return "O servico foi concluido pelo prestador e aguarda confirmacao oficial do gateway.";
  }

  return "O servico foi concluido pelo prestador e aguarda pagamento do cliente.";
});

const textoBotaoPagamento = computed(() => {
  if (pagamento.value?.status === StatusPagamento.Recusado || statusRetornoCheckout.value === "rejected") {
    return "Tentar novamente";
  }

  return "Realizar pagamento";
});

const badgePagamento = computed(() => {
  if (!pagamento.value) return HtBadgeVariant.Outline;
  switch (pagamento.value.status) {
    case StatusPagamento.Aprovado:
      return HtBadgeVariant.Success;
    case StatusPagamento.Recusado:
      return HtBadgeVariant.Error;
    case StatusPagamento.Processando:
      return HtBadgeVariant.Primary;
    default:
      return HtBadgeVariant.Outline;
  }
});

onMounted(async () => {
  retornoCheckoutVisivel.value = route.query.retornoCheckout === "1";
  statusRetornoCheckout.value = obterStatusRetornoCheckout();
  await Promise.all([carregarAtorAtual(), reconciliarRetornoCheckout()]);
  await buscarDetalhes();
});

async function carregarAtorAtual() {
  if (!auth.user?.userId) return;

  const [cliente, prestador] = await Promise.allSettled([
    api.get("/api/Cliente/ObterClientesPorUsuarioId", { params: { usuarioId: auth.user.userId } }),
    api.get("/api/Prestador/ObterPrestadorPorUsuarioId", { params: { usuarioId: auth.user.userId } }),
  ]);

  if (cliente.status === "fulfilled") {
    clienteAtualId.value = String((cliente.value.data as { id?: string }).id ?? "");
  }

  if (prestador.status === "fulfilled") {
    prestadorAtualId.value = String((prestador.value.data as { id?: string }).id ?? "");
  }
}

async function buscarDetalhes() {
  carregando.value = true;
  erro.value = null;
  try {
    const { data } = await api.get<AgendamentoResumo>("/api/Agendamento/ObterAgendamentoPorId", { params: { id } });
    agendamento.value = data;
    await buscarPagamento();
  } catch {
    erro.value = "Nao foi possivel carregar os detalhes do agendamento.";
  } finally {
    carregando.value = false;
  }
}

async function buscarPagamento() {
  try {
    const { data } = await api.get<PagamentoResumo>("/api/Pagamento/ObterPagamentoPorAgendamento", {
      params: { agendamentoId: id },
    });
    pagamento.value = data;
  } catch {
    pagamento.value = null;
  }
}

async function reconciliarRetornoCheckout() {
  if (!retornoCheckoutVisivel.value) return;

  const pagamentoExternoId =
    String(route.query.payment_id ?? "") ||
    String(route.query.collection_id ?? "");

  if (!pagamentoExternoId) return;

  try {
    await api.post("/api/Pagamento/ReconciliarPagamento", {
      pagamentoExternoId,
    });
  } catch {
    // O retorno do checkout melhora a UX, mas o webhook continua sendo a fonte oficial.
  }
}

async function acaoAgendamento(acao: string) {
  carregandoAcao.value = true;
  try {
    const payload: Record<string, unknown> = { id };
    if (acao === "Recusar" || acao === "Cancelar") {
      payload.motivoRecusa = motivo.value;
    }

    await api.post(`/api/Agendamento/${acao}Agendamento`, payload);
    mostrarModalRecusa.value = false;
    mostrarModalCancelamento.value = false;
    motivo.value = "";
    await buscarDetalhes();
  } catch {
    alert("Erro ao realizar a acao. Tente novamente.");
  } finally {
    carregandoAcao.value = false;
  }
}

async function realizarPagamento() {
  carregandoPagamento.value = true;
  try {
    const { data } = await api.post<PagamentoResumo>("/api/Pagamento/IniciarPagamento", { agendamentoId: id });
    pagamento.value = data;
    if (data.checkoutUrl) {
      window.location.href = data.checkoutUrl;
      return;
    }
    alert("Nao foi possivel iniciar o checkout.");
  } catch {
    alert("Erro ao iniciar pagamento. Tente novamente.");
  } finally {
    carregandoPagamento.value = false;
  }
}

function obterStatusRetornoCheckout() {
  const statusQuery =
    String(route.query.status ?? "") ||
    String(route.query.collection_status ?? "") ||
    String(route.query.payment_status ?? "");

  return statusQuery ? statusQuery.toLowerCase() : null;
}

function formatarEndereco(endereco: AgendamentoResumo["endereco"]) {
  return `${endereco.logradouro}, ${endereco.bairro} - ${endereco.cidade}/${endereco.estado}`;
}

const STATUS_LABEL: Record<StatusAgendamento, string> = {
  [StatusAgendamento.Aceito]: "Aceito",
  [StatusAgendamento.Solicitado]: "Solicitado",
  [StatusAgendamento.EmAndamento]: "Em andamento",
  [StatusAgendamento.Concluido]: "Concluido",
  [StatusAgendamento.Cancelado]: "Cancelado",
  [StatusAgendamento.Recusado]: "Recusado",
  [StatusAgendamento.AguardandoPagamento]: "Aguardando pagamento",
};

const STATUS_PAGAMENTO_LABEL: Record<StatusPagamento, string> = {
  [StatusPagamento.Pendente]: "Pagamento pendente",
  [StatusPagamento.Processando]: "Checkout iniciado",
  [StatusPagamento.Aprovado]: "Pagamento aprovado",
  [StatusPagamento.Recusado]: "Pagamento recusado",
  [StatusPagamento.Estornado]: "Pagamento estornado",
};

function obterVariantStatus(status: StatusAgendamento): HtBadgeVariant {
  switch (status) {
    case StatusAgendamento.Aceito:
      return HtBadgeVariant.Primary;
    case StatusAgendamento.Concluido:
      return HtBadgeVariant.Success;
    case StatusAgendamento.AguardandoPagamento:
      return HtBadgeVariant.Default;
    case StatusAgendamento.Cancelado:
    case StatusAgendamento.Recusado:
      return HtBadgeVariant.Error;
    case StatusAgendamento.Solicitado:
      return HtBadgeVariant.Outline;
    default:
      return HtBadgeVariant.Default;
  }
}
</script>
