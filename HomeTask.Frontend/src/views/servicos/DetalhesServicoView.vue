<template>
  <div class="max-w-3xl mx-auto px-4 py-8">
    <div v-if="carregando" class="flex justify-center py-20">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <div v-else-if="!servico" class="text-center py-16 flex flex-col items-center gap-4">
      <span class="material-symbols-rounded text-5xl text-error">error</span>
      <p class="text-sm text-muted">Serviço não encontrado.</p>
      <router-link to="/servicos/buscar">
        <HtButton variant="outline">Voltar à busca</HtButton>
      </router-link>
    </div>

    <template v-else>
      <router-link
        to="/servicos/buscar"
        class="inline-flex items-center gap-1.5 text-sm text-muted hover:text-primary mb-6 transition-colors"
      >
        <span class="material-symbols-rounded text-base">arrow_back</span>
        Voltar à busca
      </router-link>

      <HtCard class="mb-6">
        <div class="flex flex-wrap items-start justify-between gap-3 mb-4">
          <div>
            <h1 class="text-xl font-bold text-foreground">{{ servico.titulo }}</h1>
            <p class="text-sm text-muted mt-0.5">
              {{ ehServicoPrestador ? "Prestador:" : "Solicitante:" }}
              {{ nomeResponsavel }}
            </p>
          </div>
          <HtBadge :variant="HtBadgeVariant.Primary">{{ obterNomeCategoria(servico.categoria) }}</HtBadge>
        </div>

        <HtDivider />

        <div class="flex items-center gap-2 text-sm text-muted mb-2">
          <span class="material-symbols-rounded text-base">location_on</span>
          {{ servico.cidade || "N/A" }}/{{ servico.estado || "N/A" }}
        </div>

        <div class="flex items-center justify-between mb-4">
          <span class="text-lg font-bold text-primary">
            {{ formatarPrecoServico(servico.precoBase, servico.unidadeCobranca) }}
          </span>
          <span v-if="ehServicoPrestador && typeof servico.mediaAvaliacoes === 'number'" class="text-sm text-yellow-500">
            {{ estrelas(servico.mediaAvaliacoes) }}
          </span>
        </div>

        <div v-if="!ehServicoPrestador && servico.dataDesejada" class="mb-4 p-3 bg-primary/5 rounded-lg border border-primary/10">
          <p class="text-xs text-primary font-semibold uppercase mb-1">Data desejada para execução</p>
          <p class="text-sm text-foreground flex items-center gap-2">
            <span class="material-symbols-rounded text-base">event</span>
            {{ formatarData(servico.dataDesejada) }} às {{ formatarHora(servico.dataDesejada) }}
          </p>
        </div>

        <p v-if="servico.descricao" class="text-sm text-foreground mb-6 leading-relaxed">
          {{ servico.descricao }}
        </p>

        <HtButton v-if="auth.isLoggedIn" @click="irParaFluxo">
          <span class="material-symbols-rounded text-base">calendar_month</span>
          {{ ehServicoPrestador ? "Agendar agora" : "Enviar proposta" }}
        </HtButton>
        <router-link v-else :to="`/login?redirect=/servicos/detalhes/${props.id}`">
          <HtButton variant="outline">
            <span class="material-symbols-rounded text-base">login</span>
            Faca login para prosseguir
          </HtButton>
        </router-link>
      </HtCard>

      <div v-if="avaliacoes.length">
        <h2 class="text-title font-semibold text-foreground mb-4">Avaliacoes</h2>
        <div class="flex flex-col gap-3">
          <HtCard v-for="av in avaliacoes" :key="av.id">
            <div class="flex items-center justify-between mb-1">
              <span class="text-sm font-semibold text-foreground">{{ av.clienteNome }}</span>
              <span class="text-sm text-yellow-500">{{ estrelas(av.nota) }}</span>
            </div>
            <p class="text-sm text-foreground mb-1">{{ av.comentario }}</p>
            <p class="text-xs text-muted">{{ formatarData(av.data) }}</p>
          </HtCard>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import api from "@/services/api";
import { formatarData, formatarHora, formatarPrecoServico } from "@/shared/utils";
import { useAuthStore } from "@/stores/auth";
import type { Avaliacao, ServicoDetalhe } from "@/types";
import { CATEGORIAS_SERVICO } from "@/types";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtBadge, { HtBadgeVariant } from "@/components/ui/HtBadge.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";
import HtDivider from "@/components/ui/HtDivider.vue";

const props = defineProps<{ id: string }>();

const auth = useAuthStore();
const router = useRouter();

const servico = ref<ServicoDetalhe | null>(null);
const avaliacoes = ref<Avaliacao[]>([]);
const carregando = ref(true);

const ehServicoPrestador = computed(() => !!servico.value && "prestadorId" in servico.value);
const nomeResponsavel = computed(() => {
  if (!servico.value) return "N/A";
  return "prestadorNome" in servico.value
    ? servico.value.prestadorNome || "N/A"
    : servico.value.clienteNome || "N/A";
});

onMounted(async () => {
  try {
    const { data } = await api.get<ServicoDetalhe>("/api/ServicoOferecido/ObterServicoPorId", {
      params: { id: props.id },
    });
    servico.value = data;

    if (data && "prestadorId" in data) {
      const av = await api
        .get<Avaliacao[]>("/api/Avaliacao/ObterAvaliacoesPorPrestador", {
          params: { prestadorId: data.prestadorId },
        })
        .catch(() => ({ data: [] as Avaliacao[] }));
      avaliacoes.value = av.data ?? [];
    }
  } catch {
    servico.value = null;
  } finally {
    carregando.value = false;
  }
});

function irParaFluxo() {
  if (!servico.value) return;

  if ("prestadorId" in servico.value) {
    router.push({
      name: "agendamento-novo",
      params: { servicoId: props.id },
    });
    return;
  }

  router.push({
    name: "proposta-nova",
    params: { servicoId: props.id },
  });
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0);
  return "*".repeat(cheias) + "o".repeat(5 - cheias);
}

function obterNomeCategoria(categoria: number | { id: string; nome: string; icone: string }): string {
  if (typeof categoria === "object" && categoria !== null) {
    return categoria.nome;
  }

  const encontrada = CATEGORIAS_SERVICO.find((item) => String(item.value) === String(categoria));
  return encontrada?.label ?? String(categoria);
}
</script>
