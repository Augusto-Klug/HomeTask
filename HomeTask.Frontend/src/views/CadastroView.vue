<template>
  <div class="min-h-[calc(100vh-7rem)] flex items-center justify-center px-4 py-12">
    <div class="w-full max-w-lg">
      <!-- Logo -->
      <div class="text-center mb-8">
        <router-link to="/" class="text-2xl font-bold text-primary">HomeTask</router-link>
        <p class="text-sm text-muted mt-1">Crie sua conta gratuitamente</p>
      </div>

      <HtCard>
        <HtAlert v-if="erro" :message="erro" class="mb-5" />

        <form class="flex flex-col gap-5" @submit.prevent="handleCadastro">

          <!-- Tipo de conta -->
          <div>
            <p class="text-sm font-semibold text-foreground mb-2">Tipo de conta</p>
            <div class="flex flex-wrap gap-4">
              <label v-for="tipo in tipos" :key="tipo.value" class="flex items-center gap-2 cursor-pointer">
                <input type="radio" v-model="form.tipoUsuario" :value="tipo.value" class="accent-primary" />
                <span class="text-sm text-foreground">{{ tipo.label }}</span>
              </label>
            </div>
          </div>

          <HtDivider />

          <!-- Dados básicos -->
          <HtInput
            ref="inputNome"
            v-model="form.nome"
            label="Nome completo"
            placeholder="Seu nome completo"
            regra="required"
            required
          />

          <HtInput
            ref="inputEmail"
            v-model="form.email"
            label="E-mail"
            type="email"
            regra="email"
            placeholder="seu@email.com"
            required
          />

          <HtInput
            ref="inputDocumento"
            v-model="form.documento"
            label="CPF ou CNPJ"
            regra="documento"
            placeholder="000.000.000-00"
            required
          />

          <HtInput
            ref="inputTelefone"
            v-model="form.telefone"
            label="Telefone"
            regra="telefone"
            placeholder="(00) 00000-0000"
          />

          <HtInput
            ref="inputSenha"
            v-model="form.senha"
            label="Senha"
            type="password"
            regra="senha"
            placeholder="Mínimo 6 caracteres"
            required
          />

          <HtDivider />
          <p class="text-sm font-semibold text-foreground -mb-2">Endereço</p>

          <HtInput
            ref="inputCep"
            v-model="form.cep"
            label="CEP"
            regra="cep"
            placeholder="00000-000"
            required
          />

          <HtInput
            ref="inputEndereco"
            v-model="form.logradouro"
            label="Logradouro"
            regra="required"
            placeholder="Rua, Avenida..."
            required
          />

          <div class="grid grid-cols-2 gap-4">
            <HtInput
              ref="inputBairro"
              v-model="form.bairro"
              label="Bairro"
              regra="required"
              required
            />
            <HtSearchSelect
              ref="inputEstado"
              v-model="form.estado"
              label="Estado (UF)"
              :options="UF_OPTIONS"
              placeholder="Busque pela UF"
              required
            />
          </div>

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

          <!-- Dados de prestador -->
          <template v-if="form.tipoUsuario === 2 || form.tipoUsuario === 3">
            <HtDivider />
            <p class="text-sm font-semibold text-foreground -mb-2">Dados profissionais</p>

            <HtTextarea
              ref="inputDescricao"
              v-model="form.descricao"
              label="Descrição profissional"
              placeholder="Descreva sua experiência e serviços..."
              :rows="4"
            />
          </template>

          <HtButton type="submit" :loading="carregando" class="w-full mt-2">
            Criar Conta
          </HtButton>
        </form>

        <HtDivider />

        <p class="text-center text-sm text-muted">
          Já tem conta?
          <router-link to="/login" class="text-primary font-medium hover:underline">Entrar</router-link>
        </p>
      </HtCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue'
