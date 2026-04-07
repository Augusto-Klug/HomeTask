<template>
  <div class="flex flex-col gap-1 w-full">
    <label
      v-if="label"
      :for="fieldId"
      class="text-sm font-medium text-foreground"
      :class="{ 'text-error': hasError }"
    >{{ label }}<span v-if="required" class="text-error ml-0.5">*</span></label>

    <textarea
      :id="fieldId"
      v-bind="$attrs"
      :value="modelValue"
      :disabled="disabled"
      :placeholder="placeholder"
      :rows="rows"
      class="w-full px-3 py-2 rounded-lg border bg-card text-foreground text-sm
             placeholder:text-muted transition-colors resize-y
             focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary
             disabled:opacity-50 disabled:cursor-not-allowed"
      :class="{
        'border-border': !hasError,
        'border-error ring-1 ring-error': hasError,
      }"
      @input="emit('update:modelValue', ($event.target as HTMLTextAreaElement).value)"
      @blur="required && validar()"
    />

    <p v-if="hasError" class="text-xs text-error flex items-center gap-1">
      <span class="material-symbols-rounded text-sm">error</span>
      {{ erroAtual }}
    </p>
    <p v-else-if="hint" class="text-xs text-muted">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

defineOptions({ inheritAttrs: false })

const props = withDefaults(
  defineProps<{
    modelValue: string
    label?: string
    placeholder?: string
    hint?: string
    mensagemErro?: string
    disabled?: boolean
    required?: boolean
    rows?: number
  }>(),
  { rows: 3, disabled: false, required: false },
)

const emit = defineEmits<{ 'update:modelValue': [value: string] }>()

let counter = 0
const fieldId = `ht-textarea-${++counter}`

const hasError = ref(false)
const erroAtual = ref('')

function validar(): boolean {
  if (props.required && !props.modelValue?.trim()) {
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
