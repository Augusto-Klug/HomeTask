<template>
  <div class="max-w-2xl mx-auto px-4 py-8">
    <router-link
      to="/servicos/buscar"
      class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
    >
      <span class="material-symbols-rounded text-base">arrow_back</span>
      Voltar
    </router-link>

    <h1 class="text-title font-semibold text-foreground mb-6">
      Enviar Proposta
    </h1>

    <div v-if="carregandoServico" class="flex justify-center py-16">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <div
      v-else-if="!servico"
      class="text-center py-16 flex flex-col items-center gap-4"
    >
      <p class="text-sm text-muted">
        Parâmetros inválidos para enviar uma proposta.
      </p>
      <router-link to="/servicos/buscar">
        <HtButton variant="outline">Voltar à busca</HtButton>
      </router-link>
    </div>

    <HtCard v-else>
      <div class="mb-4">
        <h2 class="text-base font-semibold text-foreground">
          {{ servico.titulo }}
        </h2>
        <p class="text-sm text-muted">
          Solicitante:
          {{ servico.clienteNome || "Cliente não informado" }}
        </p>
      </div>

      <div class="grid grid-cols-1 sm:grid-cols-2 gap-3 mb-4 text-sm text-muted">
        <div>
          <span class="font-medium text-foreground">Preço base:</span>
          R$ {{ formatarPreco(servico.precoBase) }}
          {{ servico.unidadeCobranca === "por_hora" ? "/h" : "" }}
        </div>
        <div v-if="servico.dataDesejada">
          <span class="font-medium text-foreground">Data desejada:</span>
          {{ formatarDataHora(servico.dataDesejada) }}
        </div>
      </div>

      <p v-if="servico.descricao" class="text-sm text-foreground mb-4">
        {{ servico.descricao }}
      </p>

      <HtDivider />

      <HtAlert v-if="erro" :message="erro" class="mb-4" />

      <form class="flex flex-col gap-4" @submit.prevent="handleEnviarProposta">
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <HtInput
            ref="inputData"
            v-model="form.data"
            label="Data sugerida"
            type="date"
            :mensagemErro="'Selecione uma data'"
            required
            regra="required"
          />
          <HtInput
            ref="inputHora"
            v-model="form.hora"
            label="Horário sugerido"
            type="time"
            :mensagemErro="'Selecione um horário'"
            required
            regra="required"
          />
        </div>

        <HtTextarea
          v-model="form.observacoes"
          label="Observações"
          placeholder="Detalhes adicionais para a execução do serviço..."
          :rows="4"
        />

        <HtButton type="submit" :loading="carregando" class="w-full mt-2">
          Enviar Proposta
        </HtButton>
      </form>
    </HtCard>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from "vue";
import { useRouter } from "vue-router";
import api from "@/services/api";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtDivider from "@/components/ui/HtDivider.vue";
import HtInput from "@/components/ui/HtInput.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";
import { validarCampos } from "@/shared/validacao";
import { useAuthStore } from "@/stores/auth";
import type { AgendamentoForm, ServicoClienteDetalhe } from "@/types";

const props = defineProps<{ servicoId: string }>();

const router = useRouter();
const auth = useAuthStore();

const servico = ref<ServicoClienteDetalhe | null>(null);
const carregandoServico = ref(true);
const carregando = ref(false);
const erro = ref<string | null>(null);

const form = reactive<AgendamentoForm>({
  data: "",
  hora: "",
  logradouro: "",
  observacoes: "",
});

const inputData = ref<InstanceType<typeof HtInput> | null>(null);
const inputHora = ref<InstanceType<typeof HtInput> | null>(null);

onMounted(async () => {
  try {
    const { data } = await api.get<ServicoClienteDetalhe>(
      "/api/ServicoOferecido/ObterServicoPorId",
      {
        params: { id: props.servicoId },
      },
    );

    servico.value = data.tipoAnuncio === 2 ? data : null;
  } catch {
    servico.value = null;
  } finally {
    carregandoServico.value = false;
  }
});

async function handleEnviarProposta() {
  if (!validarCampos([inputData.value, inputHora.value])) {
    return;
  }

  if (!servico.value) {
    erro.value = "Serviço inválido para este fluxo de proposta.";
    return;
  }

  erro.value = null;
  carregando.value = true;

  try {
    const { data: prestador } = await api.get(
      "/api/Prestador/ObterPrestadorPorUsuarioId",
      {
        params: { usuarioId: auth.user?.userId },
      },
    );

    const dataHora = `${form.data}T${form.hora}:00`;

    await api.post("/api/Agendamento/CriarAgendamento", {
      prestadorId: prestador.id,
      servicosOferecidosIds: [props.servicoId],
      dataHoraAgendada: dataHora,
      observacoes: form.observacoes,
    });

    router.push("/agendamento/sucesso");
  } catch (err: unknown) {
    const e = err as { response?: { data?: unknown } };
    const msg = e.response?.data;
    erro.value =
      typeof msg === "string" && msg
        ? msg
        : "Erro ao enviar proposta. Verifique os dados e tente novamente.";
  } finally {
    carregando.value = false;
  }
}

function formatarPreco(valor: number): string {
  return Number(valor).toFixed(2).replace(".", ",");
}

function formatarDataHora(valor: string): string {
  return new Date(valor).toLocaleString("pt-BR", {
    dateStyle: "short",
    timeStyle: "short",
  });
}
</script>
