<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { useAuthStore } from "@/stores/auth";
import api from "@/services/api";
import type { AgendamentoResumo, StatusAgendamento } from "@/types";
import HtCard from "@/components/ui/HtCard.vue";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";

const auth = useAuthStore();

const agendamentosCliente = ref<AgendamentoResumo[]>([]);
const agendamentosPrestador = ref<AgendamentoResumo[]>([]);
const carregando = ref(true);

const isCliente = computed(
  () => auth.user?.tipo === 1 || auth.user?.tipo === 3,
);
const isPrestador = computed(
  () => auth.user?.tipo === 2 || auth.user?.tipo === 3,
);

const agora = new Date();
type StatusAgendamentoNumero = 1 | 2 | 3 | 4 | 5 | 6;
const STATUS_NUMERO_PARA_TEXTO: Record<StatusAgendamentoNumero, StatusAgendamento> = {
  1: "Solicitado",
  2: "Aceito",
  3: "Recusado",
  4: "EmAndamento",
  5: "Concluido",
  6: "Cancelado",
};

function isFuturo(iso: string) {
  return new Date(iso) >= agora;
}

function isPrestadorAtivo(status: StatusAgendamento) {
  return status === "Solicitado" || status === "Aceito" || status === "EmAndamento";
}

function filtrarPrestador(lista: AgendamentoResumo[]) {
  return lista
    .filter((a) => isFuturo(a.dataHoraAgendada) && isPrestadorAtivo(a.status))
    .sort(
      (a, b) =>
        new Date(a.dataHoraAgendada).getTime() -
        new Date(b.dataHoraAgendada).getTime(),
    );
}

function filtrarCliente(lista: AgendamentoResumo[]) {
  return lista
    .filter((a) => isFuturo(a.dataHoraAgendada))
    .sort(
      (a, b) =>
        new Date(a.dataHoraAgendada).getTime() -
        new Date(b.dataHoraAgendada).getTime(),
    );
}

const clienteFiltrado = computed(() => filtrarCliente(agendamentosCliente.value));
const prestadorFiltrado = computed(() => filtrarPrestador(agendamentosPrestador.value));

onMounted(async () => {
  try {
    const reqs: Promise<void>[] = [];
    if (isCliente.value) {
      reqs.push(
        api.get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosCliente").then((r) => {
          agendamentosCliente.value = r.data.map(normalizarAgendamento);
        }),
      );
    }
    if (isPrestador.value) {
      reqs.push(
        api
          .get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosPrestador")
          .then((r) => {
            agendamentosPrestador.value = r.data.map(normalizarAgendamento);
          }),
      );
    }
    await Promise.all(reqs);
  } finally {
    carregando.value = false;
  }
});

const STATUS_LABEL: Record<StatusAgendamento, string> = {
  Confirmado: "Confirmado",
  Solicitado: "Solicitado",
  Aceito: "Aceito",
  EmAndamento: "Em andamento",
  Concluido: "Concluído",
  Cancelado: "Cancelado",
  Recusado: "Recusado",
};

