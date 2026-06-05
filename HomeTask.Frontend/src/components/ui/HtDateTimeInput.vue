<template>
  <div class="flex flex-col gap-1 w-full">
    <p v-if="label" class="inline-flex w-fit items-center text-sm font-medium">
      {{ label }}<span v-if="required" class="text-error ml-0.5">*</span>
    </p>

    <div class="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <HtInput
        ref="inputData"
        v-model="estado.data"
        :label="labelData"
        type="date"
        :required="required"
        :regra="required ? 'required' : undefined"
        :mensagemErro="mensagemErroData"
        :openPickerOnFocus="openPickerOnFocus"
        @update:model-value="atualizarModel"
      />
      <HtInput
        ref="inputHora"
        v-model="estado.hora"
        :label="labelHora"
        type="time"
        :required="required"
        :regra="required ? 'required' : undefined"
        :mensagemErro="mensagemErroHora"
        :openPickerOnFocus="openPickerOnFocus"
        @update:model-value="atualizarModel"
      />
    </div>

    <p v-if="hint" class="text-xs opacity-60">{{ hint }}</p>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, watch } from "vue";
import HtInput from "@/components/ui/HtInput.vue";

const props = withDefaults(
  defineProps<{
    modelValue: string;
    label?: string;
    labelData?: string;
    labelHora?: string;
    hint?: string;
    required?: boolean;
    openPickerOnFocus?: boolean;
    mensagemErroData?: string;
    mensagemErroHora?: string;
  }>(),
  {
    label: "Data e horário",
    labelData: "Data",
    labelHora: "Horário",
    hint: undefined,
    required: false,
    openPickerOnFocus: true,
    mensagemErroData: "Selecione uma data",
    mensagemErroHora: "Selecione um horário",
  },
);

const emit = defineEmits<{
  "update:modelValue": [value: string];
}>();

const inputData = ref<InstanceType<typeof HtInput> | null>(null);
const inputHora = ref<InstanceType<typeof HtInput> | null>(null);

const estado = reactive({
  data: "",
  hora: "",
});

watch(
  () => props.modelValue,
  (valor) => {
    const [data = "", horaCompleta = ""] = valor.split("T");
    const hora = horaCompleta.slice(0, 5);

    if (estado.data !== data) estado.data = data;
    if (estado.hora !== hora) estado.hora = hora;
  },
  { immediate: true },
);

function atualizarModel() {
  if (!estado.data || !estado.hora) {
    emit("update:modelValue", "");
    return;
  }

  emit("update:modelValue", `${estado.data}T${estado.hora}`);
}

function validar() {
  return [inputData.value, inputHora.value].every((campo) => campo?.validar() ?? !props.required);
}

function clearError() {
  inputData.value?.clearError();
  inputHora.value?.clearError();
}

defineExpose({ validar, clearError });
</script>
