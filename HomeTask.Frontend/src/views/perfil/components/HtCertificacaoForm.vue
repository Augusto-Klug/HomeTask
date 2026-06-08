<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-lg font-semibold">Certificações</h3>
      <button
        v-if="!mostraForm && isPrestador"
        type="button"
        @click="mostraForm = true"
        class="btn btn-sm btn-primary"
      >
        <span class="material-symbols-rounded text-sm">add</span>
        Adicionar
      </button>
    </div>

    <HtAlert v-if="erro" variant="error" :message="erro" class="mb-2" />
    <HtAlert v-if="sucesso" variant="success" :message="sucesso" class="mb-2" />

    <form
      v-if="mostraForm"
      @submit.prevent="adicionarCertificacao"
      class="card bg-base-200 p-4 mb-4"
    >
      <div class="flex flex-col gap-3">
        <HtInput
          v-model="formCertificacao.nome"
          label="Nome da certificação"
          placeholder="Ex: Certificação em Limpeza Profissional"
          required
        />
        <HtInput
          v-model="formCertificacao.instituicao"
          label="Instituição (opcional)"
          placeholder="Ex: SENAC"
        />
        <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
          <HtInput
            v-model="formCertificacao.dataEmissao"
            label="Data de emissão (opcional)"
            type="date"
          />
          <HtInput
            v-model="formCertificacao.dataValidade"
            label="Data de validade (opcional)"
            type="date"
          />
        </div>
        <div>
          <label class="label">
            <span class="label-text text-sm">Documento (PDF, JPG ou PNG - opcional)</span>
          </label>
          <input
            type="file"
            accept=".pdf,.jpg,.jpeg,.png"
            @change="(e: Event) => {
              const target = e.target as HTMLInputElement;
              formCertificacao.documento = target.files?.[0];
            }"
            class="file-input file-input-bordered w-full"
          />
        </div>
        <div class="flex gap-2 justify-end">
          <button
            type="button"
            @click="mostraForm = false"
            class="btn btn-ghost btn-sm"
            :disabled="adicionando"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="btn btn-primary btn-sm"
            :disabled="adicionando || !formCertificacao.nome"
            :loading="adicionando"
          >
            Salvar
          </button>
        </div>
      </div>
    </form>

    <div v-if="certificacoes.length === 0 && !mostraForm" class="text-center py-8 text-base-content/50">
      <p>Nenhuma certificação adicionada ainda</p>
    </div>

    <div v-else class="grid gap-2">
      <div
        v-for="cert in certificacoes"
        :key="cert.id"
        class="card bg-base-100 shadow-sm border border-base-300 cursor-pointer hover:shadow-md transition-shadow"
        @click="emit('certificacaoClicada', cert)"
      >
        <div class="card-body p-4">
          <div class="flex items-start justify-between">
            <div class="flex-1">
              <h4 class="font-semibold">{{ cert.nome }}</h4>
              <p v-if="cert.instituicao" class="text-sm text-base-content/70">
                {{ cert.instituicao }}
              </p>
              <div v-if="cert.dataEmissao || cert.dataValidade" class="text-xs text-base-content/60 mt-2">
                <p v-if="cert.dataEmissao">Emissão: {{ formatarData(cert.dataEmissao) }}</p>
                <p v-if="cert.dataValidade">Validade: {{ formatarData(cert.dataValidade) }}</p>
              </div>
              <p v-if="cert.urlDocumento" class="text-xs mt-2 text-blue-500">
                <span class="material-symbols-rounded text-sm align-text-bottom">visibility</span>
                Ver documento
              </p>
            </div>
            <button
              type="button"
              @click.stop="removerCertificacao(cert.id)"
              class="btn btn-ghost btn-sm btn-circle"
              :disabled="removendo === cert.id"
            >
              <span class="material-symbols-rounded text-lg">delete</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import api from "@/services/api";
import HtInput from "@/components/ui/HtInput.vue";
import HtAlert from "@/components/ui/HtAlert.vue";
import type { Certificacao, CertificacaoForm } from "@/types";

const props = defineProps<{
  prestadorId: string;
  isPrestador: boolean;
  certificacoes: Certificacao[];
}>();

const emit = defineEmits<{
  certificacaoAdicionada: [cert: Certificacao];
  certificacaoRemovida: [id: string];
  certificacaoClicada: [cert: Certificacao];
}>();

const mostraForm = ref(false);
const adicionando = ref(false);
const removendo = ref<string | null>(null);
const erro = ref("");
const sucesso = ref("");

const formCertificacao = reactive<CertificacaoForm>({
  nome: "",
  instituicao: "",
  dataEmissao: "",
  dataValidade: "",
});

function formatarData(data: string): string {
  try {
    return new Date(data).toLocaleDateString("pt-BR");
  } catch {
    return data;
  }
}

async function adicionarCertificacao() {
  if (!formCertificacao.nome) {
    erro.value = "Nome da certificação é obrigatório";
    return;
  }

  adicionando.value = true;
  erro.value = "";
  sucesso.value = "";

  try {
    const formData = new FormData();
    formData.append("nome", formCertificacao.nome);
    if (formCertificacao.instituicao) {
      formData.append("instituicao", formCertificacao.instituicao);
    }
    if (formCertificacao.dataEmissao) {
      formData.append("dataEmissao", formCertificacao.dataEmissao);
    }
    if (formCertificacao.dataValidade) {
      formData.append("dataValidade", formCertificacao.dataValidade);
    }
    if (formCertificacao.documento) {
      formData.append("documento", formCertificacao.documento);
    }

    const { data } = await api.post<Certificacao>(
      `/api/prestadores/${props.prestadorId}/certificacoes`,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );

    emit("certificacaoAdicionada", data);
    sucesso.value = "Certificação adicionada com sucesso!";
    mostraForm.value = false;

    // Limpar formulário
    formCertificacao.nome = "";
    formCertificacao.instituicao = "";
    formCertificacao.dataEmissao = "";
    formCertificacao.dataValidade = "";
    delete (formCertificacao as any).documento;

    setTimeout(() => {
      sucesso.value = "";
    }, 3000);
  } catch (e: any) {
    erro.value = e.response?.data?.message ?? "Erro ao adicionar certificação";
  } finally {
    adicionando.value = false;
  }
}

async function removerCertificacao(id: string) {
  if (!confirm("Deseja remover esta certificação?")) return;

  removendo.value = id;
  erro.value = "";

  try {
    await api.delete(`/api/prestadores/${props.prestadorId}/certificacoes/${id}`);
    emit("certificacaoRemovida", id);
  } catch (e: any) {
    erro.value = e.response?.data?.message ?? "Erro ao remover certificação";
  } finally {
    removendo.value = null;
  }
}
</script>
