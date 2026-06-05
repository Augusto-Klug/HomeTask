<template>
  <div class="container mx-auto px-4 py-8 max-w-6xl">
    <div class="mb-8 flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
      <div>
        <h1 class="text-2xl font-bold">{{ titulo }}</h1>
        <p class="text-sm text-base-content/60" v-html="descricao" />
      </div>

      <div class="inline-flex rounded-2xl border border-base-300 bg-base-200/60 p-1 self-start">
        <button
          type="button"
          class="rounded-xl px-4 py-2 text-sm font-medium transition-colors"
          :class="
            abaAtual === 'painel'
              ? 'bg-base-100 text-primary shadow-sm'
              : 'text-base-content/70 hover:text-base-content'
          "
          @click="emit('selecionarAba', 'painel')"
        >
          Painel
        </button>
        <button
          type="button"
          class="rounded-xl px-4 py-2 text-sm font-medium transition-colors"
          :class="
            abaAtual === 'calendario'
              ? 'bg-base-100 text-primary shadow-sm'
              : 'text-base-content/70 hover:text-base-content'
          "
          @click="emit('selecionarAba', 'calendario')"
        >
          Calendário
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
                  <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/60">
                    {{ painel.titulo }}
                  </p>
                  <p class="mt-2 text-sm text-base-content/65">{{ painel.descricao }}</p>
                </div>
              </div>
              <div class="flex items-center gap-3">
                <span class="text-3xl font-bold" :class="classesPainel(painel.tom).count">{{ painel.quantidade }}</span>
                <span class="material-symbols-rounded text-xl text-base-content/60">
                  {{ painelAtivo === painel.id ? "expand_less" : "expand_more" }}
                </span>
              </div>
            </div>
          </button>

          <div v-if="painelAtivo === painel.id" class="border-t border-base-300 px-5 py-4">
            <slot :name="`painel-${painel.id}`" :painel="painel" />
          </div>
        </HtCard>
      </section>
    </div>

    <div v-else class="grid gap-6 xl:grid-cols-[minmax(0,1.8fr)_22rem]">
      <HtCard class="overflow-hidden">
        <div class="flex flex-col gap-4 border-b border-base-300 pb-4 md:flex-row md:items-center md:justify-between">
          <div>
            <p class="text-xs font-semibold uppercase tracking-[0.18em] text-base-content/50">
              {{ calendarioEtiqueta }}
            </p>
            <h2 class="text-xl font-semibold mt-1">{{ tituloMesVisivel }}</h2>
          </div>

          <div class="flex items-center gap-2">
            <button
              type="button"
              class="btn btn-ghost btn-sm btn-square"
              aria-label="Mês anterior"
              @click="alterarMes(-1)"
            >
              <span class="material-symbols-rounded text-lg">chevron_left</span>
            </button>
            <button type="button" class="btn btn-outline btn-sm" @click="irParaMesAtual">Hoje</button>
            <button
              type="button"
              class="btn btn-ghost btn-sm btn-square"
              aria-label="Próximo mês"
              @click="alterarMes(1)"
            >
              <span class="material-symbols-rounded text-lg">chevron_right</span>
            </button>
          </div>
        </div>

        <div
          class="mt-5 grid grid-cols-7 gap-2 text-center text-xs font-semibold uppercase tracking-wide text-base-content/45"
        >
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
                <slot name="calendario-evento" :evento="evento" />
              </div>
              <p v-if="dia.eventos.length > 2" class="text-[11px] font-medium text-base-content/55">
                +{{ dia.eventos.length - 2 }} {{ calendarioExcedenteLabel }}
              </p>
            </div>
          </button>
        </div>
      </HtCard>

      <div class="space-y-4">
        <HtCard class="border-primary/15 bg-primary/5">
          <p class="text-xs font-semibold uppercase tracking-[0.18em] text-primary/80">Resumo do mês</p>
          <p class="mt-3 text-3xl font-bold">{{ eventosMesAtual.length }}</p>
          <p class="mt-2 text-sm text-base-content/65" v-html="resumoMesTexto"></p>
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

          <div
            v-if="eventosDiaSelecionado.length === 0"
            class="mt-5 rounded-2xl border border-dashed border-base-300 px-4 py-6 text-sm text-base-content/55"
          >
            {{ vazioDiaTexto }}
          </div>

          <div v-else class="mt-5 flex flex-col gap-3">
            <slot name="dia-selecionado" :eventos="eventosDiaSelecionado" />
          </div>
        </HtCard>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";

