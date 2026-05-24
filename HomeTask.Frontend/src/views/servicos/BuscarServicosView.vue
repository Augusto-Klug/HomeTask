<template>
  <div class="max-w-5xl mx-auto px-4 py-8">
    <h1 class="text-title font-semibold text-foreground mb-6">Buscar Serviços</h1>

    <HtCard class="mb-6">
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-4">
        <HtSelect
          v-model="filtro.categoria"
          :options="categoriasOpcoes"
          label="Categoria"
          placeholder="Todas as categorias"
        />

        <HtInput
          v-model="filtro.cidade"
          label="Cidade"
          placeholder="Ex: Blumenau"
        />

        <HtInput
          v-model="filtroPrecoStr"
          label="Preço máximo (R$)"
          type="number"
          :allowNegative="false"
          placeholder="Ex: 150"
        />
      </div>

      <HtButton @click="buscar" :loading="carregando">
        <span class="material-symbols-rounded text-base">search</span>
        Buscar
      </HtButton>
    </HtCard>

    <div v-if="carregando" class="flex justify-center py-16">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <template v-else-if="buscou">
      <p v-if="servicos.length === 0" class="text-sm text-muted text-center py-12">
        Nenhum serviço encontrado com os filtros selecionados.
      </p>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <HtCard
          v-for="s in servicos"
          :key="s.id"
          class="hover:border-primary transition-colors"
        >
          <div class="flex items-start justify-between mb-2">
            <div>
              <h3 class="text-sm font-semibold text-foreground">{{ s.titulo }}</h3>
              <p class="text-xs text-muted mt-0.5">
                Prestador: {{ s.prestadorNome || "N/A" }}
              </p>
            </div>
            <HtBadge>{{ obterNomeCategoria(s.categoria) }}</HtBadge>
          </div>

          <p class="text-xs text-muted mb-3 flex items-center gap-1">
            <span class="material-symbols-rounded text-sm">location_on</span>
            {{ s.cidade || "N/A" }}/{{ s.estado || "N/A" }}
          </p>

          <div class="flex items-center justify-between mb-4">
            <span class="text-sm font-bold text-primary">
              R$ {{ formatarPreco(s.precoBase) }}{{ s.unidadeCobranca === "por_hora" ? "/h" : "" }}
            </span>
            <span v-if="s.mediaAvaliacoes != null" class="text-sm text-yellow-500">
              {{ estrelas(s.mediaAvaliacoes) }}
            </span>
          </div>

          <router-link :to="`/servicos/detalhes/${s.id}`">
            <HtButton variant="outline" size="sm" class="w-full">Ver detalhes</HtButton>
          </router-link>
        </HtCard>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from "vue";
import { useRoute } from "vue-router";
import api from "@/services/api";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtInput from "@/components/ui/HtInput.vue";
import HtSelect from "@/components/ui/HtSelect.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import { CATEGORIAS_SERVICO } from "@/types";
import type { BuscarFiltro, ServicoBuscaResumo } from "@/types";

const route = useRoute();

const categoriasOpcoes = [
  { value: "", label: "Todas as categorias" },
  { value: "1", label: "Faxina" },
  { value: "2", label: "Jardinagem" },
  { value: "3", label: "Reparos" },
  { value: "4", label: "Lavanderia" },
  { value: "5", label: "Passadoria" },
  { value: "6", label: "Babysitter" },
  { value: "7", label: "Cuidador de Idosos" },
  { value: "8", label: "Pet Sitter" },
  { value: "9", label: "Cozinheiro" },
  { value: "10", label: "Serviços Gerais" },
];

const filtro = reactive<BuscarFiltro>({
  categoria: (route.query.categoria as string) ?? "",
  cidade: "",
  precoMaximo: null,
  avaliacaoMinima: 0,
});

const filtroPrecoStr = ref("");
const servicos = ref<ServicoBuscaResumo[]>([]);
const carregando = ref(false);
const buscou = ref(false);

async function buscar() {
  carregando.value = true;
  buscou.value = false;

  try {
    const params: Record<string, unknown> = {};
    if (filtro.categoria) params.categoria = filtro.categoria;
    if (filtro.cidade) params.cidade = filtro.cidade;
    if (filtroPrecoStr.value) params.precoMaximo = Number(filtroPrecoStr.value);

    const { data } = await api.get<ServicoBuscaResumo[]>(
      "/api/ServicoOferecido/BuscarServicos",
      { params },
    );
    servicos.value = data;
  } catch {
    servicos.value = [];
  } finally {
    carregando.value = false;
    buscou.value = true;
  }
}

function formatarPreco(valor: number): string {
  return Number(valor).toFixed(2).replace(".", ",");
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0);
  return "★".repeat(cheias) + "☆".repeat(5 - cheias);
}

function obterNomeCategoria(id: number | string): string {
  return CATEGORIAS_SERVICO.find((c) => String(c.value) === String(id))?.label ?? "N/A";
}
</script>
