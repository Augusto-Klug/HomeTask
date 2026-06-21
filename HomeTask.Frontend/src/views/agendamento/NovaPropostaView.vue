<template>
  <div class="max-w-2xl mx-auto px-4 py-8">
    <div v-if="!servico?.id && !carregandoServico" class="text-center py-16 flex flex-col items-center gap-4">
      <p class="text-sm text-muted">Parametros invalidos.</p>
      <router-link to="/servicos/buscar">
        <HtButton variant="outline">Voltar a busca</HtButton>
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
          <div class="flex flex-wrap items-center gap-x-3 gap-y-1 text-sm text-muted">
            <p>
              <router-link :to="`/clientes/${servico.clienteId}`" class="font-medium text-primary hover:underline">
                {{ servico.clienteNome || "Cliente" }}
              </router-link>
              · {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
            </p>
            <p>{{ formatarEnderecoServico(servico) }}</p>
          </div>
        </div>

        <div v-if="servico.dataDesejada" class="mb-4 rounded-lg border border-primary/10 bg-primary/5 p-3 text-sm">
          Data desejada: {{ formatarData(servico.dataDesejada) }} as {{ formatarHora(servico.dataDesejada) }}
        </div>

        <HtDivider />
        <HtAlert v-if="erro" :message="erro" class="mb-4" />

        <form class="flex flex-col gap-4" @submit.prevent="handleEnviarProposta">
          <HtInput
            v-if="exibirCampoValorProposta"
            v-model="valorProposta"
            label="Valor da proposta (R$)"
            type="number"
            :allowNegative="false"
            placeholder="Ex: 180"
          />

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
              label="Horario"
              type="time"
              :mensagemErro="'Selecione um horario'"
              required
              regra="required"
            />
          </div>

          <HtTextarea
            v-model="form.observacoes"
            label="Observacoes"
            placeholder="Alguma informacao adicional..."
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
import { computed, onMounted, reactive, ref } from "vue"
import { useRouter } from "vue-router"
import api from "@/services/api"
import { validarCampos } from "@/shared/validacao"
import { formatarData, formatarHora, formatarPrecoServico } from "@/shared/utils"
import { useAuthStore } from "@/stores/auth"
import { UnidadeCobranca, type AgendamentoForm, type ServicoClienteDetalhe } from "@/types"
import HtAlert from "@/components/ui/HtAlert.vue"
import HtButton from "@/components/ui/HtButton.vue"
import HtCard from "@/components/ui/HtCard.vue"
import HtDivider from "@/components/ui/HtDivider.vue"
import HtInput from "@/components/ui/HtInput.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"
import HtTextarea from "@/components/ui/HtTextarea.vue"

const props = defineProps<{ servicoId: string }>()

const auth = useAuthStore()
const router = useRouter()

const servico = ref<ServicoClienteDetalhe | null>(null)
const carregandoServico = ref(true)
const carregando = ref(false)
const erro = ref<string | null>(null)
const valorProposta = ref("")

const form = reactive<AgendamentoForm>({
  data: "",
  hora: "",
  logradouro: "",
  observacoes: "",
})

const inputData = ref<InstanceType<typeof HtInput> | null>(null)
const inputHora = ref<InstanceType<typeof HtInput> | null>(null)

const exibirCampoValorProposta = computed(() => servico.value?.unidadeCobranca === UnidadeCobranca.ACombinar)

onMounted(async () => {
  try {
    const { data } = await api.get<ServicoClienteDetalhe>(
      "/api/ServicoOferecido/ObterServicoPorId",
      { params: { id: props.servicoId } },
    )

    if (!data || !("clienteId" in data)) {
      servico.value = null
      return
    }

    servico.value = data
  } catch {
    servico.value = null
  } finally {
    carregandoServico.value = false
  }
})

async function handleEnviarProposta() {
  if (!validarCampos([inputData.value, inputHora.value])) return;
  if (!servico.value) return;
  const servicoPrestadorPrincipalId = obterServicoPrestadorPrincipalId(servicosPrestador.value, servico.value);
  if (!servicoPrestadorPrincipalId) {
    erro.value = "Não foi possível identificar um serviço do prestador para esta proposta.";
    return;
  }

  erro.value = null
  carregando.value = true

  try {
    const { data: prestador } = await api.get("/api/Prestador/ObterPrestadorPorUsuarioId", {
      params: { usuarioId: auth.user?.userId },
    })

    const dataHora = `${form.data}T${form.hora}:00`

    await api.post("/api/Agendamento/CriarAgendamento", {
      clienteId: servico.value.clienteId,
      prestadorId: prestador.id,
      servicosOferecidosIds: [props.servicoId],
      dataHoraAgendada: dataHora,
      observacoes: form.observacoes,
      valorProposto: exibirCampoValorProposta.value ? Number(valorProposta.value) : undefined,
    })

    router.push("/agendamento/sucesso")
  } catch (err: unknown) {
    const e = err as { response?: { data?: unknown } }
    const msg = e.response?.data
    erro.value =
      typeof msg === "string" && msg
        ? msg
        : "Erro ao enviar proposta. Verifique os dados e tente novamente."
  } finally {
    carregando.value = false
  }
}

function formatarEnderecoServico(servicoAtual: ServicoClienteDetalhe) {
  const base = [servicoAtual.logradouro, servicoAtual.numero].filter(Boolean).join(", ")
  const bairro = servicoAtual.bairro ? ` - ${servicoAtual.bairro}` : ""
  const cidadeEstado = [servicoAtual.cidade || "N/A", servicoAtual.estado || "N/A"].join("/")

  return base ? `${base}${bairro} - ${cidadeEstado}` : cidadeEstado
}
</script>
