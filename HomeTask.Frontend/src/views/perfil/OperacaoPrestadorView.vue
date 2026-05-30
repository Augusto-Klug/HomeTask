<template>
  <div class="container mx-auto px-4 py-8 max-w-6xl">
    <div class="mb-8 flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
      <div>
        <h1 class="text-2xl font-bold">Minha operacao</h1>
        <p class="text-sm text-base-content/60">
          Responda solicitacoes, acompanhe a execucao e visualize seu calendario de servicos confirmados.
        </p>
      </div>

      <div class="inline-flex rounded-2xl border border-base-300 bg-base-200/60 p-1 self-start">
        <button
          type="button"
          class="rounded-xl px-4 py-2 text-sm font-medium transition-colors"
          :class="abaAtual === 'painel' ? 'bg-base-100 text-primary shadow-sm' : 'text-base-content/70 hover:text-base-content'"
          @click="selecionarAba('painel')"
        >
          Painel
        </button>
        <button
          type="button"
          class="rounded-xl px-4 py-2 text-sm font-medium transition-colors"
          :class="abaAtual === 'calendario' ? 'bg-base-100 text-primary shadow-sm' : 'text-base-content/70 hover:text-base-content'"
          @click="selecionarAba('calendario')"
        >
          Calendario
        </button>
      </div>
    </div>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <div v-else-if="abaAtual === 'painel'" class="space-y-8">
      <section class="grid gap-4 md:grid-cols-3">
        <HtCard class="border-primary/15 bg-primary/5">
          <p class="text-xs font-semibold uppercase tracking-[0.18em] text-primary/80">Agenda confirmada</p>
          <p class="mt-3 text-3xl font-bold">{{ agendaConfirmada.length }}</p>
          <p class="mt-2 text-sm text-base-content/65">Servicos confirmados e prontos para execucao.</p>
        </HtCard>

        <HtCard>
          <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/50">Proxima visita</p>
          <template v-if="proximoServico">
            <p class="mt-3 text-lg font-semibold">{{ proximoServico.clienteNome }}</p>
            <p class="mt-2 text-sm text-base-content/75">
              {{ formatarDataCurta(proximoServico.dataHoraAgendada) }} as {{ formatarHora(proximoServico.dataHoraAgendada) }}
            </p>
          </template>
          <p v-else class="mt-3 text-sm text-base-content/60">Nenhum servico confirmado no momento.</p>
        </HtCard>

        <HtCard>
          <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/50">Em andamento</p>
          <p class="mt-3 text-3xl font-bold">{{ emAndamento.length }}</p>
          <p class="mt-2 text-sm text-base-content/65">Itens que ja foram iniciados e ainda precisam ser concluidos.</p>
        </HtCard>
      </section>

      <section>
        <h2 class="text-lg font-semibold mb-4">Agendados</h2>
        <div
          v-if="agendados.length === 0"
          class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl"
        >
          Nenhum item aguardando resposta ou inicio.
        </div>
        <div v-else class="flex flex-col gap-3">
          <HtCard
            v-for="ag in agendados"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="abrirDetalhes(ag.id)"
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
                  {{ formatarDataCurta(ag.dataHoraAgendada) }} as {{ formatarHora(ag.dataHoraAgendada) }}
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
        <div
          v-if="emAndamento.length === 0"
          class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl"
        >
          Nenhum servico em andamento.
        </div>
        <div v-else class="flex flex-col gap-3">
          <HtCard
            v-for="ag in emAndamento"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="abrirDetalhes(ag.id)"
          >
            <div class="flex items-start justify-between gap-3">
              <div>
                <div class="flex items-center gap-2 mb-1">
                  <span class="font-semibold text-sm">{{ ag.clienteNome }}</span>
                  <HtBadge variant="default">Em andamento</HtBadge>
                </div>
                <p class="text-sm text-base-content/80">
                  {{ formatarDataCurta(ag.dataHoraAgendada) }} as {{ formatarHora(ag.dataHoraAgendada) }}
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
        <div
          v-if="concluidos.length === 0"
          class="text-sm text-base-content/50 py-6 text-center border border-dashed border-base-300 rounded-xl"
        >
          Nenhum servico concluido.
        </div>
        <div v-else class="flex flex-col gap-3">
          <HtCard
            v-for="ag in concluidos"
            :key="ag.id"
            class="cursor-pointer hover:border-primary/50 transition-colors"
            @click="abrirDetalhes(ag.id)"
          >
            <div class="flex items-start justify-between gap-3">
              <div>
                <div class="flex items-center gap-2 mb-1">
                  <span class="font-semibold text-sm">{{ ag.clienteNome }}</span>
                  <HtBadge variant="success">Concluido</HtBadge>
                </div>
                <p class="text-sm text-base-content/80">
                  {{ formatarDataCurta(ag.dataHoraAgendada) }} as {{ formatarHora(ag.dataHoraAgendada) }}
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

    <div v-else class="grid gap-6 xl:grid-cols-[minmax(0,1.8fr)_22rem]">
      <HtCard class="overflow-hidden">
        <div class="flex flex-col gap-4 border-b border-base-300 pb-4 md:flex-row md:items-center md:justify-between">
          <div>
            <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/50">Calendario operacional</p>
            <h2 class="text-xl font-semibold mt-1">{{ tituloMesVisivel }}</h2>
          </div>

          <div class="flex items-center gap-2">
            <button type="button" class="btn btn-ghost btn-sm btn-square" aria-label="Mes anterior" @click="alterarMes(-1)">
              <span class="material-symbols-rounded text-lg">chevron_left</span>
            </button>
            <button type="button" class="btn btn-outline btn-sm" @click="irParaMesAtual">Hoje</button>
            <button type="button" class="btn btn-ghost btn-sm btn-square" aria-label="Proximo mes" @click="alterarMes(1)">
              <span class="material-symbols-rounded text-lg">chevron_right</span>
            </button>
          </div>
        </div>

        <div class="mt-5 grid grid-cols-7 gap-2 text-center text-xs font-semibold uppercase tracking-wide text-base-content/45">
          <div v-for="dia in diasSemana" :key="dia">{{ dia }}</div>
        </div>

        <div class="mt-3 grid grid-cols-7 gap-2">
          <button
            v-for="dia in diasCalendario"
            :key="dia.chave"
            type="button"
            class="min-h-[8.5rem] rounded-2xl border p-3 text-left transition-colors"
            :class="classesDia(dia)"
            @click="selecionarDia(dia.iso)"
          >
            <div class="flex items-center justify-between">
              <span class="text-sm font-semibold">{{ dia.numero }}</span>
              <span
                v-if="dia.eventos.length"
                class="inline-flex h-5 min-w-5 items-center justify-center rounded-full bg-primary/12 px-1.5 text-[11px] font-semibold text-primary"
              >
                {{ dia.eventos.length }}
              </span>
            </div>

            <div class="mt-3 space-y-2">
              <div
                v-for="evento in dia.eventos.slice(0, 2)"
                :key="evento.id"
                class="rounded-xl bg-base-200/75 px-2 py-1.5"
              >
                <p class="text-[11px] font-semibold leading-none">{{ formatarHora(evento.dataHoraAgendada) }}</p>
                <p class="mt-1 truncate text-[11px] text-base-content/70">{{ evento.clienteNome }}</p>
              </div>
              <p v-if="dia.eventos.length > 2" class="text-[11px] font-medium text-base-content/55">
                +{{ dia.eventos.length - 2 }} servicos
              </p>
            </div>
          </button>
        </div>
      </HtCard>

      <div class="space-y-4">
        <HtCard class="border-primary/15 bg-primary/5">
          <p class="text-xs font-semibold uppercase tracking-[0.18em] text-primary/80">Resumo do mes</p>
          <p class="mt-3 text-3xl font-bold">{{ eventosMesAtual.length }}</p>
          <p class="mt-2 text-sm text-base-content/65">Servicos confirmados neste mes.</p>
        </HtCard>

        <HtCard>
          <div class="flex items-center justify-between gap-3">
            <div>
              <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/50">Dia selecionado</p>
              <h3 class="mt-1 text-lg font-semibold">{{ tituloDiaSelecionado }}</h3>
            </div>
            <button
              v-if="eventosDiaSelecionado.length"
              type="button"
              class="btn btn-ghost btn-sm"
              @click="selecionarDiaHoje"
            >
              Hoje
            </button>
          </div>

          <div v-if="eventosDiaSelecionado.length === 0" class="mt-5 rounded-2xl border border-dashed border-base-300 px-4 py-6 text-sm text-base-content/55">
            Nenhum servico confirmado neste dia.
          </div>

          <div v-else class="mt-5 flex flex-col gap-3">
            <button
              v-for="ag in eventosDiaSelecionado"
              :key="ag.id"
              type="button"
              class="rounded-2xl border border-base-300 px-4 py-3 text-left transition-colors hover:border-primary/40 hover:bg-base-200/50"
              @click="abrirDetalhes(ag.id)"
            >
              <div class="flex items-start justify-between gap-3">
                <div>
                  <p class="font-semibold">{{ ag.clienteNome }}</p>
                  <p class="mt-1 text-sm text-base-content/70">
                    {{ formatarHora(ag.dataHoraAgendada) }} · {{ ag.duracaoMinutos }} min
                  </p>
                  <p v-if="ag.servicos.length" class="mt-2 text-sm text-base-content/60">
                    {{ ag.servicos.map((s) => s.titulo).join(", ") }}
                  </p>
                </div>
                <p class="shrink-0 font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
              </div>
            </button>
          </div>
        </HtCard>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import api from "@/services/api";
