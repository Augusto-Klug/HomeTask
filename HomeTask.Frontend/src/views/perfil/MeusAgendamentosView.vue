<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import api from "@/services/api";
import { TipoUsuario, type AgendamentoResumo, type StatusAgendamento } from "@/types";
import HtCard from "@/components/ui/HtCard.vue";
import HtBadge from "@/components/ui/HtBadge.vue";
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
type PainelId =
  | "aguardando-sua-confirmacao"
  | "aguardando-confirmacao-prestador"
  | "agendados"
  | "em-andamento"
  | "concluidos"
  | "cancelados-recusados";
type PainelTom = "primary" | "secondary" | "accent" | "success" | "warning" | "error" | "neutro";

const STATUS_NUMERO_PARA_TEXTO: Record<StatusAgendamentoNumero, StatusAgendamento> = {
  1: "Solicitado",
  2: "Aceito",
  3: "Recusado",
  4: "EmAndamento",
  5: "Concluido",
  6: "Cancelado",
};

const STATUS_VARIANT: Record<StatusAgendamento, "default" | "primary" | "success" | "error" | "outline"> = {
  Confirmado: "primary",
  Solicitado: "outline",
  Aceito: "primary",
  EmAndamento: "default",
  Concluido: "success",
  Cancelado: "error",
  Recusado: "error",
};

const diasSemana = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sab"] as const;
const PAINEL_TOM_CLASSES: Record<
  PainelTom,
  { container: string; hover: string; count: string; icon: string }
> = {
  neutro: {
    container: "",
    hover: "hover:bg-base-200/50",
    count: "",
    icon: "text-base-content/60",
  },
  primary: {
    container: "border-primary/20 bg-primary/5",
    hover: "hover:bg-primary/5",
    count: "text-primary/80",
    icon: "text-primary/70",
  },
  secondary: {
    container: "border-secondary/20 bg-secondary/5",
    hover: "hover:bg-secondary/5",
    count: "text-secondary/80",
    icon: "text-secondary/70",
  },
  accent: {
    container: "border-accent/20 bg-accent/5",
    hover: "hover:bg-accent/5",
    count: "text-accent/80",
    icon: "text-accent/70",
  },
  success: {
    container: "border-success/20 bg-success/5",
    hover: "hover:bg-success/5",
    count: "text-success/80",
    icon: "text-success/70",
  },
  warning: {
    container: "border-warning/20 bg-warning/5",
    hover: "hover:bg-warning/5",
    count: "text-warning/80",
    icon: "text-warning/70",
  },
  error: {
    container: "border-error/20 bg-error/5",
    hover: "hover:bg-error/5",
    count: "text-error/80",
    icon: "text-error/70",
  },
};

const agendamentosCliente = ref<AgendamentoResumo[]>([]);
const carregando = ref(true);
const mesVisivel = ref(inicioDoMes(new Date()));
const dataSelecionada = ref(chaveData(new Date()));
const painelAtivo = ref<PainelId | null>(null);

const route = useRoute();
const router = useRouter();

const abaAtual = computed<AbaOperacao>(() => (route.query.aba === "calendario" ? "calendario" : "painel"));

function ordenarPorData(lista: AgendamentoResumo[]) {
  return [...lista].sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime());
}

const aguardandoSuaConfirmacao = computed(() =>
  ordenarPorData(
    agendamentosCliente.value.filter(
        (ag) => ag.status === "Solicitado" && ag.aguardandoRespostaDe === TipoUsuario.Cliente,
    ),
  ),
);
const aguardandoConfirmacaoPrestador = computed(() =>
  ordenarPorData(
    agendamentosCliente.value.filter(
        (ag) => ag.status === "Solicitado" && ag.aguardandoRespostaDe === TipoUsuario.Prestador,
    ),
  ),
);
const agendados = computed(() => ordenarPorData(agendamentosCliente.value.filter((ag) => ag.status === "Aceito")));
const emAndamento = computed(() => ordenarPorData(agendamentosCliente.value.filter((ag) => ag.status === "EmAndamento")));
const concluidos = computed(() => ordenarPorData(agendamentosCliente.value.filter((ag) => ag.status === "Concluido")));
const canceladosOuRecusados = computed(() =>
  ordenarPorData(
    agendamentosCliente.value.filter((ag) => ag.status === "Cancelado" || ag.status === "Recusado"),
  ),
);
const paineis = computed(() => [
  {
    id: "aguardando-sua-confirmacao" as const,
    titulo: "Aguardando sua confirmacao",
    descricao: "Solicitacoes pendentes da sua resposta.",
    itens: aguardandoSuaConfirmacao.value,
    vazio: "Nenhuma solicitacao aguardando sua confirmacao.",
    tom: "warning" as const,
    icone: "schedule",
  },
  {
    id: "aguardando-confirmacao-prestador" as const,
    titulo: "Aguardando confirmacao do prestador",
    descricao: "Pedidos enviados aguardando retorno do prestador.",
    itens: aguardandoConfirmacaoPrestador.value,
    vazio: "Nenhuma solicitacao aguardando confirmacao do prestador.",
    tom: "warning" as const,
    icone: "schedule",
  },
  {
    id: "agendados" as const,
    titulo: "Agendados",
    descricao: "Servicos confirmados e prontos para iniciar.",
    itens: agendados.value,
    vazio: "Nenhum servico agendado.",
    tom: "primary" as const,
    icone: "event_available",
  },
  {
    id: "em-andamento" as const,
    titulo: "Em andamento",
    descricao: "Servicos em execucao no momento.",
    itens: emAndamento.value,
    vazio: "Nenhum servico em andamento.",
    tom: "accent" as const,
    icone: "play_circle",
  },
  {
    id: "concluidos" as const,
    titulo: "Concluidos",
    descricao: "Historico de servicos finalizados.",
    itens: concluidos.value,
    vazio: "Nenhum servico concluido.",
    tom: "success" as const,
    icone: "task_alt",
  },
  {
    id: "cancelados-recusados" as const,
    titulo: "Cancelados ou recusados",
    descricao: "Solicitacoes encerradas sem execucao.",
    itens: canceladosOuRecusados.value,
    vazio: "Nenhum servico cancelado ou recusado.",
    tom: "error" as const,
    icone: "cancel",
  },
]);

