<template>
  <div class="container mx-auto px-4 py-8 max-w-5xl">
    <div class="mb-8">
      <h1 class="text-2xl font-bold">Minha operacao</h1>
      <p class="text-sm text-base-content/60">Solicitacoes para responder e servicos em execucao.</p>
    </div>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <div v-else class="space-y-8">
      <section>
        <h2 class="text-lg font-semibold mb-4">Agendados</h2>
        <div v-if="agendados.length === 0" class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl">
          Nenhum item aguardando resposta ou inicio.
        </div>
        <div v-else class="flex flex-col gap-3">
          <HtCard
            v-for="ag in agendados"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="$router.push({ path: `/agendamento/detalhes/${ag.id}`, query: { origem: 'operacao' } })"
          >
            <div class="flex items-start justify-between gap-3">
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 mb-1 flex-wrap">
                  <span class="font-semibold text-sm">{{ ag.clienteNome }}</span>
                  <HtBadge :variant="ag.status === 'Solicitado' ? 'outline' : 'primary'">
                    {{ ag.status === 'Solicitado' ? 'Aguardando sua resposta' : 'Pronto para iniciar' }}
                  </HtBadge>
                </div>
                <p v-if="ag.servicos.length" class="text-xs text-base-content/60 mb-2">
                  {{ ag.servicos.map((s) => s.titulo).join(", ") }}
                </p>
                <p class="text-sm text-base-content/80">
                  {{ formatarData(ag.dataHoraAgendada) }} as {{ formatarHora(ag.dataHoraAgendada) }}
                </p>
              </div>
              <div class="text-right shrink-0">
                <p class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
              </div>
            </div>
          </HtCard>
        </div>
      </section>

      <section>
        <h2 class="text-lg font-semibold mb-4">Em andamento</h2>
        <div v-if="emAndamento.length === 0" class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl">
          Nenhum servico em andamento.
        </div>
        <div v-else class="flex flex-col gap-3">
          <HtCard
            v-for="ag in emAndamento"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="$router.push({ path: `/agendamento/detalhes/${ag.id}`, query: { origem: 'operacao' } })"
          >
            <div class="flex items-start justify-between gap-3">
              <div>
                <div class="flex items-center gap-2 mb-1">
                  <span class="font-semibold text-sm">{{ ag.clienteNome }}</span>
                  <HtBadge variant="default">Em andamento</HtBadge>
                </div>
                <p class="text-sm text-base-content/80">
                  {{ formatarData(ag.dataHoraAgendada) }} as {{ formatarHora(ag.dataHoraAgendada) }}
                </p>
              </div>
              <div class="text-right shrink-0">
                <p class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
              </div>
            </div>
          </HtCard>
        </div>
      </section>

      <section>
        <h2 class="text-lg font-semibold mb-4">Concluidos</h2>
        <div v-if="concluidos.length === 0" class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl">
          Nenhum servico concluido.
        </div>
        <div v-else class="flex flex-col gap-3">
          <HtCard
            v-for="ag in concluidos"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="$router.push({ path: `/agendamento/detalhes/${ag.id}`, query: { origem: 'operacao' } })"
          >
            <div class="flex items-start justify-between gap-3">
              <div>
                <div class="flex items-center gap-2 mb-1">
                  <span class="font-semibold text-sm">{{ ag.clienteNome }}</span>
                  <HtBadge variant="success">Concluido</HtBadge>
                </div>
                <p class="text-sm text-base-content/80">
                  {{ formatarData(ag.dataHoraAgendada) }} as {{ formatarHora(ag.dataHoraAgendada) }}
                </p>
              </div>
              <div class="text-right shrink-0">
                <p class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
              </div>
            </div>
          </HtCard>
        </div>
      </section>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import api from "@/services/api";
import type { AgendamentoResumo, StatusAgendamento } from "@/types";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtCard from "@/components/ui/HtCard.vue";
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

const carregando = ref(true);
const agendamentos = ref<AgendamentoResumo[]>([]);

const agendados = computed(() =>
  agendamentos.value.filter((ag) =>
    (ag.status === "Solicitado" && ag.aguardandoRespostaDe === "Prestador") || ag.status === "Aceito",
  ),
);
const emAndamento = computed(() => agendamentos.value.filter((ag) => ag.status === "EmAndamento"));
const concluidos = computed(() => agendamentos.value.filter((ag) => ag.status === "Concluido"));

onMounted(async () => {
  try {
    const { data } = await api.get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosPrestador");
    agendamentos.value = data.map(normalizarAgendamento);
  } finally {
    carregando.value = false;
  }
});

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

function formatarMoeda(valor: number) {
  return valor.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL",
  });
}
</script>
