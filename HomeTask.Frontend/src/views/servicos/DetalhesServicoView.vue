<template>
  <div class="max-w-3xl mx-auto px-4 py-8">
    <div v-if="carregando" class="flex justify-center py-20">
      <HtSpinner size="lg" class="text-primary" />
    </div>

    <div
      v-else-if="!servico"
      class="text-center py-16 flex flex-col items-center gap-4"
    >
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
              {{ isServicoPrestador(servico) ? "Prestador:" : "Solicitante:" }}
              {{ obterNomeOrigem(servico) }}
            </p>
          </div>
          <HtBadge variant="primary">
            {{ obterNomeCategoria(servico.categoria) }}
          </HtBadge>
        </div>

        <HtDivider />

        <div class="flex items-center gap-2 text-sm text-muted mb-2">
          <span class="material-symbols-rounded text-base">location_on</span>
          {{ servico.cidade || "N/A" }}/{{ servico.estado || "N/A" }}
        </div>

        <div class="flex items-center justify-between mb-4">
          <div class="flex flex-col">
            <span class="text-lg font-bold text-primary">
              R$ {{ formatarPreco(servico.precoBase) }}
              <span class="text-sm font-normal text-muted">
                {{ servico.unidadeCobranca === "por_hora" ? "/ hora" : "/ total" }}
              </span>
            </span>
          </div>
          <span
            v-if="isServicoPrestador(servico) && servico.mediaAvaliacoes != null"
            class="text-sm text-yellow-500"
          >
            {{ estrelas(servico.mediaAvaliacoes) }}
          </span>
        </div>

        <div
          v-if="isServicoCliente(servico) && servico.dataDesejada"
          class="mb-4 p-3 bg-primary/5 rounded-lg border border-primary/10"
        >
          <p class="text-xs text-primary font-semibold uppercase mb-1">
            Data Desejada para Execução
          </p>
          <p class="text-sm text-foreground flex items-center gap-2">
            <span class="material-symbols-rounded text-base">event</span>
            {{ formatarData(servico.dataDesejada) }} às
            {{ formatarHora(servico.dataDesejada) }}
          </p>
        </div>

        <p v-if="servico.descricao" class="text-sm text-foreground mb-6 leading-relaxed">
          {{ servico.descricao }}
        </p>

        <HtButton v-if="auth.isLoggedIn" @click="irParaFluxo">
          <span class="material-symbols-rounded text-base">calendar_month</span>
          {{ isServicoPrestador(servico) ? "Agendar agora" : "Enviar proposta" }}
        </HtButton>
        <router-link v-else :to="`/login?redirect=/servicos/detalhes/${props.id}`">
          <HtButton variant="outline">
            <span class="material-symbols-rounded text-base">login</span>
            Faça login para prosseguir
          </HtButton>
        </router-link>
      </HtCard>

      <div v-if="avaliacoes.length">
        <h2 class="text-title font-semibold text-foreground mb-4">Avaliações</h2>
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
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import api from "@/services/api";
import { useAuthStore } from "@/stores/auth";
import { CATEGORIAS_SERVICO } from "@/types";
import type {
  Avaliacao,
  ServicoClienteDetalhe,
  ServicoDetalhe,
  ServicoPrestadorDetalhe,
} from "@/types";
import HtBadge from "@/components/ui/HtBadge.vue";
import HtButton from "@/components/ui/HtButton.vue";
import HtCard from "@/components/ui/HtCard.vue";
import HtDivider from "@/components/ui/HtDivider.vue";
import HtSpinner from "@/components/ui/HtSpinner.vue";

const props = defineProps<{ id: string }>();

const auth = useAuthStore();
const router = useRouter();

const servico = ref<ServicoDetalhe | null>(null);
const avaliacoes = ref<Avaliacao[]>([]);
const carregando = ref(true);

onMounted(async () => {
  try {
    const { data } = await api.get<ServicoDetalhe>(
      "/api/ServicoOferecido/ObterServicoPorId",
      { params: { id: props.id } },
    );

    servico.value = data;

    if (isServicoPrestador(data)) {
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

function isServicoPrestador(
  valor: ServicoDetalhe,
): valor is ServicoPrestadorDetalhe {
  return valor.tipoAnuncio === 1;
}

function isServicoCliente(valor: ServicoDetalhe): valor is ServicoClienteDetalhe {
  return valor.tipoAnuncio === 2;
}

function irParaFluxo() {
  if (!servico.value) return;

  router.push({
    name: isServicoPrestador(servico.value) ? "agendamento-novo" : "proposta-nova",
    params: { servicoId: props.id },
  });
}

function obterNomeOrigem(valor: ServicoDetalhe): string {
  return isServicoPrestador(valor)
    ? valor.prestadorNome || "N/A"
    : valor.clienteNome || "N/A";
}

function formatarPreco(valor: number): string {
  return Number(valor).toFixed(2).replace(".", ",");
}

function estrelas(media: number): string {
  const cheias = Math.floor(media ?? 0);
  return "★".repeat(cheias) + "☆".repeat(5 - cheias);
}

function formatarData(dataStr: string): string {
  return new Date(dataStr).toLocaleDateString("pt-BR");
}

function formatarHora(dataStr: string): string {
  return new Date(dataStr).toLocaleTimeString("pt-BR", {
    hour: "2-digit",
    minute: "2-digit",
  });
}

function obterNomeCategoria(id: number | string): string {
  const cat = CATEGORIAS_SERVICO.find((c) => String(c.value) === String(id));
  return cat ? cat.label : "N/A";
}
</script>
