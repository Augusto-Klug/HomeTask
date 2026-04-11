<template>
  <div class="max-w-2xl mx-auto px-4 py-8">
    <!-- Voltar -->
    <router-link
      to="/servicos/buscar"
      class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
    >
      <span class="material-symbols-rounded text-base">arrow_back</span>
      Voltar à busca
    </router-link>

    <!-- Sucesso -->
    <HtCard v-if="sucesso" class="text-center py-8">
      <span class="material-symbols-rounded text-5xl text-success mb-4 block">check_circle</span>
      <h2 class="text-title font-semibold text-foreground mb-2">Serviço publicado!</h2>
      <p class="text-sm text-muted mb-6">
        Seu serviço foi publicado com sucesso. Clientes poderão encontrá-lo na busca e entrar em contato.
      </p>
      <div class="flex flex-wrap justify-center gap-3">
        <router-link to="/servicos/buscar">
          <HtButton variant="outline">Ver serviços disponíveis</HtButton>
        </router-link>
        <HtButton @click="reiniciar">Publicar outro serviço</HtButton>
      </div>
    </HtCard>

    <template v-else>
      <h1 class="text-title font-semibold text-foreground mb-1">Anunciar Meu Serviço</h1>
      <p class="text-sm text-muted mb-6">
        Cadastre o serviço que você oferece. Clientes interessados poderão entrar em contato para
        combinar detalhes como quantidade de horas e data de execução.
      </p>

      <HtCard>
        <HtAlert v-if="erro" :message="erro" class="mb-4" />

        <form class="flex flex-col gap-4" @submit.prevent="handleSubmit">
          <!-- Título -->
          <HtInput
            ref="refTitulo"
            v-model="form.titulo"
            label="Título do serviço"
            placeholder="Ex: Corte de árvore e poda, Faxina completa..."
            required
            regra="required"
          />

          <!-- Descrição -->
          <HtTextarea
            v-model="form.descricao"
            label="Descrição"
            placeholder="Descreva o serviço que você oferece, sua experiência, equipamentos que possui..."
            :rows="3"
          />

          <!-- Categoria -->
          <HtSelect
            ref="refCategoria"
            v-model="form.categoria"
            :options="categoriasOpcoes"
            label="Categoria"
            placeholder="Selecione uma categoria"
            required
          />

          <!-- Modalidade de cobrança -->
          <HtSelect
            ref="refTipoValor"
            v-model="form.unidadeCobranca"
            :options="tiposValorOpcoes"
            label="Modalidade de cobrança"
            placeholder="Como você cobra pelo serviço?"
            required
          />

          <!-- Valor -->
          <HtInput
            ref="refValor"
            v-model="form.valor"
            label="Valor (R$)"
            type="number"
            :placeholder="form.unidadeCobranca === 'total' ? 'Ex: 250,00 total' : 'Ex: 80,00 por hora'"
            :hint="form.unidadeCobranca === 'total' ? 'Valor total pelo serviço' : 'Valor cobrado por hora de trabalho'"
            required
            regra="required"
          />

          <HtDivider />

          <!-- Pagamento após finalização -->
          <label class="flex items-start gap-3 cursor-pointer group">
            <input
              v-model="form.aceitaPagamentoAposFinalizacao"
              type="checkbox"
              class="mt-0.5 h-4 w-4 rounded border-border text-primary focus:ring-primary cursor-pointer"
            />
            <div>
              <span class="text-sm font-medium text-foreground">
                Aceito pagamento após a finalização do serviço
              </span>
              <p class="text-xs text-muted mt-0.5">
                Ao marcar esta opção, o cliente saberá que você aceita receber após concluir o trabalho.
              </p>
            </div>
          </label>

          <HtAlert
            v-if="!form.aceitaPagamentoAposFinalizacao"
            variant="info"
            message="Por padrão, o pagamento é realizado antes da execução do serviço, após o prestador enviar a confirmação de horas previstas."
          />

          <HtButton type="submit" :loading="carregando" class="w-full">
            <span class="material-symbols-rounded text-base">work</span>
            Publicar Serviço
          </HtButton>
        </form>
      </HtCard>
    </template>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import api from '@/services/api'
import { validarCampos, type CampoValidavel } from '@/shared/validacao'
import { CATEGORIAS_SERVICO, type ServicoPrestadorForm } from '@/types'
import HtInput from '@/components/ui/HtInput.vue'
import HtTextarea from '@/components/ui/HtTextarea.vue'
import HtSelect from '@/components/ui/HtSelect.vue'
import HtButton from '@/components/ui/HtButton.vue'
import HtCard from '@/components/ui/HtCard.vue'
import HtAlert from '@/components/ui/HtAlert.vue'
import HtDivider from '@/components/ui/HtDivider.vue'

const categoriasOpcoes = CATEGORIAS_SERVICO.map(c => ({ value: c.value, label: c.label }))

const tiposValorOpcoes = [
  { value: 'por_hora', label: 'Por hora' },
  { value: 'total',    label: 'Valor total fixo' },
]

const form = reactive<ServicoPrestadorForm>({
  titulo: '',
  descricao: '',
  categoria: '',
  unidadeCobranca: '',
  valor: '',
  aceitaPagamentoAposFinalizacao: false,
})

const refTitulo    = ref<InstanceType<typeof HtInput>   | null>(null)
const refCategoria = ref<InstanceType<typeof HtSelect>  | null>(null)
const refTipoValor = ref<InstanceType<typeof HtSelect>  | null>(null)
const refValor     = ref<InstanceType<typeof HtInput>   | null>(null)

const carregando = ref(false)
const erro       = ref<string | null>(null)
const sucesso    = ref(false)

async function handleSubmit() {
  const campos: Array<CampoValidavel | null> = [refTitulo.value, refCategoria.value, refTipoValor.value, refValor.value]
  if (!validarCampos(campos)) return

  erro.value = null
  carregando.value = true
  try {
    await api.post('/api/ServicoOferecido/CriarServico', {
      titulo:                          form.titulo,
      descricao:                       form.descricao,
      categoriaId:                     Number(form.categoria),
      UnidadeCobranca:                 form.unidadeCobranca,
      valor:                           Number(form.valor),
      aceitaPagamentoAposFinalizacao:  form.aceitaPagamentoAposFinalizacao,
    })
    sucesso.value = true
  } catch (err: unknown) {
    const e = err as { response?: { data?: unknown } }
    const msg = e.response?.data
    erro.value = typeof msg === 'string' && msg
      ? msg
      : 'Erro ao publicar o serviço. Verifique os dados e tente novamente.'
  } finally {
    carregando.value = false
  }
}

function reiniciar() {
  form.titulo = ''
  form.descricao = ''
  form.categoria = ''
  form.unidadeCobranca = ''
  form.valor = ''
  form.aceitaPagamentoAposFinalizacao = false
  erro.value = null
  sucesso.value = false
}

defineExpose({ form, handleSubmit, sucesso, erro })
</script>
