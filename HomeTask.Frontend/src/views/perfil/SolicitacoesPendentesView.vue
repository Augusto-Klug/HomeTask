<template>
  <div class="container mx-auto px-4 py-8 max-w-4xl">
    <div class="flex flex-wrap items-center justify-between gap-3 mb-6">
      <div>
        <h1 class="text-2xl font-bold">Solicitações pendentes</h1>
        <p class="text-sm text-base-content/60">
          Pedidos aguardando resposta do prestador.
        </p>
      </div>
      <router-link to="/perfil/agendamentos">
        <HtButton variant="outline">Ver meus agendamentos</HtButton>
      </router-link>
    </div>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" />
    </div>

    <div v-else class="space-y-4">
      <HtAlert
        v-if="erro"
        variant="error"
        :message="erro"
      />

      <div
        v-if="!erro && solicitacoes.length === 0"
        class="text-sm text-base-content/50 py-10 text-center border border-dashed border-base-300 rounded-xl"
      >
        Nenhuma solicitação pendente no momento.
      </div>

      <HtCard
        v-for="agendamento in solicitacoes"
        :key="agendamento.id"
      >
        <div class="flex flex-col gap-4">
          <div class="flex flex-wrap items-start justify-between gap-3">
            <div>
              <div class="flex items-center gap-2 flex-wrap mb-1">
                <p class="font-semibold">{{ agendamento.clienteNome }}</p>
                <HtBadge variant="outline">Solicitado</HtBadge>
              </div>
              <p class="text-sm text-base-content/70">
                {{ agendamento.servicos.map((s) => s.titulo).join(", ") }}
              </p>
            </div>
            <div class="text-right">
              <p class="font-semibold">{{ formatarMoeda(agendamento.valorTotal) }}</p>
              <p class="text-xs text-base-content/60">
                Solicitado em {{ formatarData(agendamento.dataSolicitacao) }}
              </p>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-2 text-sm text-base-content/80">
            <div class="flex items-center gap-2">
              <span class="material-symbols-rounded text-base text-base-content/40">
                calendar_today
              </span>
              <span>
                {{ formatarData(agendamento.dataHoraAgendada) }} às
                {{ formatarHora(agendamento.dataHoraAgendada) }}
              </span>
            </div>

            <div class="flex items-center gap-2">
              <span class="material-symbols-rounded text-base text-base-content/40">
                location_on
              </span>
              <span class="truncate">{{ formatarEndereco(agendamento.endereco) }}</span>
            </div>
          </div>

          <div class="flex flex-wrap gap-3">
            <HtButton
              :loading="acaoEmAndamentoId === agendamento.id"
              @click="aceitarSolicitacao(agendamento.id)"
            >
              Aceitar
            </HtButton>
            <HtButton
              variant="outline"
              class="border-error text-error hover:bg-error/10"
              :disabled="acaoEmAndamentoId === agendamento.id"
              @click="abrirModalRecusa(agendamento.id)"
            >
              Recusar
            </HtButton>
            <router-link :to="{ path: `/agendamento/detalhes/${agendamento.id}`, query: { origem: 'pendentes' } }">
              <HtButton variant="outline">Ver detalhes</HtButton>
            </router-link>
          </div>
        </div>
      </HtCard>
    </div>
  </div>

  <div
    v-if="modalRecusaAberto && solicitacaoSelecionadaId"
    class="fixed inset-0 bg-black/50 flex items-center justify-center p-4 z-50"
  >
    <HtCard class="w-full max-w-md">
      <h2 class="text-lg font-bold mb-4">Recusar solicitação</h2>
      <HtTextarea
        v-model="motivoRecusa"
        label="Motivo da recusa"
        placeholder="Explique o motivo para o cliente..."
        required
      />

      <div class="flex gap-3 mt-6">
        <HtButton
          variant="outline"
          class="flex-1"
          :disabled="carregandoRecusa"
          @click="fecharModalRecusa"
        >
          Voltar
        </HtButton>
        <HtButton
          variant="error"
          class="flex-1"
          :loading="carregandoRecusa"
          @click="confirmarRecusa"
        >
          Confirmar recusa
        </HtButton>
      </div>
    </HtCard>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useAuthStore } from "@/stores/auth";
import api from "@/services/api";
import type { AgendamentoResumo } from "@/types";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";

const auth = useAuthStore();

const carregando = ref(true);
const erro = ref<string | null>(null);
const solicitacoes = ref<AgendamentoResumo[]>([]);
const acaoEmAndamentoId = ref<string | null>(null);

const modalRecusaAberto = ref(false);
const solicitacaoSelecionadaId = ref<string | null>(null);
const motivoRecusa = ref("");
const carregandoRecusa = ref(false);

async function carregarSolicitacoesPendentes() {
  carregando.value = true;
  erro.value = null;

  try {
    if (!auth.user?.userId) {
      solicitacoes.value = [];
      return;
    }

    const { data: agendamentos } = await api.get<AgendamentoResumo[]>(
      "/api/Agendamento/ObterSolicitacoesPendentesPrestador",
    );

    solicitacoes.value = agendamentos
      .sort(
        (a, b) =>
          new Date(b.dataSolicitacao).getTime() -
          new Date(a.dataSolicitacao).getTime(),
      );
  } catch {
    erro.value = "Não foi possível carregar as solicitações pendentes.";
    solicitacoes.value = [];
  } finally {
    carregando.value = false;
  }
}

async function aceitarSolicitacao(agendamentoId: string) {
  acaoEmAndamentoId.value = agendamentoId;

  try {
    await api.post("/api/Agendamento/AceitarAgendamento", {
      id: agendamentoId,
    });
    solicitacoes.value = solicitacoes.value.filter((a) => a.id !== agendamentoId);
  } catch {
    erro.value = "Não foi possível aceitar a solicitação.";
  } finally {
    acaoEmAndamentoId.value = null;
  }
}

function abrirModalRecusa(agendamentoId: string) {
  solicitacaoSelecionadaId.value = agendamentoId;
  motivoRecusa.value = "";
  modalRecusaAberto.value = true;
}

function fecharModalRecusa() {
  modalRecusaAberto.value = false;
  solicitacaoSelecionadaId.value = null;
  motivoRecusa.value = "";
}

async function confirmarRecusa() {
  if (!solicitacaoSelecionadaId.value || !motivoRecusa.value.trim()) {
    erro.value = "Informe o motivo da recusa.";
    return;
  }

  carregandoRecusa.value = true;

  try {
    await api.post("/api/Agendamento/RecusarAgendamento", {
      id: solicitacaoSelecionadaId.value,
      motivoRecusa: motivoRecusa.value.trim(),
    });

    solicitacoes.value = solicitacoes.value.filter(
      (a) => a.id !== solicitacaoSelecionadaId.value,
    );
    fecharModalRecusa();
  } catch {
    erro.value = "Não foi possível recusar a solicitação.";
  } finally {
    carregandoRecusa.value = false;
  }
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
  return valor.toLocaleString("pt-BR", {
    style: "currency",
    currency: "BRL",
  });
}

onMounted(carregarSolicitacoesPendentes);
</script>
