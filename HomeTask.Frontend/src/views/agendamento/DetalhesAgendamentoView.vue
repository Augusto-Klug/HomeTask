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
        <HtCard>
          <div class="flex items-center justify-between">
            <div>
              <p class="text-xs text-muted uppercase tracking-wider mb-1">Status do Agendamento</p>
              <HtBadge :variant="STATUS_VARIANT[agendamento.status]" class="text-base px-3 py-1">
                {{ STATUS_LABEL[agendamento.status] }}
              </HtBadge>
            </div>
            <div class="text-right">
              <p class="text-xs text-muted uppercase tracking-wider mb-1">Valor Total</p>
              <p class="text-xl font-bold">{{ formatarMoeda(agendamento.valorTotal) }}</p>
            </div>
          </div>

          <div v-if="agendamento.motivoRecusa" class="mt-4 p-3 bg-error/10 border border-error/20 rounded-lg">
            <p class="text-sm font-semibold text-error">Motivo:</p>
            <p class="text-sm text-error/80">{{ agendamento.motivoRecusa }}</p>
          </div>
        </HtCard>

        <HtCard>
          <h2 class="text-lg font-bold mb-4">Serviços Agendados</h2>
          <div
            v-for="s in agendamento.servicos"
            :key="s.id"
            class="flex justify-between items-center py-2 border-b last:border-0"
          >
            <div>
              <p class="font-semibold">{{ s.titulo }}</p>
            </div>
            <p class="font-medium">{{ formatarMoeda(s.precoBase) }}</p>
          </div>
        </HtCard>

        <HtCard>
          <h2 class="text-lg font-bold mb-4">Quando e Onde</h2>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="flex items-start gap-3">
              <span class="material-symbols-rounded text-primary mt-0.5">calendar_today</span>
              <div>
                <p class="text-sm font-semibold">Data e Hora</p>
                <p class="text-sm text-muted">
                  {{ formatarData(agendamento.dataHoraAgendada) }} às
                  {{ formatarHora(agendamento.dataHoraAgendada) }}
                </p>
                <p class="text-xs text-muted mt-1">
                  Duração estimada: {{ agendamento.duracaoMinutos }} min
                </p>
              </div>
            </div>
            <div class="flex items-start gap-3">
              <span class="material-symbols-rounded text-primary mt-0.5">location_on</span>
              <div>
                <p class="text-sm font-semibold">Local de Realização</p>
                <p class="text-sm text-muted">{{ formatarEndereco(agendamento.endereco) }}</p>
              </div>
            </div>
          </div>

          <div v-if="agendamento.observacoes" class="mt-4 p-3 bg-muted/20 rounded-lg">
            <p class="text-sm font-semibold mb-1">Observações:</p>
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
          <template v-if="podeAceitar || podeRecusar || souOPrestador">
            <HtButton
              v-if="podeAceitar"
              @click="acaoAgendamento('Aceitar')"
              :loading="carregandoAcao"
              class="flex-1"
            >
              {{ agendamento.aguardandoRespostaDe === "Cliente" ? "Aceitar Proposta" : "Aceitar Agendamento" }}
            </HtButton>
            <HtButton
              v-if="podeRecusar"
              @click="mostrarModalRecusa = true"
              variant="outline"
              class="flex-1 border-error text-error hover:bg-error/10"
            >
              Recusar
            </HtButton>
            <HtButton
              v-if="souOPrestador && agendamento.status === 'Aceito'"
              @click="acaoAgendamento('Iniciar')"
              :loading="carregandoAcao"
              class="flex-1"
            >
              Iniciar Serviço
            </HtButton>
            <HtButton
              v-if="souOPrestador && agendamento.status === 'EmAndamento'"
              @click="acaoAgendamento('Concluir')"
              :loading="carregandoAcao"
              class="flex-1"
              variant="success"
            >
              Concluir Serviço
            </HtButton>
          </template>

          <HtButton
            v-if="podeCancelar"
            @click="mostrarModalCancelamento = true"
            variant="outline"
            class="flex-1"
          >
            Cancelar Agendamento
          </HtButton>
        </div>
      </div>
    </template>

    <div v-if="mostrarModalRecusa" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <HtCard class="w-full max-w-md">
        <h3 class="text-lg font-bold mb-4">Recusar Agendamento</h3>
        <HtTextarea v-model="motivo" label="Motivo da recusa" placeholder="Explique o motivo..." required />
        <div class="flex gap-3 mt-6">
          <HtButton variant="outline" @click="mostrarModalRecusa = false" class="flex-1">Voltar</HtButton>
          <HtButton variant="error" @click="acaoAgendamento('Recusar')" :loading="carregandoAcao" class="flex-1">
            Confirmar Recusa
          </HtButton>
        </div>
      </HtCard>
    </div>

    <div v-if="mostrarModalCancelamento" class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50">
      <HtCard class="w-full max-w-md">
        <h3 class="text-lg font-bold mb-4">Cancelar Agendamento</h3>
        <HtTextarea v-model="motivo" label="Motivo do cancelamento" placeholder="Por que deseja cancelar?" required />
        <div class="flex gap-3 mt-6">
          <HtButton variant="outline" @click="mostrarModalCancelamento = false" class="flex-1">Voltar</HtButton>
          <HtButton variant="error" @click="acaoAgendamento('Cancelar')" :loading="carregandoAcao" class="flex-1">
            Confirmar Cancelamento
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
import type { AgendamentoResumo, StatusAgendamento } from "@/types";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";

