<template>
  <div class="max-w-2xl mx-auto px-4 py-8">
    <div v-if="!servico?.id && !carregandoServico" class="text-center py-16 flex flex-col items-center gap-4">
      <p class="text-sm text-muted">Parâmetros inválidos.</p>
      <router-link to="/servicos/buscar">
        <HtButton variant="outline">Voltar à busca</HtButton>
      </router-link>
    </div>

    <template v-else>
      <router-link
        to="/servicos/buscar"
        class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
      >
        <span class="material-symbols-rounded text-base">arrow_back</span>
        Voltar
      </router-link>

      <h1 class="text-title font-semibold text-foreground mb-6">Enviar proposta</h1>

      <div v-if="carregandoServico" class="flex justify-center py-16">
        <HtSpinner size="lg" class="text-primary" />
      </div>

      <HtCard v-else-if="servico">
        <div class="mb-4">
          <h2 class="text-base font-semibold text-foreground">{{ servico.titulo }}</h2>
          <p class="text-sm text-muted">
            {{ servico.clienteNome || "Cliente" }} ·
            {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
          </p>
        </div>

        <div v-if="servico.dataDesejada" class="mb-4 rounded-lg border border-primary/10 bg-primary/5 p-3 text-sm">
          Data desejada: {{ formatarData(servico.dataDesejada) }} às {{ formatarHora(servico.dataDesejada) }}
        </div>

        <HtDivider />
        <HtAlert v-if="erro" :message="erro" class="mb-4" />

        <form class="flex flex-col gap-4" @submit.prevent="handleEnviarProposta">
          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <HtInput
              ref="inputData"
              v-model="form.data"
              label="Data"
              type="date"
              :mensagemErro="'Selecione uma data'"
              required
              regra="required"
            />
            <HtInput
              ref="inputHora"
              v-model="form.hora"
              label="Horário"
              type="time"
              :mensagemErro="'Selecione um horário'"
              required
              regra="required"
            />
          </div>

          <HtTextarea
            v-model="form.observacoes"
            label="Observações"
            placeholder="Alguma informação adicional..."
            :rows="3"
          />

          <HtButton type="submit" :loading="carregando" class="w-full mt-2">
            Enviar Proposta
          </HtButton>
        </form>
      </HtCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from "vue";
import { useRouter } from "vue-router";
import api from "@/services/api";
import { validarCampos } from "@/shared/validacao";
import { formatarData, formatarHora, formatarPrecoServico } from "@/shared/utils";
import { useAuthStore } from "@/stores/auth";
import type { AgendamentoForm, ServicoClienteDetalhe } from "@/types";
import HtAlert from "@/components/ui/HtAlert.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtDivider from "@/components/ui/HtDivider.vue";
import HtInput from "@/components/ui/HtInput.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtTextarea from "@/components/ui/HtTextarea.vue";

const props = defineProps<{ servicoId: string }>();

const auth = useAuthStore();
const router = useRouter();

const servico = ref<ServicoClienteDetalhe | null>(null);
const carregandoServico = ref(true);
const carregando = ref(false);
const erro = ref<string | null>(null);
const servicosPrestador = ref<ServicoPrestadorResumo[]>([]);

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
      { params: { id: props.servicoId } },
    );

    if (!data || !("clienteId" in data)) {
      servico.value = null;
      return;
    }

    servico.value = data;

    const { data: prestador } = await api.get<{ id: string }>("/api/Prestador/ObterPrestadorPorUsuarioId", {
      params: { usuarioId: auth.user?.userId },
    });

    const { data: servicos } = await api.get<ServicoPrestadorResumo[]>("/api/ServicoOferecido/ObterServicosPorPrestador", {
      params: { prestadorId: prestador.id },
    });

    servicosPrestador.value = servicos;
  } catch {
    servico.value = null;
  } finally {
    carregandoServico.value = false;
  }
});

async function handleEnviarProposta() {
  if (!validarCampos([inputData.value, inputHora.value])) return;
  if (!servico.value) return;
  const servicoPrestadorPrincipalId = obterServicoPrestadorPrincipalId(servicosPrestador.value, servico.value);
  if (!servicoPrestadorPrincipalId) {
    erro.value = "Não foi possível identificar um serviço do prestador para esta proposta.";
    return;
  }

  erro.value = null;
  carregando.value = true;

  try {
    const { data: prestador } = await api.get("/api/Prestador/ObterPrestadorPorUsuarioId", {
      params: { usuarioId: auth.user?.userId },
    });

    const dataHora = `${form.data}T${form.hora}:00`;

    await api.post("/api/Agendamento/CriarAgendamento", {
      clienteId: servico.value.clienteId,
      prestadorId: prestador.id,
      principalServicoPrestadorId: servicoPrestadorPrincipalId,
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

function obterServicoPrestadorPrincipalId(
  servicosDoPrestador: ServicoPrestadorResumo[],
  servicoClienteAtual: ServicoClienteDetalhe,
) {
  const categoriaPedido = obterCategoriaId(servicoClienteAtual.categoria);
  const servicoDaMesmaCategoria = servicosDoPrestador.find((item) => item.ativo && obterCategoriaId(item.categoria) === categoriaPedido);
  if (servicoDaMesmaCategoria) return servicoDaMesmaCategoria.id;

  const primeiroServicoAtivo = servicosDoPrestador.find((item) => item.ativo);
  if (primeiroServicoAtivo) return primeiroServicoAtivo.id;

  return servicosDoPrestador[0]?.id ?? "";
}

function obterCategoriaId(categoria: number | { id: string }) {
  return typeof categoria === "number" ? categoria : Number(categoria.id);
}

interface ServicoPrestadorResumo {
  id: string
  categoria: number | { id: string }
  ativo: boolean
}

</script>