const STATUS_LABEL_CLIENTE: Record<StatusAgendamento, string> = {
  Confirmado: "Agendado",
  Solicitado: "Pendente aprovação",
  Aceito: "Agendado",
  EmAndamento: "Em andamento",
  Concluido: "Concluído",
  Cancelado: "Cancelado",
  Recusado: "Cancelado",
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
  return `${e.logradouro}, ${e.bairro} — ${e.cidade}/${e.estado}`;
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
    if (status in STATUS_LABEL) return status as StatusAgendamento;
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

    <template v-else>
      <!-- Seção cliente -->
      <section v-if="isCliente" class="mb-10">
        <h2
          class="text-title font-semibold text-base-content/70 uppercase tracking-wide mb-4"
        >
          Cliente
        </h2>

        <div
          v-if="clienteFiltrado.length === 0"
          class="text-sm text-base-content/50 py-6 text-center"
        >
          Nenhum agendamento futuro pendente.
        </div>

        <div v-else class="flex flex-col gap-3">
          <HtCard v-for="ag in clienteFiltrado" :key="ag.id" @click="$router.push(`/agendamento/detalhes/${ag.id}`)" class="cursor-pointer hover:border-primary/50 transition-colors">
            <div class="flex items-start justify-between gap-3">
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 mb-1 flex-wrap">
                  <span class="font-semibold text-sm">{{
                    ag.prestadorNome
                  }}</span>
                  <HtBadge :variant="STATUS_VARIANT[ag.status]">{{
                    STATUS_LABEL_CLIENTE[ag.status]
                  }}</HtBadge>
                </div>
                <p
                  v-if="ag.servicos.length"
                  class="text-xs text-base-content/60 mb-2"
                >
                  {{ ag.servicos.map((s) => s.titulo).join(", ") }}
                </p>
                <div class="flex flex-col gap-1 text-sm text-base-content/80">
                  <div class="flex items-center gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40"
                      >calendar_today</span
                    >
                    <span class="capitalize">{{
                      formatarData(ag.dataHoraAgendada)
                    }}</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40"
                      >schedule</span
                    >
                    <span
                      >{{ formatarHora(ag.dataHoraAgendada) }} ·
                      {{ ag.duracaoMinutos }} min</span
                    >
                  </div>
                  <div class="flex items-center gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40"
                      >location_on</span
                    >
                    <span class="truncate">{{
                      formatarEndereco(ag.endereco)
                    }}</span>
                  </div>
                  <div v-if="ag.observacoes" class="flex items-start gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40 mt-0.5"
                      >notes</span
                    >
                    <span>{{ ag.observacoes }}</span>
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

      <!-- Seção prestador -->
      <section v-if="isPrestador">
        <h2
          class="text-title font-semibold text-base-content/70 uppercase tracking-wide mb-4"
        >
          Prestador
        </h2>

        <div
          v-if="prestadorFiltrado.length === 0"
          class="text-sm text-base-content/50 py-6 text-center"
        >
          Nenhum agendamento futuro pendente.
        </div>

        <div v-else class="flex flex-col gap-3">
          <HtCard v-for="ag in prestadorFiltrado" :key="ag.id" @click="$router.push(`/agendamento/detalhes/${ag.id}`)" class="cursor-pointer hover:border-primary/50 transition-colors">
            <div class="flex items-start justify-between gap-3">
              <div class="flex-1 min-w-0">
                <div class="flex items-center gap-2 mb-1 flex-wrap">
                  <span class="font-semibold text-sm">{{
                    ag.clienteNome
                  }}</span>
                  <HtBadge :variant="STATUS_VARIANT[ag.status]">{{
                    STATUS_LABEL[ag.status]
                  }}</HtBadge>
                </div>
                <p
                  v-if="ag.servicos.length"
                  class="text-xs text-base-content/60 mb-2"
                >
                  {{ ag.servicos.map((s) => s.titulo).join(", ") }}
                </p>
                <div class="flex flex-col gap-1 text-sm text-base-content/80">
                  <div class="flex items-center gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40"
                      >calendar_today</span
                    >
                    <span class="capitalize">{{
                      formatarData(ag.dataHoraAgendada)
                    }}</span>
                  </div>
                  <div class="flex items-center gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40"
                      >schedule</span
                    >
                    <span
                      >{{ formatarHora(ag.dataHoraAgendada) }} ·
                      {{ ag.duracaoMinutos }} min</span
                    >
                  </div>
                  <div class="flex items-center gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40"
                      >location_on</span
                    >
                    <span class="truncate">{{
                      formatarEndereco(ag.endereco)
                    }}</span>
                  </div>
                  <div v-if="ag.observacoes" class="flex items-start gap-2">
                    <span
                      class="material-symbols-rounded text-base text-base-content/40 mt-0.5"
                      >notes</span
                    >
                    <span>{{ ag.observacoes }}</span>
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
    </template>
  </div>
</template>