const route = useRoute();
const auth = useAuthStore();

const id = route.params.id as string;
const agendamento = ref<AgendamentoResumo | null>(null);
const carregando = ref(true);
const carregandoAcao = ref(false);
const erro = ref<string | null>(null);
const motivo = ref("");
const mostrarModalRecusa = ref(false);
const mostrarModalCancelamento = ref(false);
const clienteAtualId = ref<string | null>(null);
const prestadorAtualId = ref<string | null>(null);

type StatusAgendamentoNumero = 1 | 2 | 3 | 4 | 5 | 6;
const STATUS_NUMERO_PARA_TEXTO: Record<StatusAgendamentoNumero, StatusAgendamento> = {
  1: "Solicitado",
  2: "Aceito",
  3: "Recusado",
  4: "EmAndamento",
  5: "Concluido",
  6: "Cancelado",
};

const STATUS_LABEL: Record<StatusAgendamento, string> = {
  Aceito: "Aceito",
  Solicitado: "Solicitado",
  Confirmado: "Confirmado",
  EmAndamento: "Em andamento",
  Concluido: "Concluído",
  Cancelado: "Cancelado",
  Recusado: "Recusado",
};

const STATUS_VARIANT: Record<
  StatusAgendamento,
  "default" | "primary" | "success" | "error" | "outline"
> = {
  Aceito: "success",
  Solicitado: "outline",
  Confirmado: "primary",
  EmAndamento: "default",
  Concluido: "success",
  Cancelado: "error",
  Recusado: "error",
};

const rotaVolta = computed(() => {
  if (route.query.origem === "operacao") {
    return "/perfil/operacao";
  }
  return route.query.origem === "pendentes"
    ? "/perfil/solicitacoes-pendentes"
    : "/perfil/agendamentos";
});

const textoVolta = computed(() => {
  if (route.query.origem === "operacao") {
    return "Voltar para minha operação";
  }
  return route.query.origem === "pendentes"
    ? "Voltar para solicitações pendentes"
    : "Voltar para meus agendamentos";
});

const souOPrestador = computed(() => {
  return !!prestadorAtualId.value && agendamento.value?.prestadorId === prestadorAtualId.value;
});

const souOCliente = computed(() => {
  return !!clienteAtualId.value && agendamento.value?.clienteId === clienteAtualId.value;
});

const podeAceitar = computed(() => {
  if (!agendamento.value || agendamento.value.status !== "Solicitado") return false;
  return agendamento.value.aguardandoRespostaDe === "Cliente"
    ? souOCliente.value
    : souOPrestador.value;
});

const podeRecusar = computed(() => {
  if (!agendamento.value || agendamento.value.status !== "Solicitado") return false;
  return agendamento.value.aguardandoRespostaDe === "Cliente"
    ? souOCliente.value
    : souOPrestador.value;
});

const podeCancelar = computed(() => {
  if (!agendamento.value) return false;
  return agendamento.value.status === "Solicitado" || agendamento.value.status === "Aceito";
});

