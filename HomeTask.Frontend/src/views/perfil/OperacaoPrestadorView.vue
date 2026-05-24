<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import api from "@/services/api";
import type { AgendamentoResumo, StatusAgendamento } from "@/types";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";

const carregando = ref(true);
const agendamentos = ref<AgendamentoResumo[]>([]);

type StatusAgendamentoNumero = 1 | 2 | 3 | 4 | 5 | 6;
const STATUS_NUMERO_PARA_TEXTO: Record<StatusAgendamentoNumero, StatusAgendamento> = {
  1: "Solicitado",
  2: "Aceito",
  3: "Recusado",
  4: "EmAndamento",
  5: "Concluido",
  6: "Cancelado",
};

const STATUS_VARIANT: Record<
  StatusAgendamento,
  "default" | "primary" | "success" | "error" | "outline"
> = {
  Confirmado: "primary",
  Solicitado: "outline",
  Aceito: "primary",
  EmAndamento: "default",
  Concluido: "success",
  Cancelado: "error",
  Recusado: "error",
};

const agendados = computed(() =>
  agendamentos.value
    .filter((agendamento) => {
      if (agendamento.status === "Aceito") return true;
      return (
        agendamento.status === "Solicitado" &&
        agendamento.aguardandoRespostaDe === "Prestador"
      );
    })
    .sort(ordenarPorDataAsc),
);

const emAndamento = computed(() =>
  filtrarPorStatus(["EmAndamento"]).sort(ordenarPorDataAsc),
);

const concluidos = computed(() =>
  filtrarPorStatus(["Concluido"]).sort(ordenarPorDataDesc),
);

onMounted(async () => {
  try {
    const { data } = await api.get<AgendamentoResumo[]>(
      "/api/Agendamento/ObterMeusAgendamentosPrestador",
    );
    agendamentos.value = data.map(normalizarAgendamento);
  } finally {
    carregando.value = false;
  }
});

function filtrarPorStatus(statuses: StatusAgendamento[]) {
  return agendamentos.value.filter((agendamento) => statuses.includes(agendamento.status));
}

function ordenarPorDataAsc(a: AgendamentoResumo, b: AgendamentoResumo) {
  return new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime();
}

function ordenarPorDataDesc(a: AgendamentoResumo, b: AgendamentoResumo) {
  return new Date(b.dataHoraAgendada).getTime() - new Date(a.dataHoraAgendada).getTime();
}