type AbaAgendamentosOperacao = "painel" | "calendario";
type PainelAgendamentosOperacaoTom = "primary" | "secondary" | "accent" | "success" | "warning" | "error" | "neutro";
type PainelAgendamentosOperacao = {
  id: string;
  titulo: string;
  descricao: string;
  quantidade: number;
  tom?: PainelAgendamentosOperacaoTom;
  icone?: string;
};
type EventoCalendario = {
  id: string;
  dataHoraAgendada: string | Date;
  [campo: string]: any;
};
type DiaCalendario = {
  chave: string;
  iso: string;
  numero: number;
  pertenceAoMes: boolean;
  hoje: boolean;
  selecionado: boolean;
  eventos: EventoCalendario[];
};

const props = withDefaults(
  defineProps<{
    titulo: string;
    descricao: string;
    carregando: boolean;
    abaAtual: AbaAgendamentosOperacao;
    paineis: PainelAgendamentosOperacao[];
    calendarioEtiqueta: string;
    eventosCalendario: EventoCalendario[];
    resumoMesTexto: string;
    vazioDiaTexto: string;
    calendarioExcedenteLabel?: string;
  }>(),
  {
    calendarioExcedenteLabel: "serviços",
  },
);

const emit = defineEmits<{
  selecionarAba: [aba: AbaAgendamentosOperacao];
}>();

const painelAtivo = ref<string | null>(null);
const mesVisivel = ref(inicioDoMes(new Date()));
const dataSelecionada = ref(chaveData(new Date()));
const diasSemana = ["Dom", "Seg", "Ter", "Qua", "Qui", "Sex", "Sáb"] as const;

const eventosOrdenados = computed(() =>
  [...props.eventosCalendario].sort((a, b) => dataEvento(a).getTime() - dataEvento(b).getTime()),
);
const tituloMesVisivel = computed(() =>
  mesVisivel.value.toLocaleDateString("pt-BR", { month: "long", year: "numeric" }),
);
const eventosPorData = computed(() => {
  const mapa = new Map<string, EventoCalendario[]>();

  for (const evento of eventosOrdenados.value) {
    const chave = chaveData(evento.dataHoraAgendada);
    const lista = mapa.get(chave) ?? [];
    lista.push(evento);
    lista.sort((a, b) => dataEvento(a).getTime() - dataEvento(b).getTime());
    mapa.set(chave, lista);
  }

  return mapa;
});
const eventosMesAtual = computed(() =>
  eventosOrdenados.value.filter((evento) => mesmoMes(dataEvento(evento), mesVisivel.value)),
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
  return data.toLocaleDateString("pt-BR", { weekday: "long", day: "2-digit", month: "long" });
});

watch(
  () => props.eventosCalendario,
  () => alinharSelecaoCalendario(),
  { deep: true },
);

function alternarPainel(id: string) {
  painelAtivo.value = painelAtivo.value === id ? null : id;
}

function classesPainel(tom?: PainelAgendamentosOperacaoTom) {
  return PAINEL_TOM_CLASSES[tom ?? "neutro"];
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
  const noMesSelecionado = eventosMesAtual.value.find(
    (evento) => chaveData(evento.dataHoraAgendada) === dataSelecionada.value,
  );
  if (noMesSelecionado) return;

  const primeiroEventoMes = eventosOrdenados.value.find((evento) => mesmoMes(dataEvento(evento), mesVisivel.value));
  dataSelecionada.value = primeiroEventoMes
    ? chaveData(primeiroEventoMes.dataHoraAgendada)
    : chaveData(mesVisivel.value);
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

function mesmoMes(a: Date, b: Date) {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth();
}

function dataEvento(evento: EventoCalendario) {
  return new Date(evento.dataHoraAgendada);
}

function classesDia(dia: DiaCalendario) {
  if (dia.selecionado) return "border-primary bg-primary/6";
  if (!dia.pertenceAoMes) return "border-base-300/60 bg-base-200/35 text-base-content/35";
  if (dia.hoje) return "border-primary/35 bg-primary/4";
  return "border-base-300 hover:border-primary/35 hover:bg-base-200/40";
}

const PAINEL_TOM_CLASSES: Record<
  PainelAgendamentosOperacaoTom,
  { container: string; hover: string; count: string; icon: string }
> = {
  neutro: { container: "", hover: "hover:bg-base-200/50", count: "", icon: "text-base-content/60" },
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
</script>
