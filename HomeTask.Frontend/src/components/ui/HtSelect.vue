<template>
  <div class="flex flex-col gap-1 w-full">
    <label
      v-if="label"
      :for="fieldId"
      class="inline-flex w-fit items-center text-sm font-medium"
      :class="{ 'text-error': hasError }"
    >{{ label }}<span v-if="required" class="text-error ml-0.5">*</span></label>

    <select
      :id="fieldId"
      :value="modelValue"
      :disabled="disabled"
      class="select select-bordered w-full"
      :class="{
        'select-error': hasError,
        'bg-base-200/60 border-base-300/70 text-base-content opacity-100 cursor-not-allowed disabled:[-webkit-text-fill-color:var(--color-base-content)]': disabled,
      }"
      @change="onSelect"
      @blur="required && validar()"
    >
      <!-- Placeholder desabilitado (apenas se não houver opção com value vazio) -->
      <option
        v-if="!hasEmptyOption && placeholder"
        value=""
        disabled
        :selected="!modelValue"
      >{{ placeholder }}</option>

      <option
        v-for="opt in options"
        :key="opt.value"
        :value="String(opt.value)"
      >{{ opt.label }}</option>
    </select>

    <p v-if="hasError" class="text-xs text-error flex items-center gap-1">
      <span class="material-symbols-rounded text-sm">error</span>
      {{ erroAtual }}
    </p>
    <p v-else-if="hint" class="text-xs opacity-60">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'

defineOptions({ inheritAttrs: false })

const props = withDefaults(
  defineProps<{
    modelValue: string
    options: Array<{ label: string; value: string | number }>
    label?: string
    placeholder?: string
    hint?: string
    mensagemErro?: string
    disabled?: boolean
    required?: boolean
  }>(),
  { disabled: false, required: false },
)

const emit = defineEmits<{ 'update:modelValue': [value: string] }>()

let counter = 0
const fieldId = `ht-select-${++counter}`

const hasError = ref(false)
const erroAtual = ref('')

// Verifica se já existe uma opção com value vazio na lista
const hasEmptyOption = computed(() =>
  props.options.some(o => String(o.value) === ''),
)

function onSelect(e: Event) {
  const value = (e.target as HTMLSelectElement).value
  emit('update:modelValue', value)
  if (hasError.value) validar()
}

function validar(): boolean {
  if (props.required && !props.modelValue) {
    hasError.value = true
    erroAtual.value = props.mensagemErro ?? 'Campo obrigatório'
    return false
  }
  hasError.value = false
  erroAtual.value = ''
  return true
}

function clearError() {
  hasError.value = false
  erroAtual.value = ''
}

defineExpose({ validar, clearError })
</script>