function formatarData(iso: string) {
  return new Date(iso).toLocaleDateString("pt-BR", {
    day: "2-digit",
    month: "2-digit",
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

function labelStatus(status: StatusAgendamento) {
  if (status === "Solicitado") return "Aguardando sua resposta";
  if (status === "Aceito") return "Pronto para iniciar";
  if (status === "EmAndamento") return "Em andamento";
  if (status === "Concluido") return "Concluído";
  return status;
}

function normalizarStatus(status: unknown): StatusAgendamento {
  if (typeof status === "number" && status in STATUS_NUMERO_PARA_TEXTO) {
    return STATUS_NUMERO_PARA_TEXTO[status as StatusAgendamentoNumero];
  }

  if (status === "Confirmado") return "Aceito";
  if (
    status === "Solicitado" ||
    status === "Aceito" ||
    status === "EmAndamento" ||
    status === "Concluido" ||
    status === "Cancelado" ||
    status === "Recusado"
  ) {
    return status;
  }

  return "Solicitado";
}

function normalizarAgendamento(data: AgendamentoResumo): AgendamentoResumo {
  const seguro = data as Partial<AgendamentoResumo> & { status?: unknown };

  return {
    id: seguro.id ?? "",
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
</script>

<template>
  <div class="container mx-auto px-4 py-8 max-w-5xl">
    <div class="flex flex-wrap items-center justify-between gap-3 mb-8">
      <div>
        <h1 class="text-2xl font-bold">Minha operação</h1>
        <p class="text-sm text-base-content/60">
          Solicitações para responder e serviços em execução.
        </p>
      </div>
    </div>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <template v-else>
      <section class="mb-10">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-title font-semibold">Agendados</h2>
          <span class="text-sm text-base-content/50">{{ agendados.length }}</span>
        </div>

        <div
          v-if="agendados.length === 0"
          class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl"
        >
          Nenhuma solicitação pendente ou serviço agendado.
        </div>

        <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <HtCard
            v-for="ag in agendados"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="$router.push({ path: `/agendamento/detalhes/${ag.id}`, query: { origem: 'operacao' } })"
          >
            <div class="flex items-start justify-between gap-3 mb-3">
              <div>
                <p class="font-semibold">{{ ag.clienteNome }}</p>
                <p class="text-sm text-base-content/60">{{ ag.servicos.map((s) => s.titulo).join(", ") }}</p>
              </div>
              <HtBadge :variant="STATUS_VARIANT[ag.status]">{{ labelStatus(ag.status) }}</HtBadge>
            </div>

            <div class="space-y-2 text-sm text-base-content/80">
              <div class="flex items-center gap-2">
                <span class="material-symbols-rounded text-base text-base-content/40">calendar_today</span>
                <span>{{ formatarData(ag.dataHoraAgendada) }} às {{ formatarHora(ag.dataHoraAgendada) }}</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="material-symbols-rounded text-base text-base-content/40">location_on</span>
                <span class="truncate">{{ formatarEndereco(ag.endereco) }}</span>
              </div>
            </div>

            <div class="flex items-center justify-between mt-4">
              <span class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</span>
              <span class="text-sm text-primary">
                {{ ag.status === "Solicitado" ? "Abrir para responder" : "Abrir para iniciar" }}
              </span>
            </div>
          </HtCard>
        </div>
      </section>

      <section class="mb-10">
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-title font-semibold">Em andamento</h2>
          <span class="text-sm text-base-content/50">{{ emAndamento.length }}</span>
        </div>

        <div
          v-if="emAndamento.length === 0"
          class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl"
        >
          Nenhum serviço em andamento no momento.
        </div>

        <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <HtCard
            v-for="ag in emAndamento"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="$router.push({ path: `/agendamento/detalhes/${ag.id}`, query: { origem: 'operacao' } })"
          >
            <div class="flex items-start justify-between gap-3 mb-3">
              <div>
                <p class="font-semibold">{{ ag.clienteNome }}</p>
                <p class="text-sm text-base-content/60">{{ ag.servicos.map((s) => s.titulo).join(", ") }}</p>
              </div>
              <HtBadge :variant="STATUS_VARIANT[ag.status]">{{ labelStatus(ag.status) }}</HtBadge>
            </div>

            <div class="space-y-2 text-sm text-base-content/80">
              <div class="flex items-center gap-2">
                <span class="material-symbols-rounded text-base text-base-content/40">calendar_today</span>
                <span>{{ formatarData(ag.dataHoraAgendada) }} às {{ formatarHora(ag.dataHoraAgendada) }}</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="material-symbols-rounded text-base text-base-content/40">location_on</span>
                <span class="truncate">{{ formatarEndereco(ag.endereco) }}</span>
              </div>
            </div>

            <div class="flex items-center justify-between mt-4">
              <span class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</span>
              <span class="text-sm text-primary">Abrir para concluir</span>
            </div>
          </HtCard>
        </div>
      </section>

      <section>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-title font-semibold">Concluídos</h2>
          <span class="text-sm text-base-content/50">{{ concluidos.length }}</span>
        </div>

        <div
          v-if="concluidos.length === 0"
          class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl"
        >
          Nenhum serviço concluído ainda.
        </div>

        <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <HtCard
            v-for="ag in concluidos"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="$router.push({ path: `/agendamento/detalhes/${ag.id}`, query: { origem: 'operacao' } })"
          >
            <div class="flex items-start justify-between gap-3 mb-3">
              <div>
                <p class="font-semibold">{{ ag.clienteNome }}</p>
                <p class="text-sm text-base-content/60">{{ ag.servicos.map((s) => s.titulo).join(", ") }}</p>
              </div>
              <HtBadge :variant="STATUS_VARIANT[ag.status]">{{ labelStatus(ag.status) }}</HtBadge>
            </div>

            <div class="space-y-2 text-sm text-base-content/80">
              <div class="flex items-center gap-2">
                <span class="material-symbols-rounded text-base text-base-content/40">calendar_today</span>
                <span>{{ formatarData(ag.dataHoraAgendada) }} às {{ formatarHora(ag.dataHoraAgendada) }}</span>
              </div>
              <div class="flex items-center gap-2">
                <span class="material-symbols-rounded text-base text-base-content/40">payments</span>
                <span>{{ formatarMoeda(ag.valorTotal) }}</span>
              </div>
            </div>
          </HtCard>
        </div>
      </section>
    </template>
  </div>
</template>
