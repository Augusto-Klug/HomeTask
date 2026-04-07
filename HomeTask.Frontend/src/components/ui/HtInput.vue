<template>
  <div class="flex flex-col gap-1 w-full">
    <label
      v-if="label"
      :for="inputId"
      class="text-sm font-medium text-foreground"
      :class="{ 'text-error': hasError }"
    >{{ label }}<span v-if="required" class="text-error ml-0.5">*</span></label>

    <div class="relative">
      <!-- Slot de ícone à esquerda -->
      <span
        v-if="$slots.prefix"
        class="absolute left-3 top-1/2 -translate-y-1/2 text-muted material-symbols-rounded text-lg"
      >
        <slot name="prefix" />
      </span>

      <input
        :id="inputId"
        ref="inputEl"
        v-bind="$attrs"
        :value="modelValue"
        :type="inputType"
        :disabled="disabled"
        :placeholder="placeholder"
        :maxlength="maxlength"
        class="w-full h-10 px-3 rounded-lg border bg-card text-foreground text-sm
               placeholder:text-muted transition-colors
               focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary
               disabled:opacity-50 disabled:cursor-not-allowed"
        :class="{
          'pl-9': $slots.prefix,
          'pr-10': type === 'password' || $slots.suffix,
          'border-border': !hasError,
          'border-error ring-1 ring-error': hasError,
        }"
        @input="handleInput"
        @blur="handleBlur"
      />

      <!-- Toggle senha -->
      <button
        v-if="type === 'password'"
        type="button"
        tabindex="-1"
        class="absolute right-2 top-1/2 -translate-y-1/2 text-muted hover:text-foreground p-1"
        @click="showPassword = !showPassword"
      >
        <span class="material-symbols-rounded text-lg">
          {{ showPassword ? 'visibility_off' : 'visibility' }}
        </span>
      </button>

      <!-- Slot de ícone à direita -->
      <span
        v-else-if="$slots.suffix"
        class="absolute right-3 top-1/2 -translate-y-1/2 text-muted material-symbols-rounded text-lg"
      >
        <slot name="suffix" />
      </span>
    </div>

    <p v-if="hasError" class="text-xs text-error flex items-center gap-1">
      <span class="material-symbols-rounded text-sm">error</span>
      {{ erroAtual }}
    </p>
    <p v-else-if="hint" class="text-xs text-muted">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import {
  mascaraCPF,
  mascaraCNPJ,
  mascaraCEP,
  mascaraTelefone,
  mascaraDocumento,
  validarCPF,
  validarCNPJ,
  validarEmail,
  validarCEP,
  validarTelefone,
  validarDocumento,
  mensagensPadrao,
} from '@/shared/validacao'

defineOptions({ inheritAttrs: false })

type Regra = 'required' | 'email' | 'cpf' | 'cnpj' | 'documento' | 'cep' | 'telefone' | 'senha'

const props = withDefaults(
  defineProps<{
    modelValue: string
    label?: string
    placeholder?: string
    hint?: string
    regra?: Regra
    mensagemErro?: string
    type?: string
    disabled?: boolean
    required?: boolean
    maxlength?: number
    validarAoDigitar?: boolean
  }>(),
  {
    type: 'text',
    disabled: false,
    required: false,
    validarAoDigitar: false,
  },
)

const emit = defineEmits<{
  'update:modelValue': [value: string]
  'change': [value: string]
}>()

let counter = 0
const inputId = `ht-input-${++counter}`
const inputEl = ref<HTMLInputElement | null>(null)

const showPassword = ref(false)
const hasError = ref(false)
const erroAtual = ref('')

const inputType = computed(() => {
  if (props.type === 'password') return showPassword.value ? 'text' : 'password'
  return props.type
})

// Aplica máscara de acordo com a regra
function aplicarMascara(v: string): string {
  switch (props.regra) {
    case 'cpf':       return mascaraCPF(v)
    case 'cnpj':      return mascaraCNPJ(v)
    case 'documento': return mascaraDocumento(v)
    case 'cep':       return mascaraCEP(v)
    case 'telefone':  return mascaraTelefone(v)
    default:          return v
  }
}

function handleInput(e: Event) {
  const raw = (e.target as HTMLInputElement).value
  const masked = aplicarMascara(raw)
  emit('update:modelValue', masked)

  // Atualiza o valor visual se a máscara alterou
  if (masked !== raw && inputEl.value) {
    inputEl.value.value = masked
  }

  if (props.validarAoDigitar && hasError.value) validar()
}

function handleBlur() {
  if (props.regra || props.required) validar()
}

// ─── Validar (exposto via defineExpose) ───────────────────────────────────
function validar(): boolean {
  const v = props.modelValue?.trim() ?? ''

  // Campo vazio
  if (!v) {
    if (props.required || props.regra) {
      setError(props.mensagemErro ?? mensagensPadrao['required'])
      return false
    }
    clearError()
    return true
  }

  // Validação por regra
  if (props.regra) {
    let ok = true

    switch (props.regra) {
      case 'email':     ok = validarEmail(v);     break
      case 'cpf':       ok = validarCPF(v);       break
      case 'cnpj':      ok = validarCNPJ(v);      break
      case 'documento': ok = validarDocumento(v); break
      case 'cep':       ok = validarCEP(v);       break
      case 'telefone':  ok = validarTelefone(v);  break
      case 'senha':     ok = v.length >= 6;       break
    }

    if (!ok) {
      setError(props.mensagemErro ?? mensagensPadrao[props.regra] ?? 'Valor inválido')
      return false
    }
  }

  clearError()
  return true
}

function setError(msg: string) {
  hasError.value = true
  erroAtual.value = msg
}

function clearError() {
  hasError.value = false
  erroAtual.value = ''
}

defineExpose({ validar, clearError, inputEl })
</script>
