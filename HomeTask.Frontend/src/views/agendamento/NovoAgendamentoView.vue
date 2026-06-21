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

      <h1 class="text-title font-semibold text-foreground mb-6">Novo agendamento</h1>

      <div v-if="carregandoServico" class="flex justify-center py-16">
        <HtSpinner size="lg" class="text-primary" />
      </div>

      <HtCard v-else-if="servico">
        <div class="mb-4">
          <h2 class="text-base font-semibold text-foreground">{{ servico.titulo }}</h2>
          <p class="text-sm text-muted">
            {{ servico.prestadorNome || "Prestador" }} ·
            {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
          </p>
        </div>

        <HtDivider />
        <HtAlert v-if="erro" :message="erro" class="mb-4" />

        <form class="flex flex-col gap-4" @submit.prevent="handleAgendar">
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

          <HtInput
            ref="inputEndereco"
            v-model="form.logradouro"
            label="Endereço do serviço"
            placeholder="Rua, número, bairro..."
            regra="required"
            required
          />

          <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <HtSearchSelect
              ref="inputEstado"
              v-model="form.estado"
              label="Estado (UF)"
              :options="UF_OPTIONS"
              placeholder="Busque pela UF"
              required
            />

            <HtSelect
              ref="inputCidade"
              v-model="form.cidadeId"
              label="Cidade"
              :options="cidadesDisponiveis"
              :placeholder="form.estado ? 'Selecione a cidade' : 'Selecione primeiro o estado'"
              :hint="cidadeHint"
              :disabled="!form.estado || carregandoCidades"
              required
            />
          </div>

          <HtTextarea
            v-model="form.observacoes"
            label="Observações"
            placeholder="Alguma informação adicional..."
            :rows="3"
          />

          <HtButton type="submit" :loading="carregando" class="w-full mt-2">
            Confirmar agendamento
          </HtButton>
        </form>
      </HtCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, watch } from "vue"
import { useRouter } from "vue-router"
import api from "@/services/api"
import { useAuthStore } from "@/stores/auth"
import { validarCampos } from "@/shared/validacao"
import { formatarPrecoServico } from "@/shared/utils"
import type { AgendamentoForm, ServicoPrestadorDetalhe } from "@/types"
import HtInput from "@/components/ui/HtInput.vue"
import HtSearchSelect from "@/components/ui/HtSearchSelect.vue"
import HtSelect from "@/components/ui/HtSelect.vue"
import HtTextarea from "@/components/ui/HtTextarea.vue"
import HtButton from "@/components/ui/HtButton.vue"
import HtCard from "@/components/ui/HtCard.vue"
import HtAlert from "@/components/ui/HtAlert.vue"
import HtSpinner from "@/components/ui/HtSpinner.vue"
import HtDivider from "@/components/ui/HtDivider.vue"
import { UF_OPTIONS } from "@/statics/selects"
import { useCidadeEstadoOptions } from "@/composables/useCidadeEstadoOptions"

const props = defineProps<{ servicoId: string }>()

const router = useRouter()
const auth = useAuthStore()

const servico = ref<ServicoPrestadorDetalhe | null>(null)
const carregandoServico = ref(true)
const carregando = ref(false)
const erro = ref<string | null>(null)

const form = reactive<AgendamentoForm>({
  data: "",
  hora: "",
  logradouro: "",
  cidadeId: "",
  estado: "",
  observacoes: "",
})

const {
  carregandoCidades,
  cidadesDisponiveis,
  cidadeHint,
  carregarCidades,
} = useCidadeEstadoOptions(() => form.estado)

const inputData = ref<InstanceType<typeof HtInput> | null>(null)
const inputHora = ref<InstanceType<typeof HtInput> | null>(null)
const inputEndereco = ref<InstanceType<typeof HtInput> | null>(null)
const inputEstado = ref<InstanceType<typeof HtSearchSelect> | null>(null)
const inputCidade = ref<InstanceType<typeof HtSelect> | null>(null)

onMounted(async () => {
  void carregarCidades()

  try {
    const { data } = await api.get<ServicoPrestadorDetalhe>("/api/ServicoOferecido/ObterServicoPorId", {
      params: { id: props.servicoId },
    })

    if (!data || !("prestadorId" in data)) {
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

watch(
  () => form.estado,
  () => {
    const cidadeSelecionadaPermaneceValida = cidadesDisponiveis.value.some(
      (cidade) => cidade.value === form.cidadeId,
    )

    if (!cidadeSelecionadaPermaneceValida) {
      form.cidadeId = ""
    }
  },
)

async function handleAgendar() {
  if (!validarCampos([inputData.value, inputHora.value, inputEndereco.value, inputEstado.value, inputCidade.value])) return
  if (!servico.value) return

  erro.value = null
  carregando.value = true

  try {
    const { data: cliente } = await api.get("/api/Cliente/ObterClientesPorUsuarioId", {
      params: { usuarioId: auth.user?.userId },
    })

    const dataHora = `${form.data}T${form.hora}:00`

    await api.post("/api/Agendamento/CriarAgendamento", {
      clienteId: cliente.id,
      prestadorId: servico.value.prestadorId,
      principalServicoPrestadorId: props.servicoId,
      servicosOferecidosIds: [props.servicoId],
      dataHoraAgendada: dataHora,
      cidadeId: form.cidadeId,
      enderecoDescricao: form.logradouro,
      observacoes: form.observacoes,
    })

    router.push("/agendamento/sucesso")
  } catch (err: unknown) {
    const e = err as { response?: { data?: unknown } }
    const msg = e.response?.data
    erro.value =
      typeof msg === "string" && msg
        ? msg
        : "Erro ao criar agendamento. Verifique os dados e tente novamente."
  } finally {
    carregando.value = false
  }
}
</script>
