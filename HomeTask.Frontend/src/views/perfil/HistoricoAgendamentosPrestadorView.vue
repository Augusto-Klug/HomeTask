<template>
  <HistoricoAgendamentosBase
    titulo="Minha operação"
    descricao="Responda solicitações, acompanhe a execução e visualize seu calendário de serviços <strong>confirmados</strong>."
    :carregando="carregando"
    :aba-atual="abaAtual"
    :paineis="paineis"
    calendario-etiqueta="Calendário operacional"
    :eventos-calendario="agendaConfirmada"
    resumo-mes-texto="Serviços <strong>confirmados</strong> neste mês."
    vazio-dia-texto="Nenhum serviço confirmado neste dia."
    @selecionar-aba="selecionarAba"
  >
    <template v-for="painel in paineis" #[`painel-${painel.id}`] :key="painel.id">
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
                <span class="font-semibold text-sm">{{ ag.clienteNome }}</span>
                <HtBadge :variant="obterVariantStatus(ag.status)">{{ labelStatus(ag) }}</HtBadge>
              </div>
              <p v-if="ag.servicos.length" class="text-xs text-base-content/60 mb-2">
                {{ ag.servicos.map((s) => s.titulo).join(", ") }}
              </p>
              <p class="text-sm text-base-content/80">
                {{ formatarDataCurta(ag.dataHoraAgendada) }} às {{ formatarHora(ag.dataHoraAgendada) }}
              </p>
            </div>
            <div class="text-right shrink-0">
              <p class="font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
            </div>
          </div>
        </HtCard>
      </div>
    </template>

    <template #calendario-evento="{ evento }">
      <p class="text-[11px] font-semibold leading-none">{{ formatarHora(evento.dataHoraAgendada) }}</p>
      <p class="mt-1 truncate text-[11px] text-base-content/70">{{ evento.clienteNome }}</p>
    </template>

    <template #dia-selecionado="{ eventos }">
      <button
        v-for="ag in eventos"
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
              {{ titulosServicos(ag) }}
            </p>
          </div>
          <p class="shrink-0 font-semibold">{{ formatarMoeda(ag.valorTotal) }}</p>
        </div>
      </button>
    </template>
  </HistoricoAgendamentosBase>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRoute, useRouter } from "vue-router";
import HtBadge, { HtBadgeVariant } from "@/components/ui/HtBadge.vue";
import HtCard from "@/components/ui/HtCard.vue";
import api from "@/services/api";
import { formatarDataCurta, formatarHora, formatarMoeda } from "@/shared/utils";
import { StatusAgendamento, TipoUsuario, type AgendamentoResumo } from "@/types";
import HistoricoAgendamentosBase from "./components/HistoricoAgendamentosBase.vue";

type AbaAgendamentosOperacao = "painel" | "calendario";
type PainelAgendamentosOperacaoTom = "primary" | "secondary" | "accent" | "success" | "warning" | "error" | "neutro";

const agendamentos = ref<AgendamentoResumo[]>([]);
const carregando = ref(true);

const route = useRoute();
const router = useRouter();
const abaAtual = computed<AbaAgendamentosOperacao>(() => (route.query.aba === "calendario" ? "calendario" : "painel"));