const agendaConfirmada = computed(() =>
  agendamentosCliente.value
    .filter((ag) => (ag.status === "Aceito" || ag.status === "EmAndamento") && new Date(ag.dataHoraAgendada) >= inicioDoDia(new Date()))
    .sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime()),
);
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
    const { data } = await api.get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosCliente");
    agendamentosCliente.value = data.map(normalizarAgendamento);
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

function alternarPainel(id: PainelId) {
  painelAtivo.value = painelAtivo.value === id ? null : id;
}

function classesPainel(tom?: PainelTom) {
  return PAINEL_TOM_CLASSES[tom ?? "neutro"];
}

function abrirDetalhes(id: string) {
  router.push(`/agendamento/detalhes/${id}`);
}

function labelCliente(ag: AgendamentoResumo): string {
  if (ag.status === "Solicitado" && ag.aguardandoRespostaDe === TipoUsuario.Cliente) {
    return "Aguardando sua confirmacao";
  }
  if (ag.status === "Solicitado" && ag.aguardandoRespostaDe === TipoUsuario.Prestador) {
    return "Aguardando confirmacao do prestador";
  }
  if (ag.status === "Aceito") return "Agendado";
  if (ag.status === "Concluido") return "Concluido";
  if (ag.status === "EmAndamento") return "Em andamento";
  if (ag.status === "Recusado") return "Recusado";
  if (ag.status === "Cancelado") return "Cancelado";
  return "Solicitado";
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

function normalizarAguardandoRespostaDe(valor: unknown): TipoUsuario | null {
  if (typeof valor === "number" && (valor === TipoUsuario.Cliente || valor === TipoUsuario.Prestador || valor === TipoUsuario.Ambos)) {
    return valor;
  }

  if (valor === "Cliente") return TipoUsuario.Cliente;
  if (valor === "Prestador") return TipoUsuario.Prestador;
  if (valor === "Ambos") return TipoUsuario.Ambos;

  return null;
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
    aguardandoRespostaDe: normalizarAguardandoRespostaDe(seguro.aguardandoRespostaDe),
    servicos: seguro.servicos ?? [],
  };
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

<template>
  <div class="container mx-auto px-4 py-8 max-w-6xl">
    <div class="mb-8 flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
      <div>
        <h1 class="text-2xl font-bold">Meus agendamentos</h1>
        <p class="text-sm text-base-content/60">
          Acompanhe suas solicitacoes, confirme pendencias e visualize sua agenda.
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

    <div v-else-if="abaAtual === 'painel'" class="space-y-4">
      <section v-for="painel in paineis" :key="painel.id">
        <HtCard class="p-0 overflow-hidden" :class="classesPainel(painel.tom).container">
          <button
            type="button"
            class="w-full px-5 py-4 text-left transition-colors"
            :class="classesPainel(painel.tom).hover"
            :aria-expanded="painelAtivo === painel.id"
            @click="alternarPainel(painel.id)"
          >
            <div class="flex items-start justify-between gap-4">
              <div class="flex items-start gap-3">
                <span
                  v-if="painel.icone"
                  class="material-symbols-rounded text-base mt-0.5"
                  :class="classesPainel(painel.tom).icon"
                >
                  {{ painel.icone }}
                </span>
                <div>
                  <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/60">{{ painel.titulo }}</p>
                  <p class="mt-2 text-sm text-base-content/65">{{ painel.descricao }}</p>
                </div>
              </div>
              <div class="flex items-center gap-3">
                <span class="text-3xl font-bold" :class="classesPainel(painel.tom).count">{{ painel.itens.length }}</span>
                <span class="material-symbols-rounded text-xl text-base-content/60">
                  {{ painelAtivo === painel.id ? "expand_less" : "expand_more" }}
                </span>
              </div>
            </div>
          </button>

          <div v-if="painelAtivo === painel.id" class="border-t border-base-300 px-5 py-4">
            <div v-if="painel.itens.length === 0" class="text-sm text-base-content/50 py-4 text-center">
              {{ painel.vazio }}
            </div>
            <div v-else class="flex flex-col gap-3">
              <HtCard
                v-for="ag in painel.itens"
                :key="ag.id"
                class="cursor-pointer hover:border-primary/50 transition-colors"
                @click="abrirDetalhes(ag.id)"
              >
                <div class="flex items-start justify-between gap-3">
                  <div class="flex-1 min-w-0">
                    <div class="flex items-center gap-2 mb-1 flex-wrap">
                      <span class="font-semibold text-sm">{{ ag.prestadorNome }}</span>
                      <HtBadge :variant="STATUS_VARIANT[ag.status]">{{ labelCliente(ag) }}</HtBadge>
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
          </div>
        </HtCard>
      </section>
    </div>

    <div v-else class="grid gap-6 xl:grid-cols-[minmax(0,1.8fr)_22rem]">
      <HtCard class="overflow-hidden">
        <div class="flex flex-col gap-4 border-b border-base-300 pb-4 md:flex-row md:items-center md:justify-between">
          <div>
            <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/50">Calendario de agendamentos</p>
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
                <p class="mt-1 truncate text-[11px] text-base-content/70">{{ evento.prestadorNome }}</p>
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
                  <p class="font-semibold">{{ ag.prestadorNome }}</p>
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