import type { AgendamentoResumo, StatusAgendamento } from "@/types";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";

type StatusAgendamentoNumero = 1 | 2 | 3 | 4 | 5 | 6;
type AbaOperacao = "painel" | "calendario";
type DiaCalendario = {
  chave: string;
  iso: string;
  numero: number;
  pertenceAoMes: boolean;
  hoje: boolean;
  selecionado: boolean;
  eventos: AgendamentoResumo[];
};

const STATUS_NUMERO_PARA_TEXTO: Record<StatusAgendamentoNumero, StatusAgendamento> = {
  1: "Solicitado",
  2: "Aceito",
  3: "Recusado",
  4: "EmAndamento",
  5: "Concluido",
  6: "Cancelado",
};

const diasSemana = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"] as const;

const carregando = ref(true);
const agendamentos = ref<AgendamentoResumo[]>([]);
const mesVisivel = ref(inicioDoMes(new Date()));
const dataSelecionada = ref(chaveData(new Date()));

const route = useRoute();
const router = useRouter();

const abaAtual = computed<AbaOperacao>(() => (route.query.aba === "calendario" ? "calendario" : "painel"));
const agendados = computed(() =>
  agendamentos.value.filter(
    (ag) => (ag.status === "Solicitado" && ag.aguardandoRespostaDe === "Prestador") || ag.status === "Aceito",
  ),
);
const emAndamento = computed(() => agendamentos.value.filter((ag) => ag.status === "EmAndamento"));
const concluidos = computed(() => agendamentos.value.filter((ag) => ag.status === "Concluido"));
const agendaConfirmada = computed(() =>
  agendamentos.value
    .filter((ag) => (ag.status === "Aceito" || ag.status === "EmAndamento") && new Date(ag.dataHoraAgendada) >= inicioDoDia(new Date()))
    .sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime()),
);
const proximoServico = computed(() => agendaConfirmada.value[0] ?? null);
const tituloMesVisivel = computed(() =>
  mesVisivel.value.toLocaleDateString("pt-BR", { month: "long", year: "numeric" }),
);
const eventosPorData = computed(() => {
  const mapa = new Map<string, AgendamentoResumo[]>();

  for (const ag of agendaConfirmada.value) {
    const chave = chaveData(ag.dataHoraAgendada);
    const lista = mapa.get(chave) ?? [];
    lista.push(ag);
    lista.sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime());
    mapa.set(chave, lista);
  }

  return mapa;
});
const eventosMesAtual = computed(() =>
  agendaConfirmada.value.filter((ag) => mesmoMes(new Date(ag.dataHoraAgendada), mesVisivel.value)),
);
const diasCalendario = computed<DiaCalendario[]>(() => {
  const primeiroDia = inicioDoMes(mesVisivel.value);
  const inicioGrade = new Date(primeiroDia);
  inicioGrade.setDate(primeiroDia.getDate() - primeiroDia.getDay());

  return Array.from({ length: 42 }, (_, indice) => {
    const data = new Date(inicioGrade);
    data.setDate(inicioGrade.getDate() + indice);
    const iso = chaveData(data);
    return {
      chave: `${iso}-${indice}`,
      iso,
      numero: data.getDate(),
      pertenceAoMes: data.getMonth() === mesVisivel.value.getMonth(),
      hoje: iso === chaveData(new Date()),
      selecionado: iso === dataSelecionada.value,
      eventos: eventosPorData.value.get(iso) ?? [],
    };
  });
});
const eventosDiaSelecionado = computed(() => eventosPorData.value.get(dataSelecionada.value) ?? []);
const tituloDiaSelecionado = computed(() => {
  const [ano, mes, dia] = dataSelecionada.value.split("-").map(Number);
  const data = new Date(ano ?? 0, (mes ?? 1) - 1, dia ?? 1);
  return data.toLocaleDateString("pt-BR", {
    weekday: "long",
    day: "2-digit",
    month: "long",
  });
});