const aguardandoSuaConfirmacao = computed(() =>
  ordenarPorData(
    agendamentos.value.filter(
      (ag) => ag.status === StatusAgendamento.Solicitado && ag.aguardandoRespostaDe === TipoUsuario.Prestador,
    ),
  ),
);
const aguardandoOutraParte = computed(() =>
  ordenarPorData(
    agendamentos.value.filter(
      (ag) => ag.status === StatusAgendamento.Solicitado && ag.aguardandoRespostaDe === TipoUsuario.Cliente,
    ),
  ),
);
const agendados = computed(() =>
  ordenarPorData(agendamentos.value.filter((ag) => ag.status === StatusAgendamento.Aceito)),
);
const emAndamento = computed(() =>
  ordenarPorData(agendamentos.value.filter((ag) => ag.status === StatusAgendamento.EmAndamento)),
);
const aguardandoPagamento = computed(() =>
  ordenarPorData(agendamentos.value.filter((ag) => ag.status === StatusAgendamento.AguardandoPagamento)),
);
const concluidos = computed(() =>
  ordenarPorData(agendamentos.value.filter((ag) => ag.status === StatusAgendamento.Concluido)),
);
const canceladosOuRecusados = computed(() =>
  ordenarPorData(
    agendamentos.value.filter(
      (ag) => ag.status === StatusAgendamento.Cancelado || ag.status === StatusAgendamento.Recusado,
    ),
  ),
);
const paineis = computed(() => [
  painel(
    "aguardando-sua-confirmacao",
    "Aguardando sua confirmação",
    "Solicitações pendentes da sua resposta.",
    aguardandoSuaConfirmacao.value,
    "Nenhuma solicitação aguardando sua confirmação.",
    "warning",
    "schedule",
  ),
  painel(
    "aguardando-outra-parte",
    "Aguardando confirmação do cliente",
    "Pedidos enviados que aguardam retorno do cliente.",
    aguardandoOutraParte.value,
    "Nenhuma solicitação aguardando confirmação do cliente.",
    "warning",
    "schedule",
  ),
  painel(
    "agendados",
    "Agendados",
    "Serviços confirmados e prontos para iniciar.",
    agendados.value,
    "Nenhum serviço agendado.",
    "primary",
    "event_available",
  ),
  painel(
    "em-andamento",
    "Em andamento",
    "Serviços em execução no momento.",
    emAndamento.value,
    "Nenhum serviço em andamento.",
    "accent",
    "play_circle",
  ),
  painel(
    "aguardando-pagamento",
    "Aguardando pagamento",
    "Serviços concluídos e pendentes de confirmação do gateway.",
    aguardandoPagamento.value,
    "Nenhum serviço aguardando pagamento.",
    "secondary",
    "payments",
  ),
  painel(
    "concluidos",
    "Concluídos",
    "Histórico de serviços finalizados.",
    concluidos.value,
    "Nenhum serviço concluído.",
    "success",
    "task_alt",
  ),
  painel(
    "cancelados-recusados",
    "Cancelados ou recusados",
    "Solicitações encerradas sem execução.",
    canceladosOuRecusados.value,
    "Nenhum serviço cancelado ou recusado.",
    "error",
    "cancel",
  ),
]);
const agendaConfirmada = computed(() =>
  agendamentos.value
    .filter(
      (ag) =>
        (ag.status === StatusAgendamento.Aceito || ag.status === StatusAgendamento.EmAndamento) &&
        new Date(ag.dataHoraAgendada) >= inicioDoDia(new Date()),
    )
    .sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime()),
);
onMounted(async () => {
  try {
    const { data } = await api.get<AgendamentoResumo[]>("/api/Agendamento/ObterMeusAgendamentosPrestador");
    agendamentos.value = data;
  } finally {
    carregando.value = false;
  }
});

function selecionarAba(aba: AbaAgendamentosOperacao) {
  const query = { ...route.query };
  if (aba === "calendario") query.aba = "calendario";
  else delete query.aba;
  router.replace({ query });
}

function abrirDetalhes(id: string) {
  router.push({ path: `/agendamento/detalhes/${id}`, query: { origem: "operacao" } });
}

function ordenarPorData(lista: AgendamentoResumo[]) {
  return [...lista].sort((a, b) => new Date(a.dataHoraAgendada).getTime() - new Date(b.dataHoraAgendada).getTime());
}

function titulosServicos(ag: any) {
  return (ag.servicos as { titulo: string }[]).map((s) => s.titulo).join(", ");
}

function painel(
  id: string,
  titulo: string,
  descricao: string,
  itens: AgendamentoResumo[],
  vazio: string,
  tom: PainelAgendamentosOperacaoTom,
  icone: string,
) {
  return { id, titulo, descricao, itens, vazio, quantidade: itens.length, tom, icone };
}

function inicioDoDia(data: Date) {
  return new Date(data.getFullYear(), data.getMonth(), data.getDate());
}

function labelStatus(ag: AgendamentoResumo): string {
  if (ag.status === StatusAgendamento.Solicitado && ag.aguardandoRespostaDe === TipoUsuario.Prestador)
    return "Aguardando sua confirmação";
  if (ag.status === StatusAgendamento.Solicitado && ag.aguardandoRespostaDe === TipoUsuario.Cliente)
    return "Aguardando confirmação do cliente";
  if (ag.status === StatusAgendamento.Aceito) return "Agendado";
  if (ag.status === StatusAgendamento.EmAndamento) return "Em andamento";
  if (ag.status === StatusAgendamento.AguardandoPagamento) return "Aguardando pagamento";
  if (ag.status === StatusAgendamento.Concluido) return "Concluído";
  if (ag.status === StatusAgendamento.Recusado) return "Recusado";
  if (ag.status === StatusAgendamento.Cancelado) return "Cancelado";
  return "Solicitado";
}

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