async function carregarPerfisDoUsuarioAtual() {
  if (!auth.user?.userId) {
    clienteAtualId.value = null;
    prestadorAtualId.value = null;
    return;
  }

  const requisicoes: Promise<void>[] = [];

  if (auth.user.tipo === 1 || auth.user.tipo === 3) {
    requisicoes.push(
      api
        .get("/api/Cliente/ObterClientesPorUsuarioId", {
          params: { usuarioId: auth.user.userId },
        })
        .then(({ data }) => {
          clienteAtualId.value = data?.id ? String(data.id) : null;
        })
        .catch(() => {
          clienteAtualId.value = null;
        }),
    );
  }

  if (auth.user.tipo === 2 || auth.user.tipo === 3) {
    requisicoes.push(
      api
        .get("/api/Prestador/ObterPrestadorPorUsuarioId", {
          params: { usuarioId: auth.user.userId },
        })
        .then(({ data }) => {
          prestadorAtualId.value = data?.id ? String(data.id) : null;
        })
        .catch(() => {
          prestadorAtualId.value = null;
        }),
    );
  }

  await Promise.all(requisicoes);
}

async function buscarDetalhes() {
  carregando.value = true;
  erro.value = null;

  try {
    const { data } = await api.get<AgendamentoResumo>("/api/Agendamento/ObterAgendamentoPorId", {
      params: { id },
    });
    agendamento.value = normalizarAgendamento(data);
  } catch {
    erro.value = "Não foi possível carregar os detalhes do agendamento.";
  } finally {
    carregando.value = false;
  }
}

async function acaoAgendamento(acao: string) {
  carregandoAcao.value = true;

  try {
    const payload: { id: string; motivoRecusa?: string } = { id };
    if (acao === "Recusar" || acao === "Cancelar") {
      payload.motivoRecusa = motivo.value;
    }

    await api.post(`/api/Agendamento/${acao}Agendamento`, payload);

    mostrarModalRecusa.value = false;
    mostrarModalCancelamento.value = false;
    motivo.value = "";

    await buscarDetalhes();
  } catch {
    alert("Erro ao realizar ação. Tente novamente.");
  } finally {
    carregandoAcao.value = false;
  }
}

function formatarData(iso: string) {
  return new Date(iso).toLocaleDateString("pt-BR", {
    weekday: "long",
    day: "2-digit",
    month: "long",
    year: "numeric",
  });
}

function formatarHora(iso: string) {
  return new Date(iso).toLocaleTimeString("pt-BR", {
    hour: "2-digit",
    minute: "2-digit",
  });
}

function formatarEndereco(endereco: AgendamentoResumo["endereco"]) {
  return `${endereco.logradouro}, ${endereco.bairro} — ${endereco.cidade}/${endereco.estado}`;
}

function formatarMoeda(valor: number) {
  return valor.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function normalizarStatus(status: unknown): StatusAgendamento {
  if (typeof status === "number" && status in STATUS_NUMERO_PARA_TEXTO) {
    return STATUS_NUMERO_PARA_TEXTO[status as StatusAgendamentoNumero];
  }

  if (typeof status === "string") {
    if (status === "Confirmado") return "Aceito";
    if (status in STATUS_LABEL) return status as StatusAgendamento;
  }

  return "Solicitado";
}

function normalizarAgendamento(data: AgendamentoResumo): AgendamentoResumo {
  const seguro = data as Partial<AgendamentoResumo> & { status?: unknown };

  return {
    id: seguro.id ?? id,
    clienteId: seguro.clienteId ?? "",
    clienteNome: seguro.clienteNome ?? "",
    prestadorId: seguro.prestadorId ?? "",
    prestadorNome: seguro.prestadorNome ?? "",
    dataHoraAgendada: seguro.dataHoraAgendada ?? new Date().toISOString(),
    duracaoMinutos: seguro.duracaoMinutos ?? 0,
    status: normalizarStatus(seguro.status),
    endereco: seguro.endereco ?? { logradouro: "", bairro: "", cidade: "", estado: "" },
    observacoes: seguro.observacoes ?? null,
    valorTotal: seguro.valorTotal ?? 0,
    dataSolicitacao: seguro.dataSolicitacao ?? new Date().toISOString(),
    dataResposta: seguro.dataResposta ?? null,
    dataConclusao: seguro.dataConclusao ?? null,
    motivoRecusa: seguro.motivoRecusa ?? null,
    aguardandoRespostaDe:
      seguro.aguardandoRespostaDe === "Cliente" || seguro.aguardandoRespostaDe === "Prestador"
        ? seguro.aguardandoRespostaDe
        : null,
    servicos: seguro.servicos ?? [],
  };
}

onMounted(async () => {
  await carregarPerfisDoUsuarioAtual();
  await buscarDetalhes();
});
</script>