import { useRouter } from 'vue-router'
import api from '@/services/api'
import { validarCampos } from '@/shared/validacao'
import type { CadastroForm } from '@/types'
import HtInput from '@/components/ui/HtInput.vue'
import HtSearchSelect from '@/components/ui/HtSearchSelect.vue'
import HtSelect from '@/components/ui/HtSelect.vue'
import HtTextarea from '@/components/ui/HtTextarea.vue'
import HtButton from '@/components/ui/HtButton.vue'
import HtCard from '@/components/ui/HtCard.vue'
import HtAlert from '@/components/ui/HtAlert.vue'
import HtDivider from '@/components/ui/HtDivider.vue'
import { UF_OPTIONS } from '@/statics/selects'
import { useCidadeEstadoOptions } from '@/composables/useCidadeEstadoOptions'

const router = useRouter()

const tipos = [
  { value: 1, label: '🏠 Cliente' },
  { value: 2, label: '🔧 Prestador' },
]

const form = reactive<CadastroForm>({
  tipoUsuario: 1,
  nome: '',
  email: '',
  documento: '',
  telefone: '',
  senha: '',
  cep: '',
  logradouro: '',
  bairro: '',
  cidadeId: '',
  estado: '',
  raioAtendimentoKm: 10,
  descricao: '',
})

const carregando = ref(false)
const erro = ref<string | null>(null)
const {
  carregandoCidades,
  cidadesDisponiveis,
  cidadeHint,
  carregarCidades,
} = useCidadeEstadoOptions(() => form.estado)

// Refs dos campos
const inputNome      = ref<InstanceType<typeof HtInput> | null>(null)
const inputEmail     = ref<InstanceType<typeof HtInput> | null>(null)
const inputDocumento = ref<InstanceType<typeof HtInput> | null>(null)
const inputTelefone  = ref<InstanceType<typeof HtInput> | null>(null)
const inputSenha     = ref<InstanceType<typeof HtInput> | null>(null)
const inputCep       = ref<InstanceType<typeof HtInput> | null>(null)
const inputEndereco  = ref<InstanceType<typeof HtInput> | null>(null)
const inputBairro    = ref<InstanceType<typeof HtInput> | null>(null)
const inputCidade    = ref<InstanceType<typeof HtSelect> | null>(null)
const inputEstado    = ref<InstanceType<typeof HtSearchSelect> | null>(null)
const inputDescricao = ref<InstanceType<typeof HtInput> | null>(null)

watch(
  () => form.estado,
  () => {
    const cidadeSelecionadaPermaneceValida = cidadesDisponiveis.value.some(
      cidade => cidade.value === form.cidadeId,
    )

    if (!cidadeSelecionadaPermaneceValida) {
      form.cidadeId = ''
    }
  },
)

onMounted(() => {
  void carregarCidades()
})

async function handleCadastro() {
  const camposBase = [
    inputNome.value, inputEmail.value, inputDocumento.value,
    inputSenha.value, inputCep.value, inputEndereco.value,
    inputBairro.value, inputCidade.value, inputEstado.value,
  ]
  if (form.tipoUsuario !== 1) camposBase.push(inputDescricao.value as any)

  if (!validarCampos(camposBase)) return

  erro.value = null
  carregando.value = true
  try {
    await api.post('/api/Usuario/CriarUsuario', {
      nome:      form.nome,
      email:     form.email,
      documento: form.documento,
      telefone:  form.telefone,
      senha:     form.senha,
      tipo:      Number(form.tipoUsuario),
      cep:       form.cep,
      logradouro:  form.logradouro,
      bairro:    form.bairro,
      cidadeId:  form.cidadeId,
      estado:    form.estado,
    })
    router.push('/cadastro-sucesso')
  } catch (err: unknown) {
    const e = err as { response?: { status?: number; data?: unknown } }
    const status = e.response?.status
    const msg = e.response?.data
    if (status === 409)      erro.value = typeof msg === 'string' ? msg : 'E-mail ou Documento já cadastrado.'
    else if (status === 400) erro.value = typeof msg === 'string' ? msg : 'Dados inválidos. Verifique o formulário.'
    else                     erro.value = 'Erro ao cadastrar. Tente novamente em instantes.'
  } finally {
    carregando.value = false
  }
}
</script>
