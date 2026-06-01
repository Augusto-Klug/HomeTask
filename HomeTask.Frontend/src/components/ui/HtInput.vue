<template>
  <div class="flex flex-col gap-1 w-full">
    <label
      v-if="label"
      :for="inputId"
      class="inline-flex w-fit items-center text-sm font-medium"
      :class="{ 'text-error': hasError }"
    >{{ label }}<span v-if="required" class="text-error ml-0.5">*</span></label>

    <!-- Wrapper com suporte a ícones prefix/suffix -->
    <label
      :for="inputId"
      class="input input-bordered flex items-center gap-2 w-full cursor-text"
      :class="{
        'input-error': hasError,
        'bg-base-200/60 border-base-300/70 cursor-not-allowed pointer-events-none': disabled,
      }"
    >
      <span
        v-if="$slots.prefix"
        class="material-symbols-rounded text-lg opacity-50 shrink-0"
      >
        <slot name="prefix" />
      </span>

      <input
        :id="inputId"
        ref="inputEl"
        v-bind="$attrs"
        :value="modelValue ?? ''"
        :type="inputType"
        :disabled="disabled"
        :placeholder="placeholder"
        :maxlength="maxlength"
        :min="minValue"
        class="grow bg-transparent border-none outline-none text-sm disabled:text-base-content disabled:opacity-100 disabled:[-webkit-text-fill-color:var(--color-base-content)]"
        @input="handleInput"
        @focus="handleFocus"
        @blur="handleBlur"
      />

      <!-- Toggle senha -->
      <button
        v-if="type === 'password'"
        type="button"
        tabindex="-1"
        class="btn btn-ghost btn-xs btn-square shrink-0"
        @click="showPassword = !showPassword"
      >
        <span class="material-symbols-rounded text-base opacity-60">
          {{ showPassword ? 'visibility_off' : 'visibility' }}
        </span>
      </button>

      <span
        v-else-if="$slots.suffix"
        class="material-symbols-rounded text-lg opacity-50 shrink-0"
      >
        <slot name="suffix" />
      </span>
    </label>

    <p v-if="hasError" class="text-xs text-error flex items-center gap-1">
      <span class="material-symbols-rounded text-sm">error</span>
      {{ erroAtual }}
    </p>
    <p v-else-if="hint" class="text-xs opacity-60">{{ hint }}</p>
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
    modelValue: string | number | null
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
    allowNegative?: boolean
    openPickerOnFocus?: boolean
  }>(),
  {
    type: 'text',
    disabled: false,
    required: false,
    validarAoDigitar: false,
    allowNegative: true,
    openPickerOnFocus: false,
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

const minValue = computed(() => {
  if (props.type === 'number' && !props.allowNegative) return 0
  return undefined
})

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
  const sanitized = sanitizarNumero(raw)
  const masked = aplicarMascara(sanitized)
  emit('update:modelValue', masked)

  if (masked !== raw && inputEl.value) {
    inputEl.value.value = masked
  }

  if (props.validarAoDigitar && hasError.value) validar()
}

function handleBlur() {
  if (props.regra || props.required) validar()
}

function handleFocus() {
  if (!props.openPickerOnFocus) return
  if (!['date', 'time', 'datetime-local', 'month', 'week'].includes(props.type)) return

  const pickerInput = inputEl.value as (HTMLInputElement & { showPicker?: () => void }) | null
  pickerInput?.showPicker?.()
}

function sanitizarNumero(value: string): string {
  if (props.type !== 'number' || props.allowNegative || !value) return value

  const numericValue = Number(value)
  if (!Number.isNaN(numericValue) && numericValue < 0) {
    return '0'
  }

  return value
}

function validar(): boolean {
  const v = String(props.modelValue ?? '').trim()

  if (!v) {
    if (props.required || props.regra) {
      setError(props.mensagemErro ?? mensagensPadrao['required'] ?? 'Campo obrigatório')
      return false
    }
    clearError()
    return true
  }

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
