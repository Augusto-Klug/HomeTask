<template>
  <div class="flex flex-col gap-1 w-full">
    <label
      v-if="label"
      :for="fieldId"
      class="text-sm font-medium text-foreground"
      :class="{ 'text-error': hasError }"
    >{{ label }}<span v-if="required" class="text-error ml-0.5">*</span></label>

    <SelectRoot
      :model-value="modelValue"
      :disabled="disabled"
      @update:model-value="onSelect"
    >
      <SelectTrigger
        :id="fieldId"
        class="flex items-center justify-between h-10 w-full px-3 rounded-lg border bg-card text-sm
               text-foreground transition-colors cursor-pointer
               focus:outline-none focus:ring-2 focus:ring-primary focus:border-primary
               disabled:opacity-50 disabled:cursor-not-allowed"
        :class="{
          'border-border': !hasError,
          'border-error ring-1 ring-error': hasError,
        }"
        @blur="required && validar()"
      >
        <SelectValue :placeholder="placeholder ?? 'Selecione...'" class="text-left" />
        <SelectIcon>
          <span class="material-symbols-rounded text-muted text-lg">expand_more</span>
        </SelectIcon>
      </SelectTrigger>

      <SelectPortal>
        <SelectContent
          class="z-50 min-w-[var(--reka-select-trigger-width)] rounded-lg border border-border
                 bg-card shadow-lg overflow-hidden py-1"
          position="popper"
          :side-offset="4"
        >
          <SelectScrollUpButton class="flex items-center justify-center h-6 text-muted cursor-default">
            <span class="material-symbols-rounded text-sm">expand_less</span>
          </SelectScrollUpButton>

          <SelectViewport>
            <SelectItem
              v-for="opt in options"
              :key="opt.value"
              :value="String(opt.value)"
              class="relative flex items-center h-9 px-3 text-sm text-foreground cursor-pointer select-none
                     data-[highlighted]:bg-primary-light data-[highlighted]:text-primary outline-none"
            >
              <SelectItemText>{{ opt.label }}</SelectItemText>
              <SelectItemIndicator class="absolute right-3">
                <span class="material-symbols-rounded text-primary text-sm">check</span>
              </SelectItemIndicator>
            </SelectItem>
          </SelectViewport>

          <SelectScrollDownButton class="flex items-center justify-center h-6 text-muted cursor-default">
            <span class="material-symbols-rounded text-sm">expand_more</span>
          </SelectScrollDownButton>
        </SelectContent>
      </SelectPortal>
    </SelectRoot>

    <p v-if="hasError" class="text-xs text-error flex items-center gap-1">
      <span class="material-symbols-rounded text-sm">error</span>
      {{ erroAtual }}
    </p>
    <p v-else-if="hint" class="text-xs text-muted">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import {
  SelectRoot,
  SelectTrigger,
  SelectValue,
  SelectIcon,
  SelectPortal,
  SelectContent,
  SelectScrollUpButton,
  SelectScrollDownButton,
  SelectViewport,
  SelectItem,
  SelectItemText,
  SelectItemIndicator,
} from 'reka-ui'

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

function onSelect(v: string) {
  emit('update:modelValue', v)
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
