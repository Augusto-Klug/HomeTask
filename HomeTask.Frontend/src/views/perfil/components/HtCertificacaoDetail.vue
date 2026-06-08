<template>
  <div v-if="isOpen && certificacao" class="modal modal-open">
    <div class="modal-box max-w-md">
      <button
        type="button"
        class="btn btn-sm btn-circle btn-ghost absolute right-2 top-2"
        @click="fechar"
      >
        ✕
      </button>

      <h3 class="font-bold text-lg mb-4">{{ certificacao.nome }}</h3>

      <div class="space-y-4">
        <div v-if="certificacao.instituicao" class="form-control">
          <label class="label">
            <span class="label-text text-sm font-semibold">Instituição</span>
          </label>
          <p class="text-sm">{{ certificacao.instituicao }}</p>
        </div>

        <div v-if="certificacao.dataEmissao || certificacao.dataValidade" class="grid grid-cols-2 gap-4">
          <div v-if="certificacao.dataEmissao" class="form-control">
            <label class="label">
              <span class="label-text text-sm font-semibold">Emissão</span>
            </label>
            <p class="text-sm">{{ formatarData(certificacao.dataEmissao) }}</p>
          </div>
          <div v-if="certificacao.dataValidade" class="form-control">
            <label class="label">
              <span class="label-text text-sm font-semibold">Validade</span>
            </label>
            <p class="text-sm">{{ formatarData(certificacao.dataValidade) }}</p>
          </div>
        </div>

        <div v-if="certificacao.urlDocumento" class="form-control">
          <label class="label">
            <span class="label-text text-sm font-semibold">Documento</span>
          </label>
          <div class="bg-base-200 rounded p-4 h-96 overflow-auto">
            <img
              :src="certificacao.urlDocumento"
              :alt="certificacao.nome"
              class="w-full"
            />
          </div>
          <a
            :href="certificacao.urlDocumento"
            target="_blank"
            download
            class="link link-primary text-sm mt-2 inline-block"
          >
            Baixar documento
          </a>
        </div>

        <div v-else class="alert alert-info">
          <p class="text-sm">Nenhum documento anexado</p>
        </div>
      </div>

      <div class="modal-action mt-6">
        <button type="button" class="btn btn-ghost" @click="fechar">Fechar</button>
      </div>
    </div>
    <div class="modal-backdrop" @click="fechar"></div>
  </div>
</template>

<script setup lang="ts">
import type { Certificacao } from "@/types";

interface Props {
  certificacao: Certificacao | null;
  isOpen: boolean;
}

defineProps<Props>();

const emit = defineEmits<{
  close: [];
}>();

function fechar() {
  emit("close");
}

function formatarData(data: string): string {
  try {
    return new Date(data).toLocaleDateString("pt-BR");
  } catch {
    return data;
  }
}
</script>
