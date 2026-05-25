<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import api from "@/services/api";
import type { AgendamentoResumo, StatusAgendamento } from "@/types";
import HtCard from "@/components/ui/HtCard.vue";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";

type StatusAgendamentoNumero = 1 | 2 | 3 | 4 | 5 | 6;

const STATUS_NUMERO_PARA_TEXTO: Record<StatusAgendamentoNumero, StatusAgendamento> = {
  1: "Solicitado",
  2: "Aceito",
  3: "Recusado",
  4: "EmAndamento",
  5: "Concluido",
  6: "Cancelado",
};

const agendamentosCliente = ref<AgendamentoResumo[]>([]);
const carregando = ref(true);

const agora = new Date();

const clienteFiltrado = computed(() =>
  agendamentosCliente.value
    .filter((a) => new Date(a.dataHoraAgendada) >= agora)
    .sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime()),
);

onMounted(async () => {
  try {
    const { data } = await api.get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosCliente");
    agendamentosCliente.value = data.map(normalizarAgendamento);
  } finally {
    carregando.value = false;
  }
});

const STATUS_VARIANT: Record<StatusAgendamento, "default" | "primary" | "success" | "error" | "outline"> = {
  Confirmado: "primary",
  Solicitado: "outline",
  Aceito: "primary",
  EmAndamento: "default",
  Concluido: "success",
  Cancelado: "error",
  Recusado: "error",
};

function labelCliente(ag: AgendamentoResumo): string {
  if (ag.status === "Solicitado" && ag.aguardandoRespostaDe === "Cliente") {
    return "Aguardando sua aprovacao";
  }
  if (ag.status === "Solicitado" && ag.aguardandoRespostaDe === "Prestador") {
    return "Aguardando resposta do prestador";
  }
  if (ag.status === "Aceito") return "Agendado";
  if (ag.status === "Concluido") return "Concluido";
  if (ag.status === "EmAndamento") return "Em andamento";
  if (ag.status === "Recusado") return "Recusado";
  if (ag.status === "Cancelado") return "Cancelado";
  return "Solicitado";
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

function formatarEndereco(e: AgendamentoResumo["endereco"]) {
  return `${e.logradouro}, ${e.bairro} - ${e.cidade}/${e.estado}`;
}

function formatarMoeda(v: number) {
  return v.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
}

function normalizarStatus(status: unknown): StatusAgendamento {
  if (typeof status === "number" && status in STATUS_NUMERO_PARA_TEXTO) {
    return STATUS_NUMERO_PARA_TEXTO[status as StatusAgendamentoNumero];
  }

  if (typeof status === "string") {
    if (status === "Confirmado") return "Aceito";
    return status as StatusAgendamento;
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
    aguardandoRespostaDe: seguro.aguardandoRespostaDe ?? null,
    servicos: seguro.servicos ?? [],
  };
}
</script>

<template>
  <div class="container mx-auto px-4 py-8 max-w-3xl">
    <h1 class="text-2xl font-bold mb-8">Meus agendamentos</h1>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <section v-else>
      <h2 class="text-title font-semibold text-base-content/70 uppercase tracking-wide mb-4">Cliente</h2>

      <div v-if="clienteFiltrado.length === 0" class="text-sm text-base-content/50 py-6 text-center">
        Nenhum agendamento futuro pendente.
      </div>

      <div v-else class="flex flex-col gap-3">
        <HtCard v-for="ag in clienteFiltrado" :key="ag.id" @click="$router.push(`/agendamento/detalhes/${ag.id}`)" class="cursor-pointer hover:border-primary/50 transition-colors">
          <div class="flex items-start justify-between gap-3">
            <div class="flex-1 min-w-0">
              <div class="flex items-center gap-2 mb-1 flex-wrap">
                <span class="font-semibold text-sm">{{ ag.prestadorNome }}</span>
                <HtBadge :variant="STATUS_VARIANT[ag.status]">{{ labelCliente(ag) }}</HtBadge>
              </div>
              <p v-if="ag.servicos.length" class="text-xs text-base-content/60 mb-2">
                {{ ag.servicos.map((s) => s.titulo).join(", ") }}
              </p>
              <div class="flex flex-col gap-1 text-sm text-base-content/80">
                <div class="flex items-center gap-2">
                  <span class="material-symbols-rounded text-base text-base-content/40">calendar_today</span>
                  <span class="capitalize">{{ formatarData(ag.dataHoraAgendada) }}</span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="material-symbols-rounded text-base text-base-content/40">schedule</span>
                  <span>{{ formatarHora(ag.dataHoraAgendada) }} · {{ ag.duracaoMinutos }} min</span>
                </div>
                <div class="flex items-center gap-2">
                  <span class="material-symbols-rounded text-base text-base-content/40">location_on</span>
                  <span class="truncate">{{ formatarEndereco(ag.endereco) }}</span>
                </div>
              </div>
            </div>
            <div class="text-right shrink-0">
              <p class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
            </div>
          </div>
        </HtCard>
      </div>
    </section>
  </div>
</template>