onMounted(async () => {
  try {
    const { data } = await api.get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosPrestador");
    agendamentos.value = data.map(normalizarAgendamento);
    alinharSelecaoCalendario();
  } finally {
    carregando.value = false;
  }
});

function selecionarAba(aba: AbaOperacao) {
  const query = { ...route.query };
  if (aba === "calendario") query.aba = "calendario";
  else delete query.aba;
  router.replace({ query });
}

function abrirDetalhes(id: string) {
  router.push({ path: `/agendamento/detalhes/${id}`, query: { origem: "operacao" } });
}

function selecionarDia(iso: string) {
  dataSelecionada.value = iso;
}

function selecionarDiaHoje() {
  const hoje = new Date();
  mesVisivel.value = inicioDoMes(hoje);
  dataSelecionada.value = chaveData(hoje);
}

function alterarMes(direcao: number) {
  const proximo = new Date(mesVisivel.value);
  proximo.setMonth(proximo.getMonth() + direcao, 1);
  mesVisivel.value = inicioDoMes(proximo);
  alinharSelecaoCalendario();
}

function irParaMesAtual() {
  mesVisivel.value = inicioDoMes(new Date());
  alinharSelecaoCalendario();
}

function alinharSelecaoCalendario() {
  const noMesSelecionado = eventosMesAtual.value.find((ag) => chaveData(ag.dataHoraAgendada) === dataSelecionada.value);
  if (noMesSelecionado) return;

  const primeiroEventoMes = agendaConfirmada.value.find((ag) => mesmoMes(new Date(ag.dataHoraAgendada), mesVisivel.value));
  if (primeiroEventoMes) {
    dataSelecionada.value = chaveData(primeiroEventoMes.dataHoraAgendada);
    return;
  }

  dataSelecionada.value = chaveData(mesVisivel.value);
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

function formatarDataCurta(iso: string) {
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

function chaveData(data: string | Date) {
  const base = typeof data === "string" ? new Date(data) : new Date(data);
  const ano = base.getFullYear();
  const mes = String(base.getMonth() + 1).padStart(2, "0");
  const dia = String(base.getDate()).padStart(2, "0");
  return `${ano}-${mes}-${dia}`;
}

function inicioDoMes(data: Date) {
  return new Date(data.getFullYear(), data.getMonth(), 1);
}

function inicioDoDia(data: Date) {
  return new Date(data.getFullYear(), data.getMonth(), data.getDate());
}

function mesmoMes(a: Date, b: Date) {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth();
}

function classesDia(dia: DiaCalendario) {
  if (dia.selecionado) {
    return "border-primary bg-primary/6";
  }
  if (!dia.pertenceAoMes) {
    return "border-base-300/60 bg-base-200/35 text-base-content/35";
  }
  if (dia.hoje) {
    return "border-primary/35 bg-primary/4";
  }
  return "border-base-300 hover:border-primary/35 hover:bg-base-200/40";
}
</script>
