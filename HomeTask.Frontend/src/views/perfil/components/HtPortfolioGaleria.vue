<template>
  <div class="flex flex-col gap-4">
    <div class="flex items-center justify-between">
      <h3 class="text-lg font-semibold">Galeria de Portfólio</h3>
      <button
        v-if="!mostraForm && isPrestador"
        type="button"
        @click="mostraForm = true"
        class="btn btn-sm btn-primary"
      >
        <span class="material-symbols-rounded text-sm">add_a_photo</span>
        Adicionar foto
      </button>
    </div>

    <HtAlert v-if="erro" variant="error" :message="erro" class="mb-2" />
    <HtAlert v-if="sucesso" variant="success" :message="sucesso" class="mb-2" />

    <form
      v-if="mostraForm"
      @submit.prevent="adicionarPortfolio"
      class="card bg-base-200 p-4 mb-4"
    >
      <div class="flex flex-col gap-3">
        <div>
          <label class="label">
            <span class="label-text">Foto (JPG, PNG ou WebP)</span>
          </label>
          <input
            ref="inputFoto"
            type="file"
            accept="image/jpeg,image/png,image/webp"
            @change="(e: Event) => {
              const target = e.target as HTMLInputElement;
              const file = target.files?.[0];
              if (file) {
                formPortfolio.imagem = file;
                mostrarPreview(file);
              }
            }"
            required
            class="file-input file-input-bordered w-full"
          />
          <p class="text-xs text-base-content/60 mt-1">Máximo 5MB</p>
        </div>

        <div v-if="previewUrl" class="mt-2">
          <img
            :src="previewUrl"
            alt="Preview"
            class="w-full h-48 object-cover rounded"
          />
        </div>

        <HtInput
          v-model="formPortfolio.titulo"
          label="Título da foto (opcional)"
          placeholder="Ex: Sala de estar reformada"
        />
        <textarea
          v-model="formPortfolio.descricao"
          placeholder="Descrição da foto (opcional)"
          class="textarea textarea-bordered"
          rows="3"
        ></textarea>

        <div class="flex gap-2 justify-end">
          <button
            type="button"
            @click="cancelarForm"
            class="btn btn-ghost btn-sm"
            :disabled="adicionando"
          >
            Cancelar
          </button>
          <button
            type="submit"
            class="btn btn-primary btn-sm"
            :disabled="adicionando || !formPortfolio.imagem"
            :loading="adicionando"
          >
            Salvar
          </button>
        </div>
      </div>
    </form>

    <div v-if="portfolios.length === 0 && !mostraForm" class="text-center py-8 text-base-content/50">
      <p>Nenhuma foto adicionada ainda</p>
    </div>

    <div v-else class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
      <div
        v-for="item in portfolios"
        :key="item.id"
        class="card bg-base-100 shadow-sm overflow-hidden border border-base-300 hover:shadow-md transition"
      >
        <figure class="relative h-48 bg-base-200">
          <div
            v-if="imagensComErro[item.id]"
            class="w-full h-full flex flex-col items-center justify-center gap-2 text-center text-base-content/60"
          >
            <span class="material-symbols-rounded text-5xl">image_not_supported</span>
            <p class="text-sm px-4">Não foi possível carregar esta imagem.</p>
          </div>
          <img
            v-else
            :src="obterUrlImagem(item.urlImagem)"
            :alt="item.titulo || 'Portfólio'"
            class="w-full h-full object-cover"
            @error="marcarErroImagem(item.id)"
          />
          <button
            v-if="isPrestador"
            type="button"
            @click="removerPortfolio(item.id)"
            class="absolute top-2 right-2 btn btn-sm btn-circle btn-ghost bg-black/50 hover:bg-black/70 text-white"
            :disabled="removendo === item.id"
          >
            <span class="material-symbols-rounded text-lg">delete</span>
          </button>
        </figure>
        <div class="card-body p-3">
          <h4 v-if="item.titulo" class="font-semibold text-sm">{{ item.titulo }}</h4>
          <p v-if="item.descricao" class="text-xs text-base-content/70">
            {{ item.descricao }}
          </p>
          <p class="text-xs text-base-content/50 mt-1">
            {{ formatarData(item.dataCadastro) }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from "vue";
import api from "@/services/api";
import { resolverUrlArquivo } from "@/shared/utils";
import HtInput from "@/components/ui/HtInput.vue";
import HtAlert from "@/components/ui/HtAlert.vue";
import type { Portfolio, PortfolioForm } from "@/types";

const props = defineProps<{
  prestadorId: string;
  isPrestador: boolean;
  portfolios: Portfolio[];
}>();

const emit = defineEmits<{
  portfolioAdicionado: [portfolio: Portfolio];
  portfolioRemovido: [id: string];
}>();

const mostraForm = ref(false);
const adicionando = ref(false);
const removendo = ref<string | null>(null);
const erro = ref("");
const sucesso = ref("");
const previewUrl = ref("");
const inputFoto = ref<HTMLInputElement>();
const imagensComErro = reactive<Record<string, boolean>>({});

const formPortfolio = reactive<PortfolioForm>({
  titulo: "",
  descricao: "",
});

function formatarData(data: string): string {
  try {
    return new Date(data).toLocaleDateString("pt-BR");
  } catch {
    return data;
  }
}

function obterUrlImagem(url: string): string {
  return resolverUrlArquivo(url);
}

function marcarErroImagem(id: string) {
  imagensComErro[id] = true;
}

function mostrarPreview(file: File) {
  const reader = new FileReader();
  reader.onload = (e) => {
    previewUrl.value = e.target?.result as string;
  };
  reader.readAsDataURL(file);
}

function cancelarForm() {
  mostraForm.value = false;
  previewUrl.value = "";
  formPortfolio.titulo = "";
  formPortfolio.descricao = "";
  delete (formPortfolio as any).imagem;
  if (inputFoto.value) {
    inputFoto.value.value = "";
  }
}

async function adicionarPortfolio() {
  if (!formPortfolio.imagem) {
    erro.value = "Foto é obrigatória";
    return;
  }

  // Validar tamanho (5MB)
  if (formPortfolio.imagem.size > 5 * 1024 * 1024) {
    erro.value = "Arquivo excede o tamanho máximo de 5 MB";
    return;
  }

  adicionando.value = true;
  erro.value = "";
  sucesso.value = "";

  try {
    const formData = new FormData();
    formData.append("imagem", formPortfolio.imagem);
    if (formPortfolio.titulo) {
      formData.append("titulo", formPortfolio.titulo);
    }
    if (formPortfolio.descricao) {
      formData.append("descricao", formPortfolio.descricao);
    }

    const { data } = await api.post<Portfolio>(
      `/api/prestadores/${props.prestadorId}/portfolio`,
      formData,
      {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      }
    );

    emit("portfolioAdicionado", data);
    sucesso.value = "Foto adicionada com sucesso!";
    cancelarForm();

    setTimeout(() => {
      sucesso.value = "";
    }, 3000);
  } catch (e: any) {
    erro.value = e.response?.data?.message ?? "Erro ao adicionar foto";
  } finally {
    adicionando.value = false;
  }
}

async function removerPortfolio(id: string) {
  if (!confirm("Deseja remover esta foto?")) return;

  removendo.value = id;
  erro.value = "";

  try {
    await api.delete(`/api/prestadores/${props.prestadorId}/portfolio/${id}`);
    emit("portfolioRemovido", id);
  } catch (e: any) {
    erro.value = e.response?.data?.message ?? "Erro ao remover foto";
  } finally {
    removendo.value = null;
  }
}
</script>
