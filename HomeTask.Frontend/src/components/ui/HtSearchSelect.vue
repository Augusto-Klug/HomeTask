<template>
  <div class="flex flex-col gap-1 w-full">
    <label
      v-if="label"
      :for="fieldId"
      class="fieldset-legend text-sm font-medium"
      :class="{ 'text-error': hasError }"
    >{{ label }}<span v-if="required" class="text-error ml-0.5">*</span></label>

    <div class="relative">
      <label
        :for="fieldId"
        class="input input-bordered flex items-center gap-2 w-full cursor-text"
        :class="{
          'input-error': hasError,
          'bg-base-200/60 border-base-300/70 cursor-not-allowed pointer-events-none': disabled,
        }"
      >
        <span class="material-symbols-rounded text-lg opacity-50 shrink-0">search</span>

        <input
          :id="fieldId"
          :value="query"
          :disabled="disabled"
          :placeholder="placeholder"
          autocomplete="off"
          class="grow bg-transparent border-none outline-none text-sm disabled:text-base-content disabled:opacity-100 disabled:[-webkit-text-fill-color:var(--color-base-content)]"
          @input="onInput"
          @focus="isOpen = true"
          @blur="onBlur"
        />
      </label>

      <div
        v-if="isOpen && filteredOptions.length > 0 && !disabled"
        class="absolute z-20 mt-1 w-full rounded-box border border-base-300 bg-base-100 shadow-lg max-h-60 overflow-auto"
      >
        <button
          v-for="opt in filteredOptions"
          :key="opt.value"
          :data-testid="`ht-search-select-option-${String(opt.value)}`"
          type="button"
          class="w-full px-3 py-2 text-left text-sm hover:bg-base-200 transition-colors"
          @mousedown.prevent="selectOption(opt)"
        >
          {{ opt.label }} ({{ opt.value }})
        </button>
      </div>
    </div>

    <p v-if="hasError" class="text-xs text-error flex items-center gap-1">
      <span class="material-symbols-rounded text-sm">error</span>
      {{ erroAtual }}
    </p>
    <p v-else-if="hint" class="text-xs opacity-60">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, watch } from 'vue'

defineOptions({ inheritAttrs: false })

type Option = { label: string; value: string | number }

const props = withDefaults(
  defineProps<{
    modelValue: string
    options: ReadonlyArray<Option>
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
const fieldId = `ht-search-select-${++counter}`

const hasError = ref(false)
const erroAtual = ref('')
const isOpen = ref(false)
const query = ref('')

const selectedOption = computed(
  () => props.options.find(option => String(option.value) === props.modelValue) ?? null,
)

const filteredOptions = computed(() => {
  const term = normalize(query.value)
  if (!term) return props.options

  return props.options.filter((option) => {
    const haystack = `${option.label} ${option.value}`
    return normalize(haystack).includes(term)
  })
})

watch(
  () => props.modelValue,
  () => {
    query.value = selectedOption.value?.label ?? ''
  },
  { immediate: true },
)

function normalize(value: string) {
  return value
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .toLowerCase()
    .trim()
}

function onInput(event: Event) {
  query.value = (event.target as HTMLInputElement).value
  isOpen.value = true

  if (props.modelValue) {
    emit('update:modelValue', '')
  }

  if (hasError.value) clearError()
}

function selectOption(option: Option) {
  query.value = option.label
  emit('update:modelValue', String(option.value))
  isOpen.value = false
  clearError()
}

function onBlur() {
  isOpen.value = false

  const term = normalize(query.value)
  if (!term) {
    emit('update:modelValue', '')
    validar()
    return
  }

  const exactOption = props.options.find((option) => {
    const normalizedLabel = normalize(option.label)
    const normalizedValue = normalize(String(option.value))
    return normalizedLabel === term || normalizedValue === term
  })

  if (exactOption) {
    selectOption(exactOption)
    return
  }

  query.value = ''
  emit('update:modelValue', '')
  validar()
}

function validar(): boolean {
  if (props.required && !props.modelValue) {
    hasError.value = true
    erroAtual.value = props.mensagemErro ?? 'Campo obrigatório'
    return false
  }

  clearError()
  return true
}

function clearError() {
  hasError.value = false
  erroAtual.value = ''
}

defineExpose({ validar, clearError })
</script>
